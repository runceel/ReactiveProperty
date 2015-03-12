using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Reactive.Bindings
{
    public class ReadOnlyReactiveProperty<T> : IObservable<T>, INotifyPropertyChanged, IDisposable
    {
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

        public T Value
        {
            get { return this.latestValue; }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return this.source.Subscribe(observer);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            if (!this.subscription.IsDisposed)
            {
                this.subscription.Dispose();
            }
        }
    }

    public static class ReadOnlyReactiveProperty
    {
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
