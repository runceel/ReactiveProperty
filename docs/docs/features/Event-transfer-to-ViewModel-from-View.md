# Transfer events to ViewModels from Views

`EventToReactiveProperty` and `EventToReactiveCommand` classes transfer events from the View layer to a `ReactiveProperty` or `ReactiveCommand`.
These classes extend `TriggerAction` and are designed to be used with `EventTrigger`.

<b>Note:</b>
> This feature is available for WPF. Add the `ReactiveProperty.WPF` package to your project.

These classes can convert `EventArgs` to any object type using `ReactiveConverter<T, U>`.

The `ReactiveConverter` class can use an Rx method chain. It's very powerful.


WPF sample:

```csharp
using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Win32;
using System.Windows;

namespace App1
{
    public class FileOpenReactiveConverter : ReactiveConverter<RoutedEventArgs, string>
    {
        protected override IObservable<string> OnConvert(IObservable<RoutedEventArgs> source)
        {
            return source.Select(_ =>
            {
                var picker = new OpenFileDialog { Filter = "Code snippets|*.snippet" };
                return picker.ShowDialog() == true ? picker.FileName : null;
            })
            .Where(x => x != null);

        }
    }
}
```

It converts the `RoutedEventArgs` to the file path.

XAML and code-behind are below.

```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="clr-namespace:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
      xmlns:reactiveProperty="clr-namespace:Reactive.Bindings.Interactivity;assembly=ReactiveProperty.WPF"
      mc:Ignorable="d">
    <StackPanel>
        <Button Content="OpenFile...">
            <i:Interaction.Triggers>
                <i:EventTrigger EventName="Click">
                    <reactiveProperty:EventToReactiveCommand Command="{Binding SelectFileCommand}">
                        <local:FileOpenReactiveConverter />
                    </reactiveProperty:EventToReactiveCommand>
                </i:EventTrigger>
            </i:Interaction.Triggers>
        </Button>
        <TextBlock Text="{Binding FileName.Value, Mode=OneWay}" />
    </StackPanel>
</Page>
```

```csharp
using Reactive.Bindings;
using System.Windows.Controls;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; } = new MainPageViewModel();

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }
    }

    public class MainPageViewModel
    {
        public ReactiveCommand<string> SelectFileCommand { get; }
        public ReadOnlyReactiveProperty<string> FileName { get; }

        public MainPageViewModel()
        {
            this.SelectFileCommand = new ReactiveCommand<string>();
            this.FileName = this.SelectFileCommand.ToReadOnlyReactiveProperty();
        }
    }

}
```


`EventToReactiveProperty` sets the value converted by `ReactiveConverter` to `ReactiveProperty`.

```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="clr-namespace:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
      xmlns:reactiveProperty="clr-namespace:Reactive.Bindings.Interactivity;assembly=ReactiveProperty.WPF"
      mc:Ignorable="d">
    <StackPanel>
        <Button Content="OpenFile...">
            <i:Interaction.Triggers>
                <i:EventTrigger EventName="Click">
                    <reactiveProperty:EventToReactiveProperty ReactiveProperty="{Binding FileName}">
                        <local:FileOpenReactiveConverter />
                    </reactiveProperty:EventToReactiveProperty>
                </i:EventTrigger>
            </i:Interaction.Triggers>
        </Button>
        <TextBlock Text="{Binding FileName.Value, Mode=OneWay}" />
    </StackPanel>
</Page>
```

```csharp
using Reactive.Bindings;
using System.Windows.Controls;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; } = new MainPageViewModel();

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }
    }

    public class MainPageViewModel
    {
        public ReactiveProperty<string> FileName { get; } = new ReactiveProperty<string>();
    }

}
```

## Customizing EventToReactiveCommand

### CallExecuteOnScheduler property

The default behavior calls the command's Execute method on the `IScheduler` set to `ReactivePropertyScheduler.Default`. To disable this behavior, set this property to false.

### AutoEnable property

The default behavior synchronizes automatically between AssociatedObject.IsEnabled and the command's CanExecute.
To disable this behavior, set this property to false.
