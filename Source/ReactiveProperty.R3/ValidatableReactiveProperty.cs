using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using R3;

namespace Reactive.Bindings.R3;

/// <summary>
/// Represents a reactive property with validation support.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public sealed class ValidatableReactiveProperty<T> : Observable<T>, INotifyPropertyChanged, INotifyDataErrorInfo, IDisposable
{
    private readonly ReactiveProperty<T> _property;
    private readonly IEqualityComparer<T> _equalityComparer;
    private readonly object _validationSyncRoot = new();
    private readonly ReactiveProperty<IReadOnlyList<string>> _errors = new(Array.Empty<string>());
    private readonly ReactiveProperty<bool> _hasErrors = new(false);
    private readonly Subject<T> _validationTrigger = new();
    private readonly List<(int Slot, Func<T, IEnumerable<string>?> Validate)> _validators = new();
    private readonly List<string[]> _validationErrors = new();
    private readonly List<IDisposable> _subscriptions = new();
    private readonly bool _ignoreInitialValidationError;
    private bool _hasChangedValue;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatableReactiveProperty{T}"/> class.
    /// </summary>
    public ValidatableReactiveProperty(
        T initialValue = default!,
        IEqualityComparer<T>? equalityComparer = null,
        bool ignoreInitialValidationError = false)
    {
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        _property = new ReactiveProperty<T>(initialValue, _equalityComparer);
        _ignoreInitialValidationError = ignoreInitialValidationError;
    }

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public T Value
    {
        get => _property.Value;
        set
        {
            if (_equalityComparer.Equals(_property.Value, value))
            {
                return;
            }

            _hasChangedValue = true;
            _property.Value = value;
            ValidateAndNotify(value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has validation errors.
    /// </summary>
    public bool HasErrors => _hasErrors.Value;

    /// <summary>
    /// Gets the first error message, or an empty string when there is no error.
    /// </summary>
    public string ErrorMessage => _errors.Value.FirstOrDefault() ?? "";

    /// <summary>
    /// Gets an observable sequence of error changes.
    /// </summary>
    public Observable<IReadOnlyList<string>> ObserveErrorChanged => _errors;

    /// <summary>
    /// Gets an observable sequence of <see cref="HasErrors"/> changes.
    /// </summary>
    public Observable<bool> ObserveHasErrors => _hasErrors;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Gets the current validation errors.
    /// </summary>
    public string[] GetErrors() => [.. _errors.Value];

    /// <summary>
    /// Gets the current validation errors for a property.
    /// </summary>
    public IEnumerable GetErrors(string? propertyName) =>
        string.IsNullOrEmpty(propertyName) || propertyName == nameof(Value) ? GetErrors() : Array.Empty<string>();

    /// <inheritdoc />
    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => GetErrors(propertyName);

    /// <summary>
    /// Registers a synchronous single-error validator.
    /// </summary>
    public ValidatableReactiveProperty<T> SetValidateNotifyError(Func<T, string?> validate)
    {
        if (validate is null)
        {
            throw new ArgumentNullException(nameof(validate));
        }

        return SetValidateNotifyError(x =>
        {
            var error = validate(x);
            return error is null ? null : new[] { error };
        });
    }

    /// <summary>
    /// Registers a synchronous multi-error validator.
    /// </summary>
    public ValidatableReactiveProperty<T> SetValidateNotifyError(Func<T, IEnumerable<string>?> validate)
    {
        if (validate is null)
        {
            throw new ArgumentNullException(nameof(validate));
        }

        lock (_validationSyncRoot)
        {
            var slot = AddValidationSlot();
            _validators.Add((slot, validate));
        }

        ValidateAfterRegistration();
        return this;
    }

    /// <summary>
    /// Registers an asynchronous single-error validator.
    /// </summary>
    public ValidatableReactiveProperty<T> SetValidateNotifyError(Func<T, Task<string?>> validate)
    {
        if (validate is null)
        {
            throw new ArgumentNullException(nameof(validate));
        }

        return SetValidateNotifyError(xs => ToAsyncValidationObservable(xs, async x =>
        {
            var error = await validate(x).ConfigureAwait(false);
            return error is null ? null : new[] { error };
        }));
    }

    /// <summary>
    /// Registers an asynchronous multi-error validator.
    /// </summary>
    public ValidatableReactiveProperty<T> SetValidateNotifyError(Func<T, Task<IEnumerable<string>?>> validate)
    {
        if (validate is null)
        {
            throw new ArgumentNullException(nameof(validate));
        }

        return SetValidateNotifyError(xs => ToAsyncValidationObservable(xs, async x => await validate(x).ConfigureAwait(false)));
    }

    /// <summary>
    /// Registers a stream validator.
    /// </summary>
    public ValidatableReactiveProperty<T> SetValidateNotifyError(Func<Observable<T>, Observable<IEnumerable?>> validate)
    {
        if (validate is null)
        {
            throw new ArgumentNullException(nameof(validate));
        }

        int slot;
        lock (_validationSyncRoot)
        {
            slot = AddValidationSlot();
        }

        _subscriptions.Add(validate(_validationTrigger).Subscribe(errors => SetErrors(slot, ToErrorMessages(errors))));
        ValidateAfterRegistration();
        return this;
    }

    /// <summary>
    /// Registers DataAnnotations validation.
    /// </summary>
    public ValidatableReactiveProperty<T> SetValidateAttribute(Expression<Func<ValidatableReactiveProperty<T>>> selfSelector)
    {
        if (selfSelector is null)
        {
            throw new ArgumentNullException(nameof(selfSelector));
        }

        if (selfSelector.Body is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo propertyInfo)
        {
            throw new ArgumentException("The selector must point to a property.", nameof(selfSelector));
        }

        ValidationAttribute[] attributes = [.. propertyInfo.GetCustomAttributes<ValidationAttribute>(true)];
        return SetValidateNotifyError(value =>
        {
            var context = new ValidationContext(this)
            {
                MemberName = nameof(Value),
                DisplayName = propertyInfo.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? propertyInfo.Name,
            };
            var results = new List<ValidationResult>();
            return Validator.TryValidateValue(value, context, results, attributes)
                ? null
                : [.. results.Select(x => x.ErrorMessage ?? x.ToString())];
        });
    }

    /// <summary>
    /// Runs validation for the current value.
    /// </summary>
    public void ForceValidate()
    {
        ValidateAndNotify(Value);
    }

    /// <summary>
    /// Notifies observers of the current value.
    /// </summary>
    public void ForceNotify() => _property.ForceNotify();

    /// <summary>
    /// Returns the underlying R3 reactive property.
    /// </summary>
    public ReactiveProperty<T> ToReactiveProperty() => _property;

    /// <inheritdoc />
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }

        _subscriptions.Clear();
        _validationTrigger.Dispose();
        _errors.Dispose();
        _hasErrors.Dispose();
        _property.Dispose();
    }

    /// <inheritdoc />
    protected override IDisposable SubscribeCore(Observer<T> observer) => _property.Subscribe(observer);

    private void ValidateAfterRegistration()
    {
        if (_ignoreInitialValidationError && !_hasChangedValue)
        {
            return;
        }

        ForceValidate();
    }

    private void ValidateAndNotify(T value)
    {
        Validate(value);
        _validationTrigger.OnNext(value);
    }

    private void Validate(T value)
    {
        (int Slot, Func<T, IEnumerable<string>?> Validate)[] validators;
        lock (_validationSyncRoot)
        {
            validators = [.. _validators];
        }

        if (validators.Length == 0)
        {
            return;
        }

        foreach (var validator in validators)
        {
            SetErrors(validator.Slot, validator.Validate(value) ?? Array.Empty<string>());
        }
    }

    private int AddValidationSlot()
    {
        _validationErrors.Add(Array.Empty<string>());
        return _validationErrors.Count - 1;
    }

    private void SetErrors(int slot, IEnumerable<string> errors)
    {
        lock (_validationSyncRoot)
        {
            _validationErrors[slot] = [.. errors];
            string[] next = [.. _validationErrors.SelectMany(static x => x)];
            var previousHasErrors = _hasErrors.Value;
            if (_errors.Value.SequenceEqual(next))
            {
                return;
            }

            _errors.Value = next;
            _hasErrors.Value = next.Length != 0;
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Value)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            if (previousHasErrors != _hasErrors.Value)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasErrors)));
            }
        }
    }

    private static string[] ToErrorMessages(IEnumerable? errors)
    {
        if (errors is null)
        {
            return Array.Empty<string>();
        }

        if (errors is string singleError)
        {
            return new[] { singleError };
        }

        return [.. errors.Cast<object?>()
            .Where(x => x is not null)
            .Select(static x => x?.ToString())
            .Where(x => x is not null)
            .Select(static x => x!)];
    }

    private static Observable<IEnumerable?> ToAsyncValidationObservable(
        Observable<T> source,
        Func<T, Task<IEnumerable<string>?>> validate)
    {
        var latestVersion = 0;
        return Observable.Create<IEnumerable?>(observer =>
            source.Subscribe(async value =>
            {
                var version = Interlocked.Increment(ref latestVersion);
                try
                {
                    var errors = await validate(value).ConfigureAwait(false);
                    if (version == Volatile.Read(ref latestVersion))
                    {
                        observer.OnNext(errors is null ? null : (string[])[.. errors]);
                    }
                }
                catch (Exception ex)
                {
                    if (version == Volatile.Read(ref latestVersion))
                    {
                        observer.OnNext(new[] { ex.Message });
                    }
                }
            }));
    }
}
