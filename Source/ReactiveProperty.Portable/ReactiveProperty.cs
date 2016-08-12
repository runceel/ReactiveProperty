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
        private bool IsDistinctUntilChanged { get; }
        private bool IsRaiseLatestValueOnSubscribe { get; }

        private Subject<T> Source { get; } = new Subject<T>();
        private IDisposable SourceDisposable { get; }
        private bool IsValueChanging { get; set; } = false;

        // for Validation
        private Subject<T> ValidationTrigger { get; } = new Subject<T>();
        private SerialDisposable ValidateNotifyErrorSubscription { get; } = new SerialDisposable();
        private Lazy<BehaviorSubject<IEnumerable>> ErrorsTrigger { get; }
        private Lazy<List<Func<IObservable<T>, IObservable<IEnumerable>>>> ValidatorStore { get; } = new Lazy<List<Func<IObservable<T>, IObservable<IEnumerable>>>>(() => new List<Func<IObservable<T>, IObservable<IEnumerable>>>());

        /// <summary>PropertyChanged raise on ReactivePropertyScheduler</summary>
        public ReactiveProperty()
            : this(default(T), ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
        }

        /// <summary>PropertyChanged raise on ReactivePropertyScheduler</summary>
        public ReactiveProperty(
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            : this(ReactivePropertyScheduler.Default, initialValue, mode)
        { }

        /// <summary>PropertyChanged raise on selected scheduler</summary>
        public ReactiveProperty(
            IScheduler raiseEventScheduler,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        {
            this.RaiseEventScheduler = raiseEventScheduler;
            this.LatestValue = initialValue;

            this.IsRaiseLatestValueOnSubscribe = mode.HasFlag(ReactivePropertyMode.RaiseLatestValueOnSubscribe);
            this.IsDistinctUntilChanged = mode.HasFlag(ReactivePropertyMode.DistinctUntilChanged);

            this.SourceDisposable = Disposable.Empty;
            this.ErrorsTrigger = new Lazy<BehaviorSubject<IEnumerable>>(() => new BehaviorSubject<IEnumerable>(this.GetErrors(null)));
        }

        // ToReactiveProperty Only
        internal ReactiveProperty(
            IObservable<T> source,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            : this(source, ReactivePropertyScheduler.Default, initialValue, mode)
        {
        }

        internal ReactiveProperty(
            IObservable<T> source,
            IScheduler raiseEventScheduler,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            : this(raiseEventScheduler, initialValue, mode)
        {
            this.SourceDisposable = source.Subscribe(x => this.Value = x);
        }

        /// <summary>
        /// Get latestValue or push(set) value.
        /// </summary>
        public T Value
        {
            get { return LatestValue; }
            set
            {
                if (this.LatestValue == null || value == null)
                {
                    // null case
                    if (this.IsDistinctUntilChanged && this.LatestValue == null && value == null)
                    {
                        return;
                    }

                    this.SetValue(value);
                    return;
                }

                if (this.IsDistinctUntilChanged && (EqualityComparer<T>.Default.Equals(this.LatestValue, value)))
                {
                    return;
                }

                this.SetValue(value);
            }
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
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (this.IsRaiseLatestValueOnSubscribe)
            {
                observer.OnNext(this.LatestValue);
                return this.Source.Subscribe(observer);
            }
            else
            {
                return this.Source.Subscribe(observer);
            }
        }

        /// <summary>
        /// Unsubcribe all subscription.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            this.Source.OnCompleted();
            this.Source.Dispose();
            this.ValidationTrigger.Dispose();
            SourceDisposable.Dispose();
            ValidateNotifyErrorSubscription.Dispose();
            if (ErrorsTrigger.IsValueCreated)
            {
                ErrorsTrigger.Value.OnCompleted();
                ErrorsTrigger.Value.Dispose();
            }
        }

        public override string ToString() =>
            (LatestValue == null)
                ? "null"
                : "{" + LatestValue.GetType().Name + ":" + LatestValue.ToString() + "}";

        // Validations

        /// <summary>
        /// <para>Checked validation, raised value. If success return value is null.</para>
        /// </summary>
        public IObservable<IEnumerable> ObserveErrorChanged => ErrorsTrigger.Value.AsObservable();

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
            this.ValidatorStore.Value.Add(validator);     //--- cache validation functions
            var validators = this.ValidatorStore.Value
                            .Select(x => x(this.ValidationTrigger.StartWith(this.LatestValue)))
                            .ToArray();     //--- use copy
            this.ValidateNotifyErrorSubscription.Disposable
                = Observable.CombineLatest(validators)
                .Select(xs =>
                {
                    if (xs.Count == 0) return null;
                    if (xs.All(x => x == null)) return null;

                    var strings = xs
                                .OfType<string>()
                                .Where(x => x != null);
                    var others = xs
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
                    this.ErrorsTrigger.Value.OnNext(x);
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
        /// Invoke validation process.
        /// </summary>
        public void ForceValidate() => this.ValidationTrigger.OnNext(this.LatestValue);

        /// <summary>
        /// Invoke OnNext.
        /// </summary>
        public void ForceNotify() => this.SetValue(this.LatestValue);

        /// <summary>
        /// Observe HasErrors value.
        /// </summary>
        public IObservable<bool> ObserveHasErrors => this.ObserveErrorChanged.Select(_ => this.HasErrors);

        private void SetValue(T value)
        {
            this.LatestValue = value;
            this.ValidationTrigger.OnNext(value);
            this.Source.OnNext(value);
            this.RaiseEventScheduler.Schedule(() => this.PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value));
        }
    }

    /// <summary>
    /// Static methods and extension methods of ReactiveProperty&lt;T&gt;
    /// </summary>
    public static class ReactiveProperty
    {
        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -> Object) synchronized.</para>
        /// <para>PropertyChanged raise on ReactivePropertyScheduler</para>
        /// </summary>
        public static ReactiveProperty<TProperty> FromObject<TTarget, TProperty>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false) =>
            FromObject(target, propertySelector, ReactivePropertyScheduler.Default, mode, ignoreValidationErrorValue);

        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -> Object) synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler</para>
        /// </summary>
        public static ReactiveProperty<TProperty> FromObject<TTarget, TProperty>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
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
        /// <para>PropertyChanged raise on ReactivePropertyScheduler</para>
        /// </summary>
        public static ReactiveProperty<TResult> FromObject<TTarget, TProperty, TResult>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            Func<TProperty, TResult> convert,
            Func<TResult, TProperty> convertBack,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false) =>
            FromObject(target, propertySelector, convert, convertBack, ReactivePropertyScheduler.Default, mode, ignoreValidationErrorValue);

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
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
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
        /// <para>PropertyChanged raise on ReactivePropertyScheduler</para>
        /// </summary>
        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe) =>
            new ReactiveProperty<T>(source, initialValue, mode);

        /// <summary>
        /// <para>Convert to two-way bindable IObservable&lt;T&gt;</para>
        /// <para>PropertyChanged raise on selected scheduler</para>
        /// </summary>
        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            IScheduler raiseEventScheduler,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe) =>
            new ReactiveProperty<T>(source, raiseEventScheduler, initialValue, mode);
    }
}