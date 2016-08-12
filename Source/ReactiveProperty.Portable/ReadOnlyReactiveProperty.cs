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
    public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Subject<T> InnerSource { get; } = new Subject<T>();

        private T LatestValue { get; set; }

        private CompositeDisposable Subscription { get; } = new CompositeDisposable();

        private bool IsRaiseLatestValueOnSubscribe { get; }

        internal ReadOnlyReactiveProperty(
            IObservable<T> source, 
            T initialValue = default(T), 
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IScheduler eventScheduler = null)
        {
            this.LatestValue = initialValue;
            var ox = mode.HasFlag(ReactivePropertyMode.DistinctUntilChanged)
                ? source.DistinctUntilChanged()
                : source;

            ox.Do(x =>
                {
                    this.LatestValue = x;
                    this.InnerSource.OnNext(x);
                })
                .ObserveOn(eventScheduler ?? ReactivePropertyScheduler.Default)
                .Subscribe(_ =>
                {
                    this.PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value);
                })
                .AddTo(this.Subscription);
            this.IsRaiseLatestValueOnSubscribe = mode.HasFlag(ReactivePropertyMode.RaiseLatestValueOnSubscribe);
        }

        /// <summary>
        /// Get latest value.
        /// </summary>
        public T Value => this.LatestValue;

        object IReadOnlyReactiveProperty.Value => Value;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (this.Subscription.IsDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            var result = this.InnerSource.Subscribe(observer);
            if (this.IsRaiseLatestValueOnSubscribe) { observer.OnNext(this.LatestValue); }
            return result;
        }

        public void Dispose()
        {
            if (!this.Subscription.IsDisposed)
            {
                this.InnerSource.OnCompleted();
                this.Subscription.Dispose();
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
            IScheduler eventScheduler = null) =>
            new ReadOnlyReactiveProperty<T>(
                self,
                initialValue,
                mode,
                eventScheduler);
    }
}
