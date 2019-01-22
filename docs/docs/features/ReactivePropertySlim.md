# ReactivePropertySlim

ReactivePropertySlim is a lightweight version ReactiveProperty.

ReactivePropertySlim provides following features:

- Implements INotifyPropertyChanged interface.
- Implements IObservable&lt;T&gt; interface.
- Provides a Value property.
- Provides a ForceNotify method.

And ReactivePropertySlim is high performance.

This class can use like same as ReactiveProperty.

```cs
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

One different which compare the ReactiveProperty is that ReactivePropertySlim can't create from IObservable&lt;T&gt;.

```cs
// It isn't valid code.
var rp = Observable.Interval(TimeSpan.FromSeconds(1)).ToReactivePropertySlim();
```

If want to create Slim class's instance from IObservable&lt;T&gt;, then use the ToReadOnlyReactivePropertySlim extension method.

```cs
var rp = Observable.Interval(TimeSpan.FromSeconds(1)).ToReadOnlyReactivePropertySlim();
```

## Dispatch to UI thread

ReactivePropertySlim class doesn't dispatch to UI thread automaticaly.
If want this feature, then use the ReactiveProperty or dispatch to UI thread expressly.

```cs
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ObserveOnUIDispatcher() // dispatch to UI thread
    .ToReadOnlyReactivePropertySlim();
```

## Validation

Slim class doesn't provide the validation feature.
If want to this feature, then use to ReactiveProperty class.
