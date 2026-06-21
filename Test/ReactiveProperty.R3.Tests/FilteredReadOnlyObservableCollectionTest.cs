#nullable enable

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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
}
