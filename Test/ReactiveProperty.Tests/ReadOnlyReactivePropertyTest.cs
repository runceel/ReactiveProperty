using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReadOnlyReactivePropertyTest
    {
        [TestMethod]
        public void NormalPattern()
        {
            var s = new Subject<string>();

            var rp = s.ToReadOnlyReactiveProperty(eventScheduler: Scheduler.CurrentThread);
            var buffer = new List<string>();
            rp.Subscribe(buffer.Add);

            rp.Value.IsNull();
            buffer.Count.Is(1);
            buffer[0].IsNull();

            s.OnNext("Hello");
            rp.Value.Is("Hello");
            buffer.Count.Is(2);
            buffer.Is(default(string), "Hello");

            s.OnNext("Hello");
            rp.Value.Is("Hello");
            buffer.Count.Is(2); // distinct until changed.
        }

        [TestMethod]
        public void MultiSubscribeTest()
        {
            var s = new Subject<string>();

            var rp = s.ToReadOnlyReactiveProperty();
            var buffer1 = new List<string>();
            rp.Subscribe(buffer1.Add);


            buffer1.Count.Is(1);
            s.OnNext("Hello world");
            buffer1.Count.Is(2);
            buffer1.Is(default(string), "Hello world");

            var buffer2 = new List<string>();
            rp.Subscribe(buffer2.Add);
            buffer1.Is(default(string), "Hello world");
            buffer2.Is("Hello world");

            s.OnNext("ReactiveProperty");
            buffer1.Is(default(string), "Hello world", "ReactiveProperty");
            buffer2.Is("Hello world", "ReactiveProperty");
        }

        [TestMethod]
        public void NormalPatternNoDistinctUntilChanged()
        {
            var s = new Subject<string>();

            var rp = s.ToReadOnlyReactiveProperty(
                mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe,
                eventScheduler: Scheduler.CurrentThread);
            var buffer = new List<string>();
            rp.Subscribe(buffer.Add);

            rp.Value.IsNull();
            buffer.Count.Is(1);
            buffer[0].IsNull();

            s.OnNext("Hello");
            rp.Value.Is("Hello");
            buffer.Count.Is(2);
            buffer.Is(default(string), "Hello");

            s.OnNext("Hello");
            rp.Value.Is("Hello");
            buffer.Count.Is(3); // not distinct until changed.
        }

        [TestMethod]
        public void PropertyChangedTest()
        {
            var s = new Subject<string>();
            var rp = s.ToReadOnlyReactiveProperty(eventScheduler: Scheduler.CurrentThread);
            var buffer = new List<string>();
            rp.PropertyChanged += (_, args) =>
            {
                buffer.Add(args.PropertyName);
            };

            buffer.Count.Is(0);

            s.OnNext("Hello");
            buffer.Count.Is(1);

            s.OnNext("Hello");
            buffer.Count.Is(1);

            s.OnNext("World");
            buffer.Count.Is(2);
        }

        [TestMethod]
        public void PropertyChangedNoDistinctUntilChangedTest()
        {
            var s = new Subject<string>();
            var rp = s.ToReadOnlyReactiveProperty(
                mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe,
                eventScheduler: Scheduler.CurrentThread);
            var buffer = new List<string>();
            rp.PropertyChanged += (_, args) =>
            {
                buffer.Add(args.PropertyName);
            };

            buffer.Count.Is(0);

            s.OnNext("Hello");
            buffer.Count.Is(1);

            s.OnNext("Hello");
            buffer.Count.Is(2);

            s.OnNext("World");
            buffer.Count.Is(3);
        }

        class IgnoreCaseComparer : EqualityComparer<string>
        {
            public override bool Equals(string x, string y)
                => x?.ToLower() == y?.ToLower();

            public override int GetHashCode(string obj)
                => (obj?.ToLower()).GetHashCode();
        }

        [TestMethod]
        public void CustomEqualityComparerToReactivePropertyCase()
        {
            var source = new Subject<string>();
            var rp = source.ToReadOnlyReactiveProperty(equalityComparer: new IgnoreCaseComparer());
            var list = new List<string>();
            rp.Subscribe(list.Add);
            source.OnNext("Hello world");
            source.OnNext("HELLO WORLD");
            source.OnNext("Hello japan");
            list.Is(null, "Hello world", "Hello japan");
        }


        [TestMethod]
        public void BehaviorSubjectTest()
        {
            var s = new BehaviorSubject<string>("initial value");
            var rp = s.ToReadOnlyReactiveProperty();
            rp.Value.Is("initial value");
        }

        [TestMethod]
        public void ObservableCreateTest()
        {
            var i = 0;
            var s = Observable.Create<int>(ox =>
            {
                i++;
                return Disposable.Empty;
            });

            i.Is(0);
            var rp = s.ToReadOnlyReactiveProperty();
            i.Is(1);
        }
    }
}
