using System;
using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
#endif

namespace Codeplex.Reactive.Extensions
{
    public static class RetryObservableExtensions
    {
        /// <summary>
        /// <para>Repeats the source observable sequence until it successfully terminates.</para>
        /// <para>This is same as Retry().</para>
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource>(
            this IObservable<TSource> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);
            
            return source.OnErrorRetry((Exception _) => { });
        }

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError)
            where TException : Exception
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(onError != null);
            Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

            return source.OnErrorRetry(onError, TimeSpan.Zero);
        }

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence after delay time.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, TimeSpan delay)
            where TException : Exception
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(onError != null);
            Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

            return source.OnErrorRetry(onError, int.MaxValue, delay);
        }

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence during within retryCount.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount)
            where TException : Exception
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(onError != null);
            Contract.Requires<ArgumentException>(retryCount > 0);
            Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

            return source.OnErrorRetry(onError, retryCount, TimeSpan.Zero);
        }

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence after delay time during within retryCount.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay)
            where TException : Exception
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(onError != null);
            Contract.Requires<ArgumentException>(retryCount > 0);
            Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

            Contract.Assume(Scheduler.ThreadPool != null);
            return source.OnErrorRetry(onError, retryCount, delay, Scheduler.ThreadPool);
        }

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence after delay time(work on delayScheduler) during within retryCount.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay, IScheduler delayScheduler)
            where TException : Exception
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(onError != null);
            Contract.Requires<ArgumentException>(retryCount > 0);
            Contract.Requires<ArgumentNullException>(delayScheduler != null);
            Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

            var dueTime = (delay.Ticks < 0) ? TimeSpan.Zero : delay;
            var empty = Observable.Empty<TSource>();
            var count = 0;

            var result = source.Catch((TException ex) =>
            {
                onError(ex);

                return (++count < retryCount)
                    ? (dueTime == TimeSpan.Zero) ? empty : empty.Delay(dueTime, delayScheduler)
                    : Observable.Throw<TSource>(ex);
            }).Repeat();

            Contract.Assume(result != null);
            return result;
        }
    }
}
