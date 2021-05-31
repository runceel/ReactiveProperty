using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// A base class for behaviors, implementing the basic plumbing of ITrigger
    /// </summary>
    [ContentProperty(Name = nameof(Trigger.Actions))]
    public abstract class Trigger : Behavior, ITrigger
    {
        /// <summary>
        /// Identifies the <seealso cref="Actions"/> dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
            nameof(Actions),
            typeof(TriggerActionCollection),
            typeof(Trigger),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets the collection of actions associated with the behavior. This is a dependency property.
        /// </summary>
        public TriggerActionCollection Actions
        {
            get
            {
                TriggerActionCollection actionCollection = (TriggerActionCollection)GetValue(Trigger.ActionsProperty);
                if (actionCollection == null)
                {
                    actionCollection = new TriggerActionCollection();
                    SetValue(Trigger.ActionsProperty, actionCollection);
                }

                return actionCollection;
            }
        }

        internal Trigger(Type associatedType) : base(associatedType)
        {
        }
    }
}
