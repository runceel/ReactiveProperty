namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// Targeted Trigger Action
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Reactive.Bindings.Interactivity.TargetedTriggerAction"/>
    public abstract class TargetedTriggerAction<T> : TargetedTriggerAction
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetedTriggerAction{T}"/> class.
        /// </summary>
        public TargetedTriggerAction() : base(typeof(T))
        {
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        protected new T Target
        {
            get { return (T)base.Target; }
        }
    }
}
