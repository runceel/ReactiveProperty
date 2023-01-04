using System;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.TinyLinq;

public static class ObservableExtensions
{
    public static IObservable<U> Select<T, U>(this IObservable<T> self, Func<T, U> selector) => 
        new SelectObservable<T, U>(self, selector);

    public static IObservable<T> Where<T>(this IObservable<T> self, Func<T, bool> filter) =>
        new WhereObservable<T>(self, filter);
}

internal class SelectObservable<T, U> : IObservable<U>
{
    private readonly IObservable<T> _source;
    private readonly Func<T, U> _selector;

    public SelectObservable(IObservable<T> source, Func<T, U> selector)
    {
        _source = source;
        _selector = selector;
    }

    public IDisposable Subscribe(IObserver<U> observer) => 
        new Subscription(_source, _selector, observer);

    class Subscription : IDisposable
    {
        private readonly IDisposable _disposable;

        public Subscription(IObservable<T> source, Func<T, U> selector, IObserver<U> observer)
        {
            _disposable = source.Subscribe(new DelegateObserver<T>(
                x => observer.OnNext(selector(x)),
                observer.OnError,
                observer.OnCompleted));
        }

        public void Dispose() => _disposable.Dispose();
    }
}

internal class WhereObservable<T> : IObservable<T>
{
    private readonly IObservable<T> _source;
    private readonly Func<T, bool> _filter;

    public WhereObservable(IObservable<T> source, Func<T, bool> filter)
    {
        _source = source;
        _filter = filter;
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        return new Subscription(_source, _filter, observer);
    }

    class Subscription : IDisposable
    {
        private readonly IDisposable _disposable;

        public Subscription(IObservable<T> source, Func<T, bool> filter, IObserver<T> observer)
        {
            _disposable = source.Subscribe(new DelegateObserver<T>(
                x =>
                {
                    if (filter(x))
                    {
                        observer.OnNext(x);
                    }
                },
                observer.OnError,
                observer.OnCompleted));
        }

        public void Dispose() => _disposable.Dispose();
    }
}
