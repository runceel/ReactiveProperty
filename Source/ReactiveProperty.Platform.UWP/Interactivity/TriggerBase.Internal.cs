using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.DependencyObject"/>
    /// <seealso cref="Microsoft.Xaml.Interactivity.IBehavior"/>
    public abstract class TriggerBase : DependencyObject, IBehavior
    {
        /// <summary>
        /// The actions implementation property
        /// </summary>
        public static readonly DependencyProperty ActionsImplProperty =
            DependencyProperty.Register("ActionsImpl", typeof(ActionCollection), typeof(TriggerBase), new PropertyMetadata(null));

        internal TriggerBase(Type associatedType)
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

            foreach (var action in ActionsImpl.OfType<IBehavior>()) {
                action.Attach(associatedObject);
            }

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

            foreach (var action in ActionsImpl.OfType<IBehavior>()) {
                action.Detach();
            }
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

        /// <summary>
        /// Occurs when [preview invoke].
        /// </summary>
        public event EventHandler<PreviewInvokeEventArgs> PreviewInvoke;

        /// <summary>
        /// Actionsプロパティは、TriggerBehaviorのベースクラスにいる状態だと認識しないっぽいので 泣く泣くこれを継承したクラスで以下のようなコードを書くことで対応することに…。
        /// <code>
        /// public ActionCollection Actions { get { return tshi.ActionImpl; } }
        /// </code>
        /// </summary>
        public ActionCollection ActionsImpl
        {
            get
            {
                var actions = (ActionCollection)GetValue(ActionsImplProperty);
                if (actions == null) {
                    actions = new ActionCollection();
                    SetValue(ActionsImplProperty, actions);
                }
                return actions;
            }
        }

        /// <summary>
        /// Invokes the actions.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        protected IEnumerable<object> InvokeActions(object parameter)
        {
            var args = OnPreviewInvoke();
            if (args.Cancelling) {
                return Enumerable.Empty<object>();
            }

            return Interaction.ExecuteActions(this, ActionsImpl, parameter);
        }

        private PreviewInvokeEventArgs OnPreviewInvoke()
        {
            var args = new PreviewInvokeEventArgs();
            PreviewInvoke?.Invoke(this, args);

            return args;
        }
    }
}
