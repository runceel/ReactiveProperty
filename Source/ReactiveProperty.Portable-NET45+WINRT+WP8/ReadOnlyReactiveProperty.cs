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

        private readonly Subject<T> innerSource = new Subject<T>();

        private T latestValue;

        private readonly CompositeDisposable subscription = new CompositeDisposable();

        private readonly bool isRaiseLatestValueOnSubscribe;

        internal ReadOnlyReactiveProperty(
            IObservable<T> source, 
            T initialValue = default(T), 
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IScheduler eventScheduler = null)
        {
            this.latestValue = initialValue;
            var ox = mode.HasFlag(ReactivePropertyMode.DistinctUntilChanged)
                ? source.DistinctUntilChanged()
                : source;
            ox.Subscribe(x => 
                {
                    this.latestValue = x;
                    this.innerSource.OnNext(x);
                })
                .AddTo(this.subscription);
            ox.ObserveOn(eventScheduler ?? UIDispatcherScheduler.Default)
                .Subscribe(_ =>
                {
                    this.PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value);
                })
                .AddTo(this.subscription);
            this.isRaiseLatestValueOnSubscribe = mode.HasFlag(ReactivePropertyMode.RaiseLatestValueOnSubscribe);
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
            if (this.subscription.IsDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            var result = this.innerSource.Subscribe(observer);
            if (this.isRaiseLatestValueOnSubscribe) { observer.OnNext(this.latestValue); }
            return result;
        }

        public void Dispose()
        {
            if (!this.subscription.IsDisposed)
            {
                this.innerSource.OnCompleted();
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
