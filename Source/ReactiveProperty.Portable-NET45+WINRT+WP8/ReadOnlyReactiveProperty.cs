using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.Bindings
{
    /// <summary>
    /// Read only version ReactiveProperty.
    /// </summary>
    /// <typeparam name="T">Type of property.</typeparam>
    public class ReadOnlyReactiveProperty<T> : IObservable<T>, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Subject<T> source;

        private T latestValue;

        private CompositeDisposable subscription = new CompositeDisposable();

        internal ReadOnlyReactiveProperty(
            IObservable<T> source, 
            T initialValue = default(T), 
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IScheduler eventScheduler = null)
        {
            var connectable = 
                mode.HasFlag(ReactivePropertyMode.RaiseLatestValueOnSubscribe) 
                ? source.Publish(initialValue)
                : source.Publish();

            var ox = mode.HasFlag(ReactivePropertyMode.DistinctUntilChanged)
                ? connectable.DistinctUntilChanged()
                : connectable;

            ox.Subscribe(x => this.latestValue = x);

            ox.ObserveOn(eventScheduler ?? UIDispatcherScheduler.Default)
                .Subscribe(_ =>
                {
                    var h = this.PropertyChanged;
                    if (h != null) { h(this, SingletonPropertyChangedEventArgs.Value); }
                })
                .AddTo(this.subscription);

            connectable.Connect().AddTo(this.subscription);
        }

        /// <summary>
        /// Get latest value.
        /// </summary>
        public T Value
        {
            get { return this.latestValue; }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return this.source.Subscribe(observer);
        }

        public void Dispose()
        {
            if (!this.subscription.IsDisposed)
            {
                this.subscription.Dispose();
            }
        }
    }

    /// <summary>
    /// ReadOnlyReactiveProperty factory methods.
    /// </summary>
    public static class ReadOnlyReactiveProperty
    {
        /// <summary>
        /// Create ReadOnlyReactiveProperty
        /// </summary>
        /// <typeparam name="T">Type of property.</typeparam>
        /// <param name="self">source stream</param>
        /// <param name="initialValue">initial push value</param>
        /// <param name="mode">ReactivePropertyMode. Default is DistinctUntilChanged | RaiseLatestValueOnSubscribe</param>
        /// <param name="eventScheduler">Scheduler of PropertyChanged event.</param>
        /// <returns></returns>
        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> self,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IScheduler eventScheduler = null)
        {
            return new ReadOnlyReactiveProperty<T>(
                self,
                initialValue,
                mode,
                eventScheduler);
        }
    }
}
