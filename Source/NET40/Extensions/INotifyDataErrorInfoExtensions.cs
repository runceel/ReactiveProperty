using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Codeplex.Reactive.Extensions;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
#endif

namespace Codeplex.Reactive.Extensions
{
#if SILVERLIGHT

    public static class INotifyDataErrorInfoExtensions
    {
        public static IObservable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable<T>(this T source)
            where T : INotifyDataErrorInfo
        {
            return ObservableEx.FromEvent<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.ErrorsChanged += h,
                h => source.ErrorsChanged -= h);
        }

        public static IObservable<TProperty> ObserveErrorInfo<TSubject, TProperty>(
            this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector)
            where TSubject : INotifyDataErrorInfo
        {
            string propertyName;
            var accessor = AccessorCache<TSubject>.Lookup(propertySelector, out propertyName);

            return subject.ErrorsChangedAsObservable()
                .Where(e => e.PropertyName == propertyName)
                .Select(_ => ((Func<TSubject, TProperty>)accessor).Invoke(subject));
        }
    }
#endif
}