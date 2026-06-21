namespace Reactive.Bindings.R3.Notifiers;

/// <summary>
/// Represents the change status reported by <see cref="CountNotifier"/>.
/// </summary>
public enum CountChangedStatus
{
    /// <summary>
    /// The count was incremented.
    /// </summary>
    Increment,

    /// <summary>
    /// The count was decremented.
    /// </summary>
    Decrement,

    /// <summary>
    /// The count reached zero.
    /// </summary>
    Empty,

    /// <summary>
    /// The count reached its maximum value.
    /// </summary>
    Max,
}
