using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Codeplex.Reactive
{

    internal static class EnumerableEx
    {
        public static IEnumerable<T> Defer<T>(Func<IEnumerable<T>> enumerableFactory)
        {
            foreach (var item in enumerableFactory())
            {
                yield return item;
            }
        }

        public static IEnumerable<TSource[]> Buffer<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count <= 0) throw new ArgumentOutOfRangeException("count");

            var list = new List<TSource>(); // not use initial capacity
            foreach (var item in source)
            {
                list.Add(item);
                if (list.Count >= count)
                {
                    yield return list.ToArray();
                    list.Clear();
                }
            }

            if (list.Count > 0) yield return list.ToArray();
        }
    }

}
