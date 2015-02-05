using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Codeplex.Reactive.Extensions
{
    public static class ToUnitObservableExtensions
    {
        public static IObservable<Unit> ToUnit<T>(this IObservable<T> self)
        {
            return self.Select(_ => Unit.Default);
        }
    }
}
