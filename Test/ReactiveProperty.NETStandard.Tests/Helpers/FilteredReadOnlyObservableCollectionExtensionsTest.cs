using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;

namespace ReactiveProperty.Tests.Helpers
{
    [TestClass]
    public class FilteredReadOnlyObservableCollectionExtensionsTest : ReactiveTest
    {
        [TestMethod]
        public void ObserveElementProperty()
        {
            var testScheduler = new TestScheduler();
            var observer = testScheduler.CreateObserver<PropertyPack<Item, int>>();
            var source = new ObservableCollection<Item>(new[] { new Item(), new Item() });
            var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IntValue > 0);
            var d = filtered.ObserveElementProperty(x => x.IntValue)
                .Subscribe(observer);

            source[0].IntValue = 100;
            source[0].IntValue = 99;
            source[1].IntValue = 10;
            source[1].IntValue = 0;
            source[0].Value = "0: updated";
            source[1].Value = "1: updated";

            observer.Messages.Is(
                OnNext<PropertyPack<Item, int>>(0, x => x.Instance == source[0] && x.Value == 100),
                OnNext<PropertyPack<Item, int>>(0, x => x.Instance == source[0] && x.Value == 99),
                OnNext<PropertyPack<Item, int>>(0, x => x.Instance == source[1] && x.Value == 10));
        }

        [TestMethod]
        public void ObserveElementPropertyChanged()
        {
            var testScheduler = new TestScheduler();
            var observer = testScheduler.CreateObserver<SenderEventArgsPair<Item, PropertyChangedEventArgs>>();
            var source = new ObservableCollection<Item>(new[] { new Item(), new Item() });
            var filtered = source.ToFilteredReadOnlyObservableCollection(x => x.IntValue > 0);
            var d = FilteredReadOnlyObservableCollectionExtensions.ObserveElementPropertyChanged(filtered)
                .Subscribe(observer);

            source[0].IntValue = 100; // add item
            source[0].Value = "xxxx"; // property change
            source[1].IntValue = 10;  // add item
            source[1].IntValue = 0; // remove item
            source[1].Value = "1: updated"; // property change, but this is already removed from the filtered collection.
            source[0].Value = "0: updated"; // property change
            source[0].IntValue = 999; // property change

            observer.Messages.Is(
                OnNext<SenderEventArgsPair<Item, PropertyChangedEventArgs>>(0, x => x.Sender == source[0] && x.EventArgs.PropertyName == nameof(Item.Value)),
                OnNext<SenderEventArgsPair<Item, PropertyChangedEventArgs>>(0, x => x.Sender == source[0] && x.EventArgs.PropertyName == nameof(Item.Value)),
                OnNext<SenderEventArgsPair<Item, PropertyChangedEventArgs>>(0, x => x.Sender == source[0] && x.EventArgs.PropertyName == nameof(Item.IntValue)));
        }

        class Item : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) { return; }
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private string _value;
            public string Value
            {
                get => _value;
                set => SetProperty(ref _value, value);
            }

            private int _intValue;
            public int IntValue
            {
                get => _intValue;
                set => SetProperty(ref _intValue, value);
            }
        }
    }
}
