﻿using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.Binding
{
    /// <summary>
    /// RxProperty POCO binding support extension methods.
    /// </summary>
    public static class RxBindingExtensions
    {
        /// <summary>
        /// RxProperty POCO binding support method.
        /// </summary>
        /// <typeparam name="T">ReactiveProperty type parameter</typeparam>
        /// <typeparam name="TTarget">Binding target type</typeparam>
        /// <typeparam name="TProperty">Binding target property type</typeparam>
        /// <param name="self">Source ReactiveProperty</param>
        /// <param name="target">Binding target instance.</param>
        /// <param name="propertySelector">Binding target property selector.</param>
        /// <param name="mode">Binding mode</param>
        /// <param name="convert">source -&gt; target converter.</param>
        /// <param name="convertBack">target -&gt; source converter.</param>
        /// <param name="targetUpdateTrigger">targetUpdateTrigger. required TowWay and OneWayToSource</param>
        /// <param name="propertyFallbackValue">target error value.</param>
        /// <param name="sourceFallbackValue">source error value.</param>
        /// <returns>Release binding disposable.</returns>
        public static IDisposable BindTo<T, TTarget, TProperty>(
            this ReactiveProperty<T?> self,
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            BindingMode mode = BindingMode.OneWay,
            Func<T, TProperty?>? convert = null,
            Func<TProperty?, T?>? convertBack = null,
            IObservable<Unit>? targetUpdateTrigger = null,
            TProperty? propertyFallbackValue = default,
            T? sourceFallbackValue = default)
        {
            if (convert == null)
            {
                convert = value => (TProperty?)Convert.ChangeType(value, typeof(TProperty));
            }

            if (convertBack == null)
            {
                convertBack = value => (T?)Convert.ChangeType(value, typeof(T));
            }

            return mode switch
            {
                BindingMode.OneWay => CreateOneWayBinding(
                    self,
                    target,
                    propertySelector,
                    convert,
                    propertyFallbackValue),
                BindingMode.TwoWay => CreateTowWayBinding(
                    self,
                    target,
                    propertySelector,
                    convert,
                    convertBack,
                    targetUpdateTrigger,
                    propertyFallbackValue,
                    sourceFallbackValue),
                BindingMode.OneWayToSource => CreateOneWayToSourceBinding(
                    self,
                    target,
                    propertySelector,
                    convertBack,
                    targetUpdateTrigger,
                    sourceFallbackValue),
                _ => throw new NotSupportedException(),
            };
        }

        /// <summary>
        /// RxProperty POCO binding support method.
        /// </summary>
        /// <typeparam name="T">ReactiveProperty type parameter</typeparam>
        /// <typeparam name="TTarget">Binding target type</typeparam>
        /// <typeparam name="TProperty">Binding target property type</typeparam>
        /// <param name="self">Source ReactiveProperty</param>
        /// <param name="target">Binding target instance.</param>
        /// <param name="propertySelector">Binding target property selector.</param>
        /// <param name="convert">source -&gt; target converter.</param>
        /// <param name="propertyFallbackValue">target error value.</param>
        /// <returns>Release binding disposable.</returns>
        public static IDisposable BindTo<T, TTarget, TProperty>(
            this ReadOnlyReactiveProperty<T> self,
            TTarget target,
            Expression<Func<TTarget, TProperty>> propertySelector,
            Func<T, TProperty?>? convert = null,
            TProperty? propertyFallbackValue = default)
        {
            if (convert == null)
            {
                convert = value => (TProperty?)Convert.ChangeType(value, typeof(TProperty));
            }

            return CreateOneWayBinding(
                self,
                target,
                propertySelector,
                convert,
                propertyFallbackValue);
        }

        /// <summary>
        /// RxProperty POCO binding support method.
        /// </summary>
        /// <typeparam name="T">ReactiveProperty type parameter</typeparam>
        /// <typeparam name="TTarget">Binding target type</typeparam>
        /// <typeparam name="TProperty">Binding target property type</typeparam>
        /// <param name="self">Source ReactiveProperty</param>
        /// <param name="target">Binding target instance.</param>
        /// <param name="propertySelector">Binding target property selector.</param>
        /// <param name="mode">Binding mode</param>
        /// <param name="convert">source -&gt; target converter.</param>
        /// <param name="convertBack">target -&gt; source converter.</param>
        /// <param name="targetUpdateTrigger">targetUpdateTrigger. required TowWay and OneWayToSource</param>
        /// <param name="propertyFallbackValue">target error value.</param>
        /// <param name="sourceFallbackValue">source error value.</param>
        /// <returns>Release binding disposable.</returns>
        public static IDisposable BindTo<T, TTarget, TProperty>(
            this ReactivePropertySlim<T> self,
            TTarget target,
            Expression<Func<TTarget, TProperty?>> propertySelector,
            BindingMode mode = BindingMode.OneWay,
            Func<T?, TProperty?>? convert = null,
            Func<TProperty?, T?>? convertBack = null,
            IObservable<Unit>? targetUpdateTrigger = null,
            TProperty? propertyFallbackValue = default,
            T? sourceFallbackValue = default)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (convert == null)
            {
                convert = value => (TProperty?)Convert.ChangeType(value, typeof(TProperty));
            }

            if (convertBack == null)
            {
                convertBack = value => (T?)Convert.ChangeType(value, typeof(T));
            }

            switch (mode)
            {
                case BindingMode.OneWay:
                    return CreateOneWayBinding(
                        self,
                        target,
                        propertySelector,
                        convert,
                        propertyFallbackValue);

                case BindingMode.TwoWay:
                    return CreateTowWayBinding(
                        self,
                        target,
                        propertySelector,
                        convert,
                        convertBack,
                        targetUpdateTrigger,
                        propertyFallbackValue,
                        sourceFallbackValue);

                case BindingMode.OneWayToSource:
                    return CreateOneWayToSourceBinding(
                        self,
                        target,
                        propertySelector,
                        convertBack,
                        targetUpdateTrigger,
                        sourceFallbackValue);

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// RxProperty POCO binding support method.
        /// </summary>
        /// <typeparam name="T">ReactiveProperty type parameter</typeparam>
        /// <typeparam name="TTarget">Binding target type</typeparam>
        /// <typeparam name="TProperty">Binding target property type</typeparam>
        /// <param name="self">Source ReactiveProperty</param>
        /// <param name="target">Binding target instance.</param>
        /// <param name="propertySelector">Binding target property selector.</param>
        /// <param name="convert">source -&gt; target converter.</param>
        /// <param name="propertyFallbackValue">target error value.</param>
        /// <returns>Release binding disposable.</returns>
        public static IDisposable BindTo<T, TTarget, TProperty>(
            this ReadOnlyReactivePropertySlim<T?> self,
            TTarget target,
            Expression<Func<TTarget, TProperty?>> propertySelector,
            Func<T?, TProperty?>? convert = null,
            TProperty? propertyFallbackValue = default)
        {
            if (convert == null)
            {
                convert = value => (TProperty?)Convert.ChangeType(value, typeof(TProperty));
            }

            return CreateOneWayBinding(
                self,
                target,
                propertySelector,
                convert,
                propertyFallbackValue);
        }

        private static IDisposable CreateOneWayToSourceBinding<T, TTarget, TProperty>(
            IReactiveProperty<T?> self, 
            TTarget target, 
            Expression<Func<TTarget, TProperty?>> propertySelector, 
            Func<TProperty?, T?> convertBack, 
            IObservable<Unit> targetUpdateTrigger, 
            T? sourceFallbackValue)
        {
            var propertyName = default(string);
            if (targetUpdateTrigger == null)
            {
                throw new NotSupportedException("OneWayToSource binding required targetUpdateTrigger parameter.");
            }
            return targetUpdateTrigger
                .Subscribe(_ =>
                {
                    try
                    {
                        self.Value = convertBack(AccessorCache<TTarget>.LookupGet(propertySelector, out propertyName)(target));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        self.Value = sourceFallbackValue;
                    }
                });
        }

        private static IDisposable CreateOneWayBinding<T, TTarget, TProperty>(
            IObservable<T?> self, 
            TTarget target, 
            Expression<Func<TTarget, TProperty?>> propertySelector, 
            Func<T?, TProperty?>? convert, 
            TProperty? propertyFallbackValue)
        {
            var propertyName = default(string);
            return self
                .Subscribe(value =>
                {
                    var setter = AccessorCache<TTarget>.LookupSet(propertySelector, out propertyName);
                    try
                    {
                        setter(target, convert == null ? default : convert.Invoke(value));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        setter(target, propertyFallbackValue);
                    }
                });
        }

        private static IDisposable CreateTowWayBinding<T, TTarget, TProperty>(
            IReactiveProperty<T?> self, 
            TTarget target, 
            Expression<Func<TTarget, TProperty?>> propertySelector, 
            Func<T?, TProperty?>? convert, 
            Func<TProperty?, T?>? convertBack, 
            IObservable<Unit>? targetUpdateTrigger, 
            TProperty? propertyFallbackValue, 
            T? sourceFallbackValue)
        {
            if (targetUpdateTrigger == null)
            {
                throw new NotSupportedException("TwoWay binding required targetUpdateTrigger parameter.");
            }

            var propertyName = default(string);
            var d = new CompositeDisposable();
            var targetUpdating = false;
            var sourceUpdating = false;
            targetUpdateTrigger
                .Subscribe(_ =>
                {
                    if (sourceUpdating)
                    {
                        return;
                    }

                    targetUpdating = true;
                    try
                    {
                        self.Value = convertBack(AccessorCache<TTarget>.LookupGet(propertySelector, out propertyName)(target));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        self.Value = sourceFallbackValue;
                    }
                    targetUpdating = false;
                })
                .AddTo(d);
            self.Subscribe(value =>
            {
                if (targetUpdating)
                {
                    return;
                }

                var setter = AccessorCache<TTarget>.LookupSet(propertySelector, out propertyName);
                sourceUpdating = true;
                try
                {
                    setter(target, convert(value));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    setter(target, propertyFallbackValue);
                }
                sourceUpdating = false;
            })
                .AddTo(d);
            return d;
        }
    }

    /// <summary>
    /// Binding mode.
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// source to target.
        /// </summary>
        OneWay,

        /// <summary>
        /// sync source and target.
        /// </summary>
        TwoWay,

        /// <summary>
        /// target to source.
        /// </summary>
        OneWayToSource
    }
}
