using System.Windows;
using System.Windows.Interactivity;

namespace Codeplex.Reactive.Interactivity
{
    public class EventToReactive : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            ((IValue)ReactiveProperty).Value = parameter;
        }

        public object ReactiveProperty
        {
            get { return GetValue(ReactivePropertyProperty); }
            set { SetValue(ReactivePropertyProperty, value); }
        }

        public static readonly DependencyProperty ReactivePropertyProperty =
            DependencyProperty.Register("ReactiveProperty", typeof(IValue), typeof(EventToReactive), new PropertyMetadata(null));
    }
}