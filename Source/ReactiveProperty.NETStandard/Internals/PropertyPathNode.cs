using System;
using System.ComponentModel;

namespace Reactive.Bindings.Internals
{
    internal class PropertyPathNode : IDisposable
    {
        private bool _isDisposed = false;
        private readonly Action _callback;
        private Delegate _accessor;

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
            return (_accessor ?? (_accessor = AccessorCache.LookupGet(Target.GetType(), TargetPropertyName)))
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
    }
}
