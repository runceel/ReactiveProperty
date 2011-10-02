using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Diagnostics.Contracts;
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
        /// <summary>Converts ErrorsChanged to an observable sequence.</summary>
        public static IObservable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable<T>(this T subject)
            where T : INotifyDataErrorInfo
        {
            Contract.Requires<ArgumentNullException>(subject != null);
            Contract.Ensures(Contract.Result<IObservable<DataErrorsChangedEventArgs>>() != null);

            return ObservableEx.FromEvent<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
                h => (sender, e) => h(e),
                h => subject.ErrorsChanged += h,
                h => subject.ErrorsChanged -= h);
        }

        /// <summary>
        /// Converts target property's ErrorsChanged to an observable sequence.
        /// </summary>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        public static IObservable<TProperty> ObserveErrorInfo<TSubject, TProperty>(
            this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector)
            where TSubject : INotifyDataErrorInfo
        {
            Contract.Requires<ArgumentNullException>(subject != null);
            Contract.Requires<ArgumentNullException>(propertySelector != null);
            Contract.Ensures(Contract.Result<IObservable<TProperty>>() != null);

            string propertyName;
            var accessor = AccessorCache<TSubject>.Lookup(propertySelector, out propertyName);

            var result = subject.ErrorsChangedAsObservable()
                .Where(e => e.PropertyName == propertyName)
                .Select(_ => ((Func<TSubject, TProperty>)accessor).Invoke(subject));

            Contract.Assume(result != null);
            return result;
        }
    }
#endif
}