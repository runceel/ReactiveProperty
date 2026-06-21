using System;
using System.Threading.Tasks;

namespace Reactive.Bindings.R3.Notifiers;

/// <summary>
/// Publishes messages filtered by type.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Sends a message to all subscribers.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="message">The message to publish.</param>
    void Publish<T>(T message);
}

/// <summary>
/// Subscribes to messages filtered by type.
/// </summary>
public interface IMessageSubscriber
{
    /// <summary>
    /// Subscribes to messages of the specified type.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="action">The action invoked when a message is published.</param>
    /// <returns>A disposable that unsubscribes when disposed.</returns>
    IDisposable Subscribe<T>(Action<T> action);
}

/// <summary>
/// In-memory pub/sub broker filtered by type.
/// </summary>
public interface IMessageBroker : IMessagePublisher, IMessageSubscriber
{
}

/// <summary>
/// Publishes messages filtered by type asynchronously.
/// </summary>
public interface IAsyncMessagePublisher
{
    /// <summary>
    /// Sends a message to all subscribers and awaits completion.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <returns>A task that completes when all subscribers have completed.</returns>
    Task PublishAsync<T>(T message);
}

/// <summary>
/// Subscribes to messages filtered by type asynchronously.
/// </summary>
public interface IAsyncMessageSubscriber
{
    /// <summary>
    /// Subscribes to messages of the specified type.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="asyncAction">The asynchronous action invoked when a message is published.</param>
    /// <returns>A disposable that unsubscribes when disposed.</returns>
    IDisposable Subscribe<T>(Func<T, Task> asyncAction);
}

/// <summary>
/// In-memory asynchronous pub/sub broker filtered by type.
/// </summary>
public interface IAsyncMessageBroker : IAsyncMessagePublisher, IAsyncMessageSubscriber
{
}
