using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Windows.Input;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using System.Collections.Generic;

#if NETFX_CORE
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml;
#else
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using Microsoft.Xaml.Behaviors;
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
        private readonly Subject<EventArgs> autoEnableSource = new Subject<EventArgs>();

        private IDisposable disposable;
        private IDisposable disposable2;

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// The command property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(EventToReactiveCommand.Command), typeof(ICommand), typeof(EventToReactiveCommand), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets whether or not AssociatedObject.IsEnabled automatically follow the Command's CanExecute.
        /// </summary>
        public bool AutoEnable
        {
            get { return (bool)GetValue(AutoEnableProperty); }
            set { SetValue(AutoEnableProperty, value); }
        }

        /// <summary>
        /// The AutoEnable Property
        /// </summary>
        public static readonly DependencyProperty AutoEnableProperty =
            DependencyProperty.Register(nameof(EventToReactiveCommand.AutoEnable), typeof(bool), typeof(EventToReactiveCommand),
                new PropertyMetadata(true, (d, _) => ((EventToReactiveCommand)d).OnAllowDisableChanged()));

        private void OnAllowDisableChanged()
        {
            autoEnableSource.OnNext(EventArgs.Empty);
        }

        /// <summary>
        /// Ignore EventArgs. If value is false then uses Unit.Default.
        /// </summary>
        public bool IgnoreEventArgs { get; set; }

        private List<IEventToReactiveConverter> converters = new List<IEventToReactiveConverter>();

        /// <summary>
        /// set and get Value converter.
        /// </summary>
        public List<IEventToReactiveConverter> Converters { get { return converters; } }

        private BindingExpression expression;

        /// <summary>
        /// Called when [attached].
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
#if NETFX_CORE
            if (!(AssociatedObject is Control control)) return;
            var isEnabledProperty = Control.IsEnabledProperty;
#else
            var isEnabledProperty = FrameworkElement.IsEnabledProperty;
            var control = AssociatedObject;
#endif
            expression = AssociatedObject.GetBindingExpression(isEnabledProperty);

            disposable2 = Command?.CanExecuteChangedAsObservable().Merge(autoEnableSource)
                .Subscribe(_ =>
                {
                    if (AutoEnable)
                    {
                        control.IsEnabled = Command.CanExecute(null);
                    }
                    else
                    {
                        control.SetBinding(isEnabledProperty, expression.ParentBinding);
                    }
                });
        }

        /// <summary>
        /// Called when [detaching].
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            disposable?.Dispose();
            disposable2?.Dispose();
        }

        /// <summary>
        /// Invokes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void Invoke(object parameter)
        {
            if (disposable == null)
            {
                IObservable<object> ox = source;
                foreach (var c in Converters)
                {
                    c.AssociateObject = AssociatedObject;
                    ox = c.Convert(ox);
                }
                disposable = ox
                    .ObserveOnUIDispatcher()
                    .Where(_ => Command != null)
                    .Subscribe(x => Command.Execute(x));
            }

            if (!Command?.CanExecute(parameter) ?? true)
            {
                return;
            }

            if (!IgnoreEventArgs)
            {
                source.OnNext(parameter);
            }
            else
            {
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
