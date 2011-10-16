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
    public static class INotifyPropertyChangedExtensions
    {
        /// <summary>Converts PropertyChanged to an observable sequence.</summary>
        public static IObservable<PropertyChangedEventArgs> PropertyChangedAsObservable<T>(this T subject)
            where T : INotifyPropertyChanged
        {
            Contract.Requires<ArgumentNullException>(subject != null);
            Contract.Ensures(Contract.Result<IObservable<PropertyChangedEventArgs>>() != null);

            return ObservableEx.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => (sender, e) => h(e),
                h => subject.PropertyChanged += h,
                h => subject.PropertyChanged -= h);
        }

        /// <summary>
        /// Converts NotificationObject's property changed to an observable sequence.
        /// </summary>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="isPushCurrentValueOnSubscribe">Push current value on first subscribe.</param>
        public static IObservable<TProperty> ObserveProperty<TSubject, TProperty>(
            this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector,
            bool isPushCurrentValueAtFirst = true)
            where TSubject : INotifyPropertyChanged
        {
            Contract.Requires<ArgumentNullException>(subject != null);
            Contract.Requires<ArgumentNullException>(propertySelector != null);
            Contract.Ensures(Contract.Result<IObservable<TProperty>>() != null);

            string propertyName;
            var accessor = AccessorCache<TSubject>.Lookup(propertySelector, out propertyName);
            var isFirst = true;

            var result = Observable.Defer(() =>
            {
                var flag = isFirst;
                isFirst = false;

                var q = subject.PropertyChangedAsObservable()
                    .Where(e => e.PropertyName == propertyName)
                    .Select(_ => accessor.Invoke(subject));
                return (isPushCurrentValueAtFirst && flag) ? q.StartWith(accessor.Invoke(subject)) : q;
            });

            Contract.Assume(result != null);
            return result;
        }
    }
}