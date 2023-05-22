namespace Reactive.Bindings.Internals;

internal interface IObserverLinkedList<T>
{
    void UnsubscribeNode(ObserverNode<T> node);
}
