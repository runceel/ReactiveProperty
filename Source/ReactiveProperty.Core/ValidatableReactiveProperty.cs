using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings;

internal class SingletonDataErrorsChangedEventArgs
{
    public static readonly DataErrorsChangedEventArgs Value = new(nameof(IReactiveProperty.Value));
}

/// <summary>
/// IReactiveProperty implementations with validation feature.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public class ValidatableReactiveProperty<T> : IReactiveProperty<T>, IObserverLinkedList<T>
{
    private const int IsDisposedFlagNumber = 1 << 9; // (reserve 0 ~ 8)
    private ReactivePropertyMode _mode; // None = 0, DistinctUntilChanged = 1, RaiseLatestValueOnSubscribe = 2, Disposed = (1 << 9)
    private readonly IEqualityComparer<T> _equalityComparer;
    private ObserverNode<T>? _root;
    private ObserverNode<T>? _last;
    private string[] _errorMessages = Array.Empty<string>();

    private IDisposable _sourceSubscribe = default!;
    private readonly InternalSubject<string[]> _observeErrorChanged = new();
    private readonly InternalSubject<bool> _observeHasErrors = new();

    private T _latestValue;
    private Func<T, string?>[] _validators;

    /// <summary>
    /// Return the first error message from GetErrors(). If HasErrors is false, then return empty string.
    /// </summary>
    public string ErrorMessage => HasErrors ? _errorMessages[0] : "";

    /// <inheritdoc/>
    public T Value
    {
        get => _latestValue;
        set => Validate(value);
    }

    /// <summary>
    /// Get the source IReactiveProperty.
    /// </summary>
    public IReactiveProperty<T> Source { get; }

    /// <inheritdoc/>
    IObservable<IEnumerable?> IHasErrors.ObserveErrorChanged => _observeErrorChanged;

    /// <summary>
    /// Get the observe error changed.
    /// </summary>
    public IObservable<string[]> ObserveErrorChanged => _observeErrorChanged;

    /// <inheritdoc/>
    public IObservable<bool> ObserveHasErrors => _observeHasErrors;

    /// <inheritdoc/>
    public bool HasErrors => _errorMessages.Length != 0;

    /// <inheritdoc/>
    object? IReactiveProperty.Value { get => Value; set => Value = (T)value!; }

    /// <inheritdoc/>
    T IReadOnlyReactiveProperty<T>.Value => _latestValue;

    /// <inheritdoc/>
    object? IReadOnlyReactiveProperty.Value => _latestValue;

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
    public bool IsDisposed => (int)_mode == IsDisposedFlagNumber;

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
    /// Gets a value indicating whether this instance is ignore first validation errors.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance ignore first validation errors; otherwise, <c>false</c>.
    /// </value>
    public bool IsIgnoreInitialValidationError => (_mode & ReactivePropertyMode.IgnoreInitialValidationError) == ReactivePropertyMode.IgnoreInitialValidationError;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <inheritdoc/>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Create a ValidatableReactiveProperty instance.
    /// </summary>
    /// <param name="source">The source IReactiveProperty</param>
    /// <param name="validators">The validation logics.</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    public ValidatableReactiveProperty(IReactiveProperty<T> source,
        IEnumerable<Func<T, string?>> validators,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null)
    {
        if ((mode & ReactivePropertyMode.IgnoreException) == ReactivePropertyMode.IgnoreException)
        {
            throw new NotSupportedException("IgnoreException isn't supported in ValidatableReactiveProperty.");
        }

        Source = source;

        _validators = validators.ToArray();
        _latestValue = Source.Value;
        _mode = mode;
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        InitializeValidationProcess();
    }

    /// <summary>
    /// Create a ValidatableReactiveProperty instance.
    /// </summary>
    /// <param name="source">The source IReactiveProperty</param>
    /// <param name="validator">The validation logic.</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    public ValidatableReactiveProperty(IReactiveProperty<T> source,
        Func<T, string?> validator,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null) :
        this(source, new[] { validator }, mode, equalityComparer)
    {
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting
    /// unmanaged resources.
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

        _sourceSubscribe?.Dispose();
        _observeErrorChanged.Dispose();
        _observeHasErrors.Dispose();
    }

    /// <inheritdoc/>
    public void ForceNotify() => Validate(_latestValue);

    /// <inheritdoc/>
    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => _errorMessages;


    /// <summary>
    /// Get error messages. If HasErros is false, then return empty string array.
    /// </summary>
    /// <returns>The error messages.</returns>
    public string[] GetErrors() => _errorMessages;

    /// <summary>
    /// Notifies the provider that an observer is to receive notifications.
    /// </summary>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <returns>
    /// A reference to an interface that allows observers to stop receiving notifications before
    /// the provider has finished sending them.
    /// </returns>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (IsDisposed)
        {
            observer.OnCompleted();
            return InternalDisposable.Empty;
        }

        if (IsRaiseLatestValueOnSubscribe)
        {
            observer.OnNext(_latestValue);
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

    private void InitializeValidationProcess()
    {
        _sourceSubscribe = Source.Subscribe(new DelegateObserver<T>(x =>
        {
            Validate(x, ValidationKind.RequestFromSoucre);
        }));

        Validate(_latestValue, ValidationKind.FirstTime);
    }

    private void Validate(T value, ValidationKind validationKind = ValidationKind.Default)
    {
        var previousHasErrors = HasErrors;
        var previousErrors = _errorMessages;

        var isNotEquals = !_equalityComparer.Equals(_latestValue, value);
        var needValidation = 
            (validationKind is ValidationKind.FirstTime || !IsIgnoreInitialValidationError) ||
            isNotEquals;

        _latestValue = value;
        if (needValidation)
        {
            _errorMessages = _validators.Select(x => x(value))
                .Where(x => x is not null)
                .ToArray()!;
        }

        if (isNotEquals || IsDistinctUntilChanged is false)
        {
            NotifyOnNext();
        }

        if (isNotEquals)
        {
            PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value);
        }

        if (!previousErrors.SequenceEqual(_errorMessages))
        {
            ErrorsChanged?.Invoke(this, SingletonDataErrorsChangedEventArgs.Value);
            _observeErrorChanged.OnNext(_errorMessages);
        }

        if (previousHasErrors != HasErrors)
        {
            _observeHasErrors.OnNext(HasErrors);
        }

        if (validationKind is not ValidationKind.RequestFromSoucre && HasErrors is false)
        {
            Source.Value = _latestValue;
        }
    }

    private void NotifyOnNext()
    {
        // call source.OnNext
        var node = _root;
        while (node != null)
        {
            node.OnNext(_latestValue);
            node = node.Next;
        }
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

    private enum ValidationKind
    {
        Default,
        FirstTime,
        RequestFromSoucre,
    }
}

/// <summary>
/// Factory extension methods for ValidatableReactiveProperty
/// </summary>
public static class ValidatableReactiveProperty
{
    /// <summary>
    /// Create the ValidatableReactiveProperty instance.
    /// </summary>
    /// <param name="source">The source IReactiveProperty</param>
    /// <param name="validator">The validation logic.</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    public static ValidatableReactiveProperty<T> ToValidatableReactiveProperty<T>(
        this IReactiveProperty<T> source,
        Func<T, string?> validator,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null) =>
        new ValidatableReactiveProperty<T>(source, validator, mode, equalityComparer);

    /// <summary>
    /// Create the ValidatableReactiveProperty instance.
    /// </summary>
    /// <param name="source">The source IReactiveProperty</param>
    /// <param name="validators">The validation logics.</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    public static ValidatableReactiveProperty<T> ToValidatableReactiveProperty<T>(
        this IReactiveProperty<T> source,
        IEnumerable<Func<T, string?>> validators,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null) =>
        new ValidatableReactiveProperty<T>(source, validators, mode, equalityComparer);
}
