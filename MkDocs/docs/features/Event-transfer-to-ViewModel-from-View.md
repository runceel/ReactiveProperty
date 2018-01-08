EventToReactiveProperty and EventToReactiveCommand classes transfer event to ReactiveProeprty and ReactiveCommand from View layer.
Those classes extend TriggerAction. Those are designed that uses together with EventTrigger.

<b>Note:</b> 
> This feature provides to only WPF and UWP.
> Xamarin.Forms can't use this feature.

Those classes can convert EventArgs to any types object using ReactiveConverter&lt;T, U&gt;.

ReactiveConverter class can use Rx method chain. It's very powerful.


UWP sample:

```cs
using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace App1
{
    public class FileOpenReactiveConverter : ReactiveConverter<RoutedEventArgs, string>
    {
        protected override IObservable<string> OnConvert(IObservable<RoutedEventArgs> source)
        {
            return source.SelectMany(async _ =>
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".snippet");
                var f = await picker.PickSingleFileAsync();
                return f?.Path;
            })
            .Where(x => x != null);

        }
    }
}
```

It convert RoutedEventArgs to the file path.

XAML and Code behind are below.

```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="using:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      xmlns:i="using:Microsoft.Xaml.Interactivity"
      xmlns:c="using:Microsoft.Xaml.Interactions.Core"
      xmlns:reactiveProperty="using:Reactive.Bindings.Interactivity"
      mc:Ignorable="d">
    <StackPanel Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
        <Button Content="OpenFile...">
            <i:Interaction.Behaviors>
                <c:EventTriggerBehavior EventName="Click">
                    <reactiveProperty:EventToReactiveCommand Command="{x:Bind ViewModel.SelectFileCommand}">
                        <local:FileOpenReactiveConverter />
                    </reactiveProperty:EventToReactiveCommand>
                </c:EventTriggerBehavior>
            </i:Interaction.Behaviors>
        </Button>
        <TextBlock Text="{x:Bind ViewModel.FileName.Value, Mode=OneWay}" />
    </StackPanel>
</Page>
```

```cs
using Reactive.Bindings;
using Windows.UI.Xaml.Controls;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; } = new MainPageViewModel();

        public MainPage()
        {
            this.InitializeComponent();
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

![EventToReactiveCommand and EventToReactiveProperty](images/event-to-reactivexxx.gif)


EventToReactiveProperty sets the value converted by ReactiveConverter to ReactiveProperty.

```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="using:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      xmlns:i="using:Microsoft.Xaml.Interactivity"
      xmlns:c="using:Microsoft.Xaml.Interactions.Core"
      xmlns:reactiveProperty="using:Reactive.Bindings.Interactivity"
      mc:Ignorable="d">
    <StackPanel Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
        <Button Content="OpenFile...">
            <i:Interaction.Behaviors>
                <c:EventTriggerBehavior EventName="Click">
                    <reactiveProperty:EventToReactiveProperty ReactiveProperty="{x:Bind ViewModel.FileName}">
                        <local:FileOpenReactiveConverter />
                    </reactiveProperty:EventToReactiveProperty>
                </c:EventTriggerBehavior>
            </i:Interaction.Behaviors>
        </Button>
        <TextBlock Text="{x:Bind ViewModel.FileName.Value, Mode=OneWay}" />
    </StackPanel>
</Page>
```

```cs
using Reactive.Bindings;
using Windows.UI.Xaml.Controls;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; } = new MainPageViewModel();

        public MainPage()
        {
            this.InitializeComponent();
        }
    }

    public class MainPageViewModel
    {
        public ReactiveProperty<string> FileName { get; } = new ReactiveProperty<string>();
    }

}
```
