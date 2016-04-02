using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;

namespace ReactiveProperty.Tests.Extensions
{
    [TestClass]
    public class ReactiveCollectionTest
    {
        private ReactiveCollection<int> target;
        private Subject<int> source;
        private TestScheduler scheduler;

        [TestInitialize]
        public void Initialize()
        {
            this.source = new Subject<int>();
            this.scheduler = new TestScheduler();
            this.target = this.source.ToReactiveCollection(scheduler);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.scheduler = null;
            this.target = null;
            this.source = null;
        }

        [TestMethod]
        public void Create()
        {
            this.target.Count.Is(0);
        }

        [TestMethod]
        public void Add()
        {
            this.target.Add(0);
            this.target.Add(1);
            this.target.Is(0, 1);
        }

        [TestMethod]
        public void AddSource()
        {
            this.source.OnNext(0);
            this.source.OnNext(1);
            this.scheduler.AdvanceTo(1000);
            this.target.Is(0, 1);
        }

        [TestMethod]
        public void Disconnect()
        {
            this.source.OnNext(0);
            this.source.OnNext(1);
            this.scheduler.AdvanceTo(1000);
            this.target.Is(0, 1);

            this.target.Dispose();

            this.source.OnNext(0);
            this.source.OnNext(1);
            this.scheduler.AdvanceTo(1000);
            this.target.Is(0, 1);
        }
    }
}
