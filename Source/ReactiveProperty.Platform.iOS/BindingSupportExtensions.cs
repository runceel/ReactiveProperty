using System;
using System.Linq;
using System.Reactive.Linq;

namespace Reactive.Bindings
{
    /// <summary>
    /// Binding Support Extensions
    /// </summary>
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
