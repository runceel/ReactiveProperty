using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Internals;

namespace ReactiveProperty.Tests.Internals
{
    [TestClass]
    public class PropertyObserverTest : ReactiveTest
    {
        [TestMethod]
        public void ObserveProperty()
        {
            var item = new Item
            {
                Child = new Item
                {
                    RefTypeValue = "xxx",
                    ValueTypeValue = 10,
                },
            };

            var testScheduler = new TestScheduler();
            var testObserver = testScheduler.CreateObserver<int>();

            var path = PropertyObserver.CreateFromPropertySelector(item, x => x.Child.ValueTypeValue);
            path.Subscribe(testObserver);
            path.GetPropertyPathValue().Is(10);
            item.Child.ValueTypeValue = 1;

            testObserver.Messages.Is(OnNext(0, 1));
            item.IsPropertyChangedEmpty.IsFalse();
            path.Dispose();
            item.IsPropertyChangedEmpty.IsTrue();
            item.Child.ValueTypeValue = 100;
            testObserver.Messages.Is(OnNext(0, 1));
        }

        class Item : INotifyPropertyChanged
        {
            public bool IsPropertyChangedEmpty => PropertyChanged == null;
            private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value))
                {
                    return;
                }

                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private string _refTypeValue;
            public string RefTypeValue
            {
                get { return _refTypeValue; }
                set { SetProperty(ref _refTypeValue, value); }
            }

            private int _valueTypeValue;
            public int ValueTypeValue
            {
                get { return _valueTypeValue; }
                set { SetProperty(ref _valueTypeValue, value); }
            }

            private Item _child;
            public Item Child
            {
                get { return _child; }
                set { SetProperty(ref _child, value); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
