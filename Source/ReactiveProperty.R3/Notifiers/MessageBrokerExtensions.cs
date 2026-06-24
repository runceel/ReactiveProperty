using System;
using R3;

namespace Reactive.Bindings.R3.Notifiers;

/// <summary>
/// Extension methods for message brokers.
/// </summary>
public static class MessageBrokerExtensions
{
    /// <summary>
    /// Converts <see cref="IMessageSubscriber.Subscribe{T}(Action{T})"/> to an R3 <see cref="Observable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="subscriber">The message subscriber.</param>
    /// <returns>An observable that emits published messages of type <typeparamref name="T"/>.</returns>
    public static Observable<T> ToObservable<T>(this IMessageSubscriber subscriber)
    {
        if (subscriber is null)
        {
            throw new ArgumentNullException(nameof(subscriber));
        }

        return Observable.Create<T>(observer =>
        {
            var gate = new object();
            return subscriber.Subscribe<T>(x =>
            {
                lock (gate)
                {
                    observer.OnNext(x);
                }
            });
        });
    }
}
