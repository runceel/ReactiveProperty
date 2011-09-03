using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
#endif

namespace Codeplex.Reactive.Asynchronous
{
    internal static class ObservableEx
    {
#if WINDOWS_PHONE
        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, int count)
        {
            return source.BufferWithCount(count);
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

        public static IObservable<TResult> SafeFromAsyncPattern<TResult>(Func<AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
            where TResult : IDisposable
        {
            return ObservableEx.Create<TResult>(observer =>
            {
                var disposable = new BooleanDisposable();

                Observable.FromAsyncPattern<TResult>(begin, ar =>
                {
                    var result = end(ar);
                    if (disposable.IsDisposed) result.Dispose();
                    return result;
                })().Subscribe(observer);

                return disposable;
            });
        }
    }
}