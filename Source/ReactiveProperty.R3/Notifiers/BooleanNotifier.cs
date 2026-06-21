using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using R3;

namespace Reactive.Bindings.R3.Notifiers;

/// <summary>
/// Notifies a boolean flag. Backed by an R3 <see cref="Subject{T}"/>.
/// </summary>
public class BooleanNotifier : Observable<bool>, INotifyPropertyChanged
{
    private readonly Subject<bool> _trigger = new();
    private bool _value;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the current flag value.
    /// </summary>
    public bool Value
    {
        get => _value;
        set
        {
            _value = value;
            OnPropertyChanged();
            _trigger.OnNext(value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanNotifier"/> class.
    /// </summary>
    /// <param name="initialValue">The initial flag value.</param>
    public BooleanNotifier(bool initialValue = false)
    {
        Value = initialValue;
    }

    /// <summary>
    /// Sets and raises <see langword="true"/> if the current value is not already <see langword="true"/>.
    /// </summary>
    public void TurnOn()
    {
        if (Value != true)
        {
            Value = true;
        }
    }

    /// <summary>
    /// Sets and raises <see langword="false"/> if the current value is not already <see langword="false"/>.
    /// </summary>
    public void TurnOff()
    {
        if (Value != false)
        {
            Value = false;
        }
    }

    /// <summary>
    /// Sets and raises the reversed value.
    /// </summary>
    public void SwitchValue() => Value = !Value;

    /// <inheritdoc />
    protected override IDisposable SubscribeCore(Observer<bool> observer) => _trigger.Subscribe(observer.OnNext, observer.OnErrorResume, observer.OnCompleted);

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
