# View から ViewModel へイベントを転送する

`EventToReactiveProperty` クラスと `EventToReactiveCommand` クラスは、View レイヤーからのイベントを `ReactiveProperty` または `ReactiveCommand` に転送します。
これらのクラスは `TriggerAction` を拡張しており、`EventTrigger` と一緒に使うように設計されています。

<b>メモ:</b>
> この機能は WPF 向けです。`ReactiveProperty.WPF` パッケージをプロジェクトに追加してください。

これらのクラスは、`ReactiveConverter<T, U>` を使って `EventArgs` を任意のオブジェクト型に変換できます。

`ReactiveConverter` クラスでは Rx のメソッド チェーンを使用できます。非常に強力です。


WPF のサンプル:

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

これは `RoutedEventArgs` をファイル パスに変換します。

XAML とコード ビハインドは次のとおりです。

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


`EventToReactiveProperty` は、`ReactiveConverter` で変換された値を `ReactiveProperty` に設定します。

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

## EventToReactiveCommand のカスタマイズ

### CallExecuteOnScheduler プロパティ

既定の動作では、`ReactivePropertyScheduler.Default` に設定された `IScheduler` 上でコマンドの Execute メソッドを呼び出します。この動作を無効にするには、このプロパティを false に設定します。

### AutoEnable プロパティ

既定の動作では、AssociatedObject.IsEnabled とコマンドの CanExecute が自動的に同期されます。
この動作を無効にするには、このプロパティを false に設定します。
