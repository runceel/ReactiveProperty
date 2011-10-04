using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
#endif

namespace Codeplex.Reactive.Extensions
{
    /// <summary>Value pair of OldItem and NewItem.</summary>
    public class OldNewPair<T>
    {
        public T OldItem { get; private set; }
        public T NewItem { get; private set; }

        public OldNewPair(T oldItem, T newItem)
        {
            this.OldItem = oldItem;
            this.NewItem = newItem;
        }
    }

    public static class ObservableCollectionExtensions
    {
        /// <summary>Converts CollectionChangedAction = Add to observable sequence.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<T> AddChanged<T>(this ObservableCollection<T> source)
        {
            return source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Add)
                .Select(e => (T)e.NewItems[0]);
        }

        public static IObservable<T[]> AddChangedItems<T>(this ObservableCollection<T> source)
        {
            return source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Add)
                .Select(e => e.NewItems.Cast<T>().ToArray());
        }

        public static IObservable<T> RemoveChanged<T>(this ObservableCollection<T> source)
        {
            return source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
                .Select(e => (T)e.OldItems[0]);
        }

        public static IObservable<T[]> RemoveChangedItems<T>(this ObservableCollection<T> source)
        {
            return source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
                .Select(e => e.OldItems.Cast<T>().ToArray());
        }

#if !SILVERLIGHT

        public static IObservable<OldNewPair<T>> MoveChanged<T>(this ObservableCollection<T> source)
        {
            return source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Move)
                .Select(e => new OldNewPair<T>((T)e.OldItems[0], (T)e.NewItems[0]));
        }

        public static IObservable<OldNewPair<T[]>> MoveChangedItems<T>(this ObservableCollection<T> source)
        {
            return source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Move)
                .Select(e => new OldNewPair<T[]>(e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));
        }

#endif

        public static IObservable<OldNewPair<T>> ReplaceChanged<T>(this ObservableCollection<T> source)
        {
            return source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
                .Select(e => new OldNewPair<T>((T)e.OldItems[0], (T)e.NewItems[0]));
        }

        public static IObservable<OldNewPair<T[]>> ReplaceChangedItems<T>(this ObservableCollection<T> source)
        {
            return source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
                .Select(e => new OldNewPair<T[]>(e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));
        }

        public static IObservable<Unit> ResetChanged<T>(this ObservableCollection<T> source)
        {
            return source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Reset)
                .Select(_ => new Unit());
        }
    }
}