using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Codeplex.Reactive
{
    public class ReadOnlyReactiveCollection<T> : IReadOnlyCollection<T>, INotifyCollectionChanged, IDisposable
    {
        private ObservableCollection<T> innerCollection;
        private IDisposable subscription;

        public ReadOnlyReactiveCollection(IObservable<T> source, IEnumerable<T> sourceCollection, IScheduler scheduler)
        {
            this.innerCollection = new ObservableCollection<T>(sourceCollection);
            this.innerCollection.CollectionChanged += (_, e) => OnCollectionChnged(e);
            this.subscription = source.ObserveOn(scheduler).Subscribe(this.innerCollection.Add);
        }
        public ReadOnlyReactiveCollection(IObservable<T> source)
            : this(source, Enumerable.Empty<T>())
        {
        }

        public ReadOnlyReactiveCollection(IObservable<T> source, IScheduler scheduler)
            : this(source, Enumerable.Empty<T>(), scheduler)
        {

        }


        public ReadOnlyReactiveCollection(IObservable<T> source, IEnumerable<T> sourceCollection)
            : this(source, sourceCollection, UIDispatcherScheduler.Default)
        {
        }
        protected virtual void OnCollectionChnged(NotifyCollectionChangedEventArgs e)
        {
            var h = this.CollectionChanged;
            if (h != null)
            {
                h(this, e);
            }
        }

        public void Dispose()
        {
            if (subscription != null)
            {
                this.subscription.Dispose();
                this.subscription = null;
            }
        }

        public int Count
        {
            get { return this.innerCollection.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.innerCollection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    public static partial class ReactiveCollectionObservableExtensions
    {
        /// <summary>Source sequence as ObservableCollection. Operate scheduler is UIDispatcherScheduler.</summary>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<T> source)
        {
            return new ReadOnlyReactiveCollection<T>(source);
        }

        /// <summary>Source sequence as ObservableCollection. Operate scheduler is argument's scheduler.</summary>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return new ReadOnlyReactiveCollection<T>(source, scheduler);
        }
    }

}
