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
    /// <summary>
    /// Event To Reactive
    /// </summary>
    [Obsolete("Please use EventToReactiveProperty")]
    public class EventToReactive : TriggerAction<DependencyObject>
    {
        /// <summary>
        /// Invokes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void Invoke(object parameter)
        {
            if (ReactiveProperty == null) {
                return;
            }

            if (IgnoreEventArgs) {
                ((IReactiveProperty)ReactiveProperty).Value = new Unit();
            } else {
                var converter = Converter;
                ((IReactiveProperty)ReactiveProperty).Value =
                    (converter != null) ? converter(parameter) : parameter;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore event arguments]. default is false
        /// </summary>
        /// <value><c>true</c> if [ignore event arguments]; otherwise, <c>false</c>.</value>
        public bool IgnoreEventArgs { get; set; }

        /// <summary>
        /// Gets or sets the reactive property.
        /// </summary>
        /// <value>The reactive property.</value>
        public object ReactiveProperty
        {
            get { return GetValue(ReactivePropertyProperty); }
            set { SetValue(ReactivePropertyProperty, value); }
        }

        /// <summary>
        /// The reactive property property
        /// </summary>
        public static readonly DependencyProperty ReactivePropertyProperty =
            DependencyProperty.Register("ReactiveProperty", typeof(IReactiveProperty), typeof(EventToReactive), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        /// <value>The converter.</value>
        public Func<object, object> Converter
        {
            get { return (Func<object, object>)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        /// <summary>
        /// The converter property
        /// </summary>
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(Func<object, object>), typeof(EventToReactive), new PropertyMetadata(null));
    }
}
