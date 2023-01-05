using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.TinyLinq;

/// <summary>
/// Minimal LINQ subset extension methods (Select, Where and CombineLatest only).
/// If you want to use more LINQ methods, please add System.Reactive package from NuGet.
/// And Replace namespace from Reactive.Bindings.TinyLinq to System.Reactive.Linq.
/// </summary>
public static class ObservableExtensions
{
    /// <summary>
    /// Projects each element of an observable sequence into a new form with the spcified source and selector
    /// </summary>
    /// <typeparam name="TSource">The type of source.</typeparam>
    /// <typeparam name="TResule">The type of result.</typeparam>
    /// <param name="source">A sequence of elements to invoke a transform function on.</param>
    /// <param name="selector">A transform function to apply to each source element.</param>
    /// <returns></returns>
    public static IObservable<TResule> Select<TSource, TResule>(
        this IObservable<TSource> source,
        Func<TSource, TResule> selector) =>
        new SelectObservable<TSource, TResule>(source, selector);

    /// <summary>
    /// Filters the elements of an observable sequence based on a predicate.
    /// </summary>
    /// <typeparam name="TSource">The type of source.</typeparam>
    /// <param name="source">An observable sequence whose elements to filter.</param>
    /// <param name="filter">A function to test each source element for a condition.</param>
    /// <returns></returns>
    public static IObservable<TSource> Where<TSource>(
        this IObservable<TSource> source,
        Func<TSource, bool> filter) =>
        new WhereObservable<TSource>(source, filter);

    public static IObservable<TResult> CombineLatest<TSource, TResult>(
        this IEnumerable<IObservable<TSource>> sources,
        Func<IList<TSource>, TResult> resultSelector) =>
        new CombineLatestObservable<TSource, TResult>(sources, resultSelector);
}

internal class CombineLatestObservable<TSource, TResult> : IObservable<TResult>
{
    private readonly IEnumerable<IObservable<TSource>> _sources;
    private readonly Func<IList<TSource>, TResult> _resultSelector;

    public CombineLatestObservable(IEnumerable<IObservable<TSource>> sources, Func<IList<TSource>, TResult> resultSelector)
    {
        _sources = sources;
        _resultSelector = resultSelector;
    }

    public IDisposable Subscribe(IObserver<TResult> observer) => 
        new Subscription(_sources.ToArray(), _resultSelector, observer);

    private class Subscription : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        public Subscription(IObservable<TSource>[] sources, Func<IList<TSource>, TResult> resultSelector, IObserver<TResult> observer)
        {
            bool allValuesWerePublished = false;
            var latestValues = new TSource[sources.Length];
            var published = new bool[sources.Length];
            for (int i = 0; i < sources.Length; i++)
            {
                var index = i;
                sources[index].Subscribe(new DelegateObserver<TSource>(x =>
                {
                    latestValues[index] = x;
                    if (allValuesWerePublished is false)
                    {
                        published[index] = true;
                        allValuesWerePublished = published.All(x => x);
                    }

                    if (allValuesWerePublished)
                    {
                        observer.OnNext(resultSelector(latestValues));
                    }
                },
                error =>
                {
                    observer.OnError(error);
                    Dispose();
                },
                () =>
                {
                    observer.OnCompleted();
                    Dispose();
                })).AddTo(_disposables);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}

internal class SelectObservable<TSource, TResult> : IObservable<TResult>
{
    private readonly IObservable<TSource> _source;
    private readonly Func<TSource, TResult> _selector;

    public SelectObservable(IObservable<TSource> source, Func<TSource, TResult> selector)
    {
        _source = source;
        _selector = selector;
    }

    public IDisposable Subscribe(IObserver<TResult> observer) =>
        new Subscription(_source, _selector, observer);

    class Subscription : IDisposable
    {
        private readonly IDisposable _disposable;

        public Subscription(IObservable<TSource> source, Func<TSource, TResult> selector, IObserver<TResult> observer)
        {
            _disposable = source.Subscribe(new DelegateObserver<TSource>(
                x => observer.OnNext(selector(x)),
                observer.OnError,
                observer.OnCompleted));
        }

        public void Dispose() => _disposable.Dispose();
    }
}

internal class WhereObservable<TSource> : IObservable<TSource>
{
    private readonly IObservable<TSource> _source;
    private readonly Func<TSource, bool> _filter;

    public WhereObservable(IObservable<TSource> source, Func<TSource, bool> filter)
    {
        _source = source;
        _filter = filter;
    }

    public IDisposable Subscribe(IObserver<TSource> observer)
    {
        return new Subscription(_source, _filter, observer);
    }

    class Subscription : IDisposable
    {
        private readonly IDisposable _disposable;

        public Subscription(IObservable<TSource> source, Func<TSource, bool> filter, IObserver<TSource> observer)
        {
            _disposable = source.Subscribe(new DelegateObserver<TSource>(
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
