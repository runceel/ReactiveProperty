using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Subjects;

namespace Reactive.Bindings.Internals
{
    internal sealed class PropertyObserver<TProperty> : IObservable<TProperty>, IDisposable
    {
        internal PropertyPathNode RootNode { get; set; }
        public TProperty GetPropertyPathValue()
        {
            var value = RootNode?.GetPropertyPathValue();
            return value != null ? (TProperty)value : default;
        }

        public string Path => RootNode?.Path;
        public bool SetPropertyPathValue(TProperty value) => RootNode?.SetPropertyPathValue(value) ?? false;
        public void SetUpdateSource(INotifyPropertyChanged source) => RootNode?.UpdateSource(source);

        internal void RaisePropertyChanged() => _propertyChangedSource.OnNext(GetPropertyPathValue());

        private readonly Subject<TProperty> _propertyChangedSource = new Subject<TProperty>();

        public void Dispose()
        {
            _propertyChangedSource.Dispose();
            RootNode?.Dispose();
            RootNode = null;
        }

        public IDisposable Subscribe(IObserver<TProperty> observer) => _propertyChangedSource.Subscribe(observer);
    }

    internal static class PropertyObserver
    {
        public static PropertyObserver<TProperty> CreateFromPropertySelector<TSubject, TProperty>(TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector)
            where TSubject : INotifyPropertyChanged
        {
            if (!(propertySelector.Body is MemberExpression current))
            {
                throw new ArgumentException();
            }

            var result = new PropertyObserver<TProperty>();
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
                    node = new PropertyPathNode(propertyName, result.RaisePropertyChanged);
                }
                current = current.Expression as MemberExpression;
            }

            result.RootNode = node;
            result.SetUpdateSource(subject);
            return result;
        }

    }
}
