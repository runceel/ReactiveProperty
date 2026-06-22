using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.Xaml.Behaviors;
using Reactive.Bindings.Extensions;

namespace Reactive.Bindings.Interactivity;

/// <summary>
/// Converts events from a WPF behavior to a command.
/// </summary>
[ContentProperty(nameof(Converters))]
public class EventToReactiveCommandBehavior : Behavior<FrameworkElement>
{
    private readonly Subject<object?> _source = new();
    private IDisposable? _executeSubscription;
    private IDisposable? _canExecuteChangedSubscription;
    private EventInfo? _eventInfo;
    private Delegate? _eventHandler;
    private BindingExpression? _isEnabledBindingExpression;

    /// <summary>
    /// The event name property.
    /// </summary>
    public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register(
        nameof(EventName),
        typeof(string),
        typeof(EventToReactiveCommandBehavior),
        new PropertyMetadata(null, OnEventNameChanged));

    /// <summary>
    /// The command property.
    /// </summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(EventToReactiveCommandBehavior),
        new PropertyMetadata(null, OnCommandChanged));

    /// <summary>
    /// The AutoEnable property.
    /// </summary>
    public static readonly DependencyProperty AutoEnableProperty = DependencyProperty.Register(
        nameof(AutoEnable),
        typeof(bool),
        typeof(EventToReactiveCommandBehavior),
        new PropertyMetadata(true, OnAutoEnableChanged));

    /// <summary>
    /// The CallExecuteOnScheduler property.
    /// </summary>
    public static readonly DependencyProperty CallExecuteOnSchedulerProperty = DependencyProperty.Register(
        nameof(CallExecuteOnScheduler),
        typeof(bool),
        typeof(EventToReactiveCommandBehavior),
        new PropertyMetadata(true, OnCallExecuteOnSchedulerChanged));

    /// <summary>
    /// Gets or sets the name of the event to observe.
    /// </summary>
    public string? EventName
    {
        get { return (string?)GetValue(EventNameProperty); }
        set { SetValue(EventNameProperty, value); }
    }

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    public ICommand? Command
    {
        get { return (ICommand?)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    /// <summary>
    /// Gets or sets whether or not AssociatedObject.IsEnabled automatically follows the Command's CanExecute.
    /// </summary>
    public bool AutoEnable
    {
        get { return (bool)GetValue(AutoEnableProperty); }
        set { SetValue(AutoEnableProperty, value); }
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
    /// Gets or sets whether to ignore EventArgs and use Unit.Default.
    /// </summary>
    public bool IgnoreEventArgs { get; set; }

    /// <summary>
    /// Gets the event converters.
    /// </summary>
    public List<IEventToReactiveConverter> Converters { get; } = new();

    /// <summary>
    /// Called when the behavior is attached.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        _isEnabledBindingExpression = AssociatedObject.GetBindingExpression(FrameworkElement.IsEnabledProperty);
        RegisterEvent();
        SetSubscribes();
        SetCanExecuteChangedSubscription();
        UpdateAssociatedObjectIsEnabled();
    }

    /// <summary>
    /// Called when the behavior is detaching.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        UnregisterEvent();
        _executeSubscription?.Dispose();
        _canExecuteChangedSubscription?.Dispose();
    }

    private static void OnEventNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = (EventToReactiveCommandBehavior)d;
        if (behavior.AssociatedObject == null)
        {
            return;
        }

        behavior.UnregisterEvent();
        behavior.RegisterEvent();
    }

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = (EventToReactiveCommandBehavior)d;
        behavior.SetCanExecuteChangedSubscription();
        behavior.UpdateAssociatedObjectIsEnabled();
    }

    private static void OnAutoEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((EventToReactiveCommandBehavior)d).UpdateAssociatedObjectIsEnabled();
    }

    private static void OnCallExecuteOnSchedulerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = (EventToReactiveCommandBehavior)d;
        if (behavior.AssociatedObject != null)
        {
            behavior.SetSubscribes();
        }
    }

    private void RegisterEvent()
    {
        if (AssociatedObject == null || string.IsNullOrEmpty(EventName))
        {
            return;
        }

        _eventInfo = AssociatedObject.GetType().GetEvent(EventName);
        if (_eventInfo?.EventHandlerType == null)
        {
            return;
        }

        var methodInfo = typeof(EventToReactiveCommandBehavior).GetMethod(nameof(OnEvent), BindingFlags.Instance | BindingFlags.NonPublic);
        _eventHandler = methodInfo?.CreateDelegate(_eventInfo.EventHandlerType, this);
        if (_eventHandler != null)
        {
            _eventInfo.AddEventHandler(AssociatedObject, _eventHandler);
        }
    }

    private void UnregisterEvent()
    {
        if (_eventInfo != null && _eventHandler != null && AssociatedObject != null)
        {
            _eventInfo.RemoveEventHandler(AssociatedObject, _eventHandler);
        }

        _eventInfo = null;
        _eventHandler = null;
    }

    private void SetSubscribes()
    {
        _executeSubscription?.Dispose();

        IObservable<object?> ox = _source;
        foreach (var c in Converters)
        {
            c.AssociateObject = AssociatedObject;
            ox = c.Convert(ox);
        }

        if (CallExecuteOnScheduler)
        {
            ox = ox.ObserveOn(ReactivePropertyScheduler.Default);
        }

        _executeSubscription = ox
            .Where(_ => Command != null)
            .Subscribe(x => Command!.Execute(x));
    }

    private void SetCanExecuteChangedSubscription()
    {
        _canExecuteChangedSubscription?.Dispose();
        _canExecuteChangedSubscription = Command?.CanExecuteChangedAsObservable()
            .Subscribe(_ => UpdateAssociatedObjectIsEnabled());
    }

    private void UpdateAssociatedObjectIsEnabled()
    {
        if (AssociatedObject == null)
        {
            return;
        }

        if (AutoEnable && Command != null)
        {
            AssociatedObject.IsEnabled = Command.CanExecute(null);
        }
        else if (_isEnabledBindingExpression != null)
        {
            AssociatedObject.SetBinding(FrameworkElement.IsEnabledProperty, _isEnabledBindingExpression.ParentBinding);
        }
        else
        {
            AssociatedObject.IsEnabled = true;
        }
    }

    private void OnEvent(object sender, EventArgs eventArgs)
    {
        var command = Command;
        if (!command?.CanExecute(eventArgs) ?? true)
        {
            return;
        }

        _source.OnNext(IgnoreEventArgs ? Unit.Default : eventArgs);
    }
}
