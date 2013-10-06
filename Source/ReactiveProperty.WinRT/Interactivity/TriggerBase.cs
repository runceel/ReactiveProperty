using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Codeplex.Reactive.Interactivity
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
