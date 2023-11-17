﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using ReactiveProperty.Tests.TestUtilities;

namespace ReactiveProperty.Tests.Helpers;

[TestClass]
public class FilteredReadOnlyObservableCollectionTest
{
    [TestMethod]
    public void NormalCase()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
        filtered.Select(x => x.Name).Is("tanaka1", "tanaka2", "tanaka4");
        filtered.Count.Is(3);

        source.Add(new Person { Name = "tanaka5", IsRemoved = true });
        filtered.Select(x => x.Name).Is("tanaka1", "tanaka2", "tanaka4");
        filtered.Count.Is(3);

        source.RemoveAt(1); // remove tanaka2
        filtered.Select(x => x.Name).Is("tanaka1", "tanaka4");
        filtered.Count.Is(2);

        source.RemoveAt(1); // remove tanaka3
        filtered.Select(x => x.Name).Is("tanaka1", "tanaka4");
        filtered.Count.Is(2);

        source.Last().IsRemoved = false;
        filtered.Select(x => x.Name).Is("tanaka1", "tanaka4", "tanaka5");
        filtered.Count.Is(3);

        source[0] = new Person { Name = "tanaka1 replaced", IsRemoved = false };
        filtered.Select(x => x.Name).Is("tanaka1 replaced", "tanaka4", "tanaka5");
        filtered.Count.Is(3);

        source.Clear();
        filtered.Count.Is(0);
    }

    [TestMethod]
    public void NormalCase2()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = true },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
        filtered.Select(x => x.Name).Is("tanaka1");
        filtered.Count.Is(1);

        source.Add(new Person { Name = "tanaka4", IsRemoved = false });
        filtered.Select(x => x.Name).Is("tanaka1");
        filtered.Count.Is(1);

        source[3].IsRemoved = true;
        filtered.Select(x => x.Name).Is("tanaka1", "tanaka4");
        filtered.Count.Is(2);

        source.Clear();
        filtered.Count.Is(0);
    }

    [TestMethod]
    public void CollectionChangedAddTest()
    {
        var source = new ObservableCollection<Person>();
        var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source.Add(new Person { Name = "a", IsRemoved = true });
        buffer.Count.Is(0);
        source.Add(new Person { Name = "b", IsRemoved = false });
        buffer.Count.Is(1);
        buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Add && x.NewItems.Cast<Person>().First().Name == "b" && x.NewStartingIndex == 0);

        source[0].IsRemoved = false;
        buffer.Count.Is(2);
        buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Add && x.NewItems.Cast<Person>().First().Name == "a" && x.NewStartingIndex == 0);
    }

    [TestMethod]
    public void CollectionChangedRefreshAndAddTest()
    {
        var source = new ObservableCollection<Person>();
        var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
        filtered.Refresh(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source.Add(new Person { Name = "a", IsRemoved = true });
        buffer.Count.Is(0);
        source.Add(new Person { Name = "b", IsRemoved = false });
        buffer.Count.Is(1);
        buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Add && x.NewItems.Cast<Person>().First().Name == "b" && x.NewStartingIndex == 0);

        source[0].IsRemoved = false;
        buffer.Count.Is(2);
        buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Add && x.NewItems.Cast<Person>().First().Name == "a" && x.NewStartingIndex == 0);
    }

    [TestMethod]
    public void CollectionChangedRemoveTest()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source.RemoveAt(2); // tanaka3 remove
        buffer.Count.Is(0);
        source.RemoveAt(0); // tanaka1 remove
        buffer.Count.Is(1);
        buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 0 && x.OldItems.Cast<Person>().First().Name == "tanaka1");
        source[0].IsRemoved = true;
        buffer.Count.Is(2);
    }

    [TestMethod]
    public void CollectionChangedRefreshAndRemoveTest()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
        filtered.Refresh(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source.RemoveAt(2); // tanaka3 remove
        buffer.Count.Is(0);
        source.RemoveAt(0); // tanaka1 remove
        buffer.Count.Is(1);
        buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 0 && x.OldItems.Cast<Person>().First().Name == "tanaka1");
        source[0].IsRemoved = true;
        buffer.Count.Is(2);
    }

    [TestMethod]
    public void CollectionChangedReplaceTest()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source[0] = new Person { Name = "tanaka1 replaced", IsRemoved = false };
        buffer.Count.Is(1);
        buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Replace && x.NewStartingIndex == 0 && x.NewItems.Cast<Person>().First().Name == "tanaka1 replaced");

        source[1] = new Person { Name = "tanaka2 replaced", IsRemoved = true };
        buffer.Count.Is(2);
        buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka2");
    }

    [TestMethod]
    public void CollectionChangedRefreshAndReplaceTest()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
        filtered.Refresh(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source[0] = new Person { Name = "tanaka1 replaced", IsRemoved = false };
        buffer.Count.Is(1);
        buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Replace && x.NewStartingIndex == 0 && x.NewItems.Cast<Person>().First().Name == "tanaka1 replaced");

        source[1] = new Person { Name = "tanaka2 replaced", IsRemoved = true };
        buffer.Count.Is(2);
        buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka2");
    }

    [TestMethod]
    public void CollectionChangedClearTest()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source.Clear();
        buffer.Count.Is(1);
        buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Reset);
    }

    [TestMethod]
    public void CollectionChangedRefreshAndClearTest()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
                new Person { Name = "tanaka4", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
        filtered.Refresh(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source.Clear();
        buffer.Count.Is(1);
        buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Reset);
    }

    [TestMethod]
    public void RemoveIndexOutofRangeException()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = true },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = true },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source[2].IsRemoved = false;

        buffer.Count.Is(1);
        buffer[0].Action.Is(NotifyCollectionChangedAction.Remove);
        buffer[0].OldItems[0].Is(source[2]);

        filtered.Count.Is(1);
    }

    [TestMethod]
    public void InconsistencyIndexWhenSourceCollectionItemReplace()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = false },
                new Person { Name = "tanaka3", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source[1] = new Person { Name = "tanaka2 replaced", IsRemoved = true };
        buffer.Count().Is(1);
        buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka2");

        source.RemoveAt(2); // tanaka3 remove
        buffer.Count().Is(2);
        buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka3");
        filtered.ToArray().Length.Is(filtered.Count);
    }

    [TestMethod]
    public void InconsistencyIndexWhenSourceCollectionItemRemove()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = true },
                new Person { Name = "tanaka3", IsRemoved = false },
                new Person { Name = "tanaka4", IsRemoved = true },
                new Person { Name = "tanaka5", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();
        filtered.CollectionChangedAsObservable().Subscribe(buffer.Add);

        source.RemoveAt(1); // tanaka2 remove
        buffer.Count.Is(0);

        source.RemoveAt(1); // tanaka3 remove
        buffer.Count.Is(1);
        buffer[0].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka3");

        source.RemoveAt(1); //tanaka4 remove
        buffer.Count.Is(1);

        source[1].IsRemoved = true; // tanaka5 logical remove
        buffer.Count.Is(2);
        buffer[1].Is(x => x.Action == NotifyCollectionChangedAction.Remove && x.OldStartingIndex == 1 && x.OldItems.Cast<Person>().First().Name == "tanaka5");
    }

    [TestMethod]
    public void RemoveRangeTest()
    {
        var source = new RangedObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = true },
                new Person { Name = "tanaka3", IsRemoved = false },
                new Person { Name = "tanaka4", IsRemoved = true },
                new Person { Name = "tanaka5", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();

        filtered.Is(source[0], source[2], source[4]);
        source.RemoveRange(new[]
        {
                source[0], source[1], source[2],
            });
        filtered.Is(source.Last());
    }



    [TestMethod]
    public void RefreshTest()
    {
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1", IsRemoved = false },
                new Person { Name = "tanaka2", IsRemoved = true },
                new Person { Name = "tanaka3", IsRemoved = false },
                new Person { Name = "tanaka4", IsRemoved = true },
                new Person { Name = "tanaka5", IsRemoved = false },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
        var buffer = new List<NotifyCollectionChangedEventArgs>();

        filtered.Is(source[0], source[2], source[4]);
        filtered.Refresh(x => x.IsRemoved);
        filtered.Is(source[1], source[3]);
    }

    [TestMethod]
    [Timeout(10 * 1000)]
    public void PerformanceTest()
    {
        var source = new ObservableCollection<Person>(Enumerable.Range(1, 1000000).Select(x => new Person
        {
            Name = $"tanaka {x}",
            IsRemoved = x % 2 == 0,
        }));

        var filtered = source.ToFilteredReadOnlyObservableCollection(x => !x.IsRemoved);
        filtered.Count.Is(500000);
    }

    [TestMethod]
    [Timeout(10 * 1000)]
    public void AddRangeLeargeItemsCaseTest()
    {
        var c = new RangedObservableCollection<Person>();
        var filtered = c.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
        var people = Enumerable.Range(1, 1000000).Select(x => new Person
        {
            Name = $"tanaka {x}",
            IsRemoved = x % 2 == 0,
        });

        c.AddRange(people);
        filtered.Count.Is(500000);
    }

    [TestMethod]
    public void AddRangeSmallItemsCaseTest()
    {
        var c = new RangedObservableCollection<Person>();
        var filtered = c.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
        var people = Enumerable.Range(1, 6).Select(x => new Person
        {
            Name = $"tanaka {x}",
            IsRemoved = x % 2 == 0,
        });

        c.AddRange(people);
        filtered.Count.Is(3);
    }

    [TestMethod]
    public void ReplaceRangeTest()
    {
        var c = new RangedObservableCollection<Person>();
        var filtered = c.ToFilteredReadOnlyObservableCollection(x => x.IsRemoved);
        var people = Enumerable.Range(1, 1000000).Select(x => new Person
        {
            Name = $"tanaka {x}",
            IsRemoved = x % 2 == 0,
        });

        c.ReplaceRange(people);
        filtered.Count.Is(500000);
    }

    [TestMethod]
    public void NestedPropertyCase()
    {
        // Initial state of NestedObject.value is true
        var source = new ObservableCollection<Person>(new[]
        {
                new Person { Name = "tanaka1" },
                new Person { Name = "tanaka2" },
                new Person { Name = "tanaka3" },
                new Person { Name = "tanaka4" },
                new Person { Name = "tanaka5" },
            });

        var filtered = source.ToFilteredReadOnlyObservableCollection(
            x => x.NestedObject.Value,
            x => x.ObserveProperty(y => y.NestedObject.Value));
        filtered.Select(x => x.Name).Is("tanaka1", "tanaka2", "tanaka3", "tanaka4", "tanaka5");
        source[0].NestedObject.Value = false;
        filtered.Select(x => x.Name).Is("tanaka2", "tanaka3", "tanaka4", "tanaka5");
    }

    [TestMethod]
    public void ElementStatusChangedFactoryWithObservePropertyTest()
    {
        // https://github.com/runceel/ReactiveProperty/issues/379
        var observableCollection = new ObservableCollection<Person>();

        var filteredCollection = observableCollection.ToFilteredReadOnlyObservableCollection(
            filter: x => x.Name == "test",
            elementStatusChangedFactory: x => x.ObserveProperty(x => x.Name));

        AssertEx.DoesNotThrow(() =>
        {
            var element = new Person();
            observableCollection.Add(element); // Exception thrown here (Issue 379)
        });
    }

    private class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(field, value)) { return; }
            field = value;
            var h = PropertyChanged;
            if (h != null) { h(this, new PropertyChangedEventArgs(propertyName)); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private bool isRemoved;

        public bool IsRemoved
        {
            get { return isRemoved; }
            set { SetProperty(ref isRemoved, value); }
        }

        public ReactivePropertySlim<bool> NestedObject { get; } = new ReactivePropertySlim<bool>(true);
    }
}
