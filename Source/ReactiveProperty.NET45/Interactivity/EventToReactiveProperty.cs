using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Windows.Input;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;

#if NETFX_CORE
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml;
#else
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows;
#endif

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// Converts EventArgs to object
    /// </summary>
#if NETFX_CORE
    [ContentProperty(Name = "Converter")]
#else
    [ContentProperty("Converter")]
#endif
    public class EventToReactiveProperty : TriggerAction<FrameworkElement>
    {
        private readonly Subject<object> source = new Subject<object>();

        private IDisposable disposable;

        public IReactiveProperty ReactiveProperty
        {
            get { return (IReactiveProperty)GetValue(ReactivePropertyProperty); }
            set { SetValue(ReactivePropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReactivePropertyProperty =
            DependencyProperty.Register("ReactiveProperty", typeof(IReactiveProperty), typeof(EventToReactiveCommand), new PropertyMetadata(null));

        /// <summary>
        /// Ignore EventArgs. If value is false then uses Unit.Default.
        /// </summary>
        public bool IgnoreEventArgs { get; set; }

        /// <summary>
        /// set and get Value converter.
        /// </summary>
#if NETFX_CORE
#elif WINDOWS_PHONE
#else
        [TypeConverter(typeof(ExpandableObjectConverter))]
#endif
        public IEventToReactiveConverter Converter { get; set; }

        // Using a DependencyProperty as the backing store for Converter.  This enables animation, styling, binding, etc...
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (this.disposable != null)
            {
                this.disposable.Dispose();
            }
        }

        protected override void Invoke(object parameter)
        {
            if (this.disposable == null)
            {
                this.Converter = this.Converter ?? new DefaultConverter();
                this.Converter.AssociateObject = this.AssociatedObject;
                this.disposable = this.Converter.Convert(this.source)
                    .ObserveOnUIDispatcher()
                    .Where(_ => this.ReactiveProperty != null)
                    .Subscribe(x => this.ReactiveProperty.Value = x);
            }

            if (!this.IgnoreEventArgs)
            {
                this.source.OnNext(parameter);
            }
            else
            {
                this.source.OnNext(Unit.Default);
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
