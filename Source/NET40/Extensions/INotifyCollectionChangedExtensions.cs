using System;
using System.Collections.Specialized;
using System.Reactive.Linq;

namespace Codeplex.Reactive.Extensions
{
    public static class INotifyCollectionChangedExtensions
    {
        public static IObservable<NotifyCollectionChangedEventArgs> CollectionChangedAsObservable<T>(this T source)
            where T : INotifyCollectionChanged
        {
            return Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.CollectionChanged += h,
                h => source.CollectionChanged -= h);
        }
    }
}