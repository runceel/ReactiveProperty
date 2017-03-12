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
    public static class BindingSupportExtensions
    {
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

    }
}