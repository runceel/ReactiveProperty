using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeplex.Reactive.Interactivity
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
