ReactiveProperty provides execution thread control feature.
ReactiveProperty raise PropertyChanged event on UI thread automaticaly. 

# Change the scheduler

You can change this behavior using IScheduler.
When the instance creation time, set IScheduler instance to raiseEventScheduler argument.

```cs
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReactiveProperty(raiseEventScheduler: ImmediateScheduler.Instance);
```

ReactiveCollection and ReadOnlyReactiveCollection raise CollectionChanged event on UI thread same as ReactiveProperty.
This behavior can be changed using the scheduler constructor and factory method argument.

```cs
var collection = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReactiveCollection(scheduler: ImmediateScheduler.Instance);

var readOnlyCollection = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReadOnlyReactiveProperty(scheduler: ImmediateScheduler.Instance);
```

# Change the gloabl scheduler

Can change the ReactiveProperty's default scheduler using ReactivePropertyScheduler.SetDefault method.

```cs
ReactivePropertyScheduler.SetDefault(TaskPoolScheduler.Default);
var taskPoolRp = new ReactiveProperty<string>();
ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
var immediateRp = new ReactiveProperty<string>();

taskPoolRp.Value = "changed"; // raise event on the TaskPoolScheduler thread.
immediateRp.Value = "changed"; // raise event on the ImmediateScheduler thread.
```
