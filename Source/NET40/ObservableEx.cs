using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Threading;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
#endif

namespace Codeplex.Reactive
{
    /// <summary>
    /// Compatibility for Rx-Main with Phone.Reactive
    /// </summary>
    internal static class ObservableEx
    {
#if WINDOWS_PHONE
        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, int count)
        {
            return source.BufferWithCount(count);
        }

        public static void Dispose<T>(this ISubject<T> subject)
        {
            // do nothing.
        }

        public static IDisposable Schedule(this IScheduler scheduler, TimeSpan dueTime, Action action)
        {
            return scheduler.Schedule(action, dueTime);
        }

        public static IDisposable Schedule(this IScheduler scheduler, DateTimeOffset dueTime, Action action)
        {
            return scheduler.Schedule(action, dueTime);
        }
#endif

        public static IObservable<TSource> Create<TSource>(Func<IObserver<TSource>, IDisposable> subscribe)
        {
#if WINDOWS_PHONE
            return Observable.CreateWithDisposable(subscribe);
#else
            return Observable.Create(subscribe);
#endif
        }

        public static IObservable<TEventArgs> FromEvent<TDelegate, TEventArgs>(Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
        {
#if WINDOWS_PHONE
            return Observable.CreateWithDisposable<TEventArgs>(observer =>
            {
                var handler = conversion(observer.OnNext);
                addHandler(handler);
                return Disposable.Create(() => removeHandler(handler));
            });
#else
            return Observable.FromEvent(conversion, addHandler, removeHandler);
#endif
        }
    }

    internal static class EnumerableEx
    {
        public static IEnumerable<T> Defer<T>(Func<IEnumerable<T>> enumerableFactory)
        {
            foreach (var item in enumerableFactory())
            {
                yield return item;
            }
        }

        public static IEnumerable<TSource[]> Buffer<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count <= 0) throw new ArgumentOutOfRangeException("count");

            var list = new List<TSource>(count);
            foreach (var item in source)
            {
                list.Add(item);
                if (list.Count >= count)
                {
                    yield return list.ToArray();
                    list.Clear();
                }
            }

            if (list.Count > 0) yield return list.ToArray();
        }
    }
}