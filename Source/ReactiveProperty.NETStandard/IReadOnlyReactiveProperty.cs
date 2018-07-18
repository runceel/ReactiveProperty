using System;
using System.ComponentModel;

namespace Reactive.Bindings
{
    public interface IReadOnlyReactiveProperty
    {
        object Value { get; }
    }

    public interface IReadOnlyReactiveProperty<out T> : IReadOnlyReactiveProperty, IObservable<T>, INotifyPropertyChanged, IDisposable
    {
        new T Value { get; }
    }
}
