using Windows.UI.Xaml;

namespace Codeplex.Reactive.Interactivity
{
    public abstract class TriggerAction<T> : TriggerAction
        where T : DependencyObject
    {
        public TriggerAction() : base(typeof(T))
        {
        }

        public new T AssociatedObject
        {
            get { return (T)base.AssociatedObject; }
        }
    }
}
