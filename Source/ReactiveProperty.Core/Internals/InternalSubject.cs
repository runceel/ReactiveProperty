using System;

namespace Reactive.Bindings.Internals;
internal class InternalSubject<T> : IObservable<T>, IObserver<T>, IObserverLinkedList<T>, IDisposable
{
    private bool _isDisposed;
    private ObserverNode<T>? _root;
    private ObserverNode<T>? _last;

    public void Dispose() => DisposeInternal(null);

    private void DisposeInternal(Exception? error)
    {
        if (_isDisposed)
        {
            return;
        }

        var node = _root;
        _root = _last = null;
        _isDisposed = true;
        while (node != null)
        {
            if (error is null)
            {
                node.OnCompleted();
            }
            else
            {
                node.OnError(error);
            }

            node = node.Next;
        }
    }

    public void OnCompleted() => DisposeInternal(null);

    public void OnError(Exception error) => DisposeInternal(error);

    public void OnNext(T value)
    {
        // call source.OnNext
        var node = _root;
        while (node != null)
        {
            node.OnNext(value);
            node = node.Next;
        }
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (_isDisposed)
        {
            observer.OnCompleted();
            return InternalDisposable.Empty;
        }

        // subscribe node, node as subscription.
        var next = new ObserverNode<T>(this, observer);
        if (_root == null)
        {
            _root = _last = next;
        }
        else
        {
            _last!.Next = next;
            next.Previous = _last;
            _last = next;
        }

        return next;
    }

    void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
    {
        if (node == _root)
        {
            _root = node.Next;
        }
        if (node == _last)
        {
            _last = node.Previous;
        }

        if (node.Previous != null)
        {
            node.Previous.Next = node.Next;
        }
        if (node.Next != null)
        {
            node.Next.Previous = node.Previous;
        }
    }
}
