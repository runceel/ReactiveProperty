using R3;

namespace Reactive.Bindings.R3.Interactivity;

/// <summary>
/// A strongly typed base class for <see cref="IEventToReactiveConverter"/>.
/// </summary>
/// <typeparam name="T">The source value type.</typeparam>
/// <typeparam name="U">The converted value type.</typeparam>
public abstract class ReactiveConverter<T, U> : IEventToReactiveConverter
{
    /// <inheritdoc />
    public object? AssociateObject { get; set; }

    Observable<object?> IEventToReactiveConverter.Convert(Observable<object?> source) =>
        OnConvert(source.Select(static x => (T?)x)).Select(static x => (object?)x);

    /// <summary>
    /// Converts the strongly typed source stream.
    /// </summary>
    /// <param name="source">The strongly typed source stream.</param>
    /// <returns>The converted stream.</returns>
    protected abstract Observable<U?> OnConvert(Observable<T?> source);
}
