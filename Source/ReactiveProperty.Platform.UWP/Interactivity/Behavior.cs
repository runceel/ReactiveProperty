using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// Behavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Reactive.Bindings.Interactivity.Behavior"/>
    public abstract class Behavior<T> : Behavior
        where T : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Behavior{T}"/> class.
        /// </summary>
        protected Behavior() : base(typeof(T))
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
