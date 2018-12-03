using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// </summary>
    public class ConditionBehavior : Behavior<TriggerBase>
    {
        /// <summary>
        /// The compare operator property
        /// </summary>
        public static readonly DependencyProperty CompareOperatorProperty =
            DependencyProperty.Register("CompareOperator", typeof(Op), typeof(ConditionBehavior), new PropertyMetadata(Op.Eq));

        /// <summary>
        /// Gets or sets the compare operator.
        /// </summary>
        /// <value>The compare operator.</value>
        public Op CompareOperator
        {
            get { return (Op)GetValue(CompareOperatorProperty); }
            set { SetValue(CompareOperatorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the LHS.
        /// </summary>
        /// <value>The LHS.</value>
        public object Lhs
        {
            get { return GetValue(LhsProperty); }
            set { SetValue(LhsProperty, value); }
        }

        /// <summary>
        /// The LHS property
        /// </summary>
        public static readonly DependencyProperty LhsProperty =
            DependencyProperty.Register("Lhs", typeof(object), typeof(ConditionBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the RHS.
        /// </summary>
        /// <value>The RHS.</value>
        public object Rhs
        {
            get { return GetValue(RhsProperty); }
            set { SetValue(RhsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rhs. This enables animation, styling,
        // binding, etc...
        /// <summary>
        /// The RHS property
        /// </summary>
        public static readonly DependencyProperty RhsProperty =
            DependencyProperty.Register("Rhs", typeof(object), typeof(ConditionBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Called when [attached].
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.PreviewInvoke += AssociatedObjectPreviewInvoke;
        }

        /// <summary>
        /// Called when [detaching].
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewInvoke -= AssociatedObjectPreviewInvoke;
        }

        private void AssociatedObjectPreviewInvoke(object sender, PreviewInvokeEventArgs e)
        {
            try {
                var comparerType = typeof(Comparer<>).MakeGenericType(Lhs.GetType());
                var defaultProperty = comparerType.GetTypeInfo().GetDeclaredProperty("Default");
                var comparer = (IComparer)defaultProperty.GetValue(null);
                var comparerResult = comparer.Compare(Lhs, System.Convert.ChangeType(Rhs, comparerType));
                switch (CompareOperator) {
                    case Op.Eq:
                        e.Cancelling = !(comparerResult == 0);
                        break;

                    case Op.Gt:
                        e.Cancelling = !(comparerResult > 0);
                        break;

                    case Op.GtEq:
                        e.Cancelling = !(comparerResult >= 0);
                        break;

                    case Op.Lt:
                        e.Cancelling = !(comparerResult < 0);
                        break;

                    case Op.LtEq:
                        e.Cancelling = !(comparerResult <= 0);
                        break;
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
                e.Cancelling = true;
            }
        }
    }

    /// <summary>
    /// </summary>
    public enum Op
    {
        /// <summary>
        /// =
        /// </summary>
        Eq,

        /// <summary>
        /// &gt;=
        /// </summary>
        GtEq,

        /// <summary>
        /// &gt;
        /// </summary>
        Gt,

        /// <summary>
        /// &lt;=
        /// </summary>
        LtEq,

        /// <summary>
        /// &lt;
        /// </summary>
        Lt
    }
}
