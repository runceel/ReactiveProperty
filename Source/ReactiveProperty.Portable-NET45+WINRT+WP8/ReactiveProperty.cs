using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Reactive.Bindings.Extensions;

namespace Reactive.Bindings
{
    internal class SingletonPropertyChangedEventArgs
    {
        public static readonly PropertyChangedEventArgs Value = new PropertyChangedEventArgs(nameof(ReactiveProperty<object>.Value));
    }
    internal class SingletonDataErrorsChangedEventArgs
    {
        public static readonly DataErrorsChangedEventArgs Value = new DataErrorsChangedEventArgs(nameof(ReactiveProperty<object>.Value));
    }

    [Flags]
    public enum ReactivePropertyMode
    {
        None = 0x00,
        /// <summary>If next value is same as current, not set and not notify.</summary>
        DistinctUntilChanged = 0x01,
        /// <summary>Push notify on instance created and subscribed.</summary>
        RaiseLatestValueOnSubscribe = 0x02
    }

    /// <summary>
    /// Two-way bindable IObserable&lt;T&gt;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReactiveProperty<T> : IReactiveProperty<T>, IReadOnlyReactiveProperty<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private T LatestValue { get; set; }
        private bool IsDisposed { get; set; } = false;
        private IScheduler RaiseEventScheduler { get; }
        private IObservable<T> Source { get; }
        private Subject<T> AnotherTrigger { get; } = new Subject<T>();
        private ISubject<T> ValidationTrigger { get; }
        private IDisposable SourceDisposable { get; }
        private IDisposable RaiseSubscription { get; }

        // for Validation
        private bool IsValueChanged { get; set; } = false;
        private SerialDisposable ValidateNotifyErrorSubscription { get; } = new SerialDisposable();
        private BehaviorSubject<IEnumerable> ErrorsTrigger;
        private List<Func<IObservable<T>, IObservable<IEnumerable>>> ValidatorStore { get; } = new List<Func<IObservable<T>, IObservable<IEnumerable>>>();

        /// <summary>PropertyChanged raise on UIDispatcherScheduler</summary>
        public ReactiveProperty()
            : this(default(T), ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
        }

        /// <summary>PropertyChanged raise on UIDispatcherScheduler</summary>
        public ReactiveProperty(
            T initialValue = default(T), 
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            : this(UIDispatcherScheduler.Default, initialValue, mode)
        { }

        /// <summary>PropertyChanged raise on selected scheduler</summary>
        public ReactiveProperty(
            IScheduler raiseEventScheduler, 
            T initialValue = default(T), 
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            : this(Observable.Never<T>(), raiseEventScheduler, initialValue, mode)
        {
        }

        // ToReactiveProperty Only
        internal ReactiveProperty(
            IObservable<T> source, 
            T initialValue = default(T), 
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            : this(source, UIDispatcherScheduler.Default, initialValue, mode)
        {
        }

        internal ReactiveProperty(
            IObservable<T> source, 
            IScheduler raiseEventScheduler, 
            T initialValue = default(T), 
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
            this.LatestValue = initialValue;
            this.RaiseEventScheduler = raiseEventScheduler;

            // create source
            var merge = source.Merge(AnotherTrigger);
            if (mode.HasFlag(ReactivePropertyMode.DistinctUntilChanged)) merge = merge.DistinctUntilChanged();
            merge = merge.Do(x =>
            {
                // setvalue immediately
                if (!IsValueChanged) IsValueChanged = true;
                LatestValue = x;
            });

            // publish observable
            var connectable = (mode.HasFlag(ReactivePropertyMode.RaiseLatestValueOnSubscribe))
                ? merge.Publish(initialValue)
                : merge.Publish();
            this.Source = connectable.AsObservable();

            this.ValidationTrigger = mode.HasFlag(ReactivePropertyMode.RaiseLatestValueOnSubscribe)
                ? (ISubject<T>)new BehaviorSubject<T>(initialValue)
                : (ISubject<T>)new Subject<T>();
            connectable.Subscribe(x => ValidationTrigger.OnNext(x));

            // raise notification
            this.RaiseSubscription = connectable
                .ObserveOn(raiseEventScheduler)
                .Subscribe(x => this.PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value));

            // start source
            this.SourceDisposable = connectable.Connect();
            this.ErrorsTrigger = new BehaviorSubject<IEnumerable>(this.GetErrors(null));
        }

        /// <summary>
        /// Get latestValue or push(set) value.
        /// </summary>
        public T Value
        {
            get { return LatestValue; }
            set { AnotherTrigger.OnNext(value); }
        }

        object IReactiveProperty.Value
        {
            get { return (T)Value; }
            set { Value = (T)value; }
        }

        object IReadOnlyReactiveProperty.Value => Value;

        /// <summary>
        /// Subscribe source.
        /// </summary>
        public IDisposable Subscribe(IObserver<T> observer) => Source.Subscribe(observer);

        /// <summary>
        /// Unsubcribe all subscription.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            AnotherTrigger.Dispose();
            RaiseSubscription.Dispose();
            SourceDisposable.Dispose();
            ((IDisposable)ValidationTrigger).Dispose();
            ValidateNotifyErrorSubscription.Dispose();
            ErrorsTrigger.OnCompleted();
            ErrorsTrigger.Dispose();
        }

        public override string ToString() =>
            (LatestValue == null)
                ? "null"
                : "{" + LatestValue.GetType().Name + ":" + LatestValue.ToString() + "}";

        // Validations

        /// <summary>
        /// <para>Checked validation, raised value. If success return value is null.</para>
        /// </summary>
        public IObservable<IEnumerable> ObserveErrorChanged => ErrorsTrigger.AsObservable(); 

        // INotifyDataErrorInfo
        private IEnumerable CurrentErrors { get; set; }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// <para>Set INotifyDataErrorInfo's asynchronous validation, return value is self.</para>
        /// </summary>
        /// <param name="validator">If success return IO&lt;null&gt;, failure return IO&lt;IEnumerable&gt;(Errors).</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<T>, IObservable<IEnumerable>> validator)
        {
            this.ValidatorStore.Add(validator);     //--- cache validation functions
            var validators  = this.ValidatorStore
                            .Select(x => x(this.ValidationTrigger))
                            .ToArray();     //--- use copy
            this.ValidateNotifyErrorSubscription.Disposable
                = Observable.CombineLatest(validators)
                .Select(xs =>
                {
                    if (xs.Count == 0)          return null;
                    if (xs.All(x => x == null)) return null;

                    var strings = xs
                                .OfType<string>()
                                .Where(x => x != null);
                    var others  = xs
                                .Where(x => !(x is string))
                                .Where(x => x != null)
                                .SelectMany(x => x.Cast<object>());
                    return strings.Concat(others);
                })
                .Subscribe(x =>
                {
                    this.CurrentErrors = x;
                    var handler = this.ErrorsChanged;
                    if (handler != null)
                        this.RaiseEventScheduler.Schedule(() => handler(this, SingletonDataErrorsChangedEventArgs.Value));
                    this.ErrorsTrigger.OnNext(x);
                });
            return this;
        }

        /// <summary>
        /// <para>Set INotifyDataErrorInfo's asynchronous validation, return value is self.</para>
        /// </summary>
        /// <param name="validator">If success return IO&lt;null&gt;, failure return IO&lt;IEnumerable&gt;(Errors).</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<T>, IObservable<string>> validator) =>
            this.SetValidateNotifyError(xs => validator(xs).Cast<IEnumerable>());

        /// <summary>
        /// Set INotifyDataErrorInfo's asynchronous validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, Task<IEnumerable>> validator) =>
            this.SetValidateNotifyError(xs => xs.SelectMany(x => validator(x)));

        /// <summary>
        /// Set INotifyDataErrorInfo's asynchronous validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, Task<string>> validator) =>
            this.SetValidateNotifyError(xs => xs.SelectMany(x => validator(x)));

        /// <summary>
        /// Set INofityDataErrorInfo validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, IEnumerable> validator) =>
            this.SetValidateNotifyError(xs => xs.Select(x => validator(x)));

        /// <summary>
        /// Set INofityDataErrorInfo validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, string> validator) =>
            this.SetValidateNotifyError(xs => xs.Select(x => validator(x)));

        /// <summary>Get INotifyDataErrorInfo's error store</summary>
        public System.Collections.IEnumerable GetErrors(string propertyName) => CurrentErrors;

        /// <summary>Get INotifyDataErrorInfo's error store</summary>
        public bool HasErrors => CurrentErrors != null; 

        /// <summary>
        /// Observe HasErrors value.
        /// </summary>
        public IObservable<bool> ObserveHasErrors => this.ObserveErrorChanged.Select(_ => this.HasErrors);
    }

    /// <summary>
    /// Static methods and extension methods of ReactiveProperty&lt;T&gt;
    /// </summary>
    public static class ReactiveProperty
    {
        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -> Object) synchronized.</para>
        /// <para>PropertyChanged raise on UIDispatcherScheduler</para>
        /// </summary>
        public static ReactiveProperty<TProperty> FromObject<TTarget, TProperty>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false) =>
            FromObject(target, propertySelector, UIDispatcherScheduler.Default, mode, ignoreValidationErrorValue);

        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -> Object) synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler</para>
        /// </summary>
        public static ReactiveProperty<TProperty> FromObject<TTarget, TProperty>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false)
        {
            string propertyName; // no use
            var getter = AccessorCache<TTarget>.LookupGet(propertySelector, out propertyName);
            var setter = AccessorCache<TTarget>.LookupSet(propertySelector, out propertyName);

            var result = new ReactiveProperty<TProperty>(raiseEventScheduler, initialValue: getter(target), mode: mode);
            result
                .Where(_ => !ignoreValidationErrorValue || !result.HasErrors)
                .Subscribe(x => setter(target, x));

            return result;
        }

        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -> Object) synchronized.</para>
        /// <para>PropertyChanged raise on UIDispatcherScheduler</para>
        /// </summary>
        public static ReactiveProperty<TResult> FromObject<TTarget, TProperty, TResult>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            Func<TProperty, TResult> convert,
            Func<TResult, TProperty> convertBack,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false) =>
            FromObject(target, propertySelector, convert, convertBack, UIDispatcherScheduler.Default, mode, ignoreValidationErrorValue);

        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -> Object) synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler</para>
        /// </summary>
        public static ReactiveProperty<TResult> FromObject<TTarget, TProperty, TResult>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            Func<TProperty, TResult> convert,
            Func<TResult, TProperty> convertBack,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false)
        {
            string propertyName; // no use
            var getter = AccessorCache<TTarget>.LookupGet(propertySelector, out propertyName);
            var setter = AccessorCache<TTarget>.LookupSet(propertySelector, out propertyName);

            var result = new ReactiveProperty<TResult>(raiseEventScheduler, initialValue: convert(getter(target)), mode: mode);
            result
                .Where(_ => !ignoreValidationErrorValue || !result.HasErrors)
                .Select(convertBack)
                .Subscribe(x => setter(target, x));

            return result;
        }


        /// <summary>
        /// <para>Convert to two-way bindable IObservable&lt;T&gt;</para>
        /// <para>PropertyChanged raise on UIDispatcherScheduler</para>
        /// </summary>
        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe) =>
            new ReactiveProperty<T>(source, initialValue, mode);

        /// <summary>
        /// <para>Convert to two-way bindable IObservable&lt;T&gt;</para>
        /// <para>PropertyChanged raise on selected scheduler</para>
        /// </summary>
        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            IScheduler raiseEventScheduler,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged|ReactivePropertyMode.RaiseLatestValueOnSubscribe) =>
            new ReactiveProperty<T>(source, raiseEventScheduler, initialValue, mode);
    }
}