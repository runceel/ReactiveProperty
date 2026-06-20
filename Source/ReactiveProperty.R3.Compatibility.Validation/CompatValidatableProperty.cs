using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using R3;
using Reactive.Bindings.R3Compat;

namespace Reactive.Bindings.R3Compat.Validation;

/// <summary>
/// Temporary validation compatibility layer for ReactiveProperty-to-R3 migrations.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
/// <remarks>
/// Downgrade when validation is synchronous and single-value only: use <see cref="BindableReactiveProperty{T}.EnableValidation()"/> directly.
/// Remove when stream-validator usage is zero.
/// </remarks>
public sealed class CompatValidatableProperty<T> : IDisposable, INotifyDataErrorInfo
{
    private const string ValuePropertyName = nameof(Value);
    private static readonly object DummyValidationContextSource = new();
    private readonly CompatibilityOptions _options;
    private readonly BindableReactiveProperty<T> _r3;
    private readonly BehaviorSubject<bool> _observeHasErrors;
    private readonly Subject<T> _validationTrigger = new();
    private readonly List<Func<T, IEnumerable?>> _syncValidators = new();
    private readonly List<IEnumerable?> _streamErrors = new();
    private readonly List<IDisposable> _streamSubscriptions = new();
    private readonly IEqualityComparer<T> _equalityComparer;
    private IEnumerable _errors = Array.Empty<object>();
    private T _value;
    private bool _isDisposed;
    private bool _updatingR3;

    /// <summary>Initializes a new instance of the <see cref="CompatValidatableProperty{T}"/> class.</summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="options">Compatibility options.</param>
    public CompatValidatableProperty(T initialValue = default!, CompatibilityOptions? options = null)
    {
        _options = options ?? new CompatibilityOptions();
        _value = initialValue;
        _r3 = new BindableReactiveProperty<T>(initialValue);
        _r3.PropertyChanged += R3PropertyChanged;
        _observeHasErrors = new BehaviorSubject<bool>(false);
        _equalityComparer = EqualityComparer<T>.Default;
        CompatibilityTelemetry.Track("RP-R3COMPAT-VAL-000", typeof(CompatValidatableProperty<T>).FullName ?? nameof(CompatValidatableProperty<T>));
    }

    /// <inheritdoc />
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>Gets or sets the current value.</summary>
    public T Value
    {
        get => _value;
        set => SetValue(value, true, false);
    }

    /// <inheritdoc />
    public bool HasErrors => _errors.Cast<object>().Any();

    /// <summary>Observes <see cref="HasErrors"/> changes.</summary>
    public Observable<bool> ObserveHasErrors => _observeHasErrors;

    /// <summary>
    /// Adds a synchronous single-value validator.
    /// </summary>
    /// <param name="validate">Validation function that returns null for success.</param>
    /// <returns>This instance.</returns>
    public CompatValidatableProperty<T> SetValidateNotifyError(Func<T, string?> validate)
    {
        if (validate == null)
        {
            throw new ArgumentNullException(nameof(validate));
        }

        CompatibilityTelemetry.Track("RP-VAL-SYNC", validate.Method.DeclaringType?.FullName ?? validate.Method.Name);
        _syncValidators.Add(x => ToErrors(validate(x)));
        ValidateCurrent(true);
        return this;
    }

    /// <summary>
    /// Adds a stream-typed validator for validation patterns that cannot be mechanically rewritten to R3 validation.
    /// </summary>
    /// <param name="validate">Validation stream factory.</param>
    /// <returns>This instance.</returns>
    public CompatValidatableProperty<T> SetValidateNotifyError(Func<Observable<T>, Observable<IEnumerable?>> validate)
    {
        if (validate == null)
        {
            throw new ArgumentNullException(nameof(validate));
        }

        CompatibilityTelemetry.Track("RP-VAL-001", validate.Method.DeclaringType?.FullName ?? validate.Method.Name);
        var index = _streamErrors.Count;
        _streamErrors.Add(null);
        var subscription = validate(_validationTrigger).Subscribe(errors =>
        {
            _streamErrors[index] = errors;
            PublishErrors(CollectErrors());
        });
        _streamSubscriptions.Add(subscription);

        if (!_options.IgnoreInitialValidationError)
        {
            _validationTrigger.OnNext(_value);
        }

        return this;
    }

    /// <summary>
    /// Adds DataAnnotations validation using the ReactiveProperty-compatible MemberName=Value context.
    /// </summary>
    /// <param name="selfSelector">Expression selecting the property decorated with validation attributes.</param>
    /// <returns>This instance.</returns>
    public CompatValidatableProperty<T> SetValidateAttribute(Expression<Func<CompatValidatableProperty<T>>> selfSelector)
    {
        if (selfSelector == null)
        {
            throw new ArgumentNullException(nameof(selfSelector));
        }

        CompatibilityTelemetry.Track("RP-VAL-ATTR", selfSelector.Body.ToString());
        _syncValidators.Add(CreateValidatorFromDataAnnotations(selfSelector));
        ValidateCurrent(true);
        return this;
    }

    /// <summary>Runs validation for the current value.</summary>
    public void ForceValidate() => ValidateCurrent(false);

    /// <summary>Publishes the current value to validation streams and the R3 bridge.</summary>
    public void ForceNotify()
    {
        ThrowIfDisposed();
        _validationTrigger.OnNext(_value);
        UpdateR3(_value);
    }

    /// <summary>Returns the underlying R3 bindable property bridge.</summary>
    public BindableReactiveProperty<T> AsR3() => _r3;

    /// <inheritdoc />
    public IEnumerable GetErrors(string? propertyName) => propertyName == null || propertyName == ValuePropertyName ? _errors : Array.Empty<object>();

    /// <inheritdoc />
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _r3.PropertyChanged -= R3PropertyChanged;
        foreach (var subscription in _streamSubscriptions)
        {
            subscription.Dispose();
        }

        _validationTrigger.Dispose();
        _observeHasErrors.Dispose();
        _r3.Dispose();
    }

    private void SetValue(T value, bool validate, bool fromR3)
    {
        ThrowIfDisposed();
        var changed = !_equalityComparer.Equals(_value, value);
        if (!changed && _options.Mode.HasFlag(CompatibilityMode.DistinctUntilChanged))
        {
            if (validate)
            {
                ValidateCurrent(false);
            }

            return;
        }

        _value = value;
        if (validate)
        {
            ValidateCurrent(false);
        }

        _validationTrigger.OnNext(value);
        if (!fromR3)
        {
            UpdateR3(value);
        }
    }

    private void ValidateCurrent(bool initialValidatorRegistration)
    {
        ThrowIfDisposed();
        if (initialValidatorRegistration && _options.IgnoreInitialValidationError)
        {
            return;
        }

        PublishErrors(CollectErrors());
    }

    private IEnumerable<object> CollectErrors()
    {
        foreach (var validator in _syncValidators)
        {
            foreach (var error in ToObjects(validator(_value)))
            {
                yield return error;
            }
        }

        foreach (var errors in _streamErrors)
        {
            foreach (var error in ToObjects(errors))
            {
                yield return error;
            }
        }
    }

    private void PublishErrors(IEnumerable<object> errors)
    {
        var newErrors = errors.ToArray();
        var oldHasErrors = HasErrors;
        _errors = newErrors;
        var newHasErrors = HasErrors;
        if (oldHasErrors != newHasErrors)
        {
            _observeHasErrors.OnNext(newHasErrors);
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(ValuePropertyName));
    }

    private void UpdateR3(T value)
    {
        _updatingR3 = true;
        try
        {
            _r3.Value = value;
        }
        finally
        {
            _updatingR3 = false;
        }
    }

    private void R3PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_updatingR3 || e.PropertyName != ValuePropertyName)
        {
            return;
        }

        SetValue(_r3.Value, true, true);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(CompatValidatableProperty<T>));
        }
    }

    private static IEnumerable? ToErrors(string? error) => error == null ? null : new[] { error };

    private static IEnumerable<object> ToObjects(IEnumerable? errors)
    {
        if (errors == null)
        {
            yield break;
        }

        foreach (var error in errors)
        {
            if (error != null)
            {
                yield return error;
            }
        }
    }

    private static Func<T, IEnumerable?> CreateValidatorFromDataAnnotations(Expression<Func<CompatValidatableProperty<T>>> selector)
    {
        if (!(selector.Body is MemberExpression memberExpression) || !(memberExpression.Member is PropertyInfo propertyInfo))
        {
            throw new ArgumentException("The selector must point to a property.", nameof(selector));
        }

        var display = propertyInfo.GetCustomAttribute<DisplayAttribute>();
        var attrs = propertyInfo.GetCustomAttributes<ValidationAttribute>().ToArray();
        if (attrs.Length == 0)
        {
            throw new InvalidOperationException($"Data annotations does not found on {propertyInfo.Name}.");
        }

        var context = new ValidationContext(DummyValidationContextSource)
        {
            DisplayName = display?.GetName() ?? propertyInfo.Name,
            MemberName = ValuePropertyName,
        };

        return x =>
        {
            var validationResults = new List<ValidationResult>();
            if (Validator.TryValidateValue(x!, context, validationResults, attrs))
            {
                return null;
            }

            return validationResults.Select(r => r.ErrorMessage).Where(x => x != null).ToArray()!;
        };
    }
}
