using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Reactive.Bindings.Internals
{
    internal sealed class ObserverNode<T> : IObserver<T>, IDisposable
    {
        private readonly IObserver<T> _observer;
        private IObserverLinkedList<T> _list;

        public ObserverNode<T> Previous { get; set; }

        public ObserverNode<T> Next { get; set; }

        public ObserverNode(IObserverLinkedList<T> list, IObserver<T> observer)
        {
            _list = list;
            _observer = observer;
        }

        public void OnNext(T value)
        {
            _observer.OnNext(value);
        }

        public void OnError(Exception error)
        {
            _observer.OnError(error);
        }

        public void OnCompleted()
        {
            _observer.OnCompleted();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        public void Dispose()
        {
            var sourceList = Interlocked.Exchange(ref _list, null);
            if (sourceList != null)
            {
                sourceList.UnsubscribeNode(this);
                sourceList = null;
            }
        }
    }

}
