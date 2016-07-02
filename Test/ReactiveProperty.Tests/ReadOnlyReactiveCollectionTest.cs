using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Subjects;
using System.Reactive;
using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReadOnlyReactiveCollectionTest
    {
        [TestMethod]
        public void AddAndResetTest()
        {
            var s = new Subject<int>();
            var reset = new Subject<Unit>();

            var target = s.ToReadOnlyReactiveCollection(reset, scheduler: Scheduler.CurrentThread);
            target.Count.Is(0);

            s.OnNext(10);
            s.OnNext(2);

            target.Is(10, 2);
            reset.OnNext(Unit.Default);
            target.Count.Is(0);
        }

        [TestMethod]
        public void CollectionChangedTest()
        {
            var s = new Subject<CollectionChanged<int>>();

            var target = s.ToReadOnlyReactiveCollection(scheduler: Scheduler.CurrentThread);
            target.Count.Is(0);
            s.OnNext(CollectionChanged<int>.Add(0, 10));
            s.OnNext(CollectionChanged<int>.Add(1, 2));

            target.Is(10, 2);

            s.OnNext(CollectionChanged<int>.Remove(0, 0));
            target.Is(2);
            s.OnNext(CollectionChanged<int>.Add(1, 3));
            target.Is(2, 3);
            s.OnNext(CollectionChanged<int>.Replace(1, 100));
            target.Is(2, 100);
            s.OnNext(CollectionChanged<int>.Reset);
            target.Count.Is(0);
        }

        [TestMethod]
        public void ObservableCollectionSourceTest()
        {
            var s = new ObservableCollection<string>();
            var target = s.ToReadOnlyReactiveCollection(x => new StringHolder { Value = x }, Scheduler.CurrentThread);

            target.Count.Is(0);

            s.Add("abc");
            target.Count.Is(1);
            target[0].Value.Is("abc");

            s.Add("bcd");
            target.Count.Is(2);
            target[1].Value.Is("bcd");

            s[0] = "hoge";
            target.Count.Is(2);
            target[0].Value.Is("hoge");

            s.RemoveAt(0);
            target.Count.Is(1);
            target[0].Value.Is("bcd");

            s.Add("fuga");
            s.Add("homuhomu");
            s.Clear();

            target.Count.Is(0);
        }

        [TestMethod]
        public void ReadOnlyObservableCollectionSourceTest()
        {
            var s = new ObservableCollection<string>();
            var r = new ReadOnlyObservableCollection<string>(s);
            var target = r.ToReadOnlyReactiveCollection(x => new StringHolder { Value = x }, Scheduler.CurrentThread);

            target.Count.Is(0);

            s.Add("abc");
            target.Count.Is(1);
            target[0].Value.Is("abc");

            s.Add("bcd");
            target.Count.Is(2);
            target[1].Value.Is("bcd");

            s[0] = "hoge";
            target.Count.Is(2);
            target[0].Value.Is("hoge");

            s.RemoveAt(0);
            target.Count.Is(1);
            target[0].Value.Is("bcd");

            s.Add("fuga");
            s.Add("homuhomu");
            s.Clear();

            target.Count.Is(0);
        }


        [TestMethod]
        public void IEnumerableSourceTest()
        {
            var s = new ObservableCollection<string>();
            var target = ((IEnumerable<string>)s).ToReadOnlyReactiveCollection(
                s.ToCollectionChanged(),
                x => new StringHolder { Value = x },
                Scheduler.CurrentThread);

            target.Count.Is(0);

            s.Add("abc");
            target.Count.Is(1);
            target[0].Value.Is("abc");

            s.Add("bcd");
            target.Count.Is(2);
            target[1].Value.Is("bcd");

            s[0] = "hoge";
            target.Count.Is(2);
            target[0].Value.Is("hoge");

            s.RemoveAt(0);
            target.Count.Is(1);
            target[0].Value.Is("bcd");

            s.Add("fuga");
            s.Add("homuhomu");
            s.Clear();

            target.Count.Is(0);
        }

        [TestMethod]
        public void DisposeTest()
        {
            var s = new ObservableCollection<string>();
            var disposed = new List<string>();
            var target = s.ToReadOnlyReactiveCollection(x => Disposable.Create(() => disposed.Add(x)), Scheduler.CurrentThread);

            s.Add("abc");
            s.Add("bcd");
            disposed.Count.Is(0);

            s[0] = "homuhomu";
            disposed[0].Is("abc");

            s.Clear();
            disposed.Is("abc", "homuhomu", "bcd");
        }

        [TestMethod]
        public void MoveTest()
        {
            var s = new MoveSupportCollection<string>();
            s.Add("a");
            s.Add("b");
            var target = s.ToReadOnlyReactiveCollection(s.ToCollectionChanged<string>(), Scheduler.CurrentThread);

            target.Is("a", "b");
            s.Move(1, 0);
            target.Is("b", "a");
        }

        [TestMethod]
        public void ConstructorCountTest()
        {
            int counter = 0;
            var collection = new ObservableCollection<string>();
            var collectionVm = collection.ToReadOnlyReactiveCollection(_ => new ConstructorCounter(ref counter), Scheduler.CurrentThread);

            counter.Is(0);
            collection.Add("Hello");
            counter.Is(1);
            collection.Add("world");
            counter.Is(2);
        }

        [TestMethod]
        public void ConstructorCountTestRemoveCase()
        {
            var counter = 0;
            var collection = new ObservableCollection<string>();
            var collectionVm = collection.ToReadOnlyReactiveCollection(_ => new ConstructorCounter(ref counter), Scheduler.CurrentThread);

            counter.Is(0);
            collection.Add("abc");
            counter.Is(1);
            collection.Remove("abc");
            counter.Is(1);
        }

        [TestMethod]
        public void InvokeDisposeTrue()
        {
            var c1 = new ObservableCollection<IDisposable>();
            bool invoked = false;
            c1.Add(Disposable.Create(() => invoked = true));
            var item = c1.ToReadOnlyReactiveCollection();
            c1.Clear();
            invoked.IsTrue();
        }

        [TestMethod]
        public void InvokeDisposeFalse()
        {
            var c1 = new ObservableCollection<IDisposable>();
            bool invoked = false;
            c1.Add(Disposable.Create(() => invoked = true));
            var item = c1.ToReadOnlyReactiveCollection(disposeElement: false);
            c1.Clear();
            invoked.IsFalse();
        }
    }

    class ConstructorCounter
    {
        public ConstructorCounter(ref int count)
        {
            count++;
        }
    }

    class StringHolder
    {
        public string Value { get; set; }
    }

    class MoveSupportCollection<T> : Collection<T>, INotifyCollectionChanged
    {
        public void Move(int oldIndex, int newIndex)
        {
            var item = this[oldIndex];
            this.RemoveAt(oldIndex);
            this.Insert(newIndex, item);
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var h = this.CollectionChanged;
            if (h != null) { h(this, e); }
        }
    }
}
