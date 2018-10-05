using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// INotify Property Changed Extensions
    /// </summary>
    public static class INotifyPropertyChangedExtensions
    {
        /// <summary>
        /// Converts PropertyChanged to an observable sequence.
        /// </summary>
        public static IObservable<PropertyChangedEventArgs> PropertyChangedAsObservable<T>(this T subject)
            where T : INotifyPropertyChanged =>
            Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => (sender, e) => h(e),
                h => subject.PropertyChanged += h,
                h => subject.PropertyChanged -= h);

        /// <summary>
        /// Converts NotificationObject's property changed to an observable sequence.
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="isPushCurrentValueAtFirst">Push current value on first subscribe.</param>
        /// <returns></returns>
        public static IObservable<TProperty> ObserveProperty<TSubject, TProperty>(
            this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector,
            bool isPushCurrentValueAtFirst = true)
            where TSubject : INotifyPropertyChanged
        {
            var accessor = AccessorCache<TSubject>.LookupGet(propertySelector, out var propertyName);
            var isFirst = true;

            var result = Observable.Defer(() => {
                var flag = isFirst;
                isFirst = false;

                var q = subject.PropertyChangedAsObservable()
                    .Where(e => e.PropertyName == propertyName)
                    .Select(_ => accessor.Invoke(subject));
                return (isPushCurrentValueAtFirst && flag) ? q.StartWith(accessor.Invoke(subject)) : q;
            });
            return result;
        }

        /// <summary>
        /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
        /// <para>PropertyChanged raise on ReactivePropertyScheduler.</para>
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="mode">ReactiveProperty mode.</param>
        /// <param name="ignoreValidationErrorValue">Ignore validation error value.</param>
        /// <returns></returns>
        public static ReactiveProperty<TProperty> ToReactivePropertyAsSynchronized<TSubject, TProperty>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false)
            where TSubject : INotifyPropertyChanged =>
            ToReactivePropertyAsSynchronized(subject, propertySelector, ReactivePropertyScheduler.Default, mode, ignoreValidationErrorValue);

        /// <summary>
        /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler.</para>
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="raiseEventScheduler">The raise event scheduler.</param>
        /// <param name="mode">ReactiveProperty mode.</param>
        /// <param name="ignoreValidationErrorValue">Ignore validation error value.</param>
        /// <returns></returns>
        public static ReactiveProperty<TProperty> ToReactivePropertyAsSynchronized<TSubject, TProperty>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false)
            where TSubject : INotifyPropertyChanged
        {
            // no use
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out var propertyName);

            var result = subject.ObserveProperty(propertySelector, isPushCurrentValueAtFirst: true)
                .ToReactiveProperty(raiseEventScheduler, mode: mode);
            result
                .Where(_ => !ignoreValidationErrorValue || !result.HasErrors)
                .Subscribe(x => setter(subject, x));

            return result;
        }

        /// <summary>
        /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
        /// <para>PropertyChanged raise on ReactivePropertyScheduler.</para>
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="convert">Convert selector to ReactiveProperty.</param>
        /// <param name="convertBack">Convert selector to source.</param>
        /// <param name="mode">ReactiveProperty mode.</param>
        /// <param name="ignoreValidationErrorValue">Ignore validation error value.</param>
        /// <returns></returns>
        public static ReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            Func<TProperty, TResult> convert,
            Func<TResult, TProperty> convertBack,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false)
            where TSubject : INotifyPropertyChanged =>
            ToReactivePropertyAsSynchronized(subject, propertySelector, convert, convertBack, ReactivePropertyScheduler.Default, mode, ignoreValidationErrorValue);

        /// <summary>
        /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler.</para>
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="convert">Convert selector to ReactiveProperty.</param>
        /// <param name="convertBack">Convert selector to source.</param>
        /// <param name="raiseEventScheduler">The raise event scheduler.</param>
        /// <param name="mode">ReactiveProperty mode.</param>
        /// <param name="ignoreValidationErrorValue">Ignore validation error value.</param>
        /// <returns></returns>
        public static ReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            Func<TProperty, TResult> convert,
            Func<TResult, TProperty> convertBack,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false)
            where TSubject : INotifyPropertyChanged
        {
            // no use
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out var propertyName);

            var result = subject.ObserveProperty(propertySelector, isPushCurrentValueAtFirst: true)
                .Select(convert)
                .ToReactiveProperty(raiseEventScheduler, mode: mode);
            result
                .Where(_ => !ignoreValidationErrorValue || !result.HasErrors)
                .Select(convertBack)
                .Subscribe(x => setter(subject, x));

            return result;
        }
    }
}
