using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using R3;
using Reactive.Bindings.R3Compat;

namespace Reactive.Bindings.R3Compat.Validation;

public sealed class CompatValidatableProperty<T> : IDisposable, INotifyDataErrorInfo
{
    private readonly CompatibilityOptions _options;
    private readonly BehaviorSubject<T> _valueSource;
    private readonly R3.ReactiveProperty<bool> _hasErrors;
    private readonly List<Func<T, IEnumerable?>> _validators = [];
    private readonly List<IDisposable> _subscriptions = [];
    private IReadOnlyList<object> _errors = [];
    private T _value;
    private bool _isDisposed;

    public CompatValidatableProperty(T initialValue = default!, CompatibilityOptions? options = null)
    {
        _options = options ?? new CompatibilityOptions();
        _value = initialValue;
        _valueSource = new BehaviorSubject<T>(initialValue);
        _hasErrors = new R3.ReactiveProperty<bool>(false);
        CompatibilityTelemetry.Track("R3Compat.Validation.CompatValidatableProperty", string.Empty);
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (_isDisposed)
            {
                return;
            }

            _value = value;
            _valueSource.OnNext(value);
            if (_validators.Count > 0)
            {
                ForceValidate();
            }
        }
    }

    public bool HasErrors => _errors.Count > 0;

    public Observable<bool> ObserveHasErrors => _hasErrors;

    public CompatValidatableProperty<T> SetValidateNotifyError(Func<T, string?> validate)
    {
        ArgumentNullException.ThrowIfNull(validate);
        _validators.Add(value => ToErrors(validate(value)));
        if (!_options.IgnoreInitialValidationError)
        {
            ForceValidate();
        }

        return this;
    }

    public CompatValidatableProperty<T> SetValidateNotifyError(Func<Observable<T>, Observable<IEnumerable?>> validate)
    {
        ArgumentNullException.ThrowIfNull(validate);
        _subscriptions.Add(validate(_valueSource).Subscribe(ApplyErrors));
        return this;
    }

    public CompatValidatableProperty<T> SetValidateAttribute(Expression<Func<CompatValidatableProperty<T>>> selfSelector)
    {
        ArgumentNullException.ThrowIfNull(selfSelector);
        var member = selfSelector.Body as MemberExpression;
        if (member == null && selfSelector.Body is UnaryExpression unary)
        {
            member = unary.Operand as MemberExpression;
        }

        var attributes = member?.Member.GetCustomAttributes(typeof(ValidationAttribute), inherit: true)
            .OfType<ValidationAttribute>()
            .ToArray() ?? [];

        _validators.Add(value =>
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this) { MemberName = nameof(Value) };
            return Validator.TryValidateValue(value, context, results, attributes)
                ? null
                : results.Select(x => x.ErrorMessage ?? x.ToString()).ToArray();
        });

        if (!_options.IgnoreInitialValidationError)
        {
            ForceValidate();
        }

        return this;
    }

    public void ForceValidate()
    {
        var errors = _validators.SelectMany(x => ToObjects(x(Value))).ToArray();
        ApplyErrors(errors.Length == 0 ? null : errors);
    }

    public void ForceNotify()
    {
        if (_isDisposed)
        {
            return;
        }

        _valueSource.OnNext(Value);
        ForceValidate();
    }

    public BindableReactiveProperty<T> AsR3() => new(Value);

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName is null or nameof(Value) or "")
        {
            return _errors;
        }

        return Array.Empty<object>();
    }

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

        _valueSource.Dispose();
        _hasErrors.Dispose();
    }

    private static IEnumerable? ToErrors(string? error) => error is null ? null : new[] { error };

    private static IEnumerable<object> ToObjects(IEnumerable? errors)
    {
        if (errors == null)
        {
            return [];
        }

        return errors.Cast<object>().Where(x => x != null).ToArray();
    }

    private void ApplyErrors(IEnumerable? errors)
    {
        if (_isDisposed)
        {
            return;
        }

        _errors = ToObjects(errors).ToArray();
        _hasErrors.Value = HasErrors;
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Value)));
    }
}
