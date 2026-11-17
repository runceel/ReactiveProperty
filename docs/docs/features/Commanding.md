# Commanding

`ReactiveCommand` class implements the following two interfaces.

- `ICommand` interface
- `IObservable<T>`

## Basic usage

This class can be created using the `ToReactiveCommand` extension method from an `IObservable<bool>` instance.
When the `IObservable<bool>` instance publishes a value, the `CanExecuteChanged` event is raised.

If you always want an executable command, you can create a `ReactiveCommand` instance using the default constructor.

```csharp
IObservable<bool> canExecuteSource = ...;

ReactiveCommand someCommand = canExecuteSource.ToReactiveCommand(); // version without a command parameter.
ReactiveCommand<string> hasCommandParameterCommand = canExecuteSource.ToReactiveCommand<string>(); // version with a command parameter
ReactiveCommand alwaysExecutableCommand = new ReactiveCommand(); // version without a command parameter that can always execute.
ReactiveCommand<string> alwaysExecutableAndHasCommandParameterCommand = new ReactiveCommand<string>(); // version with a command parameter that can always execute.
```

You can set the initial return value of the `CanExecute` method using the factory extension method's `initialValue` argument.
The default value is `true`.

```csharp
IObservable<bool> canExecuteSource = ...;

ReactiveCommand someCommand = canExecuteSource.ToReactiveCommand(false);
ReactiveCommand<string> hasCommandParameterCommand = canExecuteSource.ToReactiveCommand<string>(false);
```

When the `Execute` method is called, `ReactiveCommand` calls the `OnNext` callback.
You can register execute logic using the `Subscribe` method.

```csharp
ReactiveCommand someCommand = new ReactiveCommand();
someCommand.Subscribe(_ => { ... some logic ... }); // set an OnNext callback

someCommand.Execute(); // OnNext callback is called.
```

## Using in ViewModel class

The first example just uses the `ReactiveCommand` class.

```csharp
public class ViewModel
{
    public ReactiveCommand UpdateTimeCommand { get; }

    public ReactiveProperty<string> Time { get; }

    public ViewModel()
    {
        Time = new ReactiveProperty<string>();
        UpdateTimeCommand = new ReactiveCommand();
        UpdateTimeCommand.Subscribe(_ => Time.Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
    }
}
```

Example for WPF.

```csharp
public sealed partial class MainPage : Page
{
    private ViewModel ViewModel { get; } = new ViewModel();
    public MainPage()
    {
        this.InitializeComponent();
        DataContext = ViewModel;
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
    <StackPanel>
        <Button Content="Update the time"
                Command="{Binding UpdateTimeCommand}"
                Margin="5" />
        <TextBlock Text="{Binding Time.Value, Mode=OneWay}"
                   Margin="5" />
    </StackPanel>
</Page>
```

## Work with LINQ

`ReactiveCommand` class implements the `IObservable<T>` interface.
You can use LINQ methods, and the `ReactiveProperty<T>` class can be created from `IObservable<T>`.
The previous example code can be changed to this:

```csharp
public class ViewModel
{
    public ReactiveCommand UpdateTimeCommand { get; }

    // You don't need to set the Value property, so this can change to ReadOnlyReactiveProperty.
    public ReadOnlyReactiveProperty<string> Time { get; }

    public ViewModel()
    {
        UpdateTimeCommand = new ReactiveCommand();
        Time = UpdateTimeCommand
            .Select(_ => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            .ToReadOnlyReactiveProperty();
    }
}
```

## Create from `IObservable<bool>`

Change this so that the `UpdateTimeCommand` can't execute for 5 seconds after the command is invoked.

```csharp
public class ViewModel
{
    public ReactiveCommand UpdateTimeCommand { get; }

    public ReadOnlyReactiveProperty<string> Time { get; }

    public ViewModel()
    {
        var updateTimeTrigger = new Subject<Unit>();
        UpdateTimeCommand = Observable.Merge(
            updateTimeTrigger.Select(_ => false),
            updateTimeTrigger.Delay(TimeSpan.FromSeconds(5)).Select(_ => true))
            .ToReactiveCommand();
        Time = UpdateTimeCommand
            .Select(_ => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            .Do(_ => updateTimeTrigger.OnNext(Unit.Default))
            .ToReadOnlyReactiveProperty();
    }
}
```

## Create command and subscribe, in one statement

If you aren't using LINQ methods, you can create a command and subscribe in one statement.
`WithSubscribe` extension method subscribes and returns the `ReactiveCommand` instance:

```csharp
public class ViewModel
{
    public ReactiveCommand UpdateTimeCommand { get; }

    public ReactiveProperty<string> Time { get; }

    public ViewModel()
    {
        Time = new ReactiveProperty<string>();

        var updateTimeTrigger = new Subject<Unit>();
        UpdateTimeCommand = Observable.Merge(
            updateTimeTrigger.Select(_ => false),
            updateTimeTrigger.Delay(TimeSpan.FromSeconds(5)).Select(_ => true))
            .ToReactiveCommand()
            .WithSubscribe(() => Time.Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")); // here
    }
}
```

`WithSubscribe` method is just a shortcut:

```csharp
// Without WithSubscribe
var command = new ReactiveCommand();
command.Subscribe(_ => { ... some actions ... });

// With WithSubscribe
var command = new ReactiveCommand()
    .WithSubscribe(() => { ... some actions ... });
```

If you use LINQ methods, create separate statements to instantiate and subscribe.

## Unsubscribe actions

If you need to unsubscribe actions, use the `Dispose` method of the `IDisposable` instance returned by the `Subscribe` method.

```csharp
var command = new ReactiveCommand();
var subscription1 = command.Subscribe(_ => { ... some actions ... });
var subscription2 = command.Subscribe(_ => { ... some actions ... });

// Unsubscribe each Subscribe method.
subscription1.Dispose();
subscription2.Dispose();

// Unsubscribe all
command.Dispose();
```

The `WithSubscribe` extension method has overloads with an `IDisposable` argument.

```csharp
IDisposable subscription = null;
var command = new ReactiveCommand().WithSubscribe(() => { ... some actions ... }, out subscription);

// Unsubscribe
subscription.Dispose();
```

It also has another overload with an `Action<IDisposable>` argument.
It is used together with the `CompositeDisposable` class.

```csharp
var subscriptions = new CompositeDisposable();
var command = new ReactiveCommand()
    .WithSubscribe(() => { ... some actions ... }, subscriptions.Add)
    .WithSubscribe(() => { ... some actions ... }, subscriptions.Add);

// Unsubscribe
subscription.Dispose();
```

When subscribing to another instance's events, you should call the `Dispose` method of the `ReactiveCommand` class at the end of the ViewModel lifecycle.

## Async version of ReactiveCommand

The `AsyncReactiveCommand` class is an `async` version of the `ReactiveCommand` class.
This class can subscribe to `async` methods, and while an async method is executing, the `CanExecute` method returns `false`.
Therefore, this class can't re-execute while the async method is running.

The `ExecuteAsync` method is an async version of the `Execute` method. It can wait for all async processes added to the command to finish. This method is useful for unit testing and for calling commands from C#.

### Basic usage

It is nearly the same as the `ReactiveCommand` class.
The only difference is that it accepts an `async` method as a `Subscribe` method argument and doesn't implement the `IObservable<T>` interface.

```csharp
public class ViewModel
{
    public AsyncReactiveCommand HeavyCommand { get; }

    public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>();

    public ViewModel()
    {
        HeavyCommand = new AsyncReactiveCommand()
            .WithSubscribe(async () =>
            {
                Message.Value = "Heavy command started.";
                await Task.Delay(TimeSpan.FromSeconds(5));
                Message.Value = "Heavy command finished.";
            });
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
    <StackPanel>
        <Button Content="Heavy command"
                Command="{Binding HeavyCommand}"
                Margin="5" />
        <TextBlock Text="{Binding Message.Value, Mode=OneWay}"
                   Margin="5" />
    </StackPanel>
</Page>
```

Of course, `AsyncReactiveCommand` can be created from `IObservable<bool>`.

```csharp
public class ViewModel
{
    public AsyncReactiveCommand HeavyCommand { get; }

    public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>();

    public ViewModel()
    {
        HeavyCommand = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(x => x % 2 == 0)
            .ToAsyncReactiveCommand()
            .WithSubscribe(async () =>
            {
                Message.Value = "Heavy command started.";
                await Task.Delay(TimeSpan.FromSeconds(5));
                Message.Value = "Heavy command finished.";
            });
    }
}
```

The `AsyncReactiveCommand` class also implements the `IDisposable` interface.
You should call the `Dispose` method when subscribing to another instance's events.

### Share `CanExecute` state

Sometimes you want only one async method to execute on a page.
In this case, you can share the `CanExecute` state between `AsyncReactiveCommand` instances.
When they are created from the same `IReactiveProperty<bool>` instance, you can synchronize the `CanExecute` state.

```csharp
public class ViewModel
{
    private ReactiveProperty<bool> HeavyCommandCanExecuteState { get; } = new ReactiveProperty<bool>(true);
    public AsyncReactiveCommand HeavyCommand1 { get; }
    public AsyncReactiveCommand HeavyCommand2 { get; }

    public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>();

    public ViewModel()
    {
        HeavyCommand1 = HeavyCommandCanExecuteState
            .ToAsyncReactiveCommand()
            .WithSubscribe(async () =>
            {
                Message.Value = "Heavy command 1 started.";
                await Task.Delay(TimeSpan.FromSeconds(5));
                Message.Value = "Heavy command 1 finished.";
            });
        HeavyCommand2 = HeavyCommandCanExecuteState
            .ToAsyncReactiveCommand()
            .WithSubscribe(async () =>
            {
                Message.Value = "Heavy command 2 started.";
                await Task.Delay(TimeSpan.FromSeconds(5));
                Message.Value = "Heavy command 2 finished.";
            });
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
    <StackPanel>
        <Button Content="Heavy command 1"
                Command="{Binding HeavyCommand1}"
                Margin="5" />
        <Button Content="Heavy command 2"
                Command="{Binding HeavyCommand2}"
                Margin="5" />
        <TextBlock Text="{Binding Message.Value, Mode=OneWay}"
                   Margin="5" />
    </StackPanel>
</Page>
```

Of course, you can combine `IObservable<bool>` and `IReactiveProperty<bool>`. Use `IObservable<bool>` as the source of `AsyncReactiveCommand`, and use `IReactiveProperty<bool>` to share state across `AsyncReactiveCommand`s.
You can use `ToAsyncReactiveCommand(this IObservable<bool> source, IReactiveProperty<bool> sharedCanExecute = null)` method, like this:

```csharp
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RPSample
{
    public class MainPageViewModel
    {
        // for shared state
        private ReactivePropertySlim<bool> SharedCanExecute { get; }
        // for command source
        [Required]
        public ReactiveProperty<string> Input { get; }

        // commands
        public AsyncReactiveCommand CommandA { get; }
        public AsyncReactiveCommand CommandB { get; }

        public MainPageViewModel()
        {
            Input = new ReactiveProperty<string>().SetValidateAttribute(() => Input);

            // create AsyncReactiveCommands from the same source and the same IReactiveProperty<bool> to share CanExecute status.
            SharedCanExecute = new ReactivePropertySlim<bool>(true);
            CommandA = Input.ObserveHasErrors
                .Inverse()
                .ToAsyncReactiveCommand(SharedCanExecute)
                .WithSubscribe(() => Task.Delay(3000));
            CommandB = Input.ObserveHasErrors
                .Inverse()
                .ToAsyncReactiveCommand(SharedCanExecute)
                .WithSubscribe(() => Task.Delay(3000));
        }
    }
}
```

After binding the ViewModel class to the view:

```csharp
// code behind
using System.Windows.Controls;

namespace RPSample
{
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel ViewModel { get; } = new MainPageViewModel();
        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
```

```xml
<Page
    x:Class="RPSample.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d">

    <StackPanel>
        <TextBox Text="{Binding Input.Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
        <Button
            HorizontalAlignment="Stretch"
            Command="{Binding CommandA}"
            Content="CommandA" />
        <Button
            HorizontalAlignment="Stretch"
            Command="{Binding CommandB}"
            Content="CommandB" />
    </StackPanel>
</Page>
```

## Threading

### `ReactiveCommand` class

When using the `ReactiveCommand` class, the class raises the `CanExecute` event on a scheduler (the default is the UI thread scheduler). If you want to change the behavior, use the overload of `ToReactiveCommand` that has an `IScheduler` argument.

See below:

```csharp
canExecuteSource.ToReactiveCommand(theSchedulerInstanceYouWant);
```

### `AsyncReactiveCommand` class

`AsyncReactiveCommand` class doesn't change thread automatically. If you want to change the thread, then use `ObserveOn` method.

See below:

```csharp
canExecuteSource.ObserveOn(theSchedulerInstanceYouWant).ToAsyncReactiveCommand();
```

### `ReactiveCommandSlim`

This is a lightweight version of `ReactiveCommand`. The main difference from the traditional `ReactiveCommand` is that the `CanExecuteChanged` event is no longer dispatched on the UI thread. If it is necessary to always trigger the event on the UI thread, use the `ObserveOn` method on the `IObservable<bool>` source of `ReactiveCommandSlim` to explicitly set it.

Additionally, by implementing it in the same way as `ReactivePropertySlim`, various performance improvements have been made. The benchmarks are as follows:

|                                        Method |         Mean |       Error |      StdDev |       Median |
|---------------------------------------------- |-------------:|------------:|------------:|-------------:|
|                         CreateReactiveCommand |   291.931 ns |   5.6965 ns |  10.5589 ns |   291.178 ns |
|                     CreateReactiveCommandSlim |     4.313 ns |   0.1293 ns |   0.1080 ns |     4.269 ns |
|                BasicUsecaseForReactiveCommand | 1,187.294 ns |  22.8930 ns |  21.4141 ns | 1,179.896 ns |
|            BasicUsecaseForReactiveCommandSlim |    91.861 ns |   1.8934 ns |   3.5096 ns |    91.750 ns |

From top to bottom, these represent the creation of a `ReactiveCommand`, the creation of a `ReactiveCommandSlim`, the basic use case for `ReactiveCommand`, and the basic use case for `ReactiveCommandSlim`. There is a performance difference of over 70 times in the instantiation case and 13 times in the basic functionality usage case.

Furthermore, like `AsyncReactiveCommand`, it is possible to easily share the execution state across multiple commands by sharing an `IReactiveProperty<bool>`.
