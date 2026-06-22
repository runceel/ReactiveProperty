using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Xaml.Behaviors;
using Reactive.Bindings.Extensions;

namespace Reactive.Bindings.Interactivity;

/// <summary>
/// Converts events from a WPF behavior to a reactive property.
/// </summary>
[ContentProperty(nameof(Converters))]
public class EventToReactivePropertyBehavior : Behavior<FrameworkElement>
{
    private readonly Subject<object?> _source = new();
    private IDisposable? _subscription;
    private EventInfo? _eventInfo;
    private Delegate? _eventHandler;

    /// <summary>
    /// The event name property.
    /// </summary>
    public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register(
        nameof(EventName),
        typeof(string),
        typeof(EventToReactivePropertyBehavior),
        new PropertyMetadata(null, OnEventNameChanged));

    /// <summary>
    /// The reactive property property.
    /// </summary>
    public static readonly DependencyProperty ReactivePropertyProperty = DependencyProperty.Register(
        nameof(ReactiveProperty),
        typeof(IReactiveProperty),
        typeof(EventToReactivePropertyBehavior),
        new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the name of the event to observe.
    /// </summary>
    public string? EventName
    {
        get { return (string?)GetValue(EventNameProperty); }
        set { SetValue(EventNameProperty, value); }
    }

    /// <summary>
    /// Gets or sets the reactive property.
    /// </summary>
    public IReactiveProperty? ReactiveProperty
    {
        get { return (IReactiveProperty?)GetValue(ReactivePropertyProperty); }
        set { SetValue(ReactivePropertyProperty, value); }
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
        RegisterEvent();
        SetSubscribes();
    }

    /// <summary>
    /// Called when the behavior is detaching.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        UnregisterEvent();
        _subscription?.Dispose();
    }

    private static void OnEventNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = (EventToReactivePropertyBehavior)d;
        if (behavior.AssociatedObject == null)
        {
            return;
        }

        behavior.UnregisterEvent();
        behavior.RegisterEvent();
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

        var methodInfo = typeof(EventToReactivePropertyBehavior).GetMethod(nameof(OnEvent), BindingFlags.Instance | BindingFlags.NonPublic);
        if (methodInfo == null)
        {
            return;
        }

        _eventHandler = methodInfo.CreateDelegate(_eventInfo.EventHandlerType, this);
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
        _subscription?.Dispose();

        IObservable<object?> ox = _source;
        foreach (var c in Converters)
        {
            c.AssociateObject = AssociatedObject;
            ox = c.Convert(ox);
        }

        _subscription = ox
            .ObserveOnUIDispatcher()
            .Where(_ => ReactiveProperty != null)
            .Subscribe(x => ReactiveProperty!.Value = x);
    }

    private void OnEvent(object sender, EventArgs eventArgs)
    {
        _source.OnNext(IgnoreEventArgs ? Unit.Default : eventArgs);
    }
}
