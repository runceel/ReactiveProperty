namespace Reactive.Bindings.Interactivity
{
    public abstract class TargetedTriggerAction<T> : TargetedTriggerAction
        where T : class
    {
        public TargetedTriggerAction() : base(typeof(T))
        {
        }

        protected new T Target
        {
            get { return (T)base.Target; }
        }
    }
}
