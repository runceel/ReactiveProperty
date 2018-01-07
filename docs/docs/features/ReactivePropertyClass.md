# ReactiveProperty class

ReactiveProperty is core class of this library.
This has following features.

- Implements the INotifyPropretyChanged interface.
    - The value property raise the PropertyChanged event.
- Implements the IObservable&lt;T&gt; interface.

Yes, The value property can bind to XAML control's property.
And the class call the IObserver&lt;T&gt;#OnNext method when set the value.

A sample code is below.

```cs
using Reactive.Bindings;
using System;

namespace ReactivePropertyEduApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // create from defualt constructor(default value is null)
            var name = new ReactiveProperty<string>();
            // setup the event handler and the onNext callback.
            name.PropertyChanged += (_, e) => Console.WriteLine($"PropertyChanged: {e.PropertyName}");
            name.Subscribe(x => Console.WriteLine($"OnNext: {x}"));

            // update the value property.
            name.Value = "neuecc";
            name.Value = "xin9le";
            name.Value = "okazuki";
        }
    }
}
```

The output of this program is below.

```
OnNext:
OnNext: neuecc
PropertyChanged: Value
OnNext: xin9le
PropertyChanged: Value
OnNext: okazuki
PropertyChanged: Value
```

What's deferent between PropertyChanged and onNext callback?
The onNext is called when subscribe. The PropertyChanged isn't called when added the event handler. And the onNext callback's argument is the property value, the PropertyChanged argument don't have the property value.

The PropertyChanged event was provided for the data binding. In the normal case, you should use the Reactive Extensions methods.

## Use with the XAML platform

The ReactiveProperty class is designed for the XAML platform which is like WPF, UWP, and Xamarin.Forms.
This class can be used the ViewModel layer. 

In the case that don't use the ReactiveProperty, the ViewModel class wrote the below. 

```cs
public class MainPageViewModel : INotifyPropertyChanged
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

    // Other properties are defined similar codes.
}
```

And those properties binds in the XAML code.

```xml
<!-- In the WPF -->
<TextBox Text="{Binding Name}" />

<!-- In the UWP(Runtime data binding) -->
<TextBox Text="{Binding Name}" />

<!-- In the UWP(Compile time data binding) -->
<TextBox Text="{x:Bind ViewModel.Name, Mode=TwoWay}" />

<!-- In the Xamarin.Forms -->
<Entry Text="{Binding Name}" />
```

In the case that uses the ReactiveProperty, The ViewModel code becomes very simple!

```cs
// The INotifyPropertyChanged interface must implement when using the WPF.
// Because, if you don't implement this, then memory leak occurred.
public class MainPageViewModel
{
    public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();

    // Other properties are defined similar codes.
}
```

When binding in the XAML code, you must add the `.Value` in the binding path.
This is an only limitation of this library.

```xml
<!-- In the WPF -->
<TextBox Text="{Binding Name.Value}" />

<!-- In the UWP(Runtime data binding) -->
<TextBox Text="{Binding Name}.Value" />

<!-- In the UWP(Compile time data binding) -->
<TextBox Text="{x:Bind ViewModel.Name.Value, Mode=TwoWay}" />

<!-- In the Xamarin.Forms -->
<Entry Text="{Binding Name.Value}" />
```

> We forget the `.Value` sometimes. If you have a ReSharper license, then you can use this plugin.
> [ReactiveProperty XAML Binding Corrector](https://resharper-plugins.jetbrains.com/packages/ReSharper.RpCorrector/)
> Highlight the missing of ReactiveProperty ".Value" in XAML.

## How to create a ReactiveProperty instance

The ReactiveProperty class can create from many methods.

### Create from the constructor

The most simplest way is that using the constructor.

```cs
// create with the default value.
var name = new ReactiveProperty<string>();
Console.WriteLine(name.Value); // -> empty output

// create with the initial value.
var name = new ReactiveProperty<string>("okazuki");
Console.WriteLine(name.Value); // -> okazuki
```

### Create from the IObservable&lt;T&gt;

This can create from the IObservable&lt;T&gt;.
Just calls the `ToReactiveProperty` method.

```cs
IObservable<long> observableInstance = Observable.Interval(TimeSpan.FromSeconds(1));

// Convert to ReactiveProperty from IObservable.
ReactiveProperty<long> counter = observableInstance.ToReactiveProperty();
```

#### Create from the ReactiveProperty

The ReactiveProperty implements the IObservable interface.
It means that a ReactiveProperty can be created from ReactiveProperty.

```cs
var name = new ReactiveProperty<string>("");

var formalName = name.Select(x => $"Dear {x}")
    .ToReactiveProperty();
```

All IObservable instances can become a ReactiveProperty. 

## Validation

The ReactiveProperty class implements the INotifyDataErrorInfo interface.
You can set the custom validation logic using the SetValidateNotifyError method.

```cs
var name = new ReactiveProperty<string>()
    .SetValidateNotifyError(x => string.IsNullOrWhiteSpace(x) ? "Error message" : null);
```

In the valid value case, the validation logic should return null.
In the invalid value case, the logic should return a error message.

### Validation with the System.Components.DataAnnotations

This class can integrate with the DataAnnotations.
You can set the validation attribute using the SetValidateAttribute method.

```cs
class ViewModel
{
    // Set validation attributes
    [Required(ErrorMessage = "The name is required.")]
    [StringLength(100, ErrorMessage = "The name length should be lower than 30.")]
    public ReactiveProperty<string> Name { get; }

    public ViewModel()
    {
        Name = new ReactiveProperty<string>()
            // Set validation attributes into the ReactiveProperty.
            .SetValidateAttribute(() => Name);
    }
}
```

