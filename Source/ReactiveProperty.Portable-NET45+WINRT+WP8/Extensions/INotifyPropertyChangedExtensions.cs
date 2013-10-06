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
            return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
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
            string propertyName;
            var accessor = AccessorCache<TSubject>.LookupGet(propertySelector, out propertyName);
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
            return result;
        }

        /// <summary>
        /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
        /// <para>PropertyChanged raise on UIDispatcherScheduler.</para>
        /// </summary>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="mode">ReactiveProperty mode.</param>
        public static ReactiveProperty<TProperty> ToReactivePropertyAsSynchronized<TSubject, TProperty>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            where TSubject : INotifyPropertyChanged
        {
            return ToReactivePropertyAsSynchronized(subject, propertySelector, UIDispatcherScheduler.Default, mode);
        }

        /// <summary>
        /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler.</para>
        /// </summary>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="mode">ReactiveProperty mode.</param>
        public static ReactiveProperty<TProperty> ToReactivePropertyAsSynchronized<TSubject, TProperty>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            where TSubject : INotifyPropertyChanged
        {
            string propertyName; // no use
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out propertyName);

            var result = subject.ObserveProperty(propertySelector, isPushCurrentValueAtFirst: true)
                .ToReactiveProperty(raiseEventScheduler, mode: mode);
            result.Subscribe(x => setter(subject, x));

            return result;
        }

        /// <summary>
        /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
        /// <para>PropertyChanged raise on UIDispatcherScheduler.</para>
        /// </summary>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="convert">Convert selector to ReactiveProperty.</param>
        /// <param name="convertBack">Convert selector to source.</param>
        /// <param name="mode">ReactiveProperty mode.</param>
        public static ReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            Func<TProperty, TResult> convert,
            Func<TResult, TProperty> convertBack,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            where TSubject : INotifyPropertyChanged
        {
            return ToReactivePropertyAsSynchronized(subject, propertySelector, convert, convertBack, UIDispatcherScheduler.Default, mode);
        }

        /// <summary>
        /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler.</para>
        /// </summary>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="convert">Convert selector to ReactiveProperty.</param>
        /// <param name="convertBack">Convert selector to source.</param>
        /// <param name="mode">ReactiveProperty mode.</param>
        public static ReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            Func<TProperty, TResult> convert,
            Func<TResult, TProperty> convertBack,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            where TSubject : INotifyPropertyChanged
        {
            string propertyName; // no use
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out propertyName);

            var result = subject.ObserveProperty(propertySelector, isPushCurrentValueAtFirst: true)
                .Select(convert)
                .ToReactiveProperty(raiseEventScheduler, mode: mode);
            result.Select(convertBack).Subscribe(x => setter(subject, x));
            return result;
        }
    }
}