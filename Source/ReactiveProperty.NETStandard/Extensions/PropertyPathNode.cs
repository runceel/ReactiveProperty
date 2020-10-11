using System;
using System.ComponentModel;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.Extensions
{
    internal class PropertyPathNode : IDisposable
    {
        private bool _isDisposed = false;
        private readonly Action _callback;

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
            Next?.UpdateTarget(GetPropertyValue());
        }

        private INotifyPropertyChanged GetPropertyValue()
        {
            EnsureDispose();
            return AccessorCache
                .LookupGet(Target.GetType(), TargetPropertyName)
                .DynamicInvoke(Target) as INotifyPropertyChanged;
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
                _callback?.Invoke();
                if (!_isDisposed)
                {
                    Next?.UpdateTarget(GetPropertyValue());
                }
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
