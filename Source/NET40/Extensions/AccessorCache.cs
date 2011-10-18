using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace Codeplex.Reactive.Extensions
{
    internal static class AccessorCache<TType>
    {
        static Dictionary<string, Delegate> getCache = new Dictionary<string, Delegate>();
        static Dictionary<string, Delegate> setCache = new Dictionary<string, Delegate>();

        public static Func<TType, TProperty> LookupGet<TProperty>(Expression<Func<TType, TProperty>> propertySelector, out string propertyName)
        {
            Contract.Requires<ArgumentNullException>(propertySelector != null);
            Contract.Ensures(Contract.Result<Func<TType, TProperty>>() != null);
            Contract.Ensures(!string.IsNullOrEmpty(Contract.ValueAtReturn(out propertyName)));

            propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
            Delegate accessor;

            lock (getCache)
            {
                if (!getCache.TryGetValue(propertyName, out accessor))
                {
                    accessor = propertySelector.Compile();
                    getCache.Add(propertyName, accessor);
                }
            }

            Contract.Assume(accessor != null);
            Contract.Assume(!string.IsNullOrEmpty(propertyName));
            return (Func<TType, TProperty>)accessor;
        }

        public static Action<TType, TProperty> LookupSet<TProperty>(Expression<Func<TType, TProperty>> propertySelector, out string propertyName)
        {
            Contract.Requires<ArgumentNullException>(propertySelector != null);
            Contract.Ensures(Contract.Result<Action<TType, TProperty>>() != null);
            Contract.Ensures(!string.IsNullOrEmpty(Contract.ValueAtReturn(out propertyName)));

            propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
            Delegate accessor;

            lock (setCache)
            {
                if (!setCache.TryGetValue(propertyName, out accessor))
                {
                    var propertyInfo = (PropertyInfo)((MemberExpression)propertySelector.Body).Member;
                    accessor = Delegate.CreateDelegate(typeof(Action<TType, TProperty>), propertyInfo.GetSetMethod());
                    setCache.Add(propertyName, accessor);
                }
            }

            Contract.Assume(accessor != null);
            Contract.Assume(!string.IsNullOrEmpty(propertyName));
            return (Action<TType, TProperty>)accessor;
        }
    }
}