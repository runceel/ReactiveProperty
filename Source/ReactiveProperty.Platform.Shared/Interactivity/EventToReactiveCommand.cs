using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Reactive.Disposables;
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

namespace Reactive.Bindings.Interactivity;

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
    private readonly Subject<object?> source = new();
    private readonly Subject<EventArgs> autoEnableSource = new();

    private CompositeDisposable Disposable { get; } = new CompositeDisposable();

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>The command.</value>
    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

#if NETFX_CORE
        /// <summary>
        /// The command property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(EventToReactiveCommand.Command), typeof(ICommand), typeof(EventToReactiveCommand),
                new PropertyMetadata(null, (d, e) => ((EventToReactiveCommand)d).OnCommandInitialized(e)));

        private void OnCommandInitialized(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == null && e.NewValue != null)
            {
                SetSubscribes();
            }
        }
#else
    /// <summary>
    /// The command property
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(EventToReactiveCommand.Command), typeof(ICommand), typeof(EventToReactiveCommand), new PropertyMetadata(null));
#endif

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
    /// Gets or sets calling or not ICommand.Execute method on IScheduler that is set to ReactivePropertyScheduler.Default.
    /// </summary>
    public bool CallExecuteOnScheduler
    {
        get { return (bool)GetValue(CallExecuteOnSchedulerProperty); }
        set { SetValue(CallExecuteOnSchedulerProperty, value); }
    }

    /// <summary>
    /// The CallExecuteOnSchedulere Property
    /// </summary>
    public static readonly DependencyProperty CallExecuteOnSchedulerProperty =
        DependencyProperty.Register(
            nameof(CallExecuteOnScheduler),
            typeof(bool),
            typeof(EventToReactiveCommand),
            new PropertyMetadata(true, OnCallExecuteOnScheduler));

    private static void OnCallExecuteOnScheduler(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((EventToReactiveCommand)d).OnCallExecuteOnScheduler(e);

    private void OnCallExecuteOnScheduler(DependencyPropertyChangedEventArgs e)
    {
        if (AssociatedObject != null)
        {
            SetSubscribes();
        }
    }


    /// <summary>
    /// Ignore EventArgs. If value is false then uses Unit.Default.
    /// </summary>
    public bool IgnoreEventArgs { get; set; }

    private List<IEventToReactiveConverter> converters = new();

    /// <summary>
    /// set and get Value converter.
    /// </summary>
    public List<IEventToReactiveConverter> Converters { get { return converters; } }

    private BindingExpression? expression;

    /// <summary>
    /// Called when [attached].
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
#if !NETFX_CORE
        SetSubscribes();
#endif
    }

    private void SetSubscribes()
    {
        IObservable<object?> ox = source;
        foreach (var c in Converters)
        {
            c.AssociateObject = AssociatedObject;
            ox = c.Convert(ox);
        }

        if (CallExecuteOnScheduler)
        {
            ox = ox.ObserveOn(ReactivePropertyScheduler.Default);
        }

        ox.Where(_ => Command != null)
            .Subscribe(x => Command.Execute(x)).AddTo(Disposable);

#if NETFX_CORE
            if (!(AssociatedObject is Control control)) return;
            var isEnabledProperty = Control.IsEnabledProperty;
#else
        var isEnabledProperty = FrameworkElement.IsEnabledProperty;
        var control = AssociatedObject;
#endif
        expression = AssociatedObject.GetBindingExpression(isEnabledProperty);

        Command?.CanExecuteChangedAsObservable().Merge(autoEnableSource)
            .Subscribe(_ =>
            {
                if (AutoEnable)
                {
                    control.IsEnabled = Command.CanExecute(null);
                }
                else if (expression != null)
                {
                    control.SetBinding(isEnabledProperty, expression.ParentBinding);
                }
                else
                {
                    control.IsEnabled = true;
                }
            }).AddTo(Disposable);
    }

    /// <summary>
    /// Called when [detaching].
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        Disposable?.Dispose();
    }

    /// <summary>
    /// Invokes the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    protected override void Invoke(object parameter)
    {
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
        public object? AssociateObject { get; set; }

        public IObservable<object?> Convert(IObservable<object?> source)
        {
            return source;
        }
    }
}
