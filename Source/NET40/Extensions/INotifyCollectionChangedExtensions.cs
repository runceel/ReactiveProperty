using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace Codeplex.Reactive.Extensions
{
    public static class INotifyCollectionChangedExtensions
    {
        /// <summary>Converts CollectionChanged to an observable sequence.</summary>
        public static IObservable<NotifyCollectionChangedEventArgs> CollectionChangedAsObservable<T>(this T source)
            where T : INotifyCollectionChanged
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IObservable<NotifyCollectionChangedEventArgs>>() != null);

            return ObservableEx.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.CollectionChanged += h,
                h => source.CollectionChanged -= h);
        }
    }
}