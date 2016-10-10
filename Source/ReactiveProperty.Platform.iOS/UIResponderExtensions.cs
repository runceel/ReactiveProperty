using Reactive.Bindings.Extensions;
using Reactive.Bindings.Internal;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using UIKit;

namespace Reactive.Bindings
{
    public static class UIResponderExtensions
    {
        /// <summary>
        /// Data binding method.
        /// </summary>
        /// <typeparam name="TView">View type</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="self">View</param>
        /// <param name="propertySelector">Target property selector</param>
        /// <param name="source">Source property</param>
        /// <param name="updateSourceTrigger">Update source trigger</param>
        /// <returns>Data binding token</returns>
        public static IDisposable SetBinding<TView, TProperty>(
            this TView self,
            Expression<Func<TView, TProperty>> propertySelector,
            ReactiveProperty<TProperty> source, Func<TView, IObservable<Unit>> updateSourceTrigger = null)
            where TView : UIResponder
        {
            var d = new CompositeDisposable();

            bool isUpdating = false;
            string propertyName;
            var setter = AccessorCache<TView>.LookupSet(propertySelector, out propertyName);
            source
                .Where(_ => !isUpdating)
                .Subscribe(x => setter(self, x))
                .AddTo(d);
            if (updateSourceTrigger != null)
            {
                var getter = AccessorCache<TView>.LookupGet(propertySelector, out propertyName);
                updateSourceTrigger(self).Subscribe(_ =>
                {
                    isUpdating = true;
                    try
                    {
                        source.Value = getter(self);
                    }
                    finally
                    {
                        isUpdating = false;
                    }
                }).AddTo(d);
            }

            return d;
        }

        /// <summary>
        /// Data binding method.
        /// </summary>
        /// <typeparam name="TView">View type</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="self">View</param>
        /// <param name="setter">Target value setter</param>
        /// <param name="getter">Target value getter</param>
        /// <param name="source">Source property</param>
        /// <param name="updateSourceTrigger">Update source trigger</param>
        /// <returns>Data binding token</returns>
        public static IDisposable SetBinding<TView, TProperty>(
            this TView self,
            Action<TView, TProperty> setter,
            Func<TView, TProperty> getter,
            ReactiveProperty<TProperty> source,
            Func<TView, IObservable<Unit>> updateSourceTrigger)
            where TView : UIResponder
        {
            var d = new CompositeDisposable();

            var isUpdating = false;
            source
                .Where(_ => !isUpdating)
                .Subscribe(x => setter(self, x))
                .AddTo(d);
            if (updateSourceTrigger != null && getter != null)
            {
                updateSourceTrigger(self).Subscribe(_ =>
                {
                    isUpdating = true;
                    try
                    {
                        source.Value = getter(self);
                    }
                    finally
                    {
                        isUpdating = false;
                    }
                });
            }

            return d;
        }

        /// <summary>
        /// Data binding method.
        /// </summary>
        /// <typeparam name="TView">View type</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="self">View</param>
        /// <param name="setter">Target value setter</param>
        /// <param name="source">Source property</param>
        /// <returns>Data binding token</returns>
        public static IDisposable SetBinding<TView, TProperty>(
            this TView self,
            Action<TView, TProperty> setter,
            ReactiveProperty<TProperty> source)
            where TView : UIResponder
        {
            return SetBinding(self, setter, null, source, null);
        }

        /// <summary>
        /// Data binding method.
        /// </summary>
        /// <typeparam name="TView">View type</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="self">View</param>
        /// <param name="propertySelector">Target property selector</param>
        /// <param name="source">Source property</param>
        /// <returns>Data binding token</returns>
        public static IDisposable SetBinding<TView, TProperty>(
            this TView self,
            Expression<Func<TView, TProperty>> propertySelector,
            ReadOnlyReactiveProperty<TProperty> source)
            where TView : UIResponder
        {
            var d = new CompositeDisposable();

            string propertyName;
            var setter = AccessorCache<TView>.LookupSet(propertySelector, out propertyName);
            source
                .Subscribe(x => setter(self, x))
                .AddTo(d);
            return d;
        }


        /// <summary>
        /// Data binding method.
        /// </summary>
        /// <typeparam name="TView">View type</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="self">View</param>
        /// <param name="setter">Target value setter</param>
        /// <param name="source">Source property</param>
        /// <returns>Data binding token</returns>
        public static IDisposable SetBinding<TView, TProperty>(
            this TView self,
            Action<TView, TProperty> setter,
            ReadOnlyReactiveProperty<TProperty> source)
            where TView : UIResponder
        {
            var d = new CompositeDisposable();

            source
                .Subscribe(x => setter(self, x))
                .AddTo(d);

            return d;
        }

    }
}