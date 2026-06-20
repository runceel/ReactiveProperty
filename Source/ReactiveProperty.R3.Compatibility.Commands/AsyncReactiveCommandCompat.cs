using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using R3;
using Reactive.Bindings.R3Compat;

namespace Reactive.Bindings.R3Compat.Commands;

/// <summary>
/// Temporary AsyncReactiveCommand compatibility wrapper for R3 migrations.
/// </summary>
/// <typeparam name="T">The command parameter type.</typeparam>
/// <remarks>
/// Downgrade when shared CanExecute and re-entrancy control are not required: use R3 ReactiveCommand directly.
/// Remove when shared CanExecute usage is zero.
/// </remarks>
public sealed class AsyncReactiveCommandCompat<T> : ICommand, IDisposable
{
    private Func<T, ValueTask>[] _actions = Array.Empty<Func<T, ValueTask>>();
    private readonly IReactiveProperty<bool> _sharedCanExecute;
    private readonly bool _ownsSharedCanExecute;
    private readonly IDisposable? _canExecuteSourceSubscription;
    private readonly BehaviorSubject<bool> _canExecute;
    private bool _sourceCanExecute = true;
    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="AsyncReactiveCommandCompat{T}"/> class.</summary>
    /// <param name="canExecuteSource">Optional R3 CanExecute source.</param>
    public AsyncReactiveCommandCompat(Observable<bool>? canExecuteSource = null)
        : this(new ReactivePropertyCompat<bool>(true), true, canExecuteSource)
    {
    }

    /// <summary>Initializes a new instance sharing CanExecute state with other commands.</summary>
    /// <param name="sharedCanExecute">The shared CanExecute state.</param>
    public AsyncReactiveCommandCompat(IReactiveProperty<bool> sharedCanExecute)
        : this(sharedCanExecute, false, null)
    {
    }

    private AsyncReactiveCommandCompat(IReactiveProperty<bool> sharedCanExecute, bool ownsSharedCanExecute, Observable<bool>? canExecuteSource)
    {
        _sharedCanExecute = sharedCanExecute ?? throw new ArgumentNullException(nameof(sharedCanExecute));
        _ownsSharedCanExecute = ownsSharedCanExecute;
        _canExecute = new BehaviorSubject<bool>(CurrentCanExecute);
        _sharedCanExecute.PropertyChanged += SharedCanExecutePropertyChanged;
        _canExecuteSourceSubscription = canExecuteSource?.Subscribe(x =>
        {
            _sourceCanExecute = x;
            PublishCanExecuteChanged();
        });
        CompatibilityTelemetry.Track("RP-CMD-002", typeof(AsyncReactiveCommandCompat<T>).FullName ?? nameof(AsyncReactiveCommandCompat<T>));
    }

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;

    /// <summary>Gets an R3 observable for CanExecute changes.</summary>
    public Observable<bool> CanExecute => _canExecute;

    /// <summary>Adds an async command action.</summary>
    /// <param name="asyncAction">The async action.</param>
    /// <returns>This instance.</returns>
    public AsyncReactiveCommandCompat<T> WithSubscribe(Func<T, ValueTask> asyncAction)
    {
        if (asyncAction == null)
        {
            throw new ArgumentNullException(nameof(asyncAction));
        }

        var actions = new Func<T, ValueTask>[_actions.Length + 1];
        Array.Copy(_actions, actions, _actions.Length);
        actions[actions.Length - 1] = asyncAction;
        _actions = actions;
        return this;
    }

    /// <summary>Executes the command asynchronously.</summary>
    /// <param name="parameter">The command parameter.</param>
    public async ValueTask ExecuteAsync(T parameter)
    {
        if (!CurrentCanExecute)
        {
            return;
        }

        _sharedCanExecute.Value = false;
        try
        {
            foreach (var action in _actions)
            {
                await action(parameter).ConfigureAwait(false);
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

    /// <inheritdoc />
    bool ICommand.CanExecute(object? parameter) => CurrentCanExecute;

    /// <inheritdoc />
    /// <remarks>Use <see cref="ExecuteAsync(T)"/> directly when callers need to observe exceptions.</remarks>
    async void ICommand.Execute(object? parameter) => await ExecuteAsync((T)parameter!).ConfigureAwait(false);

    /// <inheritdoc />
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _sharedCanExecute.PropertyChanged -= SharedCanExecutePropertyChanged;
        _canExecuteSourceSubscription?.Dispose();
        _canExecute.OnNext(false);
        _canExecute.Dispose();
        if (_ownsSharedCanExecute)
        {
            _sharedCanExecute.Dispose();
        }

        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool CurrentCanExecute => !_isDisposed && _sharedCanExecute.Value && _sourceCanExecute;

    private void SharedCanExecutePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IReactiveProperty<bool>.Value))
        {
            PublishCanExecuteChanged();
        }
    }

    private void PublishCanExecuteChanged()
    {
        var value = CurrentCanExecute;
        _canExecute.OnNext(value);
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
