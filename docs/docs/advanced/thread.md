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

## Limitations

ReactiveProperty was designed for single UI thread platform.
This means a few features don't work on multi UI thread platforms such as UWP.

UWP has multi UI threads in the single process when multiple Windows are created.
If you create multi-windows on UWP, then you should set `ImmediateScheduler` to `ReactivePropertyScheduler`, when the app was launched.
Or use `ReactivePropertySlim` / `ReadOnlyReactivePropertySlim` classes.
