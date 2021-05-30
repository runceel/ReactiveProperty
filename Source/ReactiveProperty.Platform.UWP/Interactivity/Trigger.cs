using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// A base class for behaviors, implementing the basic plumbing of ITrigger
    /// </summary>
    /// <typeparam name="T">The object type to attach to</typeparam>
    public abstract class Trigger<T> : Trigger where T : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger{T}"/> class.
        /// </summary>
        public Trigger() : base(typeof(T))
        {
        }

        /// <summary>
        /// Gets the associated object.
        /// </summary>
        /// <value>The associated object.</value>
        protected new T AssociatedObject
        {
            get { return (T)base.AssociatedObject; }
        }
    }
}
