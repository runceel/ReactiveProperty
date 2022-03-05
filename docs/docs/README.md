# What is ReactiveProperty

ReactiveProperty provides MVVM and asynchronous support features under Reactive Extensions. Target framework is .NET Standard 2.0.

![Summary](./images/rpsummary.png)

Concept of ReactiveProperty is <b>Fun programing</b>.
You can write MVVM pattern programs using ReactiveProperty. It's very fun!

![UWP](./images/launch-uwp-app.gif)

Following code is two way binding between ReactiveProperty and plain object property.

```csharp
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

ReactiveProperty is implemented through `IObservable<T>`. Yes! You can use LINQ.

```csharp
var name = new ReactiveProperty<string>();
name.Where(x => x.StartsWith("_")) // filter
    .Select(x => x.ToUpper()) // convert
    .Subscribe(x => { ... some action ... });
```

ReactiveProperty is created from `IObservable<T>`. 

```csharp
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

And we provide `ReactiveCommand` class which implements `ICommand` and `IObservable<T>` interfaces. `ReactiveCommand` can be created from an `IObservable<bool>`.
Following sample creates a `ReactiveCommand` that is able to be executed when the `Input` property is not empty.

```csharp
class ViewModel
{
    public ReactiveProperty<string> Input { get; }
    public ReactiveProperty<string> Output { get; }

    public ReactiveCommand ResetCommand { get; }

    public ViewModel()
    {
        Input = new ReactiveProperty("");
        // Same as above sample
        Output = Input
            .Delay(TimeSpan.FromSecond(1)) // Using a Rx method.
            .Select(x => x.ToUpper()) // Using a LINQ method.
            .ToReactiveProperty(); // Convert to ReactiveProperty
        
        ResetCommand = Input.Select(x => !string.IsNullOrWhitespace(x)) // Convert ReactiveProperty<string> to IObservable<bool>
            .ToReactiveCommand() // You can create ReactiveCommand from IObservable<bool> (When true value was published, then the command would be able to execute.)
            .WithSubscribe(() => Input.Value = ""); // This is a shortcut of ResetCommand.Subscribe(() => ...)
    }
}
```

Cool!! It is really declarative, really clear.

## Let's start!

You can start using ReactiveProperty from the following links.

- [Windows Presentation Foundation](getting-started/wpf.md)
- [Universal Windows Platform](getting-started/uwp.md)
- [Xamarin.Forms](getting-started/xf.md)
- [Uno Platform](getting-started/uno-platform.md)

And learn the core features on following links.

- [ReactiveProperty](features/ReactiveProperty.md)
- [Commanding](features/Commanding.md)
- [Collections](features/Collections.md)

## NuGet packages

|Package Id|Version and downloads|Description|
|----|----|----|
|ReactiveProperty|![](https://img.shields.io/nuget/v/ReactiveProperty.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.svg)|The package includes all core features, and the target platform is .NET Standard 2.0. It fits almost all situations.|
|ReactiveProperty.Core|![](https://img.shields.io/nuget/v/ReactiveProperty.Core.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.Core.svg)|The package includes minimum classes such as `ReactivePropertySlim<T>` and `ReadOnlyReactivePropertySlim<T>`. And this doesn't have any dependency even System.Reactive. If you don't need Rx features, then it fits.|
|ReactiveProperty.WPF|![](https://img.shields.io/nuget/v/ReactiveProperty.WPF.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.WPF.svg)|The package includes EventToReactiveProperty and EventToReactiveCommand for WPF. This is for .NET Core 3.0 or later and .NET Framework 4.7.2 or later.|
|ReactiveProperty.UWP|![](https://img.shields.io/nuget/v/ReactiveProperty.UWP.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.UWP.svg)|The package includes EventToReactiveProperty and EventToReactiveCommand for UWP.|
|ReactiveProperty.XamarinAndroid|![](https://img.shields.io/nuget/v/ReactiveProperty.XamarinAndroid.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.XamarinAndroid.svg)|The package includes many extension methods to create IObservable from events for Xamarin.Android native.|
|ReactiveProperty.XamariniOS|![](https://img.shields.io/nuget/v/ReactiveProperty.XamariniOS.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.XamariniOS.svg)|The package includes many extension methods to bind ReactiveProperty and ReactiveCommand to Xamarin.iOS native controls.|
