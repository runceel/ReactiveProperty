using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Reactive.Bindings.Internals;
using System.Xml;

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
            var isFirst = true;
            if (AccessorCache.IsNotNestedPropertyPath(propertySelector))
            {
                var accessor = AccessorCache<TSubject>.LookupGet(propertySelector, out var propertyName);

                var result = Observable.Defer(() =>
                {
                    var flag = isFirst;
                    isFirst = false;

                    var q = subject.PropertyChangedAsObservable()
                        .Where(e => e.PropertyName == propertyName || string.IsNullOrEmpty(e.PropertyName))
                        .Select(_ => accessor.Invoke(subject));
                    return (isPushCurrentValueAtFirst && flag) ? q.StartWith(accessor.Invoke(subject)) : q;
                });
                return result;
            }
            else
            {
                var accessor = AccessorCache<TSubject>.LookupNestedGet(propertySelector, out var propertyName);
                return Observable.Create<Unit>(ox =>
                {
                    if (!(propertySelector.Body is MemberExpression current))
                    {
                        throw new ArgumentException();
                    }

                    var topNode = default(PropertyPathNode);
                    while(current != null)
                    {
                        var propertyName = current.Member.Name;
                        var node = default(PropertyPathNode);
                        if (topNode != null)
                        {
                            topNode = topNode.InsertBefore(propertyName);
                        }
                        else
                        {
                            topNode = new PropertyPathNode(propertyName, () => ox.OnNext(Unit.Default));
                        }
                        current = current.Expression as MemberExpression;
                    }

                    topNode?.UpdateTarget(subject);
                    if (isPushCurrentValueAtFirst)
                    {
                        ox.OnNext(Unit.Default);
                    }

                    return Disposable.Create(() =>
                    {
                        topNode?.Dispose();
                    });
                }).Select(_ =>
                {
                    try
                    {
                        return accessor(subject);
                    }
                    catch
                    {
                        // ignore exception
                        return default(TProperty);
                    }
                });
            }
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
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out _);

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
            where TSubject : INotifyPropertyChanged => 
            ToReactivePropertyAsSynchronized(subject, propertySelector,
                ox => ox.Select(convert),
                ox => ox.Select(convertBack),
                raiseEventScheduler,
                mode,
                ignoreValidationErrorValue);

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
            Func<IObservable<TProperty>, IObservable<TResult>> convert,
            Func<IObservable<TResult>, IObservable<TProperty>> convertBack,
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
            Func<IObservable<TProperty>, IObservable<TResult>> convert,
            Func<IObservable<TResult>, IObservable<TProperty>> convertBack,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false)
            where TSubject : INotifyPropertyChanged
        {
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out _);
            var result = convert(subject.ObserveProperty(propertySelector, isPushCurrentValueAtFirst: true))
                .ToReactiveProperty(raiseEventScheduler, mode: mode);
            convertBack(result.Where(_ => !ignoreValidationErrorValue || !result.HasErrors))
                .Subscribe(x => setter(subject, x));
            return result;
        }

        /// <summary>
        /// <para>Converts NotificationObject's property to ReactivePropertySlim. Value is two-way synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler.</para>
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="mode">ReactiveProperty mode.</param>
        /// <returns></returns>
        public static ReactivePropertySlim<TProperty> ToReactivePropertySlimAsSynchronized<TSubject, TProperty>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            where TSubject : INotifyPropertyChanged
        {
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out _);
            var result = new ReactivePropertySlim<TProperty>(mode: mode);
            var disposable = subject.ObserveProperty(propertySelector, isPushCurrentValueAtFirst: true)
                .Subscribe(x => result.Value = x);
            result.Subscribe(x => setter(subject, x), _ => disposable.Dispose(), () => disposable.Dispose());
            return result;
        }

        /// <summary>
        /// <para>Converts NotificationObject's property to ReactivePropertySlim. Value is two-way synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler.</para>
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <param name="propertySelector">Argument is self, Return is target property.</param>
        /// <param name="convert">Convert selector to ReactiveProperty.</param>
        /// <param name="convertBack">Convert selector to source.</param>
        /// <param name="mode">ReactiveProperty mode.</param>
        /// <returns></returns>
        public static ReactivePropertySlim<TResult> ToReactivePropertySlimAsSynchronized<TSubject, TProperty, TResult>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            Func<TProperty, TResult> convert,
            Func<TResult, TProperty> convertBack,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            where TSubject : INotifyPropertyChanged =>
            ToReactivePropertySlimAsSynchronized(subject, propertySelector,
                ox => ox.Select(convert),
                ox => ox.Select(convertBack),
                mode);

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
        /// <param name="mode">ReactiveProperty mode.</param>
        /// <returns></returns>
        public static ReactivePropertySlim<TResult> ToReactivePropertySlimAsSynchronized<TSubject, TProperty, TResult>(
            this TSubject subject,
            Expression<Func<TSubject, TProperty>> propertySelector,
            Func<IObservable<TProperty>, IObservable<TResult>> convert,
            Func<IObservable<TResult>, IObservable<TProperty>> convertBack,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            where TSubject : INotifyPropertyChanged
        {
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out _);
            var result = new ReactivePropertySlim<TResult>(mode: mode);
            IDisposable disposable = null;
            disposable = convert(subject.ObserveProperty(propertySelector, isPushCurrentValueAtFirst: true))
                .Subscribe(x => result.Value = x);
            convertBack(result)
                .Subscribe(x => setter(subject, x), _ => disposable.Dispose(), () => disposable.Dispose());
            return result;
        }
    }
}
