using System;
using System.Collections.Generic;

namespace Reactive.Bindings
{
    internal static class EnumerableEx
    {
        public static IEnumerable<T> Defer<T>(Func<IEnumerable<T>> enumerableFactory) => enumerableFactory();

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
