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
    public class PropertyObservableTest : ReactiveTest
    {
        [TestMethod]
        public void ObserveProperty()
        {
            var item = new Item
            {
                Child = new Item
                {
                    Value = 10,
                },
            };

            var testScheduler = new TestScheduler();
            var testObserver = testScheduler.CreateObserver<int>();

            var path = PropertyObservable.CreateFromPropertySelector(item, x => x.Child.Value);
            path.Subscribe(testObserver);
            path.GetPropertyPathValue().Is(10);
            item.Child.Value = 1;

            testObserver.Messages.Is(OnNext(0, 1));
            item.IsPropertyChangedEmpty.IsFalse();
            path.Dispose();
            item.IsPropertyChangedEmpty.IsTrue();
            item.Child.Value = 100;
            testObserver.Messages.Is(OnNext(0, 1));
        }

        [TestMethod]
        public void ChangeObserveSourcePropertyType()
        {
            var item = new Item
            {
                Child = new Item
                {
                    Value = 10,
                },
            };

            var testScheduler = new TestScheduler();
            var testObserver = testScheduler.CreateObserver<int>();

            var path = PropertyObservable.CreateFromPropertySelector(item, x => x.Child.Value);
            path.Subscribe(testObserver);
            path.GetPropertyPathValue().Is(10);
            item.Child.Value = 1;
            testObserver.Messages.Is(OnNext(0, 1));

            item.Child = new AnotherItem
            {
                Value = 9999,
            };
            path.GetPropertyPathValue().Is(9999);
            testObserver.Messages.Is(OnNext(0, 1), OnNext(0, 9999));
        }

        class ItemBase : INotifyPropertyChanged
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

            private int _value;
            public int Value
            {
                get { return _value; }
                set { SetProperty(ref _value, value); }
            }

            private ItemBase _child;
            public ItemBase Child
            {
                get { return _child; }
                set { SetProperty(ref _child, value); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
        class Item : ItemBase { }
        class AnotherItem: ItemBase { }
    }

}
