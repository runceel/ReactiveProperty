using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Windows.Input;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings;

/// <summary>
/// <inheritdoc />
/// </summary>
public class ReactiveCommandSlim : ReactiveCommandSlim<object?>
{
    /// <summary>
    /// Create a ReactiveCommandSlim instance.
    /// </summary>
    public ReactiveCommandSlim() : base()
    {
    }

    /// <summary>
    /// Create a ReactiveCommandSlim instance.
    /// </summary>
    /// <param name="sharedCanExecute">The shared CanExecute status</param>
    public ReactiveCommandSlim(IReactiveProperty<bool> sharedCanExecute) : base(sharedCanExecute)
    {
    }

    /// <summary>
    /// Create a ReactiveCommandSlim instance.
    /// </summary>
    /// <param name="canExecuteSource">The CanExecute source</param>
    /// <param name="initialValue">The canExecuteSource initial value if not provided</param>
    public ReactiveCommandSlim(IObservable<bool> canExecuteSource, bool initialValue = true) : base(canExecuteSource, initialValue)
    {
    }

    /// <summary>
    /// Create a ReactiveCommandSlim instance.
    /// </summary>
    /// <param name="canExecuteSource">The CanExecute source</param>
    /// <param name="sharedCanExecute">The shared CanExecute status</param>
    /// <param name="initialValue">The canExecuteSource initial value if not provided</param>
    public ReactiveCommandSlim(IObservable<bool> canExecuteSource, IReactiveProperty<bool> sharedCanExecute, bool initialValue = true) : base(canExecuteSource, sharedCanExecute, initialValue)
    {
    }

    /// <summary>
    /// Push null to subscribers.
    /// </summary>
    public void Execute() => Execute(null);

    /// <summary>
    /// <see cref="ICommand.CanExecute(object?)" />
    /// </summary>
    public bool CanExecute() => CanExecute(null);

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    public IDisposable Subscribe(Action onNext)
        => this.Subscribe(new DelegateObserver(onNext));
}

/// <summary>
/// <see cref="ICommand"/> implementation for ReactiveProperty.
/// </summary>
public class ReactiveCommandSlim<T> : ICommand, IObservable<T?>, IObserver<bool>, IObserverLinkedList<T?>, IDisposable
{
    private readonly IReactiveProperty<bool> _sharedCanExecute;
    private readonly IReadOnlyReactiveProperty<bool>? _canExecuteSource;
    private readonly Action _disposeAction;
    private bool _canExecute = true;
    private ObserverNode<T?>? _root;
    private ObserverNode<T?>? _last;

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    public ReactiveCommandSlim() : this(new ReactivePropertySlim<bool>(true))
    {
    }

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    /// <param name="sharedCanExecute">The shared CanExecute status</param>
    public ReactiveCommandSlim(IReactiveProperty<bool> sharedCanExecute)
    {
        void canExecute_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not nameof(IReactiveProperty<bool>.Value)) return;

            _canExecute = _sharedCanExecute.Value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        _sharedCanExecute = sharedCanExecute;
        _sharedCanExecute.PropertyChanged += canExecute_PropertyChanged;
        _canExecute = _sharedCanExecute.Value;
        _disposeAction = () => _sharedCanExecute.PropertyChanged -= canExecute_PropertyChanged;
    }

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    /// <param name="canExecuteSource">The CanExecute source</param>
    /// <param name="initialValue">The canExecuteSource initial value if not provided</param>
    public ReactiveCommandSlim(IObservable<bool> canExecuteSource, bool initialValue = true) : this(canExecuteSource, new ReactivePropertySlim<bool>(true), initialValue)
    {
    }

    /// <summary>
    /// CanExecute is automatically changed when executing to false and finished to true.
    /// </summary>
    /// <param name="canExecuteSource">The CanExecute source</param>
    /// <param name="sharedCanExecute">The shared CanExecute status</param>
    /// <param name="initialValue">The canExecuteSource initial value if not provided</param>
    public ReactiveCommandSlim(IObservable<bool> canExecuteSource, IReactiveProperty<bool> sharedCanExecute, bool initialValue = true)
    {
        void canExecute_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not nameof(IReactiveProperty<bool>.Value)) return;

            var newValue = _sharedCanExecute.Value && _canExecuteSource!.Value;
            if (newValue == _canExecute) return;

            _canExecute = newValue;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        _sharedCanExecute = sharedCanExecute ?? new ReactivePropertySlim<bool>(true);
        _canExecuteSource = canExecuteSource.ToReadOnlyReactivePropertySlim(initialValue);
        _sharedCanExecute.PropertyChanged += canExecute_PropertyChanged;
        _canExecuteSource.PropertyChanged += canExecute_PropertyChanged;
        _canExecute = _sharedCanExecute.Value && _canExecuteSource.Value;
        _disposeAction = () =>
        {
            _sharedCanExecute.PropertyChanged -= canExecute_PropertyChanged;
            _canExecuteSource.PropertyChanged -= canExecute_PropertyChanged;
            _canExecuteSource.Dispose();
        };
    }

    /// <inheritdoc />
    public bool CanExecute(T? parameter) => _canExecute;

    /// <inheritdoc />
    public void Execute(T? parameter)
    {
        if (_canExecute is false) return;

        // call source.OnNext
        var node = _root;
        while (node != null)
        {
            node.OnNext(parameter);
            node = node.Next;
        }
    }

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    public IDisposable Subscribe(Action<T?> onNext)
        => this.Subscribe(new DelegateObserver<T?>(onNext));


    /// <inheritdoc />
    bool ICommand.CanExecute(object? parameter) => CanExecute((T?)parameter);

    /// <inheritdoc />
    void ICommand.Execute(object? parameter) => Execute((T?)parameter);

    /// <summary>
    /// Notifies the provider that an observer is to receive notifications.
    /// </summary>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <returns>
    /// A reference to an interface that allows observers to stop receiving notifications before
    /// the provider has finished sending them.
    /// </returns>
    public IDisposable Subscribe(IObserver<T?> observer)
    {
        if (IsDisposed)
        {
            observer.OnCompleted();
            return InternalDisposable.Empty;
        }

        // subscribe node, node as subscription.
        var next = new ObserverNode<T?>(this, observer);
        if (_root == null)
        {
            _root = _last = next;
        }
        else
        {
            _last!.Next = next;
            next.Previous = _last;
            _last = next;
        }
        return next;
    }

    void IObserverLinkedList<T?>.UnsubscribeNode(ObserverNode<T?> node)
    {
        if (node == _root)
        {
            _root = node.Next;
        }
        if (node == _last)
        {
            _last = node.Previous;
        }

        if (node.Previous != null)
        {
            node.Previous.Next = node.Next;
        }
        if (node.Next != null)
        {
            node.Next.Previous = node.Previous;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting
    /// unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        var node = _root;
        _root = _last = null;
        IsDisposed = true;
        _disposeAction();
        _canExecute = false;
        while (node != null)
        {
            node.OnCompleted();
            node = node.Next;
        }
    }

    void IObserver<bool>.OnCompleted() => _canExecute = false;

    void IObserver<bool>.OnError(Exception error) => ExceptionDispatchInfo.Capture(error).Throw();

    void IObserver<bool>.OnNext(bool value) => _canExecute = value;
}


/// <summary>
/// ReactiveCommand factory extension methods.
/// </summary>
public static class ReactiveCommandSlimExtensions
{
    /// <summary>
    /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
    /// </summary>
    public static ReactiveCommandSlim ToReactiveCommandSlim(this IObservable<bool> canExecuteSource, bool initialValue = true) =>
        new(canExecuteSource, initialValue);

    /// <summary>
    /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
    /// </summary>
    public static ReactiveCommandSlim<T> ToReactiveCommandSlim<T>(this IObservable<bool> canExecuteSource, bool initialValue = true) =>
        new(canExecuteSource, initialValue);

    /// <summary>
    /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
    /// </summary>
    public static ReactiveCommandSlim ToReactiveCommandSlim(this IObservable<bool> canExecuteSource, IReactiveProperty<bool> initialValue) =>
        new(canExecuteSource, initialValue);

    /// <summary>
    /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
    /// </summary>
    public static ReactiveCommandSlim<T> ToReactiveCommandSlim<T>(this IObservable<bool> canExecuteSource, IReactiveProperty<bool> initialValue) =>
        new(canExecuteSource, initialValue);

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    /// <param name="self">ReactiveCommand</param>
    /// <param name="onNext">Action</param>
    /// <param name="postProcess">Handling of the subscription.</param>
    /// <returns>Same of self argument</returns>
    public static ReactiveCommandSlim WithSubscribe(this ReactiveCommandSlim self, Action onNext, Action<IDisposable>? postProcess = null)
    {
        var d = self.Subscribe(new DelegateObserver(onNext));
        postProcess?.Invoke(d);
        return self;
    }

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    /// <typeparam name="T">ReactiveCommand type argument.</typeparam>
    /// <param name="self">ReactiveCommand</param>
    /// <param name="onNext">Action</param>
    /// <param name="postProcess">Handling of the subscription.</param>
    /// <returns>Same of self argument</returns>
    public static ReactiveCommandSlim<T> WithSubscribe<T>(this ReactiveCommandSlim<T> self, Action<T?> onNext, Action<IDisposable>? postProcess = null)
    {
        var d = self.Subscribe(new DelegateObserver<T?>(onNext));
        postProcess?.Invoke(d);
        return self;
    }

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    /// <param name="self">ReactiveCommand</param>
    /// <param name="onNext">Action</param>
    /// <param name="disposable">The return value of self.Subscribe(onNext)</param>
    /// <returns>Same of self argument</returns>
    public static ReactiveCommandSlim WithSubscribe(this ReactiveCommandSlim self, Action onNext, out IDisposable disposable)
    {
        disposable = self.Subscribe(new DelegateObserver(onNext));
        return self;
    }

    /// <summary>
    /// Subscribe execute.
    /// </summary>
    /// <typeparam name="T">ReactiveCommand type argument.</typeparam>
    /// <param name="self">ReactiveCommand</param>
    /// <param name="onNext">Action</param>
    /// <param name="disposable">The return value of self.Subscribe(onNext)</param>
    /// <returns>Same of self argument</returns>
    public static ReactiveCommandSlim<T> WithSubscribe<T>(this ReactiveCommandSlim<T> self, Action<T?> onNext, out IDisposable disposable)
    {
        disposable = self.Subscribe(new DelegateObserver<T?>(onNext));
        return self;
    }
}

