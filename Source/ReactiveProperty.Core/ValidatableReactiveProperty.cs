using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings;

internal class SingletonDataErrorsChangedEventArgs
{
    public static readonly DataErrorsChangedEventArgs Value = new(nameof(IReactiveProperty.Value));
}

internal class StringArrayEqualityComparer : IEqualityComparer<string[]>
{
    public static readonly StringArrayEqualityComparer Default = new();

    public bool Equals(string[]? x, string[]? y)
    {
        if (x == y) return true;
        if (x == null) return false;
        if (y == null) return false;
        return x.SequenceEqual(y);
    }

    public int GetHashCode(string[] obj) => obj.GetHashCode();
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
    private readonly bool _disposeSource;
    private ObserverNode<T>? _root;
    private ObserverNode<T>? _last;

    private readonly ReactivePropertySlim<string[]> _errorMessages = new(
        Array.Empty<string>(),
        equalityComparer: StringArrayEqualityComparer.Default);
    private readonly ReactivePropertySlim<bool> _observeHasErrors = new(false);

    private T _latestValue;
    private Func<T, string?>[] _validators;

    /// <summary>
    /// Return the first error message from GetErrors(). If HasErrors is false, then return empty string.
    /// </summary>
    public string ErrorMessage => _errorMessages.Value.FirstOrDefault() ?? "";

    /// <inheritdoc/>
    public T Value
    {
        get => _latestValue;
        set => Validate(value);
    }

    /// <summary>
    /// Get the source IReactiveProperty.
    /// </summary>
    public IReactiveProperty<T>? Source { get; }

    /// <inheritdoc/>
    IObservable<IEnumerable?> IHasErrors.ObserveErrorChanged => _errorMessages;

    /// <summary>
    /// Get the observe error changed.
    /// </summary>
    public IObservable<string[]> ObserveErrorChanged => _errorMessages;

    /// <inheritdoc/>
    public IObservable<bool> ObserveHasErrors => _observeHasErrors;

    /// <inheritdoc/>
    public bool HasErrors => _observeHasErrors.Value;

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
    /// <param name="disposeSource">Call Dispose of the source parameter when ValidatableReactiveProperty was called Dispose.</param>
    public ValidatableReactiveProperty(IReactiveProperty<T> source,
        IEnumerable<Func<T, string?>> validators,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null,
        bool disposeSource = false)
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
        _disposeSource = disposeSource;

        _observeHasErrors.PropertyChanged += ObserveHasErrors_PropertyChanged;
        _errorMessages.PropertyChanged += ErrorMessages_PropertyChanged;

        InitializeValidationProcess();
    }

    /// <summary>
    /// Create a ValidatableReactiveProperty instance.
    /// </summary>
    /// <param name="source">The source IReactiveProperty</param>
    /// <param name="validator">The validation logic.</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    /// <param name="disposeSource">Call Dispose of the source parameter when ValidatableReactiveProperty was called Dispose.</param>
    public ValidatableReactiveProperty(IReactiveProperty<T> source,
        Func<T, string?> validator,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null,
        bool disposeSource = false) :
        this(source, new[] { validator }, mode, equalityComparer, disposeSource)
    {
    }

    /// <summary>
    /// Create a ValidatableReactiveProperty instance.
    /// </summary>
    /// <param name="initialValue">An initial value for ValidatableReactiveProperty.</param>
    /// <param name="validators">The validation logics.</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    public ValidatableReactiveProperty(T initialValue,
        IEnumerable<Func<T, string?>> validators,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null)
    {
        if ((mode & ReactivePropertyMode.IgnoreException) == ReactivePropertyMode.IgnoreException)
        {
            throw new NotSupportedException("IgnoreException isn't supported in ValidatableReactiveProperty.");
        }

        _validators = validators.ToArray();
        _latestValue = initialValue;
        _mode = mode;
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

        _observeHasErrors.PropertyChanged += ObserveHasErrors_PropertyChanged;
        _errorMessages.PropertyChanged += ErrorMessages_PropertyChanged;

        InitializeValidationProcess();
    }

    /// <summary>
    /// Create a ValidatableReactiveProperty instance.
    /// </summary>
    /// <param name="initialValue">An initial value for ValidatableReactiveProperty.</param>
    /// <param name="validator">The validation logic.</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    public ValidatableReactiveProperty(T initialValue,
        Func<T, string?> validator,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null) :
        this(initialValue, new[] { validator }, mode, equalityComparer)
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

        _observeHasErrors.PropertyChanged -= ObserveHasErrors_PropertyChanged;
        _errorMessages.PropertyChanged -= ErrorMessages_PropertyChanged;

        _errorMessages.Dispose();
        _observeHasErrors.Dispose();

        if (Source != null)
        {
            Source.PropertyChanged -= Source_PropertyChanged;
            if (_disposeSource)
            {
                Source.Dispose();
            }
        }
    }

    /// <inheritdoc/>
    public void ForceNotify() => Validate(_latestValue, ValidationKind.Force);

    /// <inheritdoc/>
    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => _errorMessages.Value;


    /// <summary>
    /// Get error messages. If HasErros is false, then return empty string array.
    /// </summary>
    /// <returns>The error messages.</returns>
    public string[] GetErrors() => _errorMessages.Value;

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
        if (Source != null)
        {
            Source.PropertyChanged += Source_PropertyChanged;
        }

        Validate(_latestValue, ValidationKind.FirstTime);
    }

    private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IReactiveProperty.Value)) return;
        Validate(Source!.Value, ValidationKind.RequestFromSoucre);
    }

    private void Validate(T value, ValidationKind validationKind = ValidationKind.Default)
    {
        var isNotEquals = !_equalityComparer.Equals(_latestValue, value);
        var needValidation = validationKind == ValidationKind.FirstTime ? 
            !IsIgnoreInitialValidationError : 
            isNotEquals || validationKind == ValidationKind.Force;

        _latestValue = value;
        if (needValidation)
        {
            _errorMessages.Value = _validators.Select(x => x(value))
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

        if (Source != null && validationKind != ValidationKind.RequestFromSoucre && HasErrors is false)
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

    private void ErrorMessages_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IReactiveProperty.Value)) return;
        PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.ErrorMessage);
        ErrorsChanged?.Invoke(this, SingletonDataErrorsChangedEventArgs.Value);
        _observeHasErrors.Value = _errorMessages.Value.Length != 0;
    }

    private void ObserveHasErrors_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IReactiveProperty.Value)) return;
        PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.HasErrors);
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
        Force,
    }
}

/// <summary>
/// Factory extension methods for ValidatableReactiveProperty
/// </summary>
public static class ValidatableReactiveProperty
{
    private static readonly object DummyValidationContextSource = new();
    /// <summary>
    /// Create the ValidatableReactiveProperty instance.
    /// </summary>
    /// <param name="source">The source IReactiveProperty</param>
    /// <param name="validator">The validation logic.</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    /// <param name="disposeSource">Call Dispose of the source parameter when ValidatableReactiveProperty was called Dispose.</param>
    public static ValidatableReactiveProperty<T> ToValidatableReactiveProperty<T>(
        this IReactiveProperty<T> source,
        Func<T, string?> validator,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null,
        bool disposeSource = false) =>
        new ValidatableReactiveProperty<T>(source, validator, mode, equalityComparer, disposeSource);

    /// <summary>
    /// Create the ValidatableReactiveProperty instance.
    /// </summary>
    /// <param name="source">The source IReactiveProperty</param>
    /// <param name="validators">The validation logics.</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    /// <param name="disposeSource">Call Dispose of the source parameter when ValidatableReactiveProperty was called Dispose.</param>
    /// <returns>The new instance of ValidatableReactiveProperty</returns>
    public static ValidatableReactiveProperty<T> ToValidatableReactiveProperty<T>(
        this IReactiveProperty<T> source,
        IEnumerable<Func<T, string?>> validators,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null,
        bool disposeSource = false) =>
        new ValidatableReactiveProperty<T>(source, validators, mode, equalityComparer, disposeSource);

    /// <summary>
    /// Create the ValidationReactiveProperty instance from DataAnnotations attributes.
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="source">Target ReactiveProperty</param>
    /// <param name="selfSelector">Target property as expression</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    /// <param name="disposeSource">Call Dispose of the source parameter when ValidatableReactiveProperty was called Dispose.</param>
    /// <returns>The new instance of ValidatableReactiveProperty</returns>
    public static ValidatableReactiveProperty<T> ToValidatableReactiveProperty<T>(
        this IReactiveProperty<T> source,
        Expression<Func<IReactiveProperty<T>?>> selfSelector,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null,
        bool disposeSource = false) => 
        source.ToValidatableReactiveProperty(
            CreateValidatorFromDataAnnotations(selfSelector),
            mode,
            equalityComparer,
            disposeSource);

    /// <summary>
    /// Create the ValidationReactiveProperty instance from DataAnnotations attributes.
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="initialValue">Initial value of ValidatableReactiveProperty</param>
    /// <param name="selfSelector">Target property as expression</param>
    /// <param name="mode">The ReactivePropertyMode</param>
    /// <param name="equalityComparer">The EqualityComparer for T</param>
    /// <returns>The new instance of ValidatableReactiveProperty</returns>
    public static ValidatableReactiveProperty<T> CreateFromDataAnnotations<T>(
        T initialValue,
        Expression<Func<IReactiveProperty<T>?>> selfSelector,
        ReactivePropertyMode mode = ReactivePropertyMode.Default,
        IEqualityComparer<T>? equalityComparer = null) => 
        new ValidatableReactiveProperty<T>(
            initialValue,
            CreateValidatorFromDataAnnotations(selfSelector),
            mode,
            equalityComparer);

    private static Func<T, string?> CreateValidatorFromDataAnnotations<T>(Expression<Func<IReactiveProperty<T>?>> selector)
    {
        var memberExpression = (MemberExpression)selector.Body;
        var propertyInfo = (PropertyInfo)memberExpression.Member;
        var display = propertyInfo.GetCustomAttribute<DisplayAttribute>();
        var attrs = propertyInfo.GetCustomAttributes<ValidationAttribute>().ToArray();
        if (attrs.Length == 0)
        {
            throw new InvalidOperationException($"Data annotations does not found on {propertyInfo.Name}.");
        }

        var context = new ValidationContext(DummyValidationContextSource)
        {
            DisplayName = display?.GetName() ?? propertyInfo.Name,
            MemberName = nameof(IReactiveProperty<T>.Value),
        };

        return x =>
        {
            var validationResults = new List<ValidationResult>();
            // The first argument of TryValidateValue method doesn't allow nullable value.
            // But the value is passing to `GetValidationErrors` method, and the method allow nullable.
            if (Validator.TryValidateValue(x!, context, validationResults, attrs))
            {
                return null;
            }

            return validationResults[0].ErrorMessage;
        };
    }
}
