using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactive.Bindings
{
    /// <summary>
    /// for EventToReactive and Serialization
    /// </summary>
    public interface IReactiveProperty : IHasErrors
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; set; }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Reactive.Bindings.IHasErrors"/>
    public interface IReactiveProperty<T> : IReactiveProperty, IObservable<T>, IDisposable, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        new T Value { get; set; }
    }
}
