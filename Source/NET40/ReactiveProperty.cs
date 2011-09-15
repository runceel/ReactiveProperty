using System;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ComponentModel;
#endif

namespace Codeplex.Reactive
{
    [Flags]
    public enum ReactivePropertyMode
    {
        None = 0x00,
        DistinctUntilChanged = 0x01,
        PropertyChangedInvokeOnUIDispatcher = 0x02,
        RaiseLatestValueOnSubscribe = 0x04,
        /// <summary>DistinctUntilChanged | PropertyChangedInvokeOnUIDispatcher | RaiseLatestValueOnSubscribe</summary>
        All = DistinctUntilChanged | PropertyChangedInvokeOnUIDispatcher | RaiseLatestValueOnSubscribe
    }

    public interface IReactiveProperty<out T> : IObservable<T>, IDisposable
    {
        object Value { get; set; }
    }

    public class ReactiveProperty<T> : INotifyPropertyChanged, IReactiveProperty<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        T latestValue;
        readonly IObservable<T> source;
        readonly Subject<T> anotherTrigger = new Subject<T>();
        readonly IDisposable sourceDisposable;

        /// <summary>One way from Source</summary>
        public ReactiveProperty(T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.None)
            : this(Observable.Never<T>(), null, initialValue, mode)
        { }

        /// <summary>Two way binding(mainly from source)</summary>
        public ReactiveProperty(Action<T> parentRaisePropertyChanged, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.All)
            : this(Observable.Never<T>(), parentRaisePropertyChanged, initialValue, mode)
        { }

        // ToReactivePropery Only
        internal ReactiveProperty(IObservable<T> source, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.All)
            : this(source, null, initialValue, mode)
        { }

        // ToReactivePropery Only
        internal ReactiveProperty(IObservable<T> source, Action<T> parentRaisePropertyChanged, T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.All)
        {
            this.latestValue = initialValue;

            // create source
            var merge = source.Merge(anotherTrigger);
            if (mode.HasFlag(ReactivePropertyMode.DistinctUntilChanged)) merge = merge.DistinctUntilChanged();
            this.source = merge;

            // publish observable
            var connectable = (mode.HasFlag(ReactivePropertyMode.RaiseLatestValueOnSubscribe))
                ? merge.Publish(initialValue)
                : merge.Publish();

            // subscribe
            (mode.HasFlag(ReactivePropertyMode.PropertyChangedInvokeOnUIDispatcher)
                    ? connectable.ObserveOnDispatcherEx()
                    : connectable)
                .Subscribe(x =>
                {
                    latestValue = x;
                    RaisePropertyChanged("Value");
                    if (parentRaisePropertyChanged != null) parentRaisePropertyChanged(x);
                });

            // start subscription
            this.sourceDisposable = connectable.Connect();
        }

        void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public T Value
        {
            get { return latestValue; }
            set { anotherTrigger.OnNext(value); }
        }

        object IReactiveProperty<T>.Value
        {
            get { return (T)Value; }
            set { Value = (T)value; }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return source.Subscribe(observer);
        }

        public void Dispose()
        {
            sourceDisposable.Dispose();
        }
    }

    public static class ReactivePropertyObservableExtensions
    {
        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.All)
        {
            return new ReactiveProperty<T>(source, initialValue, mode);
        }

        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source,
            Action<T> parentRaisePropertyChanged, T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.All)
        {
            return new ReactiveProperty<T>(source, parentRaisePropertyChanged, initialValue, mode);
        }
    }
}