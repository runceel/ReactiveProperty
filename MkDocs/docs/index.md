# What is ReactiveProperty

ReactiveProperty provides MVVM and asynchronous support features under Reactive Extensions. Target framework is .NET 4.6, UWP, Xamarin.iOS, Xamarin.Android, Xamarin.Mac, Xamarin.Forms and .NET Standard 1.3.

![Summary](images/rpsummary.png)

Concept of ReactiveProperty is <b>Fun programing</b>.
You can write MVVM pattern program using ReactiveProperty very fun.

![UWP](images/launch-uwp-app.gif)

Following code is tow way binding between ReactiveProperty and plane object property.

```cs
class Model : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    }
}
class ViewModel
{
    private readonly Model _model = new Model();
    public ReactiveProperty<string> Name { get; }
    public ViewModel()
    {
        // TwoWay synchronize to ReactiveProperty and Model#Name property.
        Name = _model.ToReactivePropertyAsSynchronized(x => x.Name);
    }
}
```

ReactiveProperty is implemented the IObservable&lt;T&gt;. Yes! You can use LINQ.

```cs
var name = new ReactiveProperty<string>();
name.Where(x => x.StartsWith("_")) // filter
    .Select(x => x.ToUpper()) // convert
    .Subscribe(x => { ... some action ... });
```

ReactiveProperty is created from IObservable&lt;T&gt;. 

```cs
class ViewModel
{
    public ReactiveProperty<string> Input { get; }
    public ReactiveProperty<string> Output { get; }

    public ViewModel()
    {
        Input = new ReactiveProperty("");
        Output = Input
            .Delay(TimeSpan.FromSecond(1)) // Using a Rx method.
            .Select(x => x.ToUpper()) // Using a LINQ method.
            .ToReactiveProperty(); // Convert to ReactiveProperty
    }
}
```

This method chain is very cool.

And we provide ReactiveCommand class what implements ICommand and IObservable&lt;T&gt; interfaces. ReactiveCommand can create from an IObservable&lt;bool&gt;

```cs
var command = Observable.Interval(TimeSpan.FromSecond(1))
    .Select(x => x % 2 == 0) // convert to IO<bool>
    .ToReactiveCommand();
command.Subscribe(_ =>
{
    // ReactiveCommand invoke an OnNext when Execute method was called.
});
```

You can start the ReactiveProperty from following links.

- [Windows Presentation Foundation](getting-started/wpf.md)
- [Universal Windows Platform](getting-started/uwp.md)
- [Xamarin.Forms](getting-started/xf.md)

And learn to core features on following links.

- [ReactiveProperty](features/ReactiveProperty.md)
- [Commanding](features/Commanding.md)
- [Collections](features/Collections.md)
