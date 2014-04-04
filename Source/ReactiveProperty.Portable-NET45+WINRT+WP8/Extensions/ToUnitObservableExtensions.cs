using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;

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
