#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.R3.Extensions;

namespace ReactiveProperty.Tests;

[TestClass]
public sealed class FilteredReadOnlyObservableCollectionTest
{
    [TestMethod]
    public void FiltersSourceAndRefreshesOnElementChange()
    {
        var source = new ObservableCollection<Item>
        {
            new() { Name = "a", IsVisible = true },
            new() { Name = "b", IsVisible = false },
        };
        var collection = source.ToFilteredReadOnlyObservableCollection(x => x.IsVisible);
        var actions = new List<NotifyCollectionChangedAction>();

        collection.CollectionChanged += (_, e) => actions.Add(e.Action);

        collection.Select(x => x.Name).ToArray().Is("a");

        source[1].IsVisible = true;
        collection.Select(x => x.Name).ToArray().Is("a", "b");

        collection.Refresh(x => x.Name == "b");
        collection.Select(x => x.Name).ToArray().Is("b");
        actions.Is(NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Reset);
    }

    [TestMethod]
    public void RaisesCollectionChangedOnProvidedSynchronizationContext()
    {
        var source = new ObservableCollection<Item>();
        var context = new TestSynchronizationContext();
        var collection = source.ToFilteredReadOnlyObservableCollection(x => true, raiseEventContext: context);
        var raised = false;

        collection.CollectionChanged += (_, _) => raised = true;

        source.Add(new Item { Name = "a", IsVisible = true });

        raised.IsTrue();
        Assert.IsTrue(context.PostCount > 0);
    }

    [TestMethod]
    public void ReusesFixedPropertyChangedEventArgs()
    {
        var source = new ObservableCollection<Item>
        {
            new() { Name = "a", IsVisible = true },
        };
        var collection = source.ToFilteredReadOnlyObservableCollection(x => x.IsVisible);
        var propertyChangedEvents = new List<PropertyChangedEventArgs>();
        collection.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

        source[0] = new Item { Name = "b", IsVisible = true };
        source[0] = new Item { Name = "c", IsVisible = true };

        propertyChangedEvents.Select(static x => x.PropertyName).Is(
            nameof(collection.Count),
            "Item[]",
            nameof(collection.Count),
            "Item[]");
        Assert.AreSame(propertyChangedEvents[0], propertyChangedEvents[2]);
        Assert.AreSame(propertyChangedEvents[1], propertyChangedEvents[3]);
    }

    [TestMethod]
    public void RangeRemovalAvoidsExcessAllocation()
    {
        var source = new RangeObservableCollection<Item>(
            Enumerable.Range(0, 256).Select(_ => new Item { IsVisible = true }));
        var collection = source.ToFilteredReadOnlyObservableCollection(x => x.IsVisible);

        var before = GC.GetAllocatedBytesForCurrentThread();
        source.RemoveRange(0, 256);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.AreEqual(0, collection.Count);
        Assert.IsTrue(allocated < 15000, $"Range removal allocated {allocated} bytes.");
    }

    [TestMethod]
    [DataRow(0, RemovalVisibility.AllVisible)]
    [DataRow(3, RemovalVisibility.AllVisible)]
    [DataRow(5, RemovalVisibility.AllVisible)]
    [DataRow(0, RemovalVisibility.NoneVisible)]
    [DataRow(3, RemovalVisibility.NoneVisible)]
    [DataRow(5, RemovalVisibility.NoneVisible)]
    [DataRow(0, RemovalVisibility.Mixed)]
    [DataRow(3, RemovalVisibility.Mixed)]
    [DataRow(5, RemovalVisibility.Mixed)]
    public void RangeRemovalPreservesItemsAndNotification(int startIndex, RemovalVisibility visibility)
    {
        var source = CreateRangeSource(startIndex, visibility);
        var collection = source.ToFilteredReadOnlyObservableCollection(x => x.IsVisible);
        var collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
        var propertyChangedNames = new List<string?>();

        collection.CollectionChanged += (_, e) => collectionChangedEvents.Add(e);
        collection.PropertyChanged += (_, e) => propertyChangedNames.Add(e.PropertyName);

        var removedItems = source.Skip(startIndex).Take(2).ToArray();
        var firstRemovedTarget = removedItems.FirstOrDefault(x => x.IsVisible);
        var filteredIndex = firstRemovedTarget is null
            ? -1
            : source.Take(source.IndexOf(firstRemovedTarget)).Count(x => x.IsVisible);

        source.RemoveRange(startIndex, 2);

        var expectedNames = source.Where(x => x.IsVisible).Select(x => x.Name).ToArray();
        var actualNames = collection.Select(x => x.Name).ToArray();
        CollectionAssert.AreEqual(expectedNames, actualNames);

        if (firstRemovedTarget is null)
        {
            Assert.AreEqual(0, collectionChangedEvents.Count);
            Assert.AreEqual(0, propertyChangedNames.Count);
            return;
        }

        Assert.AreEqual(1, collectionChangedEvents.Count);
        var args = collectionChangedEvents[0];
        Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
        Assert.AreSame(firstRemovedTarget, args.OldItems![0]);
        Assert.AreEqual(filteredIndex, args.OldStartingIndex);
        CollectionAssert.AreEqual(new[] { nameof(collection.Count), "Item[]" }, propertyChangedNames);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void SingleItemRemovalPreservesItemsAndNotification(bool isVisible)
    {
        var source = new RangeObservableCollection<Item>(
            Enumerable.Range(0, 5)
                .Select(index => new Item { Name = $"item{index}", IsVisible = index == 2 ? isVisible : true }));
        var collection = source.ToFilteredReadOnlyObservableCollection(x => x.IsVisible);
        var collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
        collection.CollectionChanged += (_, e) => collectionChangedEvents.Add(e);
        var removedItem = source[2];
        var filteredIndex = source.Take(2).Count(x => x.IsVisible);

        source.RemoveRange(2, 1);

        CollectionAssert.AreEqual(
            source.Where(x => x.IsVisible).Select(x => x.Name).ToArray(),
            collection.Select(x => x.Name).ToArray());
        Assert.AreEqual(isVisible ? 1 : 0, collectionChangedEvents.Count);
        if (isVisible)
        {
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, collectionChangedEvents[0].Action);
            Assert.AreSame(removedItem, collectionChangedEvents[0].OldItems![0]);
            Assert.AreEqual(filteredIndex, collectionChangedEvents[0].OldStartingIndex);
        }
    }

    private static RangeObservableCollection<Item> CreateRangeSource(int startIndex, RemovalVisibility visibility)
    {
        var items = Enumerable.Range(0, 7)
            .Select(index => new Item
            {
                Name = $"item{index}",
                IsVisible = visibility switch
                {
                    RemovalVisibility.AllVisible => true,
                    RemovalVisibility.NoneVisible => false,
                    RemovalVisibility.Mixed => index != startIndex,
                    _ => throw new AssertFailedException($"Unknown visibility case: {visibility}"),
                },
            });
        return new RangeObservableCollection<Item>(items);
    }

    private sealed class TestSynchronizationContext : SynchronizationContext
    {
        public int PostCount { get; private set; }

        public override void Post(SendOrPostCallback d, object? state)
        {
            PostCount++;
            d(state);
        }
    }

    private sealed class Item : INotifyPropertyChanged
    {
        private bool _isVisible;

        public string Name { get; set; } = "";

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public enum RemovalVisibility
    {
        AllVisible,
        NoneVisible,
        Mixed,
    }

    private sealed class RangeObservableCollection<T> : ObservableCollection<T>
    {
        public RangeObservableCollection(IEnumerable<T> items)
            : base(items.ToList())
        {
        }

        public void RemoveRange(int index, int count)
        {
            var removedItems = new List<T>(count);
            for (var i = 0; i < count; i++)
            {
                removedItems.Add(Items[index]);
                Items.RemoveAt(index);
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                (IList)removedItems,
                index));
        }
    }
}
