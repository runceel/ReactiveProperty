using System;
using System.Collections;

namespace Reactive.Bindings
{
    public interface IHasErrors
    {
        IObservable<IEnumerable> ObserveErrorChanged { get; }
        IObservable<bool> ObserveHasErrors { get; }
    }
}
