using System;
using System.Reflection;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// Behavior
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.DependencyObject"/>
    /// <seealso cref="Microsoft.Xaml.Interactivity.IBehavior"/>
    public abstract class Behavior : DependencyObject, IBehavior
    {
        internal Behavior(Type associatedType)
        {
            AssociatedType = associatedType;
        }

        /// <summary>
        /// The associated object changed
        /// </summary>
        public EventHandler AssociatedObjectChanged;

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso
        /// cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }

        /// <summary>
        /// Gets the type of the associated.
        /// </summary>
        /// <value>The type of the associated.</value>
        public Type AssociatedType { get; private set; }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">
        /// The <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso
        /// cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> will be attached.
        /// </param>
        public void Attach(DependencyObject associatedObject)
        {
            if (AssociatedObject == associatedObject) {
                return;
            }

            AssertAttachArgument(associatedObject);

            AssociatedObject = associatedObject;
            OnAssociatedObjectChanged();

            OnAttached();
        }

        private void AssertAttachArgument(DependencyObject associatedObject)
        {
            if (AssociatedObject != null) {
                throw new InvalidOperationException("multiple time associate.");
            }

            if (associatedObject == null) {
                throw new ArgumentNullException("associatedObject");
            }

            if (!AssociatedType.GetTypeInfo().IsAssignableFrom(associatedObject.GetType().GetTypeInfo())) {
                throw new ArgumentException(string.Format("{0} can't assign {1}",
                    associatedObject.GetType().FullName,
                    AssociatedType.FullName));
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            OnDetaching();
            AssociatedObject = null;
            OnAssociatedObjectChanged();
        }

        /// <summary>
        /// Called when [attached].
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Called when [detaching].
        /// </summary>
        protected virtual void OnDetaching()
        {
        }

        private void OnAssociatedObjectChanged()
        {
            AssociatedObjectChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
