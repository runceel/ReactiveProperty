using System.Windows;
using System.Windows.Interactivity;
using System.Reactive.Subjects;

namespace Codeplex.Reactive
{
    public class EventToReactive : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            ReactiveProperty.Value = parameter;
        }

        public IReactiveProperty<object> ReactiveProperty
        {
            get { return (IReactiveProperty<object>)GetValue(ReactivePropertyProperty); }
            set { SetValue(ReactivePropertyProperty, value); }
        }

        public static readonly DependencyProperty ReactivePropertyProperty =
            DependencyProperty.Register("ReactiveProperty", typeof(IReactiveProperty<object>), typeof(EventToReactive), new PropertyMetadata(null));
    }
}