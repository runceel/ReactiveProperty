using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Internal;
using UIKit;

namespace Reactive.Bindings
{
    /// <summary>
    /// IUI Table View Data Source Extensions
    /// </summary>
    public static class IUITableViewDataSourceExtensions
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
        public static IDisposable SetBindingTableViewDataSource<TView, TProperty>(
            this TView self,
            Expression<Func<TView, TProperty>> propertySelector,
            ReactiveProperty<TProperty> source, Func<TView, IObservable<Unit>> updateSourceTrigger = null)
            where TView : IUITableViewDataSource
        {
            var d = new CompositeDisposable();

            var isUpdating = false;
            var setter = AccessorCache<TView>.LookupSet(propertySelector, out var propertyName);
            source
                .Where(_ => !isUpdating)
                .Subscribe(x => setter(self, x))
                .AddTo(d);
            if (updateSourceTrigger != null) {
                var getter = AccessorCache<TView>.LookupGet(propertySelector, out propertyName);
                updateSourceTrigger(self).Subscribe(_ => {
                    isUpdating = true;
                    try {
                        source.Value = getter(self);
                    } finally {
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
        public static IDisposable SetBindingTableViewDataSource<TView, TProperty>(
            this TView self,
            Action<TView, TProperty> setter,
            Func<TView, TProperty> getter,
            ReactiveProperty<TProperty> source,
            Func<TView, IObservable<Unit>> updateSourceTrigger)
            where TView : IUITableViewDataSource
        {
            var d = new CompositeDisposable();

            var isUpdating = false;
            source
                .Where(_ => !isUpdating)
                .Subscribe(x => setter(self, x))
                .AddTo(d);
            if (updateSourceTrigger != null && getter != null) {
                updateSourceTrigger(self).Subscribe(_ => {
                    isUpdating = true;
                    try {
                        source.Value = getter(self);
                    } finally {
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
        public static IDisposable SetBindingTableViewDataSource<TView, TProperty>(
            this TView self,
            Action<TView, TProperty> setter,
            ReactiveProperty<TProperty> source)
            where TView : IUITableViewDataSource
        {
            return SetBindingTableViewDataSource(self, setter, null, source, null);
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
        public static IDisposable SetBindingTableViewDataSource<TView, TProperty>(
            this TView self,
            Expression<Func<TView, TProperty>> propertySelector,
            ReadOnlyReactiveProperty<TProperty> source)
            where TView : IUITableViewDataSource
        {
            var d = new CompositeDisposable();

            var setter = AccessorCache<TView>.LookupSet(propertySelector, out var propertyName);
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
        public static IDisposable SetBindingTableViewDataSource<TView, TProperty>(
            this TView self,
            Action<TView, TProperty> setter,
            ReadOnlyReactiveProperty<TProperty> source)
            where TView : IUITableViewDataSource
        {
            var d = new CompositeDisposable();

            source
                .Subscribe(x => setter(self, x))
                .AddTo(d);

            return d;
        }
    }
}
