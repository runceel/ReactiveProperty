using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GalaSoft.MvvmLight.Threading;

namespace Codeplex.Reactive
{
    public class ReactiveCollection<T> : ObservableCollection<T>, IDisposable
    {
        readonly IDisposable subscription;
        readonly bool isRaiseOnUIDispatcher = false;

        public ReactiveCollection(IObservable<T> source, bool isRaiseOnUIDispatcher = true)
        {
            this.isRaiseOnUIDispatcher = isRaiseOnUIDispatcher;
            this.subscription = source.Subscribe(this.Add);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (isRaiseOnUIDispatcher && !DispatcherHelper.UIDispatcher.CheckAccess())
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => base.OnCollectionChanged(e)));
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
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source, bool isAddOnUIDispatcher = true)
        {
            return new ReactiveCollection<T>(source, isAddOnUIDispatcher);
        }
    }
}