using System;
using System.Collections.Generic;
using System.Text;

namespace Reactive.Bindings.Internals
{
    public interface IObserverLinkedList<T>
    {
        void UnsubscribeNode(ObserverNode<T> node);
    }

}
