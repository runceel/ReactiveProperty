using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Diagnostics.Contracts;

namespace Codeplex.Reactive.Extensions
{
    internal static class AccessorCache<TType>
    {
        static Dictionary<string, Delegate> cache = new Dictionary<string, Delegate>();

        public static Func<TType, TProperty> Lookup<TProperty>(Expression<Func<TType, TProperty>> propertySelector, out string propertyName)
        {
            Contract.Requires<ArgumentNullException>(propertySelector != null);
            Contract.Ensures(Contract.Result<Func<TType, TProperty>>() != null);
            Contract.Ensures(!string.IsNullOrEmpty(Contract.ValueAtReturn(out propertyName)));

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

            Contract.Assume(accessor != null);
            Contract.Assume(!string.IsNullOrEmpty(propertyName));
            return (Func<TType, TProperty>)accessor;
        }
    }
}