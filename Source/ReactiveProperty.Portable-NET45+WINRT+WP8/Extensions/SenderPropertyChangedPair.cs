using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// Pair of PropertyChanged event sender and EventArgs.
    /// </summary>
    /// <typeparam name="TElement">Sender type</typeparam>
    public class SenderPropertyChangedPair<T>
    {
        /// <summary>
        /// Sender of PropertyChanged event.
        /// </summary>
        public T Sender { get; private set; }
        /// <summary>
        /// PropertyChanged event arguments.
        /// </summary>
        public PropertyChangedEventArgs Args { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="sender">sender value</param>
        /// <param name="args">event arguments</param>
        public SenderPropertyChangedPair(T sender, PropertyChangedEventArgs args)
        {
            this.Sender = sender;
            this.Args = args;
        }
    }
}
