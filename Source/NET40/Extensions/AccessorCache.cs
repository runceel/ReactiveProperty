using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Codeplex.Reactive.Extensions
{
    internal static class AccessorCache<TType>
    {
        static Dictionary<string, Delegate> cache = new Dictionary<string, Delegate>();

        public static Func<TType, TProperty> Lookup<TProperty>(Expression<Func<TType, TProperty>> propertySelector, out string propertyName)
        {
            propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
            Delegate accessor;

            lock (cache)
            {
                if (!cache.TryGetValue(propertyName, out accessor))
                {
                    accessor = propertySelector.Compile();
                    cache.Add(propertyName, accessor);
                }
            }

            return (Func<TType, TProperty>)accessor;
        }
    }
}
