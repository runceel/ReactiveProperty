using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Reactive.Bindings.Internals
{
    /// <summary>
    /// Accessor Cache
    /// </summary>
    /// <typeparam name="TType">The type of the type.</typeparam>
    internal static class AccessorCache<TType>
    {
        private static readonly Dictionary<string, Delegate> s_getCache = new Dictionary<string, Delegate>();
        private static readonly Dictionary<string, Delegate> s_setCache = new Dictionary<string, Delegate>();

        /// <summary>
        /// Lookups the get.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertySelector">The property selector.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static Func<TType, TProperty> LookupGet<TProperty>(Expression<Func<TType, TProperty>> propertySelector, out string propertyName)
        {
            propertyName = GetPropertyName(propertySelector);
            Delegate accessor;

            lock (s_getCache)
            {
                if (!s_getCache.TryGetValue(propertyName, out accessor))
                {
                    accessor = propertySelector.Compile();
                    s_getCache.Add(propertyName, accessor);
                }
            }

            return (Func<TType, TProperty>)accessor;
        }

        /// <summary>
        /// Lookups the get.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertySelector">The property selector.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static Func<TType, TProperty> LookupNestedGet<TProperty>(Expression<Func<TType, TProperty>> propertySelector, out string propertyName)
        {
            propertyName = GetPropertyPath(propertySelector);
            Delegate accessor;

            lock (s_getCache)
            {
                if (!s_getCache.TryGetValue(propertyName, out accessor))
                {
                    accessor = propertySelector.Compile();
                    s_getCache.Add(propertyName, accessor);
                }
            }

            return (Func<TType, TProperty>)accessor;
        }

        private static string GetPropertyPath<TProperty>(Expression<Func<TType, TProperty>> propertySelector)
        {
            if (!(propertySelector.Body is MemberExpression memberExpression))
            {
                if (!(propertySelector.Body is UnaryExpression unaryExpression)) { throw new ArgumentException(nameof(propertySelector)); }
                memberExpression = unaryExpression.Operand as MemberExpression;
                if (memberExpression == null) { throw new ArgumentException(nameof(propertySelector)); }
            }

            var tokens = new LinkedList<string>();
            while(memberExpression != null)
            {
                tokens.AddFirst(memberExpression.Member.Name);
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            return string.Join(".", tokens);
        }


        private static string GetPropertyName<TProperty>(Expression<Func<TType, TProperty>> propertySelector)
        {
            if (!(propertySelector.Body is MemberExpression memberExpression))
            {
                if (!(propertySelector.Body is UnaryExpression unaryExpression)) { throw new ArgumentException(nameof(propertySelector)); }
                memberExpression = unaryExpression.Operand as MemberExpression;
                if (memberExpression == null) { throw new ArgumentException(nameof(propertySelector)); }
            }

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Lookups the set.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertySelector">The property selector.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static Action<TType, TProperty> LookupSet<TProperty>(Expression<Func<TType, TProperty>> propertySelector, out string propertyName)
        {
            propertyName = GetPropertyName(propertySelector);
            Delegate accessor;

            lock (s_setCache)
            {
                if (!s_setCache.TryGetValue(propertyName, out accessor))
                {
                    accessor = CreateSetAccessor(propertySelector);
                    s_setCache.Add(propertyName, accessor);
                }
            }

            return (Action<TType, TProperty>)accessor;
        }

        private static Delegate CreateSetAccessor<TProperty>(Expression<Func<TType, TProperty>> propertySelector)
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)propertySelector.Body).Member;
            var selfParameter = Expression.Parameter(typeof(TType), "self");
            var valueParameter = Expression.Parameter(typeof(TProperty), "value");
            var body = Expression.Assign(Expression.Property(selfParameter, propertyInfo), valueParameter);
            var lambda = Expression.Lambda<Action<TType, TProperty>>(body, selfParameter, valueParameter);
            return lambda.Compile();
        }
    }

    internal static class AccessorCache
    {
        private static readonly Dictionary<Type, Type> _accessorCacheTypeCache = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, Dictionary<string, Delegate>> _getCache = new Dictionary<Type, Dictionary<string, Delegate>>();
        private static readonly Dictionary<Type, Dictionary<string, Delegate>> _setCache = new Dictionary<Type, Dictionary<string, Delegate>>();

        private static Dictionary<string, Delegate> GetGetCacheByType(Type type)
        {
            lock(_getCache)
            {
                if (_getCache.TryGetValue(type, out var cache))
                {
                    return cache;
                }

                var accessorType = GetAccessorCacheTypeByType(type);
                cache = (Dictionary<string, Delegate>)accessorType.GetField("s_getCache", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                _getCache.Add(type, cache);
                return cache;
            }
        }

        private static Dictionary<string, Delegate> GetSetCacheByType(Type type)
        {
            lock(_setCache)
            {
                if (_setCache.TryGetValue(type, out var cache))
                {
                    return cache;
                }

                var accessorType = GetAccessorCacheTypeByType(type);
                cache = (Dictionary<string, Delegate>)accessorType.GetField("s_setCache", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                _setCache.Add(type, cache);
                return cache;
            }
        }

        private static Type GetAccessorCacheTypeByType(Type type)
        {
            lock(_accessorCacheTypeCache)
            {
                if (_accessorCacheTypeCache.TryGetValue(type, out var result))
                {
                    return result;
                }

                result = typeof(AccessorCache<>).MakeGenericType(type);
                _accessorCacheTypeCache.Add(type, result);
                return result;
            }
        }

        public static Delegate LookupGet(Type type, string propertyName)
        {
            var getCache = GetGetCacheByType(type);
            lock(getCache)
            {
                if (getCache.TryGetValue(propertyName, out var accessor))
                {
                    return accessor;
                }

                return CreateAndCacheGetAccessor(type, propertyName, getCache);
            }
        }

        public static Delegate LookupSet(Type type, string propertyName)
        {
            var setCache = GetSetCacheByType(type);
            lock(setCache)
            {
                if (setCache.TryGetValue(propertyName, out var accessor))
                {
                    return accessor;
                }

                return CreateAndCacheSetAccessor(type, propertyName, setCache);
            }
        }



        private static Delegate CreateAndCacheGetAccessor(Type type, string propertyName, Dictionary<string, Delegate> cache)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var accessor = CreateGetAccessor(type, propertyInfo);
            cache.Add(propertyName, accessor);
            return accessor;
        }

        private static Delegate CreateAndCacheSetAccessor(Type type, string propertyName, Dictionary<string, Delegate> cache)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var accessor = CreateSetAccessor(type, propertyInfo);
            cache.Add(propertyName, accessor);
            return accessor;
        }

        private static Delegate CreateSetAccessor(Type type, PropertyInfo propertyInfo)
        {
            var selfParameter = Expression.Parameter(type, "self");
            var valueParameter = Expression.Parameter(propertyInfo.PropertyType, "value");
            var body = Expression.Assign(Expression.Property(selfParameter, propertyInfo), valueParameter);
            var lambda = Expression.Lambda(typeof(Action<,>).MakeGenericType(type, propertyInfo.PropertyType), body, selfParameter, valueParameter);
            return lambda.Compile();
        }

        private static Delegate CreateGetAccessor(Type type, PropertyInfo propertyInfo)
        {
            var selfParameter = Expression.Parameter(type, "self");
            var body = Expression.Property(selfParameter, propertyInfo);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(type, propertyInfo.PropertyType), body, selfParameter);
            return lambda.Compile();
        }
    }
}
