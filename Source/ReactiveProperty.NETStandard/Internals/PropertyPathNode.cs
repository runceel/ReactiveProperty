using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;

namespace Reactive.Bindings.Internals;

internal class PropertyPathNode : IDisposable
{
    private bool _isDisposed = false;
    private Action _callback;
    private Delegate _getAccessor;
    private Delegate _setAccessor;

    public event EventHandler PropertyChanged;

    public PropertyPathNode(string propertyName)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }
    public object Source { get; private set; }
    private Type PrevSourceType { get; set; }
    public PropertyPathNode Next { get; private set; }
    public PropertyPathNode Prev { get; private set; }
    public void SetCallback(Action callback)
    {
        _callback = callback;
        Next?.SetCallback(callback);
    }

    public PropertyPathNode InsertBefore(string propertyName)
    {
        if (Prev != null)
        {
            Prev.Next = null;
        }

        Prev = new PropertyPathNode(propertyName);
        Prev.Next = this;
        return Prev;
    }

    public void UpdateSource(object source)
    {
        EnsureDispose();
        Cleanup();
        Source = source;
        if (PrevSourceType != Source?.GetType())
        {
            _getAccessor = null;
        }
        PrevSourceType = Source?.GetType();
        StartObservePropertyChanged();
    }

    private void StartObservePropertyChanged()
    {
        EnsureDispose();
        if (Source == null) { return; }
        if (Source is INotifyPropertyChanged inpc)
        {
            inpc.PropertyChanged += SourcePropertyChangedEventHandler;
        }
        Next?.UpdateSource(GetPropertyValue());
    }

    private object GetPropertyValue()
    {
        EnsureDispose();
        return (_getAccessor ?? (_getAccessor = AccessorCache.LookupGet(Source.GetType(), PropertyName)))
            .DynamicInvoke(Source);
    }

    public object GetPropertyPathValue()
    {
        if (Source == null)
        {
            return null;
        }

        if (Next != null)
        {
            return Next.GetPropertyPathValue();
        }

        return GetPropertyValue();
    }

    public bool SetPropertyPathValue(object value)
    {
        if (Source == null)
        {
            return false;
        }

        if (Next != null)
        {
            return Next.SetPropertyPathValue(value);
        }
        else
        {
            var setter = _setAccessor ?? (_setAccessor = AccessorCache.LookupSet(Source.GetType(), PropertyName));
            setter.DynamicInvoke(Source, value);
            return true;
        }
    }

    public string Path => $"{PropertyName}{(string.IsNullOrEmpty(Next?.Path) ? "" : $".{Next?.Path}")}";

    public override string ToString() => Path;

    private void SourcePropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == PropertyName || string.IsNullOrEmpty(e.PropertyName))
        {
            Next?.UpdateSource(GetPropertyValue());
            _callback?.Invoke();
        }
    }

    private void Cleanup()
    {
        if (Source != null)
        {
            if (Source is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged -= SourcePropertyChangedEventHandler;
            }
            Source = null;
        }

        Next?.Cleanup();
    }

    public void Dispose()
    {
        _isDisposed = true;
        Cleanup();
    }

    private void EnsureDispose()
    {
        if (_isDisposed) { throw new ObjectDisposedException(nameof(PropertyPathNode)); }
    }

    private void RaisePropertyChanged() => PropertyChanged?.Invoke(this, EventArgs.Empty);


    public static PropertyPathNode CreateFromPropertySelector<TSubject, TProperty>(
        Expression<Func<TSubject, TProperty>> propertySelector)
    {
        if (!(propertySelector.Body is MemberExpression current))
        {
            throw new ArgumentException();
        }

        var node = default(PropertyPathNode);
        while (current != null)
        {
            var propertyName = current.Member.Name;
            if (node != null)
            {
                node = node.InsertBefore(propertyName);
            }
            else
            {
                node = new PropertyPathNode(propertyName);
            }
            current = current.Expression as MemberExpression;
        }

        return node;
    }
}
