using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
