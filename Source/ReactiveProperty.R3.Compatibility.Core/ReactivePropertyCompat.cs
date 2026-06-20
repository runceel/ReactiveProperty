using System;
using System.ComponentModel;
using R3;

namespace Reactive.Bindings.R3Compat;

/// <summary>
/// Minimal R3-backed property for sharing compatibility command state.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public sealed class ReactivePropertyCompat<T> : IReactiveProperty<T>
{
    private readonly BindableReactiveProperty<T> _source;
    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="ReactivePropertyCompat{T}"/> class.</summary>
    /// <param name="initialValue">The initial value.</param>
    public ReactivePropertyCompat(T initialValue)
    {
        _source = new BindableReactiveProperty<T>(initialValue);
        _source.PropertyChanged += SourcePropertyChanged;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public T Value
    {
        get => _source.Value;
        set
        {
            if (_isDisposed)
            {
                return;
            }

            _source.Value = value;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _source.PropertyChanged -= SourcePropertyChanged;
        _source.Dispose();
    }

    private void SourcePropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
}
