#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3;
using Reactive.Bindings.R3.Extensions;

namespace ReactiveProperty.Tests;

[TestClass]
public sealed class CollectionObservationTest
{
    [TestMethod]
    public void ObserveCollectionChangedActions()
    {
        var source = new ObservableCollection<string> { "a", "b" };
        var added = new List<string>();
        var removed = new List<string>();
        var moved = new List<OldNewPair<string>>();
        var replaced = new List<OldNewPair<string>>();
        var resetCount = 0;

        using var d1 = source.ObserveAddChanged<string>().Subscribe(added.Add);
        using var d2 = source.ObserveRemoveChanged<string>().Subscribe(removed.Add);
        using var d3 = source.ObserveMoveChanged<string>().Subscribe(moved.Add);
        using var d4 = source.ObserveReplaceChanged<string>().Subscribe(replaced.Add);
        using var d5 = source.ObserveResetChanged<string>().Subscribe(_ => resetCount++);

        source.Add("c");
        source.RemoveAt(0);
        source.Move(0, 1);
        source[0] = "d";
        source.Clear();

        added.Is("c");
        removed.Is("a");
        moved.Count.Is(1);
        moved[0].OldItem.Is("b");
        moved[0].NewItem.Is("b");
        replaced.Count.Is(1);
        replaced[0].OldItem.Is("c");
        replaced[0].NewItem.Is("d");
        resetCount.Is(1);
    }

    [TestMethod]
    public void ObserveElementPropertyTracksCollectionChanges()
    {
        var first = new Item { Name = "first" };
        var second = new Item { Id = 2, Name = "second" };
        var source = new ObservableCollection<Item> { first };
        var values = new List<string>();

        using var subscription = source
            .ObserveElementProperty(x => x.Name)
            .Subscribe(x => values.Add($"{x.Instance.Id}:{x.Property.Name}:{x.Value}"));

        source.Add(second);
        first.Name = "first changed";
        source.Remove(first);
        first.Name = "ignored";
        second.Name = "second changed";

        values.Is("0:Name:first", "2:Name:second", "0:Name:first changed", "2:Name:second changed");
    }

    [TestMethod]
    public void ObserveElementObservablePropertyAndPropertyChanged()
    {
        var item = new Item();
        var source = new ObservableCollection<Item> { item };
        var observableValues = new List<string>();
        var changedNames = new List<string?>();

        using var d1 = source
            .ObserveElementObservableProperty(x => x.NameChanged)
            .Subscribe(x => observableValues.Add($"{x.Property.Name}:{x.Value}"));
        using var d2 = source
            .ObserveElementPropertyChanged()
            .Subscribe(x => changedNames.Add(x.EventArgs.PropertyName));

        item.NameChanged.OnNext("observable");
        item.Name = "property";

        observableValues.Is("NameChanged:observable");
        changedNames.Is(nameof(Item.Name));
    }

    [TestMethod]
    public void ObserveElementPropertyAcceptsExplicitBarePropertyName()
    {
        var item = new Item { Name = "first" };
        var source = new ObservableCollection<Item> { item };
        var values = new List<string>();
        Func<Item, string> selector = x => x.Name;

        using var subscription = source
            .ObserveElementProperty(selector, propertyName: nameof(Item.Name))
            .Subscribe(x => values.Add($"{x.Property.Name}:{x.Value}"));

        item.Name = "changed";

        values.Is("Name:first", "Name:changed");
    }

    [TestMethod]
    public void ObserveElementPropertyRejectsNestedPropertySelector()
    {
        var source = new ObservableCollection<Outer> { new() };

        Assert.ThrowsExactly<ArgumentException>(() =>
            source.ObserveElementProperty(x => x.Inner.Name));
    }

    private sealed class Item : INotifyPropertyChanged
    {
        private string _name = "";

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public Subject<string> NameChanged { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private sealed class Outer : INotifyPropertyChanged
    {
        private string _name = "outer";

        public Inner Inner { get; } = new();

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private sealed class Inner : INotifyPropertyChanged
    {
        private string _name = "";

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
