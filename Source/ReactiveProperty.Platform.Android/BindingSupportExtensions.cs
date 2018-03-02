using Android.Views;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Internal;
using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Reactive.Bindings
{
    public static class BindingSupportExtensions
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
            IReactiveProperty<TProperty> source, Func<TView, IObservable<Unit>> updateSourceTrigger = null)
            where TView : View
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
        /// <param name="propertySelector">Target property selector</param>
        /// <param name="source">Source property</param>
        /// <returns>Data binding token</returns>
        public static IDisposable SetBinding<TView, TProperty>(
            this TView self,
            Expression<Func<TView, TProperty>> propertySelector,
            IReadOnlyReactiveProperty<TProperty> source)
            where TView : View
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
        /// Command binding method.
        /// </summary>
        /// <typeparam name="T">Command type.</typeparam>
        /// <param name="self">IObservable</param>
        /// <param name="command">Command</param>
        /// <returns>Command binding token</returns>
        public static IDisposable SetCommand<T>(this IObservable<T> self, ReactiveCommand<T> command) =>
            self
                .Where(_ => command.CanExecute())
                .Subscribe(x => command.Execute(x));

        /// <summary>
        /// Command binding method.
        /// </summary>
        /// <typeparam name="T">IObservable type</typeparam>
        /// <param name="self">IObservable</param>
        /// <param name="command">Command</param>
        /// <returns>Command binding token</returns>
        public static IDisposable SetCommand<T>(this IObservable<T> self, ReactiveCommand command) =>
            self
                .Where(_ => command.CanExecute())
                .Subscribe(x => command.Execute());

        /// <summary>
        /// Command binding method.
        /// </summary>
        /// <typeparam name="T">Command type.</typeparam>
        /// <param name="self">IObservable</param>
        /// <param name="command">Command</param>
        /// <returns>Command binding token</returns>
        public static IDisposable SetCommand<T>(this IObservable<T> self, AsyncReactiveCommand<T> command) =>
            self
                .Where(_ => command.CanExecute())
                .Subscribe(x => command.Execute(x));

        /// <summary>
        /// Command binding method.
        /// </summary>
        /// <typeparam name="T">IObservable type</typeparam>
        /// <param name="self">IObservable</param>
        /// <param name="command">Command</param>
        /// <returns>Command binding token</returns>
        public static IDisposable SetCommand<T>(this IObservable<T> self, AsyncReactiveCommand command) =>
            self
                .Where(_ => command.CanExecute())
                .Subscribe(x => command.Execute());

    }
}