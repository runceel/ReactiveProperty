using System;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

    public class ReactiveProperty<T> : IReactiveProperty<T>
    {
        T latestValue;
        IObservable<T> source;
        Subject<T> anotherTrigger = new Subject<T>();
        IDisposable sourceDisposable;

        /// <summary>One way from Source</summary>
        public ReactiveProperty(T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.None)
            : this(Observable.Never<T>(), null, initialValue, mode)
        { }

        /// <summary>Two way binding(mainly from source)</summary>
        public ReactiveProperty(Action<T> propertyChanged, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.All)
            : this(Observable.Never<T>(), propertyChanged, initialValue, mode)
        { }

        /// <summary>Two way binding(mainly from this)</summary>
        public ReactiveProperty(IObservable<T> source, Action<T> propertyChanged, T initialValue = default(T),
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

            // call propertyChanged subscribe
            if (propertyChanged != null)
            {
                (mode.HasFlag(ReactivePropertyMode.PropertyChangedInvokeOnUIDispatcher)
                        ? connectable.ObserveOnUIUIDispatcher()
                        : connectable)
                    .Subscribe(x =>
                    {
                        latestValue = x;
                        propertyChanged(x);
                    });
            }

            // start subscription
            this.sourceDisposable = connectable.Connect();
        }

        public T Value
        {
            get
            {
                return latestValue;
            }
            set
            {
                latestValue = value;
                anotherTrigger.OnNext(value);
            }
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
            Action<T> propertyChanged,
            T initialValue = default(T),
            ReactivePropertyMode mode = ReactivePropertyMode.All)
        {
            return new ReactiveProperty<T>(source, propertyChanged, initialValue, mode);
        }
    }
}