using System;
using System.Reactive.Linq;
using Codeplex.Reactive.Notifier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class SignalNotifierTest : ReactiveTest
    {
        [TestMethod]
        public void Test()
        {
            var notifier = new SignalNotifier(10);
            var recorder = new TestScheduler().CreateObserver<SignalChangedStatus>();
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
                OnNext(0, SignalChangedStatus.Increment),
                OnNext(0, SignalChangedStatus.Increment),
                OnNext(0, SignalChangedStatus.Increment),
                OnNext(0, SignalChangedStatus.Increment),
                OnNext(0, SignalChangedStatus.Max),
                OnNext(0, SignalChangedStatus.Decrement),
                OnNext(0, SignalChangedStatus.Decrement),
                OnNext(0, SignalChangedStatus.Empty));

            notifier.Increment(10);
            notifier.Count.Is(10);
            AssertEx.Throws<InvalidOperationException>(() =>
                notifier.Increment());

            notifier.Decrement(10);
            notifier.Count.Is(0);
            AssertEx.Throws<InvalidOperationException>(() =>
                notifier.Decrement());

            notifier.Increment(5);
            notifier.Count.Is(5);
            AssertEx.Throws<ArgumentException>(() =>
                notifier.Increment(-1));
            AssertEx.Throws<ArgumentException>(() =>
                notifier.Decrement(-1));

            AssertEx.Throws<ArgumentException>(() =>
                new SignalNotifier(0));
            AssertEx.DoesNotThrow(() =>
                new SignalNotifier(1));
        }
    }
}