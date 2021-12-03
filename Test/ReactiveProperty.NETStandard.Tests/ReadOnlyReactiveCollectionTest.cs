using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

namespace ReactiveProperty.Tests;

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
    public void AddRangeTest()
    {
        var source = new RangedObservableCollection<int>();
        var target = source.ToReadOnlyReactiveCollection(x => $"{x}");
        source.Add(1);
        target.Is("1");
        source.AddRange(new[] { 2, 3, 4 });
        target.Is("1", "2", "3", "4");
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
        var s = new MoveSupportCollection<string>
            {
                "a",
                "b"
            };
        var target = s.ToReadOnlyReactiveCollection(s.ToCollectionChanged<string>(), Scheduler.CurrentThread);

        target.Is("a", "b");
        s.Move(1, 0);
        target.Is("b", "a");
    }

    [TestMethod]
    public void ConstructorCountTest()
    {
        var counter = 0;
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
        var invoked = false;
        c1.Add(Disposable.Create(() => invoked = true));
        var item = c1.ToReadOnlyReactiveCollection();
        c1.Clear();
        invoked.IsTrue();
    }

    [TestMethod]
    public void InvokeDisposeFalse()
    {
        var c1 = new ObservableCollection<IDisposable>();
        var invoked = false;
        c1.Add(Disposable.Create(() => invoked = true));
        var item = c1.ToReadOnlyReactiveCollection(disposeElement: false);
        c1.Clear();
        invoked.IsFalse();
    }

    [TestMethod]
    public void MoveCollectionChangedEventTest()
    {
        var records = new List<NotifyCollectionChangedEventArgs>();
        var source = new ObservableCollection<string>();
        var rc = source.ToReadOnlyReactiveCollection();
        ((INotifyCollectionChanged)rc).CollectionChanged += (s, e) => records.Add(e);
        source.Add("a");
        source.Add("b");
        source.Move(0, 1);

        source.Is("b", "a");
        rc.Is("b", "a");

        records.Select(x => x.Action).Is(
            NotifyCollectionChangedAction.Add,
            NotifyCollectionChangedAction.Add,
            NotifyCollectionChangedAction.Move);
    }

    [TestMethod]
    public void ToCollectionChangedTest()
    {
        var source = new ObservableCollection<string>();
        var target = source.ToReadOnlyReactiveCollection();
        var values = new List<CollectionChanged<string>>();
        target.ToCollectionChanged().Subscribe(x => values.Add(x));

        source.Add("abc");
        source.Add("def");
        source.Remove("abc");
        source.Clear();
        values.Is(
            new[]
            {
                    CollectionChanged<string>.Add(0, "abc"),
                    CollectionChanged<string>.Add(1, "def"),
                    CollectionChanged<string>.Remove(0, "abc"),
                    CollectionChanged<string>.Reset,
            },
            (x, y) => x.Action == y.Action && x.Value == y.Value);
    }

    [TestMethod]
    public void ClearAndAddItems()
    {
        var testScheduler = new TestScheduler();
        var source = new ObservableCollection<string>();
        var target = source.ToReadOnlyReactiveCollection(
            x => new StringHolder { Value = x }, 
            testScheduler);

        source.Clear();
        source.Add("a");
        source.Add("b");
        testScheduler.AdvanceBy(1);
        testScheduler.AdvanceBy(1);
        testScheduler.AdvanceBy(1);
        target.Is(
            new StringHolder { Value = "a" },
            new StringHolder {  Value = "b" });

        source.Clear();
        source.Add("b");
        source.Add("c");

        testScheduler.AdvanceBy(1);
        testScheduler.AdvanceBy(1);
        testScheduler.AdvanceBy(1);
        target.Is(
            new StringHolder { Value = "b" },
            new StringHolder { Value = "c" });
    }
}

internal class ConstructorCounter
{
    public ConstructorCounter(ref int count)
    {
        count++;
    }
}

internal class StringHolder
{
    public string Value { get; set; }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is not StringHolder x)
        {
            return false;
        }

        return Value == x.Value;

        
    }
}

internal class MoveSupportCollection<T> : Collection<T>, INotifyCollectionChanged
{
    public void Move(int oldIndex, int newIndex)
    {
        var item = this[oldIndex];
        RemoveAt(oldIndex);
        Insert(newIndex, item);
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        var h = CollectionChanged;
        if (h != null) { h(this, e); }
    }
}
