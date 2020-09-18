# Threading

ReactiveProperty provides execution thread control feature.
ReactiveProperty raises `PropertyChanged` event on UI thread automatically. 

## Change the scheduler

You can change this behavior using `IScheduler`.
When the instance is created, set `IScheduler` instance to `raiseEventScheduler` argument.

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReactiveProperty(raiseEventScheduler: ImmediateScheduler.Instance);
```

`ReactiveCollection` and `ReadOnlyReactiveCollection` raise `CollectionChanged` event on UI thread same as `ReactiveProperty`.
This behavior can be changed using the scheduler constructor and factory method argument.

```csharp
var collection = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReactiveCollection(scheduler: ImmediateScheduler.Instance);

var readOnlyCollection = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReadOnlyReactiveProperty(scheduler: ImmediateScheduler.Instance);
```

## Change the global scheduler

You can change the ReactiveProperty's default scheduler using `ReactivePropertyScheduler.SetDefault` method.

```csharp
ReactivePropertyScheduler.SetDefault(TaskPoolScheduler.Default);
var taskPoolRp = new ReactiveProperty<string>();
ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
var immediateRp = new ReactiveProperty<string>();

taskPoolRp.Value = "changed"; // raise event on the TaskPoolScheduler thread.
immediateRp.Value = "changed"; // raise event on the ImmediateScheduler thread.
```

## Change the global scheduler factory

Using the `ReactivePropertyScheduler.SetDefaultSchedulerFactory` method, you can change a factory method to create the ReactiveProperty's default scheduler instance.

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
            // Set to create a DispatcherScheduler instance when every instance is created 
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

And we provide the `ObserveOnUIDispatcher` extension method. 
This is a shortcut of `ObserveOn(ReactiveProeprtyScheduler.Default)`.

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ObserveOnUIDispatcher()
    .ToReactiveProperty();
```

## Caution

As a default, ReactiveProperty was designed for a single UI thread platform.
It means a few features don't work on multi UI thread platforms such as UWP.

UWP has multi UI threads in the single process when multiple Windows are created.
So, in case of creating multi-windows on UWP, then you should set `ImmediateScheduler` using the `ReactivePropertyScheduler.SetDefault` method to disable a feature of auto dispatch events to UI thread or change to create different scheduler instances for each UI thread using the `ReactivePropertyScheduler.SetDefaultSchedulerFactory` method. Or please use `ReactivePropertySlim` / `ReadOnlyReactivePropertySlim` classes instead of ReactiveProperty/ReadOnlyReactiveProperty classes.
