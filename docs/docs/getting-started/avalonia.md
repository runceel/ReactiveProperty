Avalonia is cross platform .NET UI Framework!

See below:

[Avalonia UI Framework](http://avaloniaui.net/)

## Create a project
- Create a Avalonia .NET Core Application project.(Of cource! Avalonia Application project can use ReactiveProperty same as .NET Core project.)
- Install ReactiveProperty from NuGet.

## Edit codes
- Create a MainWindowViewModel.cs file.
- Edit files like following.

MainWindowViewModel.cs
```cs
using Reactive.Bindings;
using System;
using System.Reactive.Linq;

namespace AvaloniaApp
{
    public class MainWindowViewModel
    {
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
<Window xmlns="https://github.com/avaloniaui" 
        MinWidth="200" 
        MinHeight="300"
        xmlns:local="clr-namespace:AvaloniaApp;assembly=AvaloniaApp">
  <Window.DataContext>
    <local:MainWindowViewModel />
  </Window.DataContext>
  <StackPanel>
    <TextBox Text="{Binding Input.Value, Mode=TwoWay}" />
    <TextBlock Text="{Binding Output.Value}" />
  </StackPanel>
</Window>
```

## Launch the application.

After launch the app, You can see the below window.
The output value was displayed to upper case, after 1sec from the input.

![Launch the app](images/launch-avalonia-app.gif)
