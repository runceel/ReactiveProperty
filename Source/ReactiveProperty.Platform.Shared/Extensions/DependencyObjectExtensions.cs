using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Reactive.Bindings.Extensions;

/// <summary>
/// DependencyObject extension methods.
/// </summary>
public static class DependencyObjectExtensions
{
    /// <summary>
    /// Observe DependencyProperty
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="dp"></param>
    /// <returns></returns>
    public static IObservable<Unit> ObserveDependencyProperty<T>(this T self, DependencyProperty dp)
        where T : DependencyObject
    {
        return Observable.Create<Unit>(ox =>
        {
            void h(object? _, EventArgs __) => ox.OnNext(Unit.Default);
            var descriptor = DependencyPropertyDescriptor.FromProperty(dp, typeof(T));
            descriptor.AddValueChanged(self, h);
            return () => descriptor.RemoveValueChanged(self, h);
        });
    }

    /// <summary>
    /// Create ReadOnlyReactiveProperty from DependencyObject
    /// </summary>
    public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this DependencyObject self,
        DependencyProperty dp,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        IScheduler? eventScheduler = null) =>
        new(
            self.ObserveDependencyProperty(dp).Select(_ => (T)self.GetValue(dp)),
            (T)self.GetValue(dp),
            mode,
            eventScheduler);

    /// <summary>
    /// Create ReactiveProperty from DependencyObject
    /// </summary>
    public static ReactiveProperty<T> ToReactiveProperty<T>(this DependencyObject self,
        DependencyProperty dp,
        IScheduler? eventScheduler = null,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe) =>
        new(
            self.ObserveDependencyProperty(dp).Select(_ => (T)self.GetValue(dp)),
            eventScheduler ?? ReactivePropertyScheduler.Default,
            (T)self.GetValue(dp),
            mode);
}
