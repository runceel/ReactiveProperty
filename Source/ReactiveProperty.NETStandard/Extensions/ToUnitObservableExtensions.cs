using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Reactive.Bindings.Extensions
{
    public static class ToUnitObservableExtensions
    {
        public static IObservable<Unit> ToUnit<T>(this IObservable<T> self) =>
            self.Select(_ => Unit.Default);
    }
}
