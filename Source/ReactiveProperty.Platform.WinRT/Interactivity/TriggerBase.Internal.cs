using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace Reactive.Bindings.Interactivity
{
    public abstract class TriggerBase : DependencyObject, IBehavior
    {
        public static readonly DependencyProperty ActionsImplProperty =
            DependencyProperty.Register("ActionsImpl", typeof(ActionCollection), typeof(TriggerBase), new PropertyMetadata(null));



        internal TriggerBase(Type associatedType)
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

            foreach (var action in this.ActionsImpl.OfType<IBehavior>())
            {
                action.Attach(associatedObject);
            }

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

            foreach (var action in this.ActionsImpl.OfType<IBehavior>())
            {
                action.Detach();
            }
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
        public event EventHandler<PreviewInvokeEventArgs> PreviewInvoke;

        /// <summary>
        /// Actionsプロパティは、TriggerBehaviorのベースクラスにいる状態だと認識しないっぽいので
        /// 泣く泣くこれを継承したクラスで以下のようなコードを書くことで対応することに…。
        /// <code>
        /// public ActionCollection Actions { get { return tshi.ActionImpl; } }
        /// </code>
        /// </summary>
        public ActionCollection ActionsImpl
        {
            get
            {
                var actions = (ActionCollection)GetValue(ActionsImplProperty);
                if (actions == null)
                {
                    actions = new ActionCollection();
                    this.SetValue(ActionsImplProperty, actions);
                }
                return actions;
            }
        }


        protected IEnumerable<object> InvokeActions(object parameter)
        {
            var args = this.OnPreviewInvoke();
            if (args.Cancelling)
            {
                return Enumerable.Empty<object>();
            }

            return Interaction.ExecuteActions(this, this.ActionsImpl, parameter);
        }

        private PreviewInvokeEventArgs OnPreviewInvoke()
        {
            var args = new PreviewInvokeEventArgs();
            var h = this.PreviewInvoke;
            if (h != null)
            {
                h(this, args);
            }

            return args;
        }
    }
}
