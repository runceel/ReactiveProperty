# Collections

ReactiveProperty provides some collection classes.

- ReactiveCollection&lt;T&gt;
- ReadOnlyReactiveCollection&lt;T&gt;
- IFilteredReadOnlyObservableCollection&lt;T&gt;

## ReactiveCollection

`ReactiveCollection` inherits `ObservableCollection`.
This class is created from `IObservable`.
It adds an item when a value is provided from the source `IObservable`.
`ReactiveCollection` executes this process using `IScheduler`. The default `IScheduler` dispatches to the UI thread.

```csharp
public class ViewModel
{
    public ReactiveCollection<DateTime> Records { get; }

    public ReactiveCommand StartRecordCommand { get; }

    public ViewModel()
    {
        StartRecordCommand = new ReactiveCommand();
        // Create a ReactiveCollection instance from IObservable
        Records = StartRecordCommand
            .ToUnit()
            .Take(1)
            .Concat(Observable.Defer(() => Observable.Interval(TimeSpan.FromSeconds(1)).ToUnit()))
            .Select(_ => DateTime.Now)
            .ToReactiveCollection();
    }
}
```

> `ToUnit` extension method is defined in the Reactive.Bindings.Extensions namespace.
> This extension method is the same as `.Select(_ => Unit.Default)`.

Example for WPF.

MainPage.xaml.cs
```csharp
public sealed partial class MainPage : Page
{
    public ViewModel ViewModel { get; } = new ViewModel();
    public MainPage()
    {
        this.InitializeComponent();
        DataContext = ViewModel;
    }
}
```

MainPage.xaml
```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="clr-namespace:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      mc:Ignorable="d">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition />
        </Grid.RowDefinitions>
        <StackPanel Orientation="Horizontal">
            <Button Content="Start"
                    Command="{Binding StartRecordCommand}"
                    Margin="5" />
        </StackPanel>
        <ListView ItemsSource="{Binding Records}"
                  Grid.Row="1" />
    </Grid>
</Page>
```

## Collection operations

The `ReactiveCollection` class has `XxxxOnScheduler` methods, such as `AddOnScheduler`, `RemoveOnScheduler`, `ClearOnScheduler`, and `GetOnScheduler`.
These methods run on the `IScheduler` and can be called from outside the UI thread.

```csharp
public class ViewModel
{
    public ReactiveCollection<DateTime> Records { get; }

    public ReactiveCommand StartRecordCommand { get; }

    public ReactiveCommand ClearCommand { get; }

    public ViewModel()
    {
        StartRecordCommand = new ReactiveCommand();
        // Create a ReactiveCollection instance from IObservable
        Records = StartRecordCommand
            .ToUnit()
            .Take(1)
            .Concat(Observable.Defer(() => Observable.Interval(TimeSpan.FromSeconds(1)).ToUnit()))
            .Select(_ => DateTime.Now)
            .ToReactiveCollection();

        ClearCommand = new ReactiveCommand();
        ClearCommand.ObserveOn(TaskPoolScheduler.Default) // run on another thread
            .Subscribe(_ => Records.ClearOnScheduler());
    }
}
```

```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="clr-namespace:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      mc:Ignorable="d">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition />
        </Grid.RowDefinitions>
        <StackPanel Orientation="Horizontal">
            <Button Content="Start"
                    Command="{Binding StartRecordCommand}"
                    Margin="5" />
            <Button Content="Clear"
                    Command="{Binding ClearCommand}"
                    Margin="5" />
        </StackPanel>
        <ListView ItemsSource="{Binding Records}"
                  Grid.Row="1" />
    </Grid>
</Page>
```

When a `ReactiveCollection` instance is disposed, it unsubscribes from the source IObservable instance.

## ReadOnlyReactiveCollection

The `ReadOnlyReactiveCollection` class provides one-way synchronization from `ObservableCollection`. You can set conversion logic and dispatch the `CollectionChanged` event on the `IScheduler`. The default `IScheduler` dispatches to the UI thread.

First, create POCO classes.

```csharp
public class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged([CallerMemberName]string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        RaisePropertyChanged(propertyName);
    }
}

public class TimerObject : BindableBase, IDisposable
{
    private IDisposable Disposable { get; }

    private long _count;
    public long Count
    {
        get { return _count; }
        private set { SetProperty(ref _count, value); }
    }

    public TimerObject()
    {
        Disposable = Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(_ => Count++);
    }

    public void Dispose()
    {
        Disposable.Dispose();
    }
}
```

This is a simple class that increments the `Count` property once per second.

Wrap the class in the ViewModel layer using ReactiveProperty.

```csharp
public class TimerObjectViewModel : IDisposable
{
    public TimerObject Model { get; }

    public ReadOnlyReactiveProperty<string> CountMessage { get; }

    public TimerObjectViewModel(TimerObject timerObject)
    {
        Model = timerObject;
        CountMessage = Model.ObserveProperty(x => x.Count)
            .Select(x => $"Count value is {x}.")
            .ToReadOnlyReactiveProperty();
    }

    public void Dispose()
    {
        Model.Dispose();
    }
}
```

Manage `TimerObject` instances using the `ObservableCollection`.
To provide `TimerObjectViewModel` instances to the View layer, use the `ReadOnlyReactiveCollection` class.
A `ReadOnlyReactiveCollection` instance is created using the `ToReadOnlyReactiveCollection` extension method.

```csharp
public class ViewModel
{
    // TimerObject collection
    private ReactiveCollection<TimerObject> ModelCollection { get; }
    // TimerObjectViewModel collection
    public ReadOnlyReactiveCollection<TimerObjectViewModel> ViewModelCollection { get; }

    public ReactiveCommand AddCommand { get; }

    public ReactiveCommand<TimerObjectViewModel> RemoveCommand { get; }

    public ViewModel()
    {
        AddCommand = new ReactiveCommand();
        ModelCollection = AddCommand
            .Select(_ => new TimerObject())
            .ToReactiveCollection();
        // Create a ReadOnlyReactiveCollection instance using the converting logic.
        ViewModelCollection = ModelCollection
            .ToReadOnlyReactiveCollection(x => new TimerObjectViewModel(x));

        RemoveCommand = new ReactiveCommand<TimerObjectViewModel>()
            .WithSubscribe(x => ModelCollection.Remove(x.Model));
    }
}
```

The test view is below.

```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="clr-namespace:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      xmlns:viewModels="clr-namespace:ViewModels"
      mc:Ignorable="d"
      x:Name="root">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition />
        </Grid.RowDefinitions>
        <Button Content="Add"
                Command="{Binding AddCommand}"
                Margin="5" />
        <ListView ItemsSource="{Binding ViewModelCollection}"
                  Grid.Row="1">
            <ListView.ItemTemplate>
                <DataTemplate DataType="{x:Type viewModels:TimerObjectViewModel}">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition />
                        </Grid.ColumnDefinitions>
                        <Button Content="Remove"
                                Command="{Binding DataContext.RemoveCommand, RelativeSource={RelativeSource AncestorType=Page}}"
                                CommandParameter="{Binding}"
                                Margin="5" />
                        <TextBlock Text="{Binding CountMessage.Value, Mode=OneWay}"
                                   VerticalAlignment="Center"
                                   Grid.Column="1" />
                    </Grid>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
    </Grid>
</Page>
```


When an instance is removed from the `ReadOnlyReactiveCollection`, the `Dispose` method is called. If you don't need this behavior, set the `disposeElement` argument to `false` in `ToReadOnlyReactiveCollection()`.

```csharp
ViewModelCollection = ModelCollection
    .ToReadOnlyReactiveCollection(x => new TimerObjectViewModel(x), disposeElement: false);
```

### Create from IObservable

`ReadOnlyReactiveCollection` can be created from `IObservable`, the same as `ReactiveCollection`. However, `ReadOnlyReactiveCollection` doesn't have collection operation methods.
`ToReadOnlyReactiveCollection` extension method has an `onReset` argument which is `IObservable<Unit>`.
When this argument publishes a value, the collection is cleared.

```csharp
public class ViewModel
{
    public ReadOnlyReactiveCollection<string> Messages { get; }

    public ReactiveCommand ResetCommand { get; }

    public ViewModel()
    {
        ResetCommand = new ReactiveCommand();
        Messages = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(_ => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            .ToReadOnlyReactiveCollection(ResetCommand.ToUnit());
    }
}
```

```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="clr-namespace:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      mc:Ignorable="d">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition />
        </Grid.RowDefinitions>
        <Button Content="Reset"
                Command="{Binding ResetCommand}"
                Margin="5" />
        <ListView ItemsSource="{Binding Messages}"
                  Grid.Row="1" />
    </Grid>
</Page>
```

When the `ResetCommand` is executed, the Messages collection is cleared.

## IFilteredReadOnlyObservableCollection

A collection that filters in real time from `ObservableCollection`.
`IFilteredReadOnlyObservableCollection` watches the `PropertyChanged` event of the source collection item and the `CollectionChanged` event.

```csharp
public class ValueHolder : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public int Id { get; set; }

    private int _value;
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    public ValueHolder()
    {
        var r = new Random();
        Observable.Interval(TimeSpan.FromSeconds(1))
            .ObserveOnUIDispatcher()
            .Subscribe(_ => Value = r.Next(10));
    }
}

public class ViewModel
{
    public ReactiveCollection<ValueHolder> ValuesSource { get; }

    public IFilteredReadOnlyObservableCollection<ValueHolder> Values { get; }

    public ReactiveCommand AddCommand { get; }

    public ViewModel()
    {
        AddCommand = new ReactiveCommand();
        ValuesSource = AddCommand
            .Select(_ => new ValueHolder { Id = ValuesSource.Count })
            .ToReactiveCollection();
        Values = ValuesSource.ToFilteredReadOnlyObservableCollection(
            x => x.Value > 7);
    }
}
```

> `ObserveOnUIDispatcher` extension method switches to the UI thread from the current thread.


```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="clr-namespace:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      xmlns:viewModels="clr-namespace:ViewModels"
      mc:Ignorable="d"
      x:Name="root">
    <Page.Resources>
        <DataTemplate x:Key="valueHolderDataTemplate"
                      DataType="{x:Type viewModels:ValueHolder}">
            <TextBlock>
                <Run Text="Id: " />
                <Run Text="{Binding Id}" />
                <Run Text=", Value: " />
                <Run Text="{Binding Value, Mode=OneWay}" />
            </TextBlock>
        </DataTemplate>
    </Page.Resources>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition />
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <Button Content="Add"
                Command="{Binding AddCommand}"
                Margin="5" />
        <TextBlock Text="Values"
                   Grid.Row="1" />
        <ListView ItemsSource="{Binding ValuesSource}"
                  ItemTemplate="{StaticResource valueHolderDataTemplate}"
                  Grid.Row="2" />
        <TextBlock Text="Filtered Values"
                   Grid.Row="1"
                   Grid.Column="1" />
        <ListView ItemsSource="{Binding Values}"
                  ItemTemplate="{StaticResource valueHolderDataTemplate}"
                  Grid.Row="2"
                  Grid.Column="1" />
    </Grid>
</Page>
```

When the Value property is greater than 7, display the value in the Filtered Values ListView (right side).

### Customize how to observe collection elements

If you want to change the trigger that updates elements from the CollectionChanged event to another trigger, you can customize it using another overload that has an `IObservable<T> sourceElementStatusChanged` argument.

For example, you may want to filter by a nested property of elements in a collection.

```csharp
// An object that has a nested object property
public class NestedPropertyObject : INotifyPropertyChanged
{
    // omit INPC impl

    public string Id { get; } => Guid.NewGuid().ToString();
    public ReactivePropertySlim<bool> NestedObject { get; } = new ReactivePropertySlim<bool>(true);
}

// --------------------
// Trigger
var sourceCollection = new ObservableCollection<NestedPropertyObject>
{
    new NestedPropertyObject(),
    new NestedPropertyObject(),
    new NestedPropertyObject(),
};

var filteredCollection = sourceCollection.ToFilteredReadOnlyObservableCollection(
    // a lambda expression for the filter condition
    x => x.NestedObject.Value,
    // create an IObservable instance for the collection element update trigger
    x => x.ObserveProperty(y => NestedObject.Value)
);

Console.WriteLine(filteredCollection.Count); // 3
// filteredCollection is observing the NestedObject.Value property path.
// Then the following line triggers re-evaluation of the filter condition
sourceCollection[1].NestedObject.Value = false;
Console.WriteLine(filteredCollection.Count); // 2
```

The following two lines are the same:

```csharp
collection.ToFilteredReadOnlyObservableCollection(x => x.SomeProperty);
collection.ToFilteredReadOnlyObservableCollection(x => x.SomeProperty, x => x.PropertyChangedAsObservable());
```
