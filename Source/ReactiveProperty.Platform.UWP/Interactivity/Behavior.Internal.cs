using System;
using System.Reflection;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace Reactive.Bindings.Interactivity
{
    public abstract class Behavior : DependencyObject, IBehavior
    {
        internal Behavior(Type associatedType)
        {
            this.AssociatedType = associatedType;
        }

        public EventHandler AssociatedObjectChanged;

        public DependencyObject AssociatedObject { get; private set; }
        public Type AssociatedType { get; private set; }

        public void Attach(DependencyObject associatedObject)
        {
            if (this.AssociatedObject == associatedObject)
            {
                return;
            }

            this.AssertAttachArgument(associatedObject);

            this.AssociatedObject = associatedObject;
            this.OnAssociatedObjectChanged();

            this.OnAttached();
        }

        private void AssertAttachArgument(DependencyObject associatedObject)
        {
            if (this.AssociatedObject != null)
            {
                throw new InvalidOperationException("multiple time associate.");
            }

            if (associatedObject == null)
            {
                throw new ArgumentNullException("associatedObject");
            }

            if (!this.AssociatedType.GetTypeInfo().IsAssignableFrom(associatedObject.GetType().GetTypeInfo()))
            {
                throw new ArgumentException(string.Format("{0} can't assign {1}",
                    associatedObject.GetType().FullName,
                    this.AssociatedType.FullName));
            }
        }

        public void Detach()
        {
            this.OnDetaching();
            this.AssociatedObject = null;
            this.OnAssociatedObjectChanged();
        }

        protected virtual void OnAttached()
        {
        }

        protected virtual void OnDetaching()
        {
        }

        private void OnAssociatedObjectChanged()
        {
            var h = this.AssociatedObjectChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }
    }
}
