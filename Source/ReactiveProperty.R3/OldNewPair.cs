namespace Reactive.Bindings.R3;

/// <summary>
/// Represents old and new values.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public readonly struct OldNewPair<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OldNewPair{T}"/> struct.
    /// </summary>
    /// <param name="oldItem">Old value.</param>
    /// <param name="newItem">New value.</param>
    public OldNewPair(T oldItem, T newItem)
    {
        OldItem = oldItem;
        NewItem = newItem;
    }

    /// <summary>
    /// Gets the old value.
    /// </summary>
    public T OldItem { get; }

    /// <summary>
    /// Gets the new value.
    /// </summary>
    public T NewItem { get; }
}
