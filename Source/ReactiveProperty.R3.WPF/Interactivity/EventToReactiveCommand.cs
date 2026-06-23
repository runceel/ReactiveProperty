using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.Xaml.Behaviors;
using R3;

namespace Reactive.Bindings.R3.Interactivity;

/// <summary>
/// A trigger action that converts a routed event and executes an <see cref="ICommand"/>,
/// designed for R3-based view models.
/// </summary>
[ContentProperty(nameof(Converters))]
public class EventToReactiveCommand : TriggerAction<FrameworkElement>
{
    private readonly Subject<object?> _source = new();
    private IDisposable? _executeSubscription;
    private ICommand? _subscribedCommand;
    private BindingExpression? _isEnabledExpression;

    /// <summary>
    /// Gets or sets the command executed when the event is raised.
    /// </summary>
    public ICommand? Command
    {
        get { return (ICommand?)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Command"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EventToReactiveCommand), new PropertyMetadata(null, OnCommandChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the attached element's IsEnabled follows the command's CanExecute.
    /// </summary>
    public bool AutoEnable
    {
        get { return (bool)GetValue(AutoEnableProperty); }
        set { SetValue(AutoEnableProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AutoEnable"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AutoEnableProperty =
        DependencyProperty.Register(nameof(AutoEnable), typeof(bool), typeof(EventToReactiveCommand), new PropertyMetadata(true, OnAutoEnableChanged));

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="ICommand.Execute(object)"/> is marshaled onto the
    /// current synchronization context.
    /// </summary>
    public bool CallExecuteOnScheduler
    {
        get { return (bool)GetValue(CallExecuteOnSchedulerProperty); }
        set { SetValue(CallExecuteOnSchedulerProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CallExecuteOnScheduler"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CallExecuteOnSchedulerProperty =
        DependencyProperty.Register(nameof(CallExecuteOnScheduler), typeof(bool), typeof(EventToReactiveCommand), new PropertyMetadata(true, OnCallExecuteOnSchedulerChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the event args are ignored. When <see langword="true"/>
    /// <see cref="Unit.Default"/> is pushed instead of the event args.
    /// </summary>
    public bool IgnoreEventArgs { get; set; }

    /// <summary>
    /// Gets the converter chain applied to the event stream before it reaches the command.
    /// </summary>
    public List<IEventToReactiveConverter> Converters { get; } = new();

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        _isEnabledExpression = AssociatedObject.GetBindingExpression(FrameworkElement.IsEnabledProperty);
        SetSubscribes();
        SubscribeCanExecuteChanged(Command);
        UpdateIsEnabled();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();
        _executeSubscription?.Dispose();
        UnsubscribeCanExecuteChanged();
        _source.Dispose();
    }

    /// <inheritdoc />
    protected override void Invoke(object parameter)
    {
        var command = Command;
        if (command == null || !command.CanExecute(parameter))
        {
            return;
        }

        _source.OnNext(IgnoreEventArgs ? Unit.Default : parameter);
    }

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (EventToReactiveCommand)d;
        self.SubscribeCanExecuteChanged((ICommand?)e.NewValue);
        self.UpdateIsEnabled();
    }

    private static void OnAutoEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((EventToReactiveCommand)d).UpdateIsEnabled();

    private static void OnCallExecuteOnSchedulerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (EventToReactiveCommand)d;
        if (self.AssociatedObject != null)
        {
            self.SetSubscribes();
        }
    }

    private void SetSubscribes()
    {
        _executeSubscription?.Dispose();
        if (AssociatedObject == null)
        {
            return;
        }

        Observable<object?> ox = _source;
        foreach (var c in Converters)
        {
            c.AssociateObject = AssociatedObject;
            ox = c.Convert(ox);
        }

        if (CallExecuteOnScheduler)
        {
            ox = ox.ObserveOnCurrentSynchronizationContext();
        }

        _executeSubscription = ox
            .Where(_ => Command != null)
            .Subscribe(x => Command!.Execute(x));
    }

    private void SubscribeCanExecuteChanged(ICommand? command)
    {
        UnsubscribeCanExecuteChanged();
        if (command != null)
        {
            command.CanExecuteChanged += OnCanExecuteChanged;
            _subscribedCommand = command;
        }
    }

    private void UnsubscribeCanExecuteChanged()
    {
        if (_subscribedCommand != null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCanExecuteChanged;
            _subscribedCommand = null;
        }
    }

    private void OnCanExecuteChanged(object? sender, EventArgs e) => UpdateIsEnabled();

    private void UpdateIsEnabled()
    {
        var control = AssociatedObject;
        if (control == null)
        {
            return;
        }

        if (AutoEnable && Command != null)
        {
            control.IsEnabled = Command.CanExecute(null);
        }
        else if (_isEnabledExpression != null)
        {
            control.SetBinding(FrameworkElement.IsEnabledProperty, _isEnabledExpression.ParentBinding);
        }
        else
        {
            control.IsEnabled = true;
        }
    }
}
