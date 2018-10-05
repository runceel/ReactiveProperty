using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Reactive.Bindings.Extensions;

namespace Reactive.Bindings
{
    /// <summary>
    /// Read only version ReactiveProperty.
    /// </summary>
    /// <typeparam name="T">Type of property.</typeparam>
    public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>
    {
        private IEqualityComparer<T> EqualityComparer { get; }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        private Subject<T> InnerSource { get; } = new Subject<T>();

        private T LatestValue { get; set; }

        private CompositeDisposable Subscription { get; } = new CompositeDisposable();

        private bool IsRaiseLatestValueOnSubscribe { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyReactiveProperty{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="eventScheduler">The event scheduler.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        public ReadOnlyReactiveProperty(
            IObservable<T> source,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IScheduler eventScheduler = null,
            IEqualityComparer<T> equalityComparer = null)
        {
            LatestValue = initialValue;
            EqualityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            var ox = mode.HasFlag(ReactivePropertyMode.DistinctUntilChanged)
                ? source.DistinctUntilChanged(EqualityComparer)
                : source;

            ox.Do(x => {
                LatestValue = x;

                InnerSource.OnNext(x);
            })
                .ObserveOn(eventScheduler ?? ReactivePropertyScheduler.Default)
                .Subscribe(_ => {
                    PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value);
                })
                .AddTo(Subscription);
            IsRaiseLatestValueOnSubscribe = mode.HasFlag(ReactivePropertyMode.RaiseLatestValueOnSubscribe);
        }

        /// <summary>
        /// Get latest value.
        /// </summary>
        public T Value => LatestValue;

        object IReadOnlyReactiveProperty.Value => Value;

        /// <summary>
        /// Notifies the provider that an observer is to receive notifications.
        /// </summary>
        /// <param name="observer">The object that is to receive notifications.</param>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving notifications before
        /// the provider has finished sending them.
        /// </returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (Subscription.IsDisposed) {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            var result = InnerSource.Subscribe(observer);
            if (IsRaiseLatestValueOnSubscribe) { observer.OnNext(LatestValue); }
            return result;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!Subscription.IsDisposed) {
                InnerSource.OnCompleted();
                Subscription.Dispose();
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
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns></returns>
        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> self,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IScheduler eventScheduler = null,
            IEqualityComparer<T> equalityComparer = null) =>
            new ReadOnlyReactiveProperty<T>(
                self,
                initialValue,
                mode,
                eventScheduler,
                equalityComparer);
    }
}
