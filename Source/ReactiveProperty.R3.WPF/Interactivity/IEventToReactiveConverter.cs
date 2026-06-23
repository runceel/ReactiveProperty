using R3;

namespace Reactive.Bindings.R3.Interactivity;

/// <summary>
/// Converts the event stream raised by a <see cref="EventToReactiveProperty"/> or
/// <see cref="EventToReactiveCommand"/> before the value is pushed to the target reactive property or command.
/// </summary>
public interface IEventToReactiveConverter
{
    /// <summary>
    /// Gets or sets the <see cref="System.Windows.FrameworkElement"/> the trigger action is attached to.
    /// </summary>
    object? AssociateObject { get; set; }

    /// <summary>
    /// Converts the specified source event stream.
    /// </summary>
    /// <param name="source">The source event stream.</param>
    /// <returns>The converted stream.</returns>
    Observable<object?> Convert(Observable<object?> source);
}
