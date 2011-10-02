using System;
using System.Collections.Specialized;

namespace Codeplex.Reactive.Extensions
{
    public static class INotifyCollectionChangedExtensions
    {
        public static IObservable<NotifyCollectionChangedEventArgs> CollectionChangedAsObservable<T>(this T source)
            where T : INotifyCollectionChanged
        {
            return ObservableEx.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.CollectionChanged += h,
                h => source.CollectionChanged -= h);
        }
    }
}