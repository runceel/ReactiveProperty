using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Windows.Threading;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
#endif

namespace Codeplex.Reactive
{
    /// <summary>
    /// ObservableCollection that notify OnCollectionChanged on IScheduler.
    /// </summary>
    public class ReactiveCollection<T> : ObservableCollection<T>, IDisposable
    {
        readonly IDisposable subscription;
        readonly IScheduler notifyScheduler;

        /// <summary>Use scheduler is UIDispatcherScheduler.</summary>
        public ReactiveCollection()
        {
            this.notifyScheduler = UIDispatcherScheduler.Default;
        }

        /// <summary>Use scheduler is argument's scheduler.</summary>
        public ReactiveCollection(IScheduler scheduler)
        {
            Contract.Requires<ArgumentNullException>(scheduler != null);

            this.notifyScheduler = scheduler;
            this.subscription = Disposable.Empty;
        }

        /// <summary>Use scheduler is UIDispatcherScheduler and subscribe source sequence.</summary>
        public ReactiveCollection(IObservable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            this.notifyScheduler = UIDispatcherScheduler.Default;
            this.subscription = source.Subscribe(this.Add);
        }

        /// <summary>Use scheduler is argument's scheduler and subscribe source sequence.</summary>
        public ReactiveCollection(IObservable<T> source, IScheduler scheduler)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(scheduler != null);

            this.notifyScheduler = scheduler;
            this.subscription = source.Subscribe(this.Add);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            notifyScheduler.Schedule(() =>
            {
                base.OnCollectionChanged(e);
            });
        }

        /// <summary>Unsubcribe source sequence.</summary>
        public void Dispose()
        {
            subscription.Dispose();
        }
    }

    public static class ReactiveCollectionObservableExtensions
    {
        /// <summary>Use scheduler is UIDispatcherScheduler and subscribe source sequence.</summary>
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<ReactiveCollection<T>>() != null);

            return new ReactiveCollection<T>(source);
        }

        /// <summary>Use scheduler is argument's scheduler and subscribe source sequence.</summary>
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source, IScheduler scheduler)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(scheduler != null);
            Contract.Ensures(Contract.Result<ReactiveCollection<T>>() != null);

            return new ReactiveCollection<T>(source, scheduler);
        }
    }
}