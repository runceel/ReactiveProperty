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
using System.Threading;
using System.Threading.Tasks;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Internals;

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

    /// <summary>
    /// Mode of ReactiveProperty
    /// </summary>
    [Flags]
    public enum ReactivePropertyMode
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0x00,

        /// <summary>
        /// If next value is same as current, not set and not notify.
        /// </summary>
        DistinctUntilChanged = 0x01,

        /// <summary>
        /// Push notify on instance created and subscribed.
        /// </summary>
        RaiseLatestValueOnSubscribe = 0x02,

        /// <summary>
        /// Ignore initial validation error
        /// </summary>
        IgnoreInitialValidationError = 0x04,

        /// <summary>
        /// Default mode value. It is same as DistinctUntilChanged | RaiseLatestValueOnSubscribe.
        /// </summary>
        Default = DistinctUntilChanged | RaiseLatestValueOnSubscribe,
    }

    /// <summary>
    /// Two-way bindable IObserable&lt;T&gt;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReactiveProperty<T> : IReactiveProperty<T>, IReadOnlyReactiveProperty<T>
    {
        /// <summary>
        /// Implements of INotifyPropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private T LatestValue { get; set; }

        private bool IsDisposed { get; set; } = false;

        /// <summary>
        /// Gets the raise event scheduler.
        /// </summary>
        /// <value>The raise event scheduler.</value>
        public IScheduler RaiseEventScheduler { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is distinct until changed.
        /// </summary>
        /// <value><c>true</c> if this instance is distinct until changed; otherwise, <c>false</c>.</value>
        public bool IsDistinctUntilChanged { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is raise latest value on subscribe.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is raise latest value on subscribe; otherwise, <c>false</c>.
        /// </value>
        public bool IsRaiseLatestValueOnSubscribe { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is ignore initial validation error.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is ignore initial validation error; otherwise, <c>false</c>.
        /// </value>
        public bool IsIgnoreInitialValidationError { get; }

        private readonly IEqualityComparer<T> equalityComparer;

        private Subject<T> Source { get; } = new Subject<T>();

        private IDisposable SourceDisposable { get; }

        private bool IsValueChanging { get; set; } = false;

        // for Validation
        private Subject<T> ValidationTrigger { get; } = new Subject<T>();

        private SerialDisposable ValidateNotifyErrorSubscription { get; } = new SerialDisposable();

        private Lazy<BehaviorSubject<IEnumerable>> ErrorsTrigger { get; }

        private Lazy<List<Func<IObservable<T>, IObservable<IEnumerable>>>> ValidatorStore { get; } = new Lazy<List<Func<IObservable<T>, IObservable<IEnumerable>>>>(() => new List<Func<IObservable<T>, IObservable<IEnumerable>>>());

        /// <summary>
        /// PropertyChanged raise on ReactivePropertyScheduler
        /// </summary>
        public ReactiveProperty()
            : this(default(T), ReactivePropertyMode.Default)
        {
        }

        /// <summary>
        /// PropertyChanged raise on ReactivePropertyScheduler
        /// </summary>
        public ReactiveProperty(
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.Default,
            IEqualityComparer<T> equalityComparer = null)
            : this(ReactivePropertyScheduler.Default, initialValue, mode, equalityComparer)
        { }

        /// <summary>
        /// PropertyChanged raise on selected scheduler
        /// </summary>
        public ReactiveProperty(
            IScheduler raiseEventScheduler,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IEqualityComparer<T> equalityComparer = null)
        {
            RaiseEventScheduler = raiseEventScheduler;
            LatestValue = initialValue;
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

            IsRaiseLatestValueOnSubscribe = (mode & ReactivePropertyMode.RaiseLatestValueOnSubscribe) == ReactivePropertyMode.RaiseLatestValueOnSubscribe;
            IsDistinctUntilChanged = (mode & ReactivePropertyMode.DistinctUntilChanged) == ReactivePropertyMode.DistinctUntilChanged;
            IsIgnoreInitialValidationError = (mode & ReactivePropertyMode.IgnoreInitialValidationError) == ReactivePropertyMode.IgnoreInitialValidationError;

            SourceDisposable = Disposable.Empty;
            ErrorsTrigger = new Lazy<BehaviorSubject<IEnumerable>>(() => new BehaviorSubject<IEnumerable>(GetErrors(null)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        public ReactiveProperty(
            IObservable<T> source,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IEqualityComparer<T> equalityComparer = null)
            : this(source, ReactivePropertyScheduler.Default, initialValue, mode, equalityComparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="raiseEventScheduler">The raise event scheduler.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        public ReactiveProperty(
            IObservable<T> source,
            IScheduler raiseEventScheduler,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IEqualityComparer<T> equalityComparer = null)
            : this(raiseEventScheduler, initialValue, mode, equalityComparer)
        {
            SourceDisposable = source.Subscribe(x => Value = x);
        }

        /// <summary>
        /// Get latestValue or push(set) value.
        /// </summary>
        public T Value
        {
            get
            {
                return LatestValue;
            }

            set
            {
                if (IsDistinctUntilChanged && equalityComparer.Equals(LatestValue, value)) {
                    return;
                }

                SetValue(value);
            }
        }

        object IReactiveProperty.Value
        {
            get { return Value; }
            set { Value = (T)value; }
        }

        object IReadOnlyReactiveProperty.Value => Value;

        /// <summary>
        /// Subscribe source.
        /// </summary>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (IsRaiseLatestValueOnSubscribe) {
                observer.OnNext(LatestValue);
                return Source.Subscribe(observer);
            } else {
                return Source.Subscribe(observer);
            }
        }

        /// <summary>
        /// Unsubcribe all subscription.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) {
                return;
            }

            IsDisposed = true;
            Source.OnCompleted();
            Source.Dispose();
            ValidationTrigger.Dispose();
            SourceDisposable.Dispose();
            ValidateNotifyErrorSubscription.Dispose();
            if (ErrorsTrigger.IsValueCreated) {
                ErrorsTrigger.Value.OnCompleted();
                ErrorsTrigger.Value.Dispose();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString() =>
            (LatestValue == null)
                ? "null"
                : "{" + LatestValue.GetType().Name + ":" + LatestValue.ToString() + "}";

        /// <summary>
        /// <para>Checked validation, raised value. If success return value is null.</para>
        /// </summary>
        public IObservable<IEnumerable> ObserveErrorChanged => ErrorsTrigger.Value.AsObservable();

        private IEnumerable CurrentErrors { get; set; }

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        /// <returns></returns>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// <para>Set INotifyDataErrorInfo's asynchronous validation, return value is self.</para>
        /// </summary>
        /// <param name="validator">If success return IO&lt;null&gt;, failure return IO&lt;IEnumerable&gt;(Errors).</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<T>, IObservable<IEnumerable>> validator)
        {
            ValidatorStore.Value.Add(validator);     //--- cache validation functions
            var validators = ValidatorStore.Value
                            .Select(x => x(IsIgnoreInitialValidationError ? ValidationTrigger : ValidationTrigger.StartWith(LatestValue)))
                            .ToArray();     //--- use copy
            ValidateNotifyErrorSubscription.Disposable
                = Observable.CombineLatest(validators)
                .Select(xs => {
                    if (xs.Count == 0) {
                        return null;
                    }

                    if (xs.All(x => x == null)) {
                        return null;
                    }

                    var strings = xs
                                .OfType<string>()
                                .Where(x => x != null);
                    var others = xs
                                .Where(x => !(x is string))
                                .Where(x => x != null)
                                .SelectMany(x => x.Cast<object>());
                    return strings.Concat(others);
                })
                .Subscribe(x => {
                    CurrentErrors = x;
                    var handler = ErrorsChanged;
                    if (handler != null) {
                        RaiseEventScheduler.Schedule(() => handler(this, SingletonDataErrorsChangedEventArgs.Value));
                    }

                    ErrorsTrigger.Value.OnNext(x);
                });
            return this;
        }

        /// <summary>
        /// <para>Set INotifyDataErrorInfo's asynchronous validation, return value is self.</para>
        /// </summary>
        /// <param name="validator">If success return IO&lt;null&gt;, failure return IO&lt;IEnumerable&gt;(Errors).</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<T>, IObservable<string>> validator) =>
            SetValidateNotifyError(xs => validator(xs).Cast<IEnumerable>());

        /// <summary>
        /// Set INotifyDataErrorInfo's asynchronous validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, Task<IEnumerable>> validator) =>
            SetValidateNotifyError(xs => xs.SelectMany(x => validator(x)));

        /// <summary>
        /// Set INotifyDataErrorInfo's asynchronous validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, Task<string>> validator) =>
            SetValidateNotifyError(xs => xs.SelectMany(x => validator(x)));

        /// <summary>
        /// Set INofityDataErrorInfo validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, IEnumerable> validator) =>
            SetValidateNotifyError(xs => xs.Select(x => validator(x)));

        /// <summary>
        /// Set INofityDataErrorInfo validation.
        /// </summary>
        /// <param name="validator">Validation logic</param>
        /// <returns>Self.</returns>
        public ReactiveProperty<T> SetValidateNotifyError(Func<T, string> validator) =>
            SetValidateNotifyError(xs => xs.Select(x => validator(x)));

        /// <summary>
        /// Get INotifyDataErrorInfo's error store
        /// </summary>
        public System.Collections.IEnumerable GetErrors(string propertyName) => CurrentErrors;

        /// <summary>
        /// Get INotifyDataErrorInfo's error store
        /// </summary>
        public bool HasErrors => CurrentErrors != null;

        /// <summary>
        /// Invoke validation process.
        /// </summary>
        public void ForceValidate() => ValidationTrigger.OnNext(LatestValue);

        /// <summary>
        /// Invoke OnNext.
        /// </summary>
        public void ForceNotify() => SetValue(LatestValue);

        /// <summary>
        /// Observe HasErrors value.
        /// </summary>
        public IObservable<bool> ObserveHasErrors => ObserveErrorChanged.Select(_ => HasErrors);

        private void SetValue(T value)
        {
            LatestValue = value;
            ValidationTrigger.OnNext(value);
            Source.OnNext(value);
            RaiseEventScheduler.Schedule(() => PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value));
        }
    }

    /// <summary>
    /// Static methods and extension methods of ReactiveProperty&lt;T&gt;
    /// </summary>
    public static class ReactiveProperty
    {
        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -&gt; Object) synchronized.</para>
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
        /// <para>Value is OneWayToSource(ReactiveProperty -&gt; Object) synchronized.</para>
        /// <para>PropertyChanged raise on selected scheduler</para>
        /// </summary>
        public static ReactiveProperty<TProperty> FromObject<TTarget, TProperty>(
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            IScheduler raiseEventScheduler,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            bool ignoreValidationErrorValue = false)
        {
            // no use
            var getter = AccessorCache<TTarget>.LookupGet(propertySelector, out var propertyName);
            var setter = AccessorCache<TTarget>.LookupSet(propertySelector, out propertyName);

            var result = new ReactiveProperty<TProperty>(raiseEventScheduler, initialValue: getter(target), mode: mode);
            result
                .Where(_ => !ignoreValidationErrorValue || !result.HasErrors)
                .Subscribe(x => setter(target, x));

            return result;
        }

        /// <summary>
        /// <para>Convert plain object to ReactiveProperty.</para>
        /// <para>Value is OneWayToSource(ReactiveProperty -&gt; Object) synchronized.</para>
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
        /// <para>Value is OneWayToSource(ReactiveProperty -&gt; Object) synchronized.</para>
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
            // no use
            var getter = AccessorCache<TTarget>.LookupGet(propertySelector, out var propertyName);
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
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe, IEqualityComparer<T> equalityComparer = null) =>
            new ReactiveProperty<T>(source, initialValue, mode, equalityComparer);

        /// <summary>
        /// <para>Convert to two-way bindable IObservable&lt;T&gt;</para>
        /// <para>PropertyChanged raise on selected scheduler</para>
        /// </summary>
        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            IScheduler raiseEventScheduler,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IEqualityComparer<T> equalityComparer = null) =>
            new ReactiveProperty<T>(source, raiseEventScheduler, initialValue, mode, equalityComparer);
    }
}
