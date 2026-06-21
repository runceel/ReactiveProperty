using System;
using System.Threading.Tasks;
using R3;

namespace Reactive.Bindings.R3;

/// <summary>
/// Provides factory and fluent extensions for asynchronous commands.
/// </summary>
public static class AsyncReactiveCommandExtensions
{
    /// <summary>
    /// Converts a can-execute observable to an asynchronous command.
    /// </summary>
    public static AsyncReactiveCommand ToAsyncReactiveCommand(this Observable<bool> canExecuteSource) => new(canExecuteSource);

    /// <summary>
    /// Converts a can-execute observable to an asynchronous command.
    /// </summary>
    public static AsyncReactiveCommand ToAsyncReactiveCommand(this Observable<bool> canExecuteSource, ReactiveProperty<bool> sharedCanExecute) => new(canExecuteSource, sharedCanExecute);

    /// <summary>
    /// Converts a can-execute observable to an asynchronous command.
    /// </summary>
    public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this Observable<bool> canExecuteSource) => new(canExecuteSource);

    /// <summary>
    /// Converts a can-execute observable to an asynchronous command.
    /// </summary>
    public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this Observable<bool> canExecuteSource, ReactiveProperty<bool> sharedCanExecute) => new(canExecuteSource, sharedCanExecute);

    /// <summary>
    /// Converts a shared can-execute property to an asynchronous command.
    /// </summary>
    public static AsyncReactiveCommand ToAsyncReactiveCommand(this ReactiveProperty<bool> sharedCanExecute) => new(sharedCanExecute);

    /// <summary>
    /// Converts a shared can-execute property to an asynchronous command.
    /// </summary>
    public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this ReactiveProperty<bool> sharedCanExecute) => new(sharedCanExecute);

    /// <summary>
    /// Subscribes an asynchronous action and returns the command.
    /// </summary>
    public static AsyncReactiveCommand WithSubscribe(this AsyncReactiveCommand self, Func<Task> asyncAction, Action<IDisposable>? postProcess = null)
    {
        if (self is null)
        {
            throw new ArgumentNullException(nameof(self));
        }

        var subscription = self.Subscribe(asyncAction);
        postProcess?.Invoke(subscription);
        return self;
    }

    /// <summary>
    /// Subscribes an asynchronous action and returns the command.
    /// </summary>
    public static AsyncReactiveCommand<T> WithSubscribe<T>(this AsyncReactiveCommand<T> self, Func<T, Task> asyncAction, Action<IDisposable>? postProcess = null)
    {
        if (self is null)
        {
            throw new ArgumentNullException(nameof(self));
        }

        var subscription = self.Subscribe(asyncAction);
        postProcess?.Invoke(subscription);
        return self;
    }
}

/// <summary>
/// Provides fluent extensions for R3 commands.
/// </summary>
public static class ReactiveCommandExtensions
{
    /// <summary>
    /// Subscribes an action and returns the command.
    /// </summary>
    public static ReactiveCommand<T> WithSubscribe<T>(this ReactiveCommand<T> self, Action<T> action, Action<IDisposable>? postProcess = null)
    {
        if (self is null)
        {
            throw new ArgumentNullException(nameof(self));
        }

        var subscription = self.Subscribe(action);
        postProcess?.Invoke(subscription);
        return self;
    }
}
