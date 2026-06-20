using System;
using System.Collections.Generic;
using System.Windows.Input;
using R3;
using Reactive.Bindings.R3Compat;

namespace Reactive.Bindings.R3Compat.Commands;

/// <summary>
/// Temporary ReactiveCommandSlim compatibility wrapper for R3 migrations.
/// </summary>
/// <typeparam name="T">The command parameter type.</typeparam>
/// <remarks>
/// Downgrade when the command no longer depends on the Slim API surface: use R3 ReactiveCommand directly.
/// Remove when Slim command usage is zero.
/// </remarks>
public sealed class ReactiveCommandSlimCompat<T> : ICommand, IDisposable
{
    private readonly List<Action<T>> _actions = new();
    private readonly IDisposable? _canExecuteSourceSubscription;
    private bool _canExecute = true;
    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="ReactiveCommandSlimCompat{T}"/> class.</summary>
    /// <param name="canExecuteSource">Optional R3 CanExecute source.</param>
    public ReactiveCommandSlimCompat(Observable<bool>? canExecuteSource = null)
    {
        _canExecuteSourceSubscription = canExecuteSource?.Subscribe(x =>
        {
            if (_canExecute == x)
            {
                return;
            }

            _canExecute = x;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        });
        CompatibilityTelemetry.Track("RP-CMD-SLIM", typeof(ReactiveCommandSlimCompat<T>).FullName ?? nameof(ReactiveCommandSlimCompat<T>));
    }

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;

    /// <summary>Adds a command action.</summary>
    /// <param name="action">The action.</param>
    /// <returns>This instance.</returns>
    public ReactiveCommandSlimCompat<T> WithSubscribe(Action<T> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        _actions.Add(action);
        return this;
    }

    /// <summary>Returns whether the command can execute.</summary>
    /// <param name="parameter">The command parameter.</param>
    public bool CanExecute(T parameter) => !_isDisposed && _canExecute;

    /// <summary>Executes the command.</summary>
    /// <param name="parameter">The command parameter.</param>
    public void Execute(T parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        foreach (var action in _actions.ToArray())
        {
            action(parameter);
        }
    }

    /// <inheritdoc />
    bool ICommand.CanExecute(object? parameter) => CanExecute((T)parameter!);

    /// <inheritdoc />
    void ICommand.Execute(object? parameter) => Execute((T)parameter!);

    /// <inheritdoc />
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _canExecuteSourceSubscription?.Dispose();
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
