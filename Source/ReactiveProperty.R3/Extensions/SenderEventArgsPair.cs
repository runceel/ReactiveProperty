namespace Reactive.Bindings.R3.Extensions;

/// <summary>
/// Represents an event sender and event arguments pair.
/// </summary>
/// <typeparam name="TSender">The sender type.</typeparam>
/// <typeparam name="TEventArgs">The event arguments type.</typeparam>
public sealed class SenderEventArgsPair<TSender, TEventArgs>
{
    internal SenderEventArgsPair(TSender sender, TEventArgs eventArgs)
    {
        Sender = sender;
        EventArgs = eventArgs;
    }

    /// <summary>
    /// Gets the event sender.
    /// </summary>
    public TSender Sender { get; }

    /// <summary>
    /// Gets the event arguments.
    /// </summary>
    public TEventArgs EventArgs { get; }
}
