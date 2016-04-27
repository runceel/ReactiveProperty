[Japanese](README-ja.md)

ReactiveProperty
================

![ReactiveProperty overview](Images/rpsummary.png)

ReactiveProperty is MVVM and Asynchronous Extensions for Reactive Extensions.
Target Framework is .NET 4.0, .NET 4.5, .NET 4.6, Windows Phone 8.0/8.1, Windows store app 8.1, UWP, Xamarin.iOS, Xamarin.Android.

## Release note

[Release note](ReleaseNote.md)

## Features

ReactiveProperty is MVVM support library. ReactiveProperty has functions follows.

- ReactiveProeprty class.
- ReactiveCommand class.
- ReactiveCollection class.
- ReadOnlyReactiveCollection class.
- Convert to IObservable from INotifyPropertyChanged/INotifyCollectionChanged.
- Many IObservable factory methods.
- Notifier classes.
- IFilteredReadOnlyObservableCollection interface.

## ReactiveProperty class.
This is core class in this library. ReactiveProperty can define notify property very easy.
But, little longer more than normal property when XAML data binding.

For example, define Name property in ViewModel follows.

```cs
public class PersonViewModel
{
    public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
}
```

Set and get value to Value property from ReactiveProperty class.
So, It implements INotifyPropertyChanged interface.
It can use follows.

```cs
// Must set Scheduler in Console Application
ReactivePropertyScheduler.SetDefault(CurrentThreadScheduler.Default);

var vm = new PersonViewModel();
vm.Name.PropertyChanged += (_, __) => Console.WriteLine($"Name changed {vm.Name.Value}");

vm.Name.Value = "tanaka";
Console.ReadKey();
vm.Name.Value = "kimura";
Console.ReadKey();
```

Output is follows.

```
Name changed tanaka
Name changed kimura
```

## How to use XAML platform

ReactiveProperty can use XAML platform application.
It can use Universal Windows Platform, WPF and Xamarin.
Here, I would like to explain the use of in the UWP.

Binding to MainPage with previous PersonViewModel.
Write code behind.

```cs
public sealed partial class MainPage : Page
{
    public PersonViewModel ViewModel { get; } = new PersonViewModel();

    public MainPage()
    {
        this.InitializeComponent();
    }
}
```

MUST use Value property in BindingPath.

```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="using:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      mc:Ignorable="d">

    <StackPanel Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
        <TextBox Text="{x:Bind ViewModel.Name.Value, Mode=TwoWay}" />
        <TextBlock Text="{x:Bind ViewModel.Name.Value, Mode=OneWay}" />
    </StackPanel>
</Page>
```

When execute this program, synchronize TextBox and TextBlock.

![Synchronize TextBox and TextBlock](Images/synchronize.png)

## ReactiveProperty can generate from IObservable

ReactiveProperty provide to easy way notify property. It doesn't extend ViewModel base class.
Otherwise ReactiveProperty can create from IObservable interface.

Call ToReactiveProperty extension method to IObservable interface.
ReactiveProperty will store value when pushed value from IObservable.

For example follows.

```cs
public class PersonViewModel
{
    public ReactiveProperty<string> Name { get; } = Observable.Interval(TimeSpan.FromSeconds(1))
        .Select(x => $"tanaka {x}")
        .ToReactiveProperty();
}
```

Update the value at one-second intervals.

![Update the value at one-second intervals](Images/interval.png)

## ReactiveProperty is IObservable

You understand that ReactiveProperty can generate from IObservable.
Otherwise ReactiveProperty is IObservable.
ReactiveProperty can generate from ReactiveProperty.


For example follows.

```cs
public class PersonViewModel
{
    public ReactiveProperty<string> Input { get; } = new ReactiveProperty<string>("");

    public ReactiveProperty<string> Output { get; }

    public PersonViewModel()
    {
        this.Output = this.Input
            .Delay(TimeSpan.FromSeconds(1))
            .Select(x => x.ToUpper())
            .ToReactiveProperty();
    }
}
```

XAML is fllows.

```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="using:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      x:Name="Page"
      mc:Ignorable="d">

    <StackPanel Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
        <TextBox Text="{Binding ViewModel.Input.Value, ElementName=Page, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
        <TextBlock Text="{x:Bind ViewModel.Output.Value, Mode=OneWay}" />
    </StackPanel>
</Page>
```

It will appear uppercase characters at delay one-second.

![delay](Images/delay.png)

## ReadOnlyReactiveProperty

There is also a case where it is preferred not set a value in the Value of ReactiveProperty as Output property of the previous example.
This case is use ToReadOnlyReactiveProperty extension method.
This method create ReadOnlyReactiveProperty. This class can not write Value property.

```cs
public class PersonViewModel
{
    public ReactiveProperty<string> Input { get; } = new ReactiveProperty<string>("");

    public ReadOnlyReactiveProperty<string> Output { get; }

    public PersonViewModel()
    {
        this.Output = this.Input
            .Delay(TimeSpan.FromSeconds(1))
            .Select(x => x.ToUpper())
            .ToReadOnlyReactiveProperty();
    }
}
```

## Mode of ReactiveProperty

ReactiveProperty can set mode when called ToReactiveProperty extension method.
Mode define follows.

```cs
[Flags]
public enum ReactivePropertyMode
{
    None = 0,
    DistinctUntilChanged = 1,
    RaiseLatestValueOnSubscribe = 2
}
```

DistinctUntilChanged does't call OnNext/PropertyChanged when set same values.
RaiseLatestValueOnSubscribe push value when subscribed.

Default mode is here.

```cs
mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueSubscribe;
```

## Connect to exist models

ReactiveProperty will use ViewModel of MVVM pattern.(But you can use model layer.)
Therefore, ReactiveProperty have useful methods to synchronized plane models.

### Create ReactiveProperty from model that implement INotifyPropertyChanged

For example, that is class of implements INotifyPropertyChanged.
Follows.

```cs
public class Person : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private static readonly PropertyChangedEventArgs NamePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Name));

    private string name;

    public string Name
    {
        get { return this.name; }
        set
        {
            if (this.name == value) { return; }
            this.name = value;
            this.PropertyChanged?.Invoke(this, NamePropertyChangedEventArgs);
        }
    }

}
```

ViewModel class that observe name property is follows.


```cs
public class PersonViewModel : IDisposable
{
    private Person Model { get; set; }
    public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();

    public PersonViewModel(Person model)
    {
        this.Model = model;
        model.PropertyChanged += this.Model_PropertyChanged;
    }

    private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Person.Name):
                this.Name.Value = ((Person)sender).Name;
                break;
        }
    }

    public void Dispose()
    {
        this.Model.PropertyChanged -= this.Model_PropertyChanged;
        this.Model = null;
    }
}
```

If Person class have many property...very troublesome.

ReactiveProperty have ObserveProperty extension method.
This extension method create from IObservable from INotifyPropertyChanged.
ReactiveProperty can create from IObservable. Because...

```cs
public class PersonViewModel : IDisposable
{
    private CompositeDisposable Disposable { get; } = new CompositeDisposable();
    public ReactiveProperty<string> Name { get; }

    public PersonViewModel(Person model)
    {
        this.Name = model.ObserveProperty(x => x.Name)
            .ToReactiveProperty()
            .AddTo(this.Disposable);
    }

    public void Dispose()
    {
        this.Disposable.Dispose();
    }
}
```

Very shortry. ReactiveProperty stop subscribe when dispose method called.
If there is a lot of property in the Model, it is useful to collect ReactiveProperty to CompositeDisposable.

And use select extension method. Then can convert model value to ViewModel.

```cs
public PersonViewModel(Person model)
{
    this.Name = model.ObserveProperty(x => x.Name)
        .Select(x => $"{x}В≥Вс")
        .ToReactiveProperty()
        .AddTo(this.Disposable);
}
```




## Sample program 1

- [MVVM pattern and Reactive programming sample](https://code.msdn.microsoft.com/MVVM-pattern-and-Reactive-2f71560a)

## Sample program 2

UWPTodoMVVM project in Sample folder is like [TodoMVC](http://todomvc.com/) todo application.

![TodoMVVM](Images/todomvvm.png)

## Author info

Yoshifumi Kawai a.k.a. neuecc is software developer in Tokyo, Japan.
Awarded Microsoft MVP for Visual Studio Development Technorogy April, 2011.

Takaaki Suzuki a.k.a. xin9le software devleoper in Fukui, Japan.
Awarded Microsoft MVP for Visual Studio Development Technorogy since July, 2012.

Kazuki Ota a.k.a. okazuki software developer in Tokyo, Japan.
Awarded Microsoft MVP for Windows Developer since July, 2011.



