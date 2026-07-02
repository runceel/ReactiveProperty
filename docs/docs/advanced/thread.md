# Threading

ReactiveProperty provides execution thread control features.
ReactiveProperty raises the `PropertyChanged` event on the UI thread automatically.

## Change the scheduler

You can change this behavior using `IScheduler`.
When the instance is created, set an `IScheduler` instance to the `raiseEventScheduler` argument.

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReactiveProperty(raiseEventScheduler: ImmediateScheduler.Instance);
```

`ReactiveCollection` and `ReadOnlyReactiveCollection` raise the `CollectionChanged` event on the UI thread, same as `ReactiveProperty`.
This behavior can be changed using the scheduler constructor and factory method argument.

```csharp
var collection = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReactiveCollection(scheduler: ImmediateScheduler.Instance);

var readOnlyCollection = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReadOnlyReactiveProperty(scheduler: ImmediateScheduler.Instance);
```

## Change the global scheduler

You can change ReactiveProperty's default scheduler using the `ReactivePropertyScheduler.SetDefault` method.

```csharp
ReactivePropertyScheduler.SetDefault(TaskPoolScheduler.Default);
var taskPoolRp = new ReactiveProperty<string>();
ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
var immediateRp = new ReactiveProperty<string>();

taskPoolRp.Value = "changed"; // raise event on the TaskPoolScheduler thread.
immediateRp.Value = "changed"; // raise event on the ImmediateScheduler thread.
```

## Change the global scheduler factory

Using the `ReactivePropertyScheduler.SetDefaultSchedulerFactory` method, you can change the factory method that creates ReactiveProperty's default scheduler instance.

```csharp
using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Threading;
using Reactive.Bindings;

namespace MultiUIThreadApp
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Set to create a DispatcherScheduler instance when each instance is created
            // for ReactiveProperty, ReadOnlyReactiveProperty, ReactiveCollection, and ReadOnlyReactiveProperty.
            ReactivePropertyScheduler.SetDefaultSchedulerFactory(() =>
                new DispatcherScheduler(Dispatcher.CurrentDispatcher));
        }
    }
}
```

## Rx operator

Of course, you can use the `ObserveOn` extension method.

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ObserveOn(someScheduler)
    .ToReactiveProperty();
```

We also provide the `ObserveOnUIDispatcher` extension method.
This is a shortcut for `ObserveOn(ReactivePropertyScheduler.Default)`.

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ObserveOnUIDispatcher()
    .ToReactiveProperty();
```

## Caution

By default, ReactiveProperty was designed for single-UI-thread platforms.
This means a few features don't work on multi-UI-thread platforms such as UWP.

UWP has multiple UI threads in a single process when multiple windows are created.
When creating multiple windows on UWP, set `ImmediateScheduler` using the `ReactivePropertyScheduler.SetDefault` method to disable automatic event dispatch to the UI thread, or create different scheduler instances for each UI thread using the `ReactivePropertyScheduler.SetDefaultSchedulerFactory` method. Alternatively, use the `ReactivePropertySlim` / `ReadOnlyReactivePropertySlim` classes instead of the ReactiveProperty / ReadOnlyReactiveProperty classes.
