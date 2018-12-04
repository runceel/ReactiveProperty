using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

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
            source = new Subject<int>();
            scheduler = new TestScheduler();
            target = source.ToReactiveCollection(scheduler);
        }

        [TestCleanup]
        public void Cleanup()
        {
            scheduler = null;
            target = null;
            source = null;
        }

        [TestMethod]
        public void Create()
        {
            target.Count.Is(0);
        }

        [TestMethod]
        public void Add()
        {
            target.Add(0);
            target.Add(1);
            target.Is(0, 1);
        }

        [TestMethod]
        public void AddSource()
        {
            source.OnNext(0);
            source.OnNext(1);
            scheduler.AdvanceTo(1000);
            target.Is(0, 1);
        }

        [TestMethod]
        public void Disconnect()
        {
            source.OnNext(0);
            source.OnNext(1);
            scheduler.AdvanceTo(1000);
            target.Is(0, 1);

            target.Dispose();

            source.OnNext(0);
            source.OnNext(1);
            scheduler.AdvanceTo(1000);
            target.Is(0, 1);
        }
    }
}
