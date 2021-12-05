using System;
using System.Collections.Generic;
using System.Text;

namespace Reactive.Bindings.Internals;

internal interface IObserverLinkedList<T>
{
    void UnsubscribeNode(ObserverNode<T> node);
}
