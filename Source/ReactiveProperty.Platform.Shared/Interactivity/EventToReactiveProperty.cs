using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Windows.Input;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;

#if NETFX_CORE
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml;
using System.Collections.Generic;
#else
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
#endif

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// Converts EventArgs to object
    /// </summary>
#if NETFX_CORE
    [ContentProperty(Name = nameof(EventToReactiveProperty.Converters))]
#else
    [ContentProperty(nameof(EventToReactiveProperty.Converters))]
#endif
    public class EventToReactiveProperty : TriggerAction<FrameworkElement>
    {
        private readonly Subject<object> source = new Subject<object>();

        private IDisposable disposable;

        /// <summary>
        /// Gets or sets the reactive property.
        /// </summary>
        /// <value>The reactive property.</value>
        public IReactiveProperty ReactiveProperty
        {
            get { return (IReactiveProperty)GetValue(ReactivePropertyProperty); }
            set { SetValue(ReactivePropertyProperty, value); }
        }

        /// <summary>
        /// The reactive property property
        /// </summary>
        public static readonly DependencyProperty ReactivePropertyProperty =
            DependencyProperty.Register(nameof(EventToReactiveProperty.ReactiveProperty), typeof(IReactiveProperty), typeof(EventToReactiveProperty), new PropertyMetadata(null));

        /// <summary>
        /// Ignore EventArgs. If value is false then uses Unit.Default.
        /// </summary>
        public bool IgnoreEventArgs { get; set; }

        private List<IEventToReactiveConverter> converters = new List<IEventToReactiveConverter>();
        /// <summary>
        /// set and get Value converter.
        /// </summary>
        public List<IEventToReactiveConverter> Converters { get { return converters; } }

        /// <summary>
        /// Called when [detaching].
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            disposable?.Dispose();
        }

        /// <summary>
        /// Invokes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void Invoke(object parameter)
        {
            if (disposable == null) {
                IObservable<object> ox = source;
                foreach (var c in Converters) {
                    c.AssociateObject = AssociatedObject;
                    ox = c.Convert(ox);
                }
                disposable = ox
                    .ObserveOnUIDispatcher()
                    .Where(_ => ReactiveProperty != null)
                    .Subscribe(x => ReactiveProperty.Value = x);
            }

            if (!IgnoreEventArgs) {
                source.OnNext(parameter);
            } else {
                source.OnNext(Unit.Default);
            }
        }

        private class DefaultConverter : IEventToReactiveConverter
        {
            public object AssociateObject { get; set; }

            public IObservable<object> Convert(IObservable<object> source)
            {
                return source;
            }
        }
    }
}
