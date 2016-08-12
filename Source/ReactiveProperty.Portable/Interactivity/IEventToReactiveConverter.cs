using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
#if NETFX_CORE
using Windows.UI.Xaml;
#endif

namespace Reactive.Bindings.Interactivity
{
    public interface IEventToReactiveConverter
    {
        object AssociateObject { get; set; }
        IObservable<object> Convert(IObservable<object> source);
    }
}
