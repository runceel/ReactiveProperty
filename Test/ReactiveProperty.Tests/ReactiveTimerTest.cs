using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using System.Reactive.Subjects;
using System.Threading;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReactiveTimerTest : ReactiveTest
    {
        [TestMethod]
        public void TimerTest1()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<long>();
            var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), testScheduler);
            timer.Subscribe(recorder);

            timer.Start(TimeSpan.FromSeconds(10));
            testScheduler.AdvanceTo(TimeSpan.FromSeconds(13).Ticks);
            recorder.Messages.Is(
                OnNext(TimeSpan.FromSeconds(10).Ticks, 0L),
                OnNext(TimeSpan.FromSeconds(11).Ticks, 1L),
                OnNext(TimeSpan.FromSeconds(12).Ticks, 2L),
                OnNext(TimeSpan.FromSeconds(13).Ticks, 3L));
            recorder.Messages.Clear();

            timer.Stop();
            testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);
            recorder.Messages.Count.Is(0);

            timer.Start();
            var reStartClock = testScheduler.Clock + 1;
            testScheduler.AdvanceBy(TimeSpan.FromSeconds(3).Ticks + 1);
            recorder.Messages.Is(
                OnNext(reStartClock, 4L),
                OnNext(reStartClock + TimeSpan.FromSeconds(1).Ticks, 5L),
                OnNext(reStartClock + TimeSpan.FromSeconds(2).Ticks, 6L),
                OnNext(reStartClock + TimeSpan.FromSeconds(3).Ticks, 7L));
            recorder.Messages.Clear();

            timer.Reset();
            testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);
            recorder.Messages.Count.Is(0);

            timer.Dispose();

            recorder.Messages.Is(OnCompleted<long>(testScheduler.Clock));

            timer.Dispose(); // dispose again
        }

        [TestMethod]
        public void TimerTestStart2()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<long>();

            var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), testScheduler);
            timer.Subscribe(recorder);

            timer.Start();

            testScheduler.AdvanceTo(TimeSpan.FromSeconds(3).Ticks + 1);

            recorder.Messages.Is(
                OnNext(TimeSpan.FromSeconds(0).Ticks + 1, 0L),
                OnNext(TimeSpan.FromSeconds(1).Ticks + 1, 1L),
                OnNext(TimeSpan.FromSeconds(2).Ticks + 1, 2L),
                OnNext(TimeSpan.FromSeconds(3).Ticks + 1, 3L));
            
            timer.Stop();
            recorder.Messages.Clear();

            testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            recorder.Messages.Count.Is(0);
        }

        [TestMethod]
        public void IsEnabledTest()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<long>();

            var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), testScheduler);
            var isEnabledChangedCounter = 0;
            timer.ObserveProperty(x => x.IsEnabled, false).Subscribe(_ => isEnabledChangedCounter++);
            timer.IsEnabled.IsFalse();
            timer.Start();
            timer.IsEnabled.IsTrue();
            timer.Stop();
            timer.IsEnabled.IsFalse();
            isEnabledChangedCounter.Is(2);
        }

        [TestMethod]
        public void IsEnabledMultipleStartCaseTest()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<long>();

            var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), testScheduler);
            var isEnabledChangedCounter = 0;
            timer.ObserveProperty(x => x.IsEnabled, false).Subscribe(_ => isEnabledChangedCounter++);
            timer.IsEnabled.IsFalse();
            timer.Start(); // start
            timer.IsEnabled.IsTrue();
            timer.Start(); // stop and start
            timer.IsEnabled.IsTrue();
            timer.Stop(); // stop
            timer.IsEnabled.IsFalse();
            isEnabledChangedCounter.Is(4);
        }
    }
}
