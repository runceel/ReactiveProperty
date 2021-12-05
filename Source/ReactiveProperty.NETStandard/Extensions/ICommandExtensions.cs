using System;
using System.Reactive.Concurrency;
using System.Windows.Input;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.Extensions;

/// <summary>
/// ICommand Extensions
/// </summary>
public static class ICommandExtensions
{
    /// <summary>
    /// Converts CanExecuteChanged to an observable sequence.
    /// </summary>
    public static IObservable<EventArgs> CanExecuteChangedAsObservable<T>(this T source)
        where T : ICommand =>
        InternalObservable.FromEvent<EventHandler, EventArgs>(
            h => (sender, e) => h(e),
            h => source.CanExecuteChanged += h,
            h => source.CanExecuteChanged -= h);
}
