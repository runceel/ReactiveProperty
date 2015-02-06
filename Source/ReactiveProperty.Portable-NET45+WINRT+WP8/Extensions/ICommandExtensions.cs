using System;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Codeplex.Reactive.Extensions
{
    public static class ICommandExtensions
    {
        /// <summary>Converts CanExecuteChanged to an observable sequence.</summary>
        public static IObservable<EventArgs> CanExecuteChangedAsObservable<T>(this T source)
            where T : ICommand
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (sender, e) => h(e),
                h => source.CanExecuteChanged += h,
                h => source.CanExecuteChanged -= h);
        }
    }
}