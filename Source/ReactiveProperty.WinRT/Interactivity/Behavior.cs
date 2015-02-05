using Windows.UI.Xaml;

namespace Codeplex.Reactive.Interactivity
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
