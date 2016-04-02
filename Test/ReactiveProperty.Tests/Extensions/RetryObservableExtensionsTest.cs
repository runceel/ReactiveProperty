using Reactive.Bindings.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Reactive.Bindings;
using Microsoft.Reactive.Testing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Net;

namespace ReactiveProperty.Tests
{
    [TestClass()]
    public class RetryObservableExtensionsTest : ReactiveTest
    {
        [TestMethod()]
        public void SameAsRetry_OnCompleted()
        {
            var scheduler = new TestScheduler();
            var source = scheduler.CreateHotObservable<int>(
                OnNext(0, 1),
                OnNext(0, 2),
                OnCompleted<int>(0),
                OnNext(0, 3));

            var called = 0;
            var retryRecorder = scheduler.CreateObserver<int>();
            source.Retry().Subscribe(retryRecorder);
            var onErrorRecorder = scheduler.CreateObserver<int>();
            source.OnErrorRetry((Exception e) => { ++called; }).Subscribe(onErrorRecorder);

            scheduler.Start();

            retryRecorder.Messages.Is(
                OnNext(1, 1),
                OnNext(1, 2),
                OnCompleted<int>(1));

            onErrorRecorder.Messages.Is(retryRecorder.Messages);
            called.Is(0);
        }

        [TestMethod()]
        public void SameAsRetry_OnError()
        {
            var scheduler = new TestScheduler();
            var ex = new Exception();
            var source = scheduler.CreateColdObservable<int>(
                OnNext(0, 1),
                OnNext(0, 2),
                OnError<int>(0, ex));

            var called = 0;
            var retryRecorder = scheduler.CreateObserver<int>();
            source.Retry().Take(5).Subscribe(retryRecorder);
            var onErrorRecorder = scheduler.CreateObserver<int>();
            source.OnErrorRetry((Exception e) => { ++called; }).Take(5).Subscribe(onErrorRecorder);

            scheduler.Start();

            retryRecorder.Messages.Is(
                OnNext(1, 1),
                OnNext(1, 2),
                OnNext(2, 1),
                OnNext(2, 2),
                OnNext(3, 1),
                OnCompleted<int>(3));

            var retryResult = retryRecorder.Messages.ToArray();

            onErrorRecorder.Messages.Is(retryRecorder.Messages);
            called.Is(2);
        }

        [TestMethod()]
        public void RetryCountAndDelay()
        {
            var scheduler = new TestScheduler();

            var ex = new Exception();
            var source = scheduler.CreateColdObservable<int>(
                OnNext(0, 1),
                OnNext(0, 2),
                OnError<int>(0, ex));

            var called = 0;
            var retryRecorder = scheduler.CreateObserver<int>();
            source.Retry(3).Subscribe(retryRecorder);
            var onErrorRecorder = scheduler.CreateObserver<int>();
            source.OnErrorRetry((Exception e) => { ++called; },
                    3, TimeSpan.FromSeconds(3), scheduler)
                .Subscribe(onErrorRecorder);

            scheduler.Start();

            retryRecorder.Messages.Is(
                OnNext(1, 1),
                OnNext(1, 2),
                // error
                OnNext(2, 1),
                OnNext(2, 2),
                // error
                OnNext(3, 1),
                OnNext(3, 2),
                OnError<int>(3, ex));

            onErrorRecorder.Messages.Is(
                OnNext(1, 1),
                OnNext(1, 2),
                // error
                OnNext(TimeSpan.FromSeconds(3).Ticks + 2, 1),
                OnNext(TimeSpan.FromSeconds(3).Ticks + 2, 2),
                // error
                OnNext(TimeSpan.FromSeconds(6).Ticks + 3, 1),
                OnNext(TimeSpan.FromSeconds(6).Ticks + 3, 2),
                OnError<int>(TimeSpan.FromSeconds(6).Ticks + 3, ex));

            called.Is(3);
        }
    }
}