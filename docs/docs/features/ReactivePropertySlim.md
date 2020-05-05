# ReactivePropertySlim

`ReactivePropertySlim` is a lightweight version `ReactiveProperty`. 
`ReactivePropertySlim` is five times faster than `ReactiveProperty`.

`ReactivePropertySlim` provides following features:

- Implements `INotifyPropertyChanged` interface.
- Implements `IObservable<T>` interface.
- Provides a `Value` property.
- Provides a `ForceNotify` method.

`ReactivePropertySlim` is high performance.

This class can be used like a `ReactiveProperty`.

```csharp
var rp = new ReactivePropertySlim<string>("neuecc");
rp.Select(x => $"{x}-san").Subscribe(x => Console.WriteLine(x));
rp.Value = "xin9le";
rp.Value = "okazuki";
```

Output is as below.

```
neuecc-san
xin9le-san
okazuki-san
```

One difference to `ReactiveProperty` is that `ReactivePropertySlim` can't be created from `IObservable<T>`.

```csharp
// It isn't valid code.
var rp = Observable.Interval(TimeSpan.FromSeconds(1)).ToReactivePropertySlim();
```

If you want to create Slim class's instance from `IObservable<T>`, then use the `ToReadOnlyReactivePropertySlim` extension method.

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1)).ToReadOnlyReactivePropertySlim();
```

## Dispatch to UI thread

`ReactivePropertySlim` class doesn't dispatch to the UI thread automatically.
If you need this, then use the `ReactiveProperty` or dispatch to the UI thread explicitly.

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ObserveOnUIDispatcher() // dispatch to UI thread
    .ToReadOnlyReactivePropertySlim();
```

## Validation

`ReactivePropertySlim` class doesn't provide the validation feature.
If you want this feature, then use the `ReactiveProperty` class.
