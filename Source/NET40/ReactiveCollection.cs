using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace Codeplex.Reactive
{
    public class ReactiveCollection<T> : ObservableCollection<T>, IDisposable
    {
        readonly IDisposable subscription;
        readonly Dispatcher notifyDispatcher;

        public ReactiveCollection(IObservable<T> source, Dispatcher notifyDispatcher = null)
        {
            this.notifyDispatcher = notifyDispatcher;
            this.subscription = source.Subscribe(this.Add);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (notifyDispatcher != null && !notifyDispatcher.CheckAccess())
            {
                notifyDispatcher.BeginInvoke(new Action(() => base.OnCollectionChanged(e)));
            }
            else
            {
                base.OnCollectionChanged(e);
            }
        }

        public void Dispose()
        {
            subscription.Dispose();
        }
    }

    public static class ReactiveCollectionObservableExtensions
    {
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source, Dispatcher collectionChangedDispatcher = null)
        {
            return new ReactiveCollection<T>(source, collectionChangedDispatcher);
        }
    }
}