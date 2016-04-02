using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    public abstract class Behavior<T> : Behavior
        where T : DependencyObject
    {
        protected Behavior() : base(typeof(T))
        {
        }

        protected new T AssociatedObject
        {
            get { return (T)base.AssociatedObject; }
        }
    }
}
