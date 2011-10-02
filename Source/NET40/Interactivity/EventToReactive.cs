using System.Windows;
using System.Windows.Interactivity;

namespace Codeplex.Reactive
{
    public class EventToReactive : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            ReactiveProperty.Value = parameter;
        }

        public IPushableValue ReactiveProperty
        {
            get { return (IPushableValue)GetValue(ReactivePropertyProperty); }
            set { SetValue(ReactivePropertyProperty, value); }
        }

        public static readonly DependencyProperty ReactivePropertyProperty =
            DependencyProperty.Register("ReactiveProperty", typeof(IPushableValue), typeof(EventToReactive), new PropertyMetadata(null));
    }
}