using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Xaml.Behaviors;
using R3;

namespace Reactive.Bindings.R3.Interactivity;

/// <summary>
/// A trigger action that converts a routed event and pushes the result into an R3
/// <see cref="IBindableReactiveProperty"/>.
/// </summary>
[ContentProperty(nameof(Converters))]
public class EventToReactiveProperty : TriggerAction<FrameworkElement>
{
    private readonly Subject<object?> _source = new();
    private IDisposable? _disposable;

    /// <summary>
    /// Gets or sets the reactive property updated when the event is raised.
    /// </summary>
    public IBindableReactiveProperty? ReactiveProperty
    {
        get { return (IBindableReactiveProperty?)GetValue(ReactivePropertyProperty); }
        set { SetValue(ReactivePropertyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ReactiveProperty"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ReactivePropertyProperty =
        DependencyProperty.Register(nameof(ReactiveProperty), typeof(IBindableReactiveProperty), typeof(EventToReactiveProperty), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets a value indicating whether the event args are ignored. When <see langword="true"/>
    /// <see cref="Unit.Default"/> is pushed instead of the event args.
    /// </summary>
    public bool IgnoreEventArgs { get; set; }

    /// <summary>
    /// Gets the converter chain applied to the event stream before it reaches the reactive property.
    /// </summary>
    public List<IEventToReactiveConverter> Converters { get; } = new();

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();
        _disposable?.Dispose();
        _source.Dispose();
    }

    /// <inheritdoc />
    protected override void Invoke(object parameter)
    {
        if (_disposable == null)
        {
            Observable<object?> ox = _source;
            foreach (var c in Converters)
            {
                c.AssociateObject = AssociatedObject;
                ox = c.Convert(ox);
            }

            _disposable = ox
                .ObserveOnCurrentSynchronizationContext()
                .Where(_ => ReactiveProperty != null)
                .Subscribe(x => ReactiveProperty!.Value = x);
        }

        _source.OnNext(IgnoreEventArgs ? Unit.Default : parameter);
    }
}
