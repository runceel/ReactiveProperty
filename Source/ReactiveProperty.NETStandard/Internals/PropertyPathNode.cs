using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Reactive.Bindings.Internals
{
    internal class PropertyPathNode : IDisposable
    {
        private bool _isDisposed = false;
        private readonly Action _callback;
        private Delegate _getAccessor;
        private Delegate _setAccessor;

        public PropertyPathNode(string targetPropertyName, Action callback)
        {
            TargetPropertyName = targetPropertyName;
            _callback = callback;
        }

        public string TargetPropertyName { get; }
        public INotifyPropertyChanged Target { get; private set; }
        public PropertyPathNode Next { get; private set; }
        public PropertyPathNode Prev { get; private set; }

        public PropertyPathNode InsertBefore(string propertyName)
        {
            if (Prev != null)
            {
                Prev.Next = null;
            }

            Prev = new PropertyPathNode(propertyName, _callback);
            Prev.Next = this;
            return Prev;
        }

        public void UpdateTarget(INotifyPropertyChanged target)
        {
            EnsureDispose();
            Cleanup();
            Target = target;
            StartObservePropertyChanged();
        }

        private void StartObservePropertyChanged()
        {
            EnsureDispose();
            if (Target == null) { return; }
            Target.PropertyChanged += TargetPropertyChangedEventHandler;
            Next?.UpdateTarget(GetPropertyValue() as INotifyPropertyChanged);
        }

        private object GetPropertyValue()
        {
            EnsureDispose();
            return (_getAccessor ?? (_getAccessor = AccessorCache.LookupGet(Target.GetType(), TargetPropertyName)))
                .DynamicInvoke(Target);
        }

        public object GetPropertyPathValue()
        {
            if (Target == null)
            {
                return null;
            }

            if (Next != null)
            {
                return Next.GetPropertyPathValue();
            }

            return GetPropertyValue();
        }

        public (bool ok, Exception error) SetPropertyPathValue(object value)
        {
            if (Target == null)
            {
                return (false, new NullReferenceException($"Access to null when evaluate {this}"));
            }

            if (Next != null)
            {
                return Next.SetPropertyPathValue(value);
            }
            else
            {
                var setter = _setAccessor ?? (_setAccessor = AccessorCache.LookupSet(Target.GetType(), TargetPropertyName));
                setter.DynamicInvoke(Target, value);
                return (true, null);
            }
        }


        public override string ToString()
        {
            EnsureDispose();
            var tail = Next?.ToString();
            return $"{TargetPropertyName}{(string.IsNullOrEmpty(tail) ? "" : $".{tail}")}";
        }

        private void TargetPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TargetPropertyName || string.IsNullOrEmpty(e.PropertyName))
            {
                Next?.UpdateTarget(GetPropertyValue() as INotifyPropertyChanged);
                _callback?.Invoke();
            }
        }

        private void Cleanup()
        {
            if (Target != null)
            {
                Target.PropertyChanged -= TargetPropertyChangedEventHandler;
                Target = null;
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

        public static PropertyPathNode CreateFromPropertySelector<TSubject, TProperty>(Expression<Func<TSubject, TProperty>> propertySelector)
        {
            if (!(propertySelector.Body is MemberExpression current))
            {
                throw new ArgumentException();
            }

            var result = default(PropertyPathNode);
            while(current != null)
            {
                var propertyName = current.Member.Name;
                if (result != null)
                {
                    result = result.InsertBefore(propertyName);
                }
                else
                {
                    result = new PropertyPathNode(propertyName, () => ox.OnNext(Unit.Default));
                }
                current = current.Expression as MemberExpression;
            }

            return result;
        }
    }
}
