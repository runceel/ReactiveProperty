using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Reactive.Bindings.Internals
{
    internal static class InternalObservable
    {
        public static IObservable<TEventArgs> FromEvent<TEventHandler, TEventArgs>(
            Func<Action<TEventArgs>, TEventHandler> convert,
            Action<TEventHandler> addHandler,
            Action<TEventHandler> removeHandler)
            where TEventArgs : EventArgs =>
            Observable.Create<TEventArgs>(ox =>
            {
                void handler(TEventArgs args)
                {
                    ox.OnNext(args);
                }

                var h = convert(handler);
                addHandler(h);
                return Disposable.Create((h, removeHandler), state => state.removeHandler(state.h));
            });
    }
}
