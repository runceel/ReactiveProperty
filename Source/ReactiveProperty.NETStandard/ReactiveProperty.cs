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
using Reactive.Bindings.Internals;

namespace Reactive.Bindings;

/// <summary>
/// Two-way bindable IObservable&lt;T&gt;
/// </summary>
/// <typeparam name="T"></typeparam>
public class ReactiveProperty<T> : IReactiveProperty<T>, IObserverLinkedList<T>
{
    private const int IsDisposedFlagNumber = 1 << 9; // (reserve 0 ~ 8)

    /// <summary>
    /// Implements of INotifyPropertyChanged.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    private T LatestValue { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
    public bool IsDisposed => (int)_mode == IsDisposedFlagNumber;

    /// <summary>
    /// Gets the raise event scheduler.
    /// </summary>
    /// <value>The raise event scheduler.</value>
    public IScheduler RaiseEventScheduler { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is distinct until changed.
    /// </summary>
    /// <value><c>true</c> if this instance is distinct until changed; otherwise, <c>false</c>.</value>
    public bool IsDistinctUntilChanged => (_mode & ReactivePropertyMode.DistinctUntilChanged) == ReactivePropertyMode.DistinctUntilChanged;

    /// <summary>
    /// Gets a value indicating whether this instance is raise latest value on subscribe.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is raise latest value on subscribe; otherwise, <c>false</c>.
    /// </value>
    public bool IsRaiseLatestValueOnSubscribe => (_mode & ReactivePropertyMode.RaiseLatestValueOnSubscribe) == ReactivePropertyMode.RaiseLatestValueOnSubscribe;

    /// <summary>
    /// Gets a value indicating whether this instance is ignore initial validation error.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is ignore initial validation error; otherwise, <c>false</c>.
    /// </value>
    public bool IsIgnoreInitialValidationError => (_mode & ReactivePropertyMode.IgnoreInitialValidationError) == ReactivePropertyMode.IgnoreInitialValidationError;

    private readonly IEqualityComparer<T> _equalityComparer;

    private ReactivePropertyMode _mode; // None = 0, DistinctUntilChanged = 1, RaiseLatestValueOnSubscribe = 2, Disposed = (1 << 9)
    private ObserverNode<T>? _root;
    private ObserverNode<T>? _last;

    private IDisposable SourceDisposable { get; }

    // for Validation
    private Subject<T> ValidationTrigger { get; } = new Subject<T>();

    private SerialDisposable ValidateNotifyErrorSubscription { get; } = new SerialDisposable();

    private Lazy<BehaviorSubject<IEnumerable?>> ErrorsTrigger { get; }

    private Lazy<List<Func<IObservable<T>, IObservable<IEnumerable?>>>> ValidatorStore { get; } = new Lazy<List<Func<IObservable<T>, IObservable<IEnumerable?>>>>(() => new List<Func<IObservable<T>, IObservable<IEnumerable?>>>());

    /// <summary>
    /// PropertyChanged raise on ReactivePropertyScheduler
    /// </summary>
    public ReactiveProperty()
#pragma warning disable CS8604 // Nullable
        : this(default, ReactivePropertyMode.Default)
#pragma warning restore CS8604 // Nullable
    {
    }

    /// <summary>
    /// PropertyChanged raise on ReactivePropertyScheduler
    /// </summary>
    public ReactiveProperty(
#pragma warning disable CS8601 // Nullable
        T initialValue = default,
#pragma warning restore CS8601 // Nllable
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null)
        : this(ReactivePropertyScheduler.Default, initialValue, mode, equalityComparer)
    { }

    /// <summary>
    /// PropertyChanged raise on selected scheduler
    /// </summary>
    public ReactiveProperty(
        IScheduler raiseEventScheduler,
#pragma warning disable CS8601 // Nullable
        T initialValue = default,
#pragma warning restore CS8601 // Nllable
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        IEqualityComparer<T>? equalityComparer = null)
    {
        RaiseEventScheduler = raiseEventScheduler;
        LatestValue = initialValue;
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

        _mode = mode;

        SourceDisposable = Disposable.Empty;
        ErrorsTrigger = new Lazy<BehaviorSubject<IEnumerable?>>(() => new BehaviorSubject<IEnumerable?>(GetErrors(null)));
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
#pragma warning disable CS8601 // Nullable
        T initialValue = default,
#pragma warning restore CS8601 // Nllable
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        IEqualityComparer<T>? equalityComparer = null)
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
#pragma warning disable CS8601 // Nullable
        T initialValue = default,
#pragma warning restore CS8601 // Nllable
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        IEqualityComparer<T>? equalityComparer = null)
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
            if (IsDistinctUntilChanged && _equalityComparer.Equals(LatestValue, value))
            {
                return;
            }

            SetValue(value);
        }
    }

    object? IReactiveProperty.Value
    {
        get { return Value; }
        set { Value = (T)value!; }
    }

    object? IReadOnlyReactiveProperty.Value => Value;

    /// <summary>
    /// Subscribe source.
    /// </summary>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (IsDisposed)
        {
            observer.OnCompleted();
            return Disposable.Empty;
        }

        if (IsRaiseLatestValueOnSubscribe)
        {
            observer.OnNext(LatestValue);
        }

        // subscribe node, node as subscription.
        var next = new ObserverNode<T>(this, observer);
        if (_root == null)
        {
            _root = _last = next;
        }
        else
        {
            _last!.Next = next;
            next.Previous = _last;
            _last = next;
        }
        return next;
    }

    /// <summary>
    /// Unsubscribe all subscription.
    /// </summary>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        var node = _root;
        _root = _last = null;
        _mode = (ReactivePropertyMode)IsDisposedFlagNumber;

        while (node != null)
        {
            node.OnCompleted();
            node = node.Next;
        }

        ValidationTrigger.Dispose();
        SourceDisposable.Dispose();
        ValidateNotifyErrorSubscription.Dispose();
        if (ErrorsTrigger.IsValueCreated)
        {
            ErrorsTrigger.Value.OnCompleted();
            ErrorsTrigger.Value.Dispose();
        }
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string ToString() =>
        (LatestValue == null)
            ? "null"
            : "{" + LatestValue.GetType().Name + ":" + LatestValue.ToString() + "}";

    /// <summary>
    /// <para>Checked validation, raised value. If success return value is null.</para>
    /// </summary>
    public IObservable<IEnumerable?> ObserveErrorChanged => ErrorsTrigger.Value.AsObservable();

    private IEnumerable? CurrentErrors { get; set; }

    /// <summary>
    /// Occurs when the validation errors have changed for a property or for the entire entity.
    /// </summary>
    /// <returns></returns>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// <para>Set INotifyDataErrorInfo's asynchronous validation, return value is self.</para>
    /// </summary>
    /// <param name="validator">If success return IO&lt;null&gt;, failure return IO&lt;IEnumerable&gt;(Errors).</param>
    /// <returns>Self.</returns>
    public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<T>, IObservable<IEnumerable?>> validator)
    {
        ValidatorStore.Value.Add(validator);     //--- cache validation functions
        var validators = ValidatorStore.Value
            .Select(x => x(IsIgnoreInitialValidationError ? ValidationTrigger : ValidationTrigger.StartWith(LatestValue)))
            .ToArray();     //--- use copy
        ValidateNotifyErrorSubscription.Disposable = Observable
            .CombineLatest(validators)
            .Select(xs =>
            {
                if (xs.Count == 0)
                {
                    return null;
                }

                if (xs.All(x => x == null))
                {
                    return null;
                }

                var strings = xs
                    .Where(x => x != null)
                    .OfType<string>();
                var others = xs
                    .Where(x => x is not string)
                    .Where(x => x != null)
                    .SelectMany(x => x!.Cast<object?>());
                return strings.Concat(others);
            })
            .Subscribe(x =>
            {
                var prevHasErrors = HasErrors;
                CurrentErrors = x;
                var currentHasErrors = HasErrors;
                var handler = ErrorsChanged;
                if (handler != null)
                {
                    RaiseEventScheduler.Schedule(() => handler(this, SingletonDataErrorsChangedEventArgs.Value));
                }

                var propertyChangedHandler = PropertyChanged;
                if (prevHasErrors != currentHasErrors && propertyChangedHandler != null)
                {
                    RaiseEventScheduler.Schedule(() => propertyChangedHandler(this, SingletonPropertyChangedEventArgs.HasErrors));
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
    public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<T>, IObservable<string?>> validator) =>
        SetValidateNotifyError(xs => validator(xs).Select(x => (IEnumerable?)x));

    /// <summary>
    /// Set INotifyDataErrorInfo's asynchronous validation.
    /// </summary>
    /// <param name="validator">Validation logic</param>
    /// <returns>Self.</returns>
    public ReactiveProperty<T> SetValidateNotifyError(Func<T, Task<IEnumerable?>> validator) =>
        SetValidateNotifyError(xs => xs.SelectMany(x => validator(x)));

    /// <summary>
    /// Set INotifyDataErrorInfo's asynchronous validation.
    /// </summary>
    /// <param name="validator">Validation logic</param>
    /// <returns>Self.</returns>
    public ReactiveProperty<T> SetValidateNotifyError(Func<T, Task<string?>> validator) =>
        SetValidateNotifyError(xs => xs.SelectMany(x => validator(x)));

    /// <summary>
    /// Set INotifyDataErrorInfo validation.
    /// </summary>
    /// <param name="validator">Validation logic</param>
    /// <returns>Self.</returns>
    public ReactiveProperty<T> SetValidateNotifyError(Func<T, IEnumerable?> validator) =>
        SetValidateNotifyError(xs => xs.Select(x => validator(x)));

    /// <summary>
    /// Set INotifyDataErrorInfo validation.
    /// </summary>
    /// <param name="validator">Validation logic</param>
    /// <returns>Self.</returns>
    public ReactiveProperty<T> SetValidateNotifyError(Func<T, string?> validator) =>
        SetValidateNotifyError(xs => xs.Select(x => validator(x)));

    /// <summary>
    /// Get INotifyDataErrorInfo's error store
    /// </summary>
    public IEnumerable? GetErrors(string? propertyName) => CurrentErrors;
    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => CurrentErrors ?? Enumerable.Empty<object>();

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
        if (!IsDisposed)
        {
            ValidationTrigger.OnNext(value);
            OnNextAndRaiseValueChanged(ref value);
        }
    }

    private void OnNextAndRaiseValueChanged(ref T value)
    {
        // call source.OnNext
        var node = _root;
        while (node != null)
        {
            node.OnNext(value);
            node = node.Next;
        }

        RaiseEventScheduler.Schedule(() => PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value));
    }


    void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
    {
        if (node == _root)
        {
            _root = node.Next;
        }
        if (node == _last)
        {
            _last = node.Previous;
        }

        if (node.Previous != null)
        {
            node.Previous.Next = node.Next;
        }
        if (node.Next != null)
        {
            node.Next.Previous = node.Previous;
        }
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
        if (ExpressionTreeUtils.IsNestedPropertyPath(propertySelector))
        {
            var propertyPath = PropertyPathNodeLegacy.CreateFromPropertySelector(propertySelector);
            propertyPath.UpdateSource(target);

            var initialValue = propertyPath.GetPropertyPathValue();
            var result = new ReactiveProperty<TProperty>(raiseEventScheduler, 
                initialValue: (TProperty)initialValue!, mode: mode);
            result
                .Where(_ => !ignoreValidationErrorValue || !result.HasErrors)
                .Subscribe(x => propertyPath.SetPropertyPathValue(x), _ => propertyPath.Dispose(), () => propertyPath.Dispose());
            return result;
        }
        else
        {
            var getter = AccessorCache<TTarget>.LookupGet(propertySelector, out var propertyName);
            var setter = AccessorCache<TTarget>.LookupSet(propertySelector, out propertyName);

            var result = new ReactiveProperty<TProperty>(raiseEventScheduler, initialValue: getter(target), mode: mode);
            result
                .Where(_ => !ignoreValidationErrorValue || !result.HasErrors)
                .Subscribe(x => setter(target, x));

            return result;
        }
        // no use
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
        if (ExpressionTreeUtils.IsNestedPropertyPath(propertySelector))
        {
            var propertyPath = PropertyPathNode.CreateFromPropertySelector(propertySelector);
            propertyPath.UpdateSource(target);

            var initialValue = propertyPath.GetPropertyPathValue();
            var result = new ReactiveProperty<TResult>(raiseEventScheduler, initialValue: convert((TProperty)initialValue!), mode: mode);
            result
                .Where(_ => !ignoreValidationErrorValue || !result.HasErrors)
                .Select(convertBack)
                .Subscribe(x => propertyPath.SetPropertyPathValue(x), _ => propertyPath.Dispose(), () => propertyPath.Dispose());
            return result;
        }
        else
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
    }

    /// <summary>
    /// <para>Convert to two-way bindable IObservable&lt;T&gt;</para>
    /// <para>PropertyChanged raise on ReactivePropertyScheduler</para>
    /// </summary>
    public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
#pragma warning disable CS8601 // Nullable
        T initialValue = default,
#pragma warning restore CS8601 // Nllable
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe, 
        IEqualityComparer<T>? equalityComparer = null) =>
        new(source, initialValue, mode, equalityComparer);

    /// <summary>
    /// <para>Convert to two-way bindable IObservable&lt;T&gt;</para>
    /// <para>PropertyChanged raise on selected scheduler</para>
    /// </summary>
    public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
        IScheduler raiseEventScheduler,
#pragma warning disable CS8601 // Nullable
        T initialValue = default,
#pragma warning restore CS8601 // Nllable
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        IEqualityComparer<T>? equalityComparer = null) =>
        new(source, raiseEventScheduler, initialValue, mode, equalityComparer);
}
