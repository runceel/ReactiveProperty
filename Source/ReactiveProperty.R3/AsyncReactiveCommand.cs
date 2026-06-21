using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using R3;

namespace Reactive.Bindings.R3;

/// <summary>
/// Represents an asynchronous command.
/// </summary>
/// <typeparam name="T">Parameter type.</typeparam>
public class AsyncReactiveCommand<T> : ICommand, IDisposable
{
    private readonly object _syncRoot = new();
    private readonly List<Func<T, Task>> _asyncActions = new();
    private readonly ReactiveProperty<bool> _sharedCanExecute;
    private readonly IDisposable? _canExecuteSubscription;
    private readonly bool _ownsSharedCanExecute;
    private bool _sourceCanExecute = true;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncReactiveCommand{T}"/> class.
    /// </summary>
    public AsyncReactiveCommand()
        : this(new ReactiveProperty<bool>(true), true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncReactiveCommand{T}"/> class.
    /// </summary>
    public AsyncReactiveCommand(Observable<bool> canExecuteSource)
        : this(canExecuteSource, new ReactiveProperty<bool>(true), true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncReactiveCommand{T}"/> class.
    /// </summary>
    public AsyncReactiveCommand(ReactiveProperty<bool> sharedCanExecute)
        : this(sharedCanExecute, false)
    {
    }

    private AsyncReactiveCommand(ReactiveProperty<bool> sharedCanExecute, bool ownsSharedCanExecute)
    {
        _sharedCanExecute = sharedCanExecute ?? throw new ArgumentNullException(nameof(sharedCanExecute));
        _ownsSharedCanExecute = ownsSharedCanExecute;
        _canExecuteSubscription = _sharedCanExecute.Subscribe(_ => OnCanExecuteChanged());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncReactiveCommand{T}"/> class.
    /// </summary>
    public AsyncReactiveCommand(Observable<bool> canExecuteSource, ReactiveProperty<bool> sharedCanExecute)
        : this(canExecuteSource, sharedCanExecute, false)
    {
    }

    private AsyncReactiveCommand(Observable<bool> canExecuteSource, ReactiveProperty<bool> sharedCanExecute, bool ownsSharedCanExecute)
        : this(sharedCanExecute, ownsSharedCanExecute)
    {
        if (canExecuteSource is null)
        {
            throw new ArgumentNullException(nameof(canExecuteSource));
        }

        _sourceCanExecute = false;
        var sharedSubscription = _canExecuteSubscription;
        var sourceSubscription = canExecuteSource.Subscribe(x =>
            {
                _sourceCanExecute = x;
                OnCanExecuteChanged();
            });
        _canExecuteSubscription = Disposable.Create(() =>
        {
            sharedSubscription?.Dispose();
            sourceSubscription.Dispose();
        });
    }

    /// <summary>
    /// Occurs when the executable state changes.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Gets a value indicating whether this command can execute.
    /// </summary>
    public bool CanExecute() => !_isDisposed && _sourceCanExecute && _sharedCanExecute.Value;

    /// <inheritdoc />
    bool ICommand.CanExecute(object? parameter) => CanExecute();

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    public async Task ExecuteAsync(T parameter)
    {
        Func<T, Task>[] actions;
        lock (_syncRoot)
        {
            if (!CanExecute())
            {
                return;
            }

            actions = [.. _asyncActions];
            _sharedCanExecute.Value = false;
        }

        try
        {
            if (actions.Length == 1)
            {
                var actionTask = actions[0](parameter) ?? Task.CompletedTask;
                await actionTask.ConfigureAwait(false);
            }
            else
            {
                var tasks = new Task[actions.Length];
                for (var i = 0; i < actions.Length; i++)
                {
                    tasks[i] = actions[i](parameter) ?? Task.CompletedTask;
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }
        finally
        {
            if (ShouldRestoreSharedCanExecuteAfterRun)
            {
                _sharedCanExecute.Value = true;
            }
        }
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    public void Execute(T parameter) => _ = ExecuteAsync(parameter);

    /// <inheritdoc />
    void ICommand.Execute(object? parameter) => Execute((T)parameter!);

    /// <summary>
    /// Adds an asynchronous action.
    /// </summary>
    public IDisposable Subscribe(Func<T, Task> asyncAction)
    {
        if (asyncAction is null)
        {
            throw new ArgumentNullException(nameof(asyncAction));
        }

        lock (_syncRoot)
        {
            _asyncActions.Add(asyncAction);
        }

        return Disposable.Create(() =>
        {
            lock (_syncRoot)
            {
                _asyncActions.Remove(asyncAction);
            }
        });
    }

    /// <summary>
    /// Disposes this command.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        if (_ownsSharedCanExecute)
        {
            _sharedCanExecute.Value = false;
        }

        _canExecuteSubscription?.Dispose();
        OnCanExecuteChanged();
    }

    private void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    private bool ShouldRestoreSharedCanExecuteAfterRun => !_isDisposed || !_ownsSharedCanExecute;
}

/// <summary>
/// Represents an asynchronous command without a parameter.
/// </summary>
public class AsyncReactiveCommand : AsyncReactiveCommand<object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncReactiveCommand"/> class.
    /// </summary>
    public AsyncReactiveCommand()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncReactiveCommand"/> class.
    /// </summary>
    public AsyncReactiveCommand(Observable<bool> canExecuteSource)
        : base(canExecuteSource)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncReactiveCommand"/> class.
    /// </summary>
    public AsyncReactiveCommand(ReactiveProperty<bool> sharedCanExecute)
        : base(sharedCanExecute)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncReactiveCommand"/> class.
    /// </summary>
    public AsyncReactiveCommand(Observable<bool> canExecuteSource, ReactiveProperty<bool> sharedCanExecute)
        : base(canExecuteSource, sharedCanExecute)
    {
    }

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    public Task ExecuteAsync() => ExecuteAsync(null);

    /// <summary>
    /// Executes the command.
    /// </summary>
    public void Execute() => Execute(null);

    /// <summary>
    /// Adds an asynchronous action.
    /// </summary>
    public IDisposable Subscribe(Func<Task> asyncAction)
    {
        if (asyncAction is null)
        {
            throw new ArgumentNullException(nameof(asyncAction));
        }

        return Subscribe(_ => asyncAction());
    }
}
