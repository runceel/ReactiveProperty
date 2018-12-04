using System;

namespace Reactive.Bindings.Binding
{
    /// <summary>
    /// ReactiveCommand bind to EventHandler
    /// </summary>
    public static class RxCommandExtensions
    {
        /// <summary>
        /// To the event handler.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns></returns>
        public static EventHandler ToEventHandler(this ReactiveCommand self)
        {
            EventHandler h = (s, e) =>
            {
                if (self.CanExecute())
                {
                    self.Execute();
                }
            };
            return h;
        }

        /// <summary>
        /// To the event handler.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="self">The self.</param>
        /// <returns></returns>
        public static EventHandler<TEventArgs> ToEventHandler<TEventArgs>(this ReactiveCommand self)
        {
            EventHandler<TEventArgs> h = (s, e) =>
            {
                if (self.CanExecute())
                {
                    self.Execute();
                }
            };
            return h;
        }

        /// <summary>
        /// To the event handler.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="converter">The converter.</param>
        /// <returns></returns>
        public static EventHandler<TEventArgs> ToEventHandler<TEventArgs, T>(this ReactiveCommand<T> self, Func<TEventArgs, T> converter)
        {
            EventHandler<TEventArgs> h = (s, e) =>
            {
                var parameter = converter(e);
                if (self.CanExecute())
                {
                    self.Execute(parameter);
                }
            };
            return h;
        }
    }
}
