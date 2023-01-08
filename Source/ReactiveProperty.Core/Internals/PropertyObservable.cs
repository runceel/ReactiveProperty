using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Reactive.Bindings.Internals;

internal sealed class PropertyObservable<TProperty> : IObservable<TProperty>, IDisposable, IObserverLinkedList<TProperty>
{
    private ObserverNode<TProperty>? _root;
    private ObserverNode<TProperty>? _last;
    private bool _isDisposed;
    private bool _onError;

    private PropertyPathNode? RootNode { get; set; }
    public string? Path => RootNode?.Path;
    public bool SetPropertyPathValue(TProperty value) => RootNode?.SetPropertyPathValue(value) ?? false;
    public void SetSource(INotifyPropertyChanged source) => RootNode?.UpdateSource(source);

    private void RaisePropertyChanged()
    {
        try
        {
            var value = GetPropertyPathValue();
            // call source.OnNext
            var node = _root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }
        catch (Exception ex)
        {
            _onError = true;
            // call source.OnError
            var node = _root;
            while (node != null)
            {
                node.OnError(ex);
                node = node.Next;
            }

            Dispose();
        }
    }

    internal void SetRootNode(PropertyPathNode rootNode)
    {
        RootNode?.SetCallback(null);
        rootNode?.SetCallback(RaisePropertyChanged);
        RootNode = rootNode;
    }

    public TProperty GetPropertyPathValue()
    {
        var value = RootNode?.GetPropertyPathValue();
        return value != null ? (TProperty)value : default!;
    }



    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        var node = _root;
        _root = _last = null;

        if (_onError is false)
        {
            while (node != null)
            {
                try
                {
                    node.OnCompleted();
                }
                catch 
                {
                    // noop
                }

                node = node.Next;
            }
        }

        RootNode?.Dispose();
        RootNode = null;
    }

    public IDisposable Subscribe(IObserver<TProperty> observer)
    {
        if (_isDisposed)
        {
            observer.OnCompleted();
            return InternalDisposable.Empty;
        }

        // subscribe node, node as subscription.
        var next = new ObserverNode<TProperty>(this, observer);
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

    void IObserverLinkedList<TProperty>.UnsubscribeNode(ObserverNode<TProperty> node)
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

internal static class PropertyObservable
{
    public static PropertyObservable<TProperty> CreateFromPropertySelector<TSubject, TProperty>(TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector)
        where TSubject : INotifyPropertyChanged
    {
        if (!(propertySelector.Body is MemberExpression current))
        {
            throw new ArgumentException();
        }

        var result = new PropertyObservable<TProperty>();
        result.SetRootNode(PropertyPathNode.CreateFromPropertySelector(propertySelector));
        result.SetSource(subject);
        return result;
    }

}
