# Getting start for WPF

## Create a project
- Create a WPF App (.NET Framework) project.
    - Have to use .NET Framework 4.6.1 or later, or .NET Core 3.0 or later.
- Install ReactiveProperty.WPF package from NuGet.

## Edit codes

- Add Startup event handler on App class.
- Add code to initialize scheduler for ReactiveProperty classes.

```csharp
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Schedulers;

namespace WpfApp1
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ReactivePropertyScheduler.SetDefault(new ReactivePropertyWpfScheduler(Dispatcher));
        }
    }
}
```

- Create a MainWindowViewModel.cs file.
- Edit files like following.

MainWindowViewModel.cs
```csharp
using Reactive.Bindings;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace WpfApp1
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ReactiveProperty<string> Input { get; }
        public ReadOnlyReactiveProperty<string> Output { get; }

        public MainWindowViewModel()
        {
            Input = new ReactiveProperty<string>("");
            Output = Input
                .Delay(TimeSpan.FromSeconds(1))
                .Select(x => x.ToUpper())
                .ToReadOnlyReactiveProperty();
        }
    }
}
```

MainWindow.xaml
```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfApp1"
        mc:Ignorable="d"
        Title="MainWindow"
        Height="350"
        Width="525">
    <Window.DataContext>
        <local:MainWindowViewModel />
    </Window.DataContext>
    <StackPanel>
        <Label Content="Input" />
        <TextBox Text="{Binding Input.Value, UpdateSourceTrigger=PropertyChanged}"
                 Margin="5" />
        <Label Content="Output" />
        <TextBlock Text="{Binding Output.Value}"
                   Margin="5" />
    </StackPanel>
</Window>
```

## Launch the application.

After launching the app, you can see the below window.
The output value was displayed to upper case, after 1sec from the input.

![Launch the app](./images/launch-wpf-app.gif)
