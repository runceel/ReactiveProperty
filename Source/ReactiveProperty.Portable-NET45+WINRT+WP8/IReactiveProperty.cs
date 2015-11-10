using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactive.Bindings
{
    // for EventToReactive and Serialization
    public interface IReactiveProperty : IHasErrors
    {
        object Value { get; set; }
    }

    public interface IReactiveProperty<T> : IReactiveProperty, IObservable<T>, IDisposable, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        new T Value { get; set; }
    }
}
