using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeplex.Reactive;
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
        public void ReactiveCommandAllFlow()
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
        public void TimerTest()
        {
            // テスト用の自由に時間を動かせるスケジューラ
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<long>();

            // 作成時点では動き出さない
            var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), testScheduler);
            timer.Subscribe(recorder); // Subscribeしても動き出さない

            timer.Start(TimeSpan.FromSeconds(3)); // ここで開始。初期値を与えるとその時間後にスタート

            // 時間を絶対時間10秒のポイントまで進める(AdvanceTo)
            testScheduler.AdvanceTo(TimeSpan.FromSeconds(5).Ticks);

            // MessagesにSubscribeに届いた時間と値が記録されているので、Assertする
            recorder.Messages.Is(
                OnNext(TimeSpan.FromSeconds(3).Ticks, 0L),
                OnNext(TimeSpan.FromSeconds(4).Ticks, 1L),
                OnNext(TimeSpan.FromSeconds(5).Ticks, 2L));
            
            timer.Stop(); // timerを止める
            recorder.Messages.Clear(); // 記録をクリア

            // 時間を現在時間から5秒だけ進める(AdvanceBy)
            testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            // timerは止まっているので値は届いてないことが確認できる
            recorder.Messages.Count.Is(0);
        }
    }
}
