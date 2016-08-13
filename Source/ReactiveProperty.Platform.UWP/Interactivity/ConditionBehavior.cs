using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    public class ConditionBehavior : Behavior<TriggerBase>
    {

        public static readonly DependencyProperty CompareOperatorProperty =
            DependencyProperty.Register("CompareOperator", typeof(Op), typeof(ConditionBehavior), new PropertyMetadata(Op.Eq));


        public Op CompareOperator
        {
            get { return (Op)GetValue(CompareOperatorProperty); }
            set { SetValue(CompareOperatorProperty, value); }
        }



        public object Lhs
        {
            get { return (object)GetValue(LhsProperty); }
            set { SetValue(LhsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Lhs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LhsProperty =
            DependencyProperty.Register("Lhs", typeof(object), typeof(ConditionBehavior), new PropertyMetadata(null));

        public object Rhs
        {
            get { return (object)GetValue(RhsProperty); }
            set { SetValue(RhsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rhs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RhsProperty =
            DependencyProperty.Register("Rhs", typeof(object), typeof(ConditionBehavior), new PropertyMetadata(null));




        protected override void OnAttached()
        {
            this.AssociatedObject.PreviewInvoke += this.AssociatedObjectPreviewInvoke;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewInvoke -= this.AssociatedObjectPreviewInvoke;
        }

        private void AssociatedObjectPreviewInvoke(object sender, PreviewInvokeEventArgs e)
        {
            try
            {
                var comparerType = typeof(Comparer<>).MakeGenericType(this.Lhs.GetType());
                var defaultProperty = comparerType.GetTypeInfo().GetDeclaredProperty("Default");
                var comparer = (IComparer)defaultProperty.GetValue(null);
                var comparerResult = comparer.Compare(this.Lhs, System.Convert.ChangeType(this.Rhs, comparerType));
                switch (this.CompareOperator)
                {
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                e.Cancelling = true;
            }
        }
    }

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
