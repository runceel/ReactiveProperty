using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Diagnostics.Contracts;
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
#if !SL4

    public static class INotifyPropertyChangingExtensions
    {
        /// <summary>Converts PropertyChanging to an observable sequence.</summary>
        public static IObservable<PropertyChangingEventArgs> PropertyChangingAsObservable<T>(this T subject)
            where T : INotifyPropertyChanging
        {
            Contract.Requires<ArgumentNullException>(subject != null);
            Contract.Ensures(Contract.Result<IObservable<PropertyChangingEventArgs>>() != null);

            return ObservableEx.FromEvent<PropertyChangingEventHandler, PropertyChangingEventArgs>(
                h => (sender, e) => h(e),
                h => subject.PropertyChanging += h,
                h => subject.PropertyChanging -= h);
        }

        /// <summary>
        /// Converts NotificationObject's property changing to an observable sequence.
        /// </summary>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        public static IObservable<TProperty> ObservePropertyChanging<TSubject, TProperty>(
            this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector)
            where TSubject : INotifyPropertyChanging
        {
            Contract.Requires<ArgumentNullException>(subject != null);
            Contract.Requires<ArgumentNullException>(propertySelector != null);
            Contract.Ensures(Contract.Result<IObservable<TProperty>>() != null);

            string propertyName;
            var accessor = AccessorCache<TSubject>.Lookup(propertySelector, out propertyName);

            var result = subject.PropertyChangingAsObservable()
                .Where(e => e.PropertyName == propertyName)
                .Select(_ => ((Func<TSubject, TProperty>)accessor).Invoke(subject));

            Contract.Assume(result != null);
            return result;
        }
    }

#endif
}