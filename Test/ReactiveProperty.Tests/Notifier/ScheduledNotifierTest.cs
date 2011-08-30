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

            notifier.Raise(1);
            notifier.Raise(2);
            notifier.Raise(3, TimeSpan.FromMinutes(10));
            notifier.Raise(4, TimeSpan.FromMinutes(1));
            notifier.Raise(5, TimeSpan.FromMinutes(5));
            notifier.Raise(6);

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
        public void Constructor()
        {
            var e = AssertEx.Throws<ArgumentNullException>(() => new ScheduledNotifier<int>(null));
            e.ParamName.Is("scheduler");
        }

        [TestMethod]
        public void Subscribe()
        {
            AssertEx.Throws<ArgumentNullException>(() => 
                new ScheduledNotifier<int>(Scheduler.CurrentThread).Subscribe(null));
        }
    }
}