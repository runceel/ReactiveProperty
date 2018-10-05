using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// INotify Data Error Info Extensions
    /// </summary>
    public static class INotifyDataErrorInfoExtensions
    {
        /// <summary>
        /// Converts ErrorsChanged to an observable sequence.
        /// </summary>
        public static IObservable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable<T>(this T subject)
            where T : INotifyDataErrorInfo =>
            Observable.FromEvent<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
                h => (sender, e) => h(e),
                h => subject.ErrorsChanged += h,
                h => subject.ErrorsChanged -= h);

        /// <summary>
        /// Converts target property's ErrorsChanged to an observable sequence.
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <returns></returns>
        public static IObservable<TProperty> ObserveErrorInfo<TSubject, TProperty>(
            this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector)
            where TSubject : INotifyDataErrorInfo
        {
            var accessor = AccessorCache<TSubject>.LookupGet(propertySelector, out var propertyName);

            var result = subject.ErrorsChangedAsObservable()
                .Where(e => e.PropertyName == propertyName)
                .Select(_ => accessor.Invoke(subject));

            return result;
        }
    }
}
