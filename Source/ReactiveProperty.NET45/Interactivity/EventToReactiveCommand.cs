using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
#if NETFX_CORE
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml;
#else
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.ComponentModel;
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
    public class EventToReactiveCommand : TriggerAction<FrameworkElement>
    {
        private readonly Subject<object> source = new Subject<object>();

        private readonly SerialDisposable disposable = new SerialDisposable();

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToReactiveCommand), new PropertyMetadata(null));

        /// <summary>
        /// Ignore EventArgs. If value is false then uses Unit.Default.
        /// </summary>
        public bool IgnoreEventArgs { get; set; }

        /// <summary>
        /// set and get Value converter.
        /// </summary>
#if NETFX_CORE
#else
        [TypeConverter(typeof(ExpandableObjectConverter))]
#endif
        public IEventToReactiveConverter Converter
        {
            get { return (IEventToReactiveConverter)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Converter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(IEventToReactiveConverter), typeof(EventToReactiveCommand), new PropertyMetadata(new DefaultConverter(), ConverterChanged));

        private static void ConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EventToReactiveCommand)d).ConverterChanged();
        }

        private void ConverterChanged()
        {
            if (this.Converter == null)
            {
                this.disposable.Disposable = Disposable.Empty;
                return;
            }
            this.disposable.Disposable = this.Converter
                .Convert(this.source.Where(x => this.Command.CanExecute(x)))
                .ObserveOnUIDispatcher()
                .Subscribe(x => this.Command.Execute(x));
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.disposable.Dispose();
        }

        protected override void Invoke(object parameter)
        {
            this.Converter.AssociateObject = this.AssociatedObject;

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
            public FrameworkElement AssociateObject { get; set; }

            public IObservable<object> Convert(IObservable<object> source)
            {
                return source;
            }
        }

    }
}
