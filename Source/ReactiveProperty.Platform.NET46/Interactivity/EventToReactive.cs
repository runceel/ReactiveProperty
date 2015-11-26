using System;
using System.Reactive;
#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows;
using System.Windows.Interactivity;
#endif

namespace Reactive.Bindings.Interactivity
{
    [Obsolete("Please use EventToReactiveProperty")]
    public class EventToReactive : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            if (ReactiveProperty == null) return;

            if (IgnoreEventArgs)
            {
                ((IReactiveProperty)ReactiveProperty).Value = new Unit();
            }
            else
            {
                var converter = Converter;
                ((IReactiveProperty)ReactiveProperty).Value =
                    (converter != null) ? converter(parameter) : parameter;
            }
        }

        // default is false
        public bool IgnoreEventArgs { get; set; }

        public object ReactiveProperty
        {
            get { return GetValue(ReactivePropertyProperty); }
            set { SetValue(ReactivePropertyProperty, value); }
        }

        public static readonly DependencyProperty ReactivePropertyProperty =
            DependencyProperty.Register("ReactiveProperty", typeof(IReactiveProperty), typeof(EventToReactive), new PropertyMetadata(null));

        public Func<object, object> Converter
        {
            get { return (Func<object, object>)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(Func<object, object>), typeof(EventToReactive), new PropertyMetadata(null));
    }
}
