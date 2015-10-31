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

        public IReactiveProperty ReactiveProperty
        {
            get { return (IReactiveProperty)GetValue(ReactivePropertyProperty); }
            set { SetValue(ReactivePropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
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
        public List<IEventToReactiveConverter> Converters { get { return this.converters; } }

        // Using a DependencyProperty as the backing store for Converter.  This enables animation, styling, binding, etc...
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.disposable?.Dispose();
        }

        protected override void Invoke(object parameter)
        {
            if (this.disposable == null)
            {
                IObservable<object> ox = this.source;
                foreach (var c in this.Converters)
                {
                    c.AssociateObject = this.AssociatedObject;
                    ox = c.Convert(ox);
                }
                this.disposable = ox
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
