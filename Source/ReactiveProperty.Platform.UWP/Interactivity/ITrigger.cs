using Microsoft.Xaml.Interactivity;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// Interface implemented by all custom triggers.
    /// </summary>
    public interface ITrigger : IBehavior
    {
       /// <summary>
       /// Gets the collection of actions associated with the behavior.
       /// </summary>
        TriggerActionCollection Actions { get; }
    }
}
