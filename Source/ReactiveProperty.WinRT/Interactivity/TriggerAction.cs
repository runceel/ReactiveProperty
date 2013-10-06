using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
