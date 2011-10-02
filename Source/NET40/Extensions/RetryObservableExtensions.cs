using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Codeplex.Reactive.Extensions
{
    // TODO:add Contract, add UnitTest

    public static class RetryObservableExtensions
    {
        public static IObservable<TSource> OnErrorRetry<TSource>(
            this IObservable<TSource> source)
        {
            return source.OnErrorRetry((Exception _) => { });
        }

        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError)
            where TException : Exception
        {
            return source.OnErrorRetry(onError, TimeSpan.Zero);
        }

        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, TimeSpan delay)
            where TException : Exception
        {
            return source.OnErrorRetry(onError, int.MaxValue, delay);
        }

        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount)
            where TException : Exception
        {
            return source.OnErrorRetry(onError, retryCount, TimeSpan.Zero);
        }

        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay)
            where TException : Exception
        {
            return source.OnErrorRetry(onError, retryCount, delay, Scheduler.ThreadPool);
        }

        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay, IScheduler delayScheduler)
            where TException : Exception
        {
            var dueTime = Scheduler.Normalize(delay);
            var empty = Observable.Empty<TSource>();
            var count = 0;

            return source.Catch((TException ex) =>
            {
                onError(ex);

                return (++count < retryCount)
                    ? (dueTime == TimeSpan.Zero) ? empty : empty.Delay(dueTime, delayScheduler)
                    : Observable.Throw<TSource>(ex);
            }).Repeat();
        }
    }
}
