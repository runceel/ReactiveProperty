using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace ReactivePropertySamples.WPF.Behaviors
{
    public class DisposeViewModelWhenClosedBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closed += AssociatedObject_Closed;
        }

        private void AssociatedObject_Closed(object sender, EventArgs e) => (AssociatedObject.DataContext as IDisposable)?.Dispose();

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closed -= AssociatedObject_Closed;
        }
    }
}
