using System;
using System.Reactive.Concurrency;
using Reactive.Bindings.Notifiers;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ScheduledNotifierTest : ReactiveTest
    {
        [TestMethod]
        public void Test()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<int>();

            var notifier = new ScheduledNotifier<int>(testScheduler);
            notifier.Subscribe(recorder);

            notifier.Report(1);
            notifier.Report(2);
            notifier.Report(3, TimeSpan.FromMinutes(10));
            notifier.Report(4, TimeSpan.FromMinutes(1));
            notifier.Report(5, TimeSpan.FromMinutes(5));
            notifier.Report(6);

            testScheduler.Start();

            recorder.Messages.Is(
                OnNext(1, 1),
                OnNext(1, 2),
                OnNext(1, 6),
                OnNext(TimeSpan.FromMinutes(1).Ticks, 4),
                OnNext(TimeSpan.FromMinutes(5).Ticks, 5),
                OnNext(TimeSpan.FromMinutes(10).Ticks, 3));
        }

        [TestMethod]
        public void TestOffset()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<int>();

            var notifier = new ScheduledNotifier<int>(testScheduler);
            notifier.Subscribe(recorder);

            var origin = new DateTimeOffset(1999, 1, 1, 1, 1, 1, TimeSpan.Zero);

            notifier.Report(1);
            notifier.Report(2);
            notifier.Report(3, origin);
            notifier.Report(4, origin.AddDays(10));
            notifier.Report(5, origin.AddYears(1));
            notifier.Report(6);

            testScheduler.Start();

            recorder.Messages.Is(
                OnNext(1, 1),
                OnNext(1, 2),
                OnNext(1, 6),
                OnNext(origin.Ticks, 3),
                OnNext(origin.AddDays(10).Ticks, 4),
                OnNext(origin.AddYears(1).Ticks, 5));
        }

        [TestMethod]
        public void CancelTest()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<int>();

            var notifier = new ScheduledNotifier<int>(testScheduler);
            notifier.Subscribe(recorder);

            var noCancel1 = notifier.Report(10, TimeSpan.FromMinutes(1));
            var cancel1 = notifier.Report(20, TimeSpan.FromMinutes(3));
            cancel1.Dispose();

            recorder.Messages.Count.Is(0);
            testScheduler.AdvanceBy(TimeSpan.FromMinutes(5).Ticks);
            recorder.Messages.Count.Is(1);
            recorder.Messages[0].Is(OnNext(TimeSpan.FromMinutes(1).Ticks, 10));

            var lastTime = recorder.Messages.Last().Time;
            recorder.Messages.Clear();

            var origin = new DateTimeOffset(1999, 1, 1, 1, 1, 1, TimeSpan.Zero);
            var noCancel2 = notifier.Report(30, origin.AddMinutes(1));
            var cancel2 = notifier.Report(40, origin.AddMinutes(3));
            cancel2.Dispose();

            testScheduler.AdvanceTo(origin.AddMinutes(5).Ticks);
            recorder.Messages.Is(
                OnNext(origin.AddMinutes(1).Ticks, 30));
        }

        [TestMethod]
        public void Constructor()
        {
            AssertEx.Throws<ArgumentNullException>(() => new ScheduledNotifier<int>(null));
        }

        [TestMethod]
        public void Subscribe()
        {
            var e = AssertEx.Throws<ArgumentNullException>(() =>
                new ScheduledNotifier<int>().Subscribe(null));
        }
    }
}