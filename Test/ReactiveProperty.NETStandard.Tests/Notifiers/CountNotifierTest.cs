using System;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Notifiers;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class CountNotifierTest : ReactiveTest
    {
        [TestMethod]
        public void Test()
        {
            var notifier = new CountNotifier(10);
            var recorder = new TestScheduler().CreateObserver<CountChangedStatus>();
            notifier.Subscribe(recorder);

            notifier.Max.Is(10);
            notifier.Count.Is(0);

            notifier.Increment(3);
            notifier.Increment(3);
            notifier.Increment(3);
            notifier.Increment();
            notifier.Decrement(5);
            notifier.Decrement(5);

            recorder.Messages.Is(
                OnNext(0, CountChangedStatus.Increment),
                OnNext(0, CountChangedStatus.Increment),
                OnNext(0, CountChangedStatus.Increment),
                OnNext(0, CountChangedStatus.Increment),
                OnNext(0, CountChangedStatus.Max),
                OnNext(0, CountChangedStatus.Decrement),
                OnNext(0, CountChangedStatus.Decrement),
                OnNext(0, CountChangedStatus.Empty));

            // over max
            notifier.Increment(10);
            notifier.Count.Is(10);
            recorder.Messages.Clear();
            notifier.Increment();
            notifier.Count.Is(10);
            recorder.Messages.Count.Is(0);

            // under zero
            notifier.Decrement(10);
            notifier.Count.Is(0);
            recorder.Messages.Clear();
            notifier.Decrement();
            notifier.Count.Is(0);
            recorder.Messages.Count.Is(0);

            // over inc
            recorder.Messages.Clear();
            notifier.Increment(5);
            notifier.Increment(15);
            notifier.Count.Is(10);
            recorder.Messages.Is(
                OnNext(0, CountChangedStatus.Increment),
                OnNext(0, CountChangedStatus.Increment),
                OnNext(0, CountChangedStatus.Max));

            // over dec
            recorder.Messages.Clear();
            notifier.Decrement(5);
            notifier.Decrement(15);
            notifier.Count.Is(0);
            recorder.Messages.Is(
                OnNext(0, CountChangedStatus.Decrement),
                OnNext(0, CountChangedStatus.Decrement),
                OnNext(0, CountChangedStatus.Empty));

            AssertEx.Throws<ArgumentException>(() =>
                notifier.Increment(-1));
            AssertEx.Throws<ArgumentException>(() =>
                notifier.Decrement(-1));
            AssertEx.Throws<ArgumentException>(() =>
                new CountNotifier(0));
            AssertEx.DoesNotThrow(() =>
                new CountNotifier(1));
        }
    }
}
