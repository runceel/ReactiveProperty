using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Windows.Input;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using System.Collections.Generic;

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
    [ContentProperty(Name = nameof(EventToReactiveCommand.Converters))]
#else
    [ContentProperty(nameof(EventToReactiveCommand.Converters))]
#endif
    public class EventToReactiveCommand : TriggerAction<FrameworkElement>
    {
        private readonly Subject<object> source = new Subject<object>();

        private IDisposable disposable;

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(EventToReactiveCommand.Command), typeof(ICommand), typeof(EventToReactiveCommand), new PropertyMetadata(null));

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
                    .Where(_ => this.Command != null)
                    .Subscribe(x => this.Command.Execute(x));
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
