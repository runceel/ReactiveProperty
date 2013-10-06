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
    public static class CatchIgnoreObservableExtensions
    {
        /// <summary>Catch exception and return Observable.Empty.</summary>
        public static IObservable<TSource> CatchIgnore<TSource>(
            this IObservable<TSource> source)
        {
            var result = source.Catch(Observable.Empty<TSource>());
            return result;
        }

        /// <summary>Catch exception and return Observable.Empty.</summary>
        public static IObservable<TSource> CatchIgnore<TSource, TException>(
            this IObservable<TSource> source, Action<TException> errorAction)
            where TException : Exception
        {
            var result = source.Catch((TException ex) =>
            {
                errorAction(ex);
                return Observable.Empty<TSource>();
            });
            return result;
        }
    }
}