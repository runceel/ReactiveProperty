using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using R3;
using Reactive.Bindings.R3Compat;

namespace Reactive.Bindings.R3Compat.Commands;

public sealed class AsyncReactiveCommandCompat<T> : ICommand, IDisposable
{
    private readonly IReactiveProperty<bool> _sharedCanExecute;
    private readonly List<Func<T, ValueTask>> _actions = [];
    private readonly List<IDisposable> _subscriptions = [];
    private readonly R3.ReactiveProperty<bool> _canExecuteState;
    private bool _sourceCanExecute;
    private bool _isDisposed;

    public AsyncReactiveCommandCompat(Observable<bool>? canExecuteSource = null)
        : this(canExecuteSource, new CompatReactiveProperty<bool>(true))
    {
    }

    public AsyncReactiveCommandCompat(IReactiveProperty<bool> sharedCanExecute)
        : this(null, sharedCanExecute)
    {
    }

    private AsyncReactiveCommandCompat(Observable<bool>? canExecuteSource, IReactiveProperty<bool> sharedCanExecute)
    {
        _sharedCanExecute = sharedCanExecute;
        _sourceCanExecute = canExecuteSource is null;
        _canExecuteState = new R3.ReactiveProperty<bool>(CurrentCanExecute());
        _sharedCanExecute.PropertyChanged += SharedCanExecutePropertyChanged;
        if (canExecuteSource != null)
        {
            _subscriptions.Add(canExecuteSource.Subscribe(value =>
            {
                _sourceCanExecute = value;
                PublishCanExecute();
            }));
        }

        CompatibilityTelemetry.Track("R3Compat.Commands.AsyncReactiveCommandCompat", "AsyncReactiveCommandCompat.ctor");
    }

    public event EventHandler? CanExecuteChanged;

    public Observable<bool> CanExecute => _canExecuteState;

    public AsyncReactiveCommandCompat<T> WithSubscribe(Func<T, ValueTask> asyncAction)
    {
        ArgumentNullException.ThrowIfNull(asyncAction);
        _actions.Add(asyncAction);
        return this;
    }

    public async ValueTask ExecuteAsync(T parameter)
    {
        if (!CurrentCanExecute())
        {
            return;
        }

        _sharedCanExecute.Value = false;
        try
        {
            foreach (var action in _actions.ToArray())
            {
                await action(parameter);
            }
        }
        finally
        {
            if (!_isDisposed)
            {
                _sharedCanExecute.Value = true;
            }
        }
    }

    bool ICommand.CanExecute(object? parameter) => CurrentCanExecute();

    async void ICommand.Execute(object? parameter) => await ExecuteAsync((T)parameter!);

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _sharedCanExecute.PropertyChanged -= SharedCanExecutePropertyChanged;
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }

        _canExecuteState.Value = false;
        _canExecuteState.Dispose();
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool CurrentCanExecute() => !_isDisposed && _sharedCanExecute.Value && _sourceCanExecute;

    private void SharedCanExecutePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IReactiveProperty<bool>.Value))
        {
            PublishCanExecute();
        }
    }

    private void PublishCanExecute()
    {
        var value = CurrentCanExecute();
        if (_canExecuteState.Value == value)
        {
            return;
        }

        _canExecuteState.Value = value;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
