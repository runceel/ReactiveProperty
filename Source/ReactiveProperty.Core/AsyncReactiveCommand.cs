using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings;

/// <summary>
/// Represents AsyncReactiveCommand&lt;object&gt;
/// </summary>
public class AsyncReactiveCommand : AsyncReactiveCommand<object?>
{
    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public AsyncReactiveCommand()
        : base()
    {
    }

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public AsyncReactiveCommand(IObservable<bool> canExecuteSource)
        : base(canExecuteSource)
    {
    }

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public AsyncReactiveCommand(IObservable<bool> canExecuteSource, IReactiveProperty<bool> sharedCanExecute)
        : base(canExecuteSource, sharedCanExecute)
    {
    }

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true. The
    /// source is shared between other AsyncReactiveCommand.
    /// </summary>
    public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute)
        : base(sharedCanExecute)
    {
    }

    /// <summary>
    /// Push null to subscribers.
    /// </summary>
    public void Execute() => Execute(null);

    /// <summary>
    /// Push null to subscribers.
    /// </summary>
    public Task ExecuteAsync() => ExecuteAsync(null);

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    public IDisposable Subscribe(Func<Task> asyncAction)
        => Subscribe(async _ => await asyncAction());
}

/// <summary>
/// Async version ReactiveCommand
/// </summary>
/// <typeparam name="T"></typeparam>
public class AsyncReactiveCommand<T> : ICommand, IDisposable
{
    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    /// <returns></returns>
    public event EventHandler? CanExecuteChanged;

    private readonly object _gate = new();
    private readonly IReactiveProperty<bool> _canExecute;
    private readonly IReadOnlyReactiveProperty<bool>? _canExecuteSource;
    private bool _isCanExecute;
    private readonly Action _disposeAction;
    private bool _isDisposed = false;
    private ImmutableList<Func<T, Task>> _asyncActions = ImmutableList<Func<T, Task>>.Empty;

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public AsyncReactiveCommand() : this(new ReactivePropertySlim<bool>(true))
    {
    }

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public AsyncReactiveCommand(IObservable<bool> canExecuteSource)
        : this(canExecuteSource, null)
    {
    }

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public AsyncReactiveCommand(IObservable<bool> canExecuteSource, IReactiveProperty<bool>? sharedCanExecute)
    {
        void canExecute_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not nameof(IReactiveProperty<bool>.Value)) return;

            var newValue = _canExecute.Value && _canExecuteSource!.Value;
            if (newValue == _isCanExecute) return;

            _isCanExecute = newValue;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        _canExecute = sharedCanExecute ?? new ReactivePropertySlim<bool>(true);
        _canExecute.PropertyChanged += canExecute_PropertyChanged;
        _canExecuteSource = canExecuteSource.ToReadOnlyReactivePropertySlim();
        _canExecuteSource.PropertyChanged += canExecute_PropertyChanged;

        _isCanExecute = _canExecute.Value && _canExecuteSource.Value;

        _disposeAction = () =>
        {
            _canExecute.PropertyChanged -= canExecute_PropertyChanged;
            _canExecuteSource.PropertyChanged -= canExecute_PropertyChanged;
            _canExecuteSource.Dispose();
        };
    }

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true. The
    /// source is shared between other AsyncReactiveCommand.
    /// </summary>
    public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute)
    {
        void canExecute_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not nameof(IReactiveProperty<bool>.Value)) return;

            _isCanExecute = _canExecute.Value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        _canExecute = sharedCanExecute;
        _canExecute.PropertyChanged += canExecute_PropertyChanged;
        _isCanExecute = _canExecute.Value;
        _disposeAction = () => _canExecute.PropertyChanged -= canExecute_PropertyChanged;
    }

    /// <summary>
    /// Return current canExecute status.
    /// </summary>
    public bool CanExecute()
    {
        return _isDisposed ? false : _isCanExecute;
    }

    /// <summary>
    /// Return current canExecute status. parameter is ignored.
    /// </summary>
    bool ICommand.CanExecute(object? parameter)
    {
        return _isDisposed ? false : _isCanExecute;
    }

    /// <summary>
    /// Push parameter to subscribers, when executing CanExecuting is changed to false.
    /// </summary>
    public async void Execute(T parameter) => await ExecuteAsync(parameter);

    /// <summary>
    /// Push parameter to subscribers, when executing CanExecuting is changed to false.
    /// </summary>
    public async Task ExecuteAsync(T parameter)
    {
        if (_isCanExecute)
        {
            _canExecute.Value = false;
            var a = _asyncActions.Data;

            if (a.Length == 1)
            {
                try
                {
                    var asyncState = a[0].Invoke(parameter) ?? Task.CompletedTask;
                    await asyncState;
                }
                finally
                {
                    _canExecute.Value = true;
                }
            }
            else
            {
                var xs = new Task[a.Length];
                try
                {
                    for (var i = 0; i < a.Length; i++)
                    {
                        xs[i] = a[i].Invoke(parameter) ?? Task.CompletedTask;
                    }

                    await Task.WhenAll(xs);
                }
                finally
                {
                    _canExecute.Value = true;
                }
            }
        }
    }

    /// <summary>
    /// Push parameter to subscribers, when executing CanExecuting is changed to false.
    /// </summary>
    void ICommand.Execute(object? parameter) => Execute((T)parameter!);

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    public IDisposable Subscribe(Func<T, Task> asyncAction)
    {
        lock (_gate)
        {
            _asyncActions = _asyncActions.Add(asyncAction);
        }

        return new Subscription(this, asyncAction);
    }

    /// <summary>
    /// Stop all subscription and lock CanExecute is false.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _disposeAction();
        if (_isCanExecute)
        {
            _isCanExecute = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private class Subscription : IDisposable
    {
        private readonly AsyncReactiveCommand<T> parent;
        private readonly Func<T, Task> asyncAction;

        public Subscription(AsyncReactiveCommand<T> parent, Func<T, Task> asyncAction)
        {
            this.parent = parent;
            this.asyncAction = asyncAction;
        }

        public void Dispose()
        {
            lock (parent._gate)
            {
                parent._asyncActions = parent._asyncActions.Remove(asyncAction);
            }
        }
    }
}

/// <summary>
/// AsyncReactiveCommand factory and extension methods.
/// </summary>
public static class AsyncReactiveCommandExtensions
{
    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public static AsyncReactiveCommand ToAsyncReactiveCommand(this IObservable<bool> canExecuteSource) =>
        new(canExecuteSource);

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public static AsyncReactiveCommand ToAsyncReactiveCommand(this IObservable<bool> canExecuteSource, IReactiveProperty<bool> sharedCanExecute) =>
        new(canExecuteSource, sharedCanExecute);

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IObservable<bool> canExecuteSource) =>
        new(canExecuteSource);

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IObservable<bool> canExecuteSource, IReactiveProperty<bool> sharedCanExecute) =>
        new(canExecuteSource, sharedCanExecute);

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true. The
    /// source is shared between other AsyncReactiveCommand.
    /// </summary>
    public static AsyncReactiveCommand ToAsyncReactiveCommand(this IReactiveProperty<bool> sharedCanExecute) =>
        new(sharedCanExecute);

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true. The
    /// source is shared between other AsyncReactiveCommand.
    /// </summary>
    public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IReactiveProperty<bool> sharedCanExecute) =>
        new(sharedCanExecute);

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    /// <param name="self">AsyncReactiveCommand</param>
    /// <param name="asyncAction">Action</param>
    /// <param name="postProcess">Handling of the subscription.</param>
    /// <returns>Same of self argument</returns>
    public static AsyncReactiveCommand WithSubscribe(this AsyncReactiveCommand self, Func<Task> asyncAction, Action<IDisposable>? postProcess = null)
    {
        var d = self.Subscribe(asyncAction);
        postProcess?.Invoke(d);
        return self;
    }

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    /// <typeparam name="T">AsyncReactiveCommand type argument.</typeparam>
    /// <param name="self">AsyncReactiveCommand</param>
    /// <param name="asyncAction">Action</param>
    /// <param name="postProcess">Handling of the subscription.</param>
    /// <returns>Same of self argument</returns>
    public static AsyncReactiveCommand<T> WithSubscribe<T>(this AsyncReactiveCommand<T> self, Func<T, Task> asyncAction, Action<IDisposable>? postProcess = null)
    {
        var d = self.Subscribe(asyncAction);
        postProcess?.Invoke(d);
        return self;
    }

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    /// <param name="self">AsyncReactiveCommand</param>
    /// <param name="asyncAction">Action</param>
    /// <param name="disposable">The return value of self.Subscribe(asyncAction)</param>
    /// <returns>Same of self argument</returns>
    public static AsyncReactiveCommand WithSubscribe(this AsyncReactiveCommand self, Func<Task> asyncAction, out IDisposable disposable)
    {
        disposable = self.Subscribe(asyncAction);
        return self;
    }

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    /// <typeparam name="T">AsyncReactiveCommand type argument.</typeparam>
    /// <param name="self">AsyncReactiveCommand</param>
    /// <param name="asyncAction">Action</param>
    /// <param name="disposable">The return value of self.Subscribe(asyncAction)</param>
    /// <returns>Same of self argument</returns>
    public static AsyncReactiveCommand<T> WithSubscribe<T>(this AsyncReactiveCommand<T> self, Func<T, Task> asyncAction, out IDisposable disposable)
    {
        disposable = self.Subscribe(asyncAction);
        return self;
    }
}
