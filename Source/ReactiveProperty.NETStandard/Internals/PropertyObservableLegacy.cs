using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Subjects;

namespace Reactive.Bindings.Internals;

internal sealed class PropertyObservableLegacy<TProperty> : IObservable<TProperty>, IDisposable
{
    private PropertyPathNodeLegacy? RootNode { get; set; }
    internal void SetRootNode(PropertyPathNodeLegacy rootNode)
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

    public string Path => RootNode?.Path;
    public bool SetPropertyPathValue(TProperty value) => RootNode?.SetPropertyPathValue(value) ?? false;
    public void SetSource(INotifyPropertyChanged source) => RootNode?.UpdateSource(source);

    private void RaisePropertyChanged() => _propertyChangedSource.OnNext(GetPropertyPathValue());

    private readonly Subject<TProperty> _propertyChangedSource = new();

    public void Dispose()
    {
        _propertyChangedSource.Dispose();
        RootNode?.Dispose();
        RootNode = null;
    }

    public IDisposable Subscribe(IObserver<TProperty> observer) => _propertyChangedSource.Subscribe(observer);
}

internal static class PropertyObservableLegacy
{
    public static PropertyObservableLegacy<TProperty> CreateFromPropertySelector<TSubject, TProperty>(TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector)
        where TSubject : INotifyPropertyChanged
    {
        if (!(propertySelector.Body is MemberExpression current))
        {
            throw new ArgumentException();
        }

        var result = new PropertyObservableLegacy<TProperty>();
        result.SetRootNode(PropertyPathNodeLegacy.CreateFromPropertySelector(propertySelector));
        result.SetSource(subject);
        return result;
    }

}
