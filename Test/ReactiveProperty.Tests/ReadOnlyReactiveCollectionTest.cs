using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Subjects;
using System.Reactive;
using Codeplex.Reactive;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Reactive.Concurrency;

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

            s.OnNext(CollectionChanged<int>.Remove(0));
            target.Is(2);
            s.OnNext(CollectionChanged<int>.Add(1, 3));
            target.Is(2, 3);
            s.OnNext(CollectionChanged<int>.Replace(1, 100));
            target.Is(2, 100);
            s.OnNext(CollectionChanged<int>.Reset);
            target.Count.Is(0);
        }

    }
}
