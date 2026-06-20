using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using R3;
using Reactive.Bindings.R3Compat;

namespace Reactive.Bindings.R3Compat.Commands;

public sealed class ReactiveCommandSlimCompat<T> : ICommand, IDisposable
{
    private readonly List<Action<T>> _actions = [];
    private readonly List<IDisposable> _subscriptions = [];
    private bool _canExecute;
    private bool _isDisposed;

    public ReactiveCommandSlimCompat(Observable<bool>? canExecuteSource = null)
    {
        _canExecute = canExecuteSource is null;
        if (canExecuteSource != null)
        {
            _subscriptions.Add(canExecuteSource.Subscribe(value =>
            {
                if (_canExecute == value)
                {
                    return;
                }

                _canExecute = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }));
        }

        CompatibilityTelemetry.Track("R3Compat.Commands.ReactiveCommandSlimCompat", ".ctor");
    }

    public event EventHandler? CanExecuteChanged;

    public ReactiveCommandSlimCompat<T> WithSubscribe(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        _actions.Add(action);
        return this;
    }

    public bool CanExecute(T parameter) => !_isDisposed && _canExecute;

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

    bool ICommand.CanExecute(object? parameter) => CanExecute((T)parameter!);

    void ICommand.Execute(object? parameter) => Execute((T)parameter!);

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

        if (_canExecute)
        {
            _canExecute = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
