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

            return (Func<TType, TProperty>)accessor;
        }

        public static Action<TType, TProperty> LookupSet<TProperty>(Expression<Func<TType, TProperty>> propertySelector, out string propertyName)
        {
            propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
            Delegate accessor;

            lock (setCache)
            {
                if (!setCache.TryGetValue(propertyName, out accessor))
                {
                    var propertyInfo = (PropertyInfo)((MemberExpression)propertySelector.Body).Member;
                    accessor = new Action<TType, TProperty>((type, property) => propertyInfo.SetValue(type, property));
                    setCache.Add(propertyName, accessor);
                }
            }

            return (Action<TType, TProperty>)accessor;
        }
    }
}