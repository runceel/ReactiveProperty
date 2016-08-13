using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    public class TriggerBase<T> : TriggerBase
        where T : DependencyObject
    {
        public TriggerBase() : base(typeof(T))
        {
        }

        public new T AssociatedObject
        {
            get { return (T)base.AssociatedObject; }
        }
    }
}
