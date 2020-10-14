# ReactivePropertySlim

`ReactivePropertySlim` is a lightweight version `ReactiveProperty`. 
`ReactivePropertySlim` is five times faster than `ReactiveProperty`.

`ReactivePropertySlim` provides following features:

- Implements `INotifyPropertyChanged` interface.
- Implements `IObservable<T>` interface.
- Provides a `Value` property.
- Provides a `ForceNotify` method.

`ReactivePropertySlim` is high performance.
The following table is a result of the benchmark test between `ReactiveProperty` and `ReactivePropertySlim`.
ReactivePropertySlim is 16 times performance to create an instance, 36 times performance on the primary use case.

```
|                             Method |         Mean |     Error |    StdDev |
|----------------------------------- |-------------:|----------:|----------:|
|     CreateReactivePropertyInstance |    87.146 ns | 0.8331 ns | 0.7385 ns |
| CreateReactivePropertySlimInstance |     5.460 ns | 0.0537 ns | 0.0502 ns |
|           BasicForReactiveProperty | 2,470.957 ns | 9.1934 ns | 8.1497 ns |
|       BasicForReactivePropertySlim |    68.773 ns | 1.3841 ns | 1.8478 ns |
```

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
