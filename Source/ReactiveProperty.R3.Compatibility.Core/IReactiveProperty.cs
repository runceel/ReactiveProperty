using System;
using System.ComponentModel;

namespace Reactive.Bindings.R3Compat;

/// <summary>
/// Minimal shared reactive property contract used only by compatibility commands.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public interface IReactiveProperty<T> : INotifyPropertyChanged, IDisposable
{
    /// <summary>Gets or sets the current value.</summary>
    T Value { get; set; }
}
