using System;
using System.Reactive.Concurrency;
using Codeplex.Reactive.Notifier;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var origin = new DateTimeOffset();

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
                OnNext(1, 3),
                OnNext(1, 6),
                OnNext(origin.AddDays(10).Ticks, 4),
                OnNext(origin.AddYears(1).Ticks, 5));
        }

        [TestMethod]
        public void Constructor()
        {
            AssertEx.Throws<ArgumentNullException>(() => new ScheduledNotifier<int>(null));
        }

        [TestMethod]
        public void Subscribe()
        {
            AssertEx.Throws<ArgumentNullException>(() =>
                new ScheduledNotifier<int>().Subscribe(null));
        }
    }
}