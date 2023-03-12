using System.Windows.Input;
using Microsoft.AspNetCore.Components;

namespace Reactive.Bindings;

/// <summary>
/// Command extension methods for Blazor.
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// Convert from ReactiveCommand to EventCallback.
    /// </summary>
    /// <typeparam name="TValue">The type argument of EventCallback</typeparam>
    /// <param name="command">The command</param>
    /// <returns>EventCallback instance</returns>
    public static EventCallback<TValue> ToEvent<TValue>(this ReactiveCommand command) => new EventCallback<TValue>(null, (TValue _) => command.Execute());

    /// <summary>
    /// Convert from ReactiveCommand to EventCallback.
    /// </summary>
    /// <typeparam name="TValue">The type argument of EventCallback</typeparam>
    /// <param name="command">The command</param>
    /// <returns>EventCallback instance</returns>
    public static EventCallback<TValue> ToEvent<TValue>(this ReactiveCommand<TValue> command) => new EventCallback<TValue>(null, command.Execute);

    /// <summary>
    /// Convert from ReactiveCommand to EventCallback.
    /// </summary>
    /// <typeparam name="TValue">The type argument of EventCallback</typeparam>
    /// <param name="command">The command</param>
    /// <returns>EventCallback instance</returns>
    public static EventCallback<TValue> ToEvent<TValue>(this ReactiveCommandSlim command) => new EventCallback<TValue>(null, (TValue _) => command.Execute());

    /// <summary>
    /// Convert from ReactiveCommand to EventCallback.
    /// </summary>
    /// <typeparam name="TValue">The type argument of EventCallback</typeparam>
    /// <param name="command">The command</param>
    /// <returns>EventCallback instance</returns>
    public static EventCallback<TValue> ToEvent<TValue>(this ReactiveCommandSlim<TValue> command) => new EventCallback<TValue>(null, command.Execute);

    /// <summary>
    /// Convert from ReactiveCommand to EventCallback.
    /// </summary>
    /// <typeparam name="TValue">The type argument of EventCallback</typeparam>
    /// <param name="command">The command</param>
    /// <returns>EventCallback instance</returns>
    public static EventCallback<TValue> ToEvent<TValue>(this AsyncReactiveCommand command) => new EventCallback<TValue>(null, (TValue _) => command.ExecuteAsync());

    /// <summary>
    /// Convert from ReactiveCommand to EventCallback.
    /// </summary>
    /// <typeparam name="TValue">The type argument of EventCallback</typeparam>
    /// <param name="command">The command</param>
    /// <returns>EventCallback instance</returns>
    public static EventCallback<TValue> ToEvent<TValue>(this AsyncReactiveCommand<TValue> command) => new EventCallback<TValue>(null, command.ExecuteAsync);

    /// <summary>
    /// Convert from ReactiveCommand to EventCallback.
    /// </summary>
    /// <param name="command">The command</param>
    /// <returns>EventCallback instance</returns>
    public static EventCallback ToEvent(this ReactiveCommand command) => new EventCallback(null, () => command.Execute());

    /// <summary>
    /// Convert from ReactiveCommand to EventCallback.
    /// </summary>
    /// <param name="command">The command</param>
    /// <returns>EventCallback instance</returns>
    public static EventCallback ToEvent(this ReactiveCommandSlim command) => new EventCallback(null, () => command.Execute());

    /// <summary>
    /// Convert from ReactiveCommand to EventCallback.
    /// </summary>
    /// <param name="command">The command</param>
    /// <returns>EventCallback instance</returns>
    public static EventCallback ToEvent(this AsyncReactiveCommand command) => new EventCallback(null, () => command.ExecuteAsync());

    /// <summary>
    /// Return a boolean value that is inverted CanExecute method.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>Inverted value of CanExecute method.</returns>
    public static bool IsDisabled(this ICommand command)
    {
        return !command.CanExecute(null);
    }
}
