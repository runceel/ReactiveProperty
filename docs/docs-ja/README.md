# ReactiveProperty とは

ReactiveProperty は Reactive Extensions 向けに MVVM と非同期処理を支援する機能を提供します。ターゲット フレームワークは .NET Standard 2.0 です。

![概要](../docs/images/rpsummary.png)

ReactiveProperty のコンセプトは <b>楽しいプログラミング</b> です。
ReactiveProperty を使うと MVVM アプリケーションを記述できます。とても楽しいですよ！

![UWP](../docs/images/launch-uwp-app.gif)

次のコードは、ReactiveProperty と通常のオブジェクト プロパティの間の双方向バインディングを示しています。

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
        // ReactiveProperty と Model#Name プロパティの双方向同期。
        Name = _model.ToReactivePropertyAsSynchronized(x => x.Name);
    }
}
```

ReactiveProperty は `IObservable<T>` を通じて実装されています。そうです！LINQ を使えます。

```csharp
var name = new ReactiveProperty<string>();
name.Where(x => x.StartsWith("_")) // フィルター
    .Select(x => x.ToUpper()) // 変換
    .Subscribe(x => { ... 何らかの処理 ... });
```

ReactiveProperty は `IObservable<T>` から作成されます。

```csharp
class ViewModel
{
    public ReactiveProperty<string> Input { get; }
    public ReactiveProperty<string> Output { get; }

    public ViewModel()
    {
        Input = new ReactiveProperty("");
        Output = Input
            .Delay(TimeSpan.FromSeconds(1)) // Rx メソッドを使用。
            .Select(x => x.ToUpper()) // LINQ メソッドを使用。
            .ToReactiveProperty(); // ReactiveProperty に変換
    }
}
```

このメソッド チェーンはとてもクールです。

`ICommand` と `IObservable<T>` インターフェイスを実装する `ReactiveCommand` クラスも提供しています。`ReactiveCommand` は `IObservable<bool>` から作成できます。
次のサンプルでは、`Input` プロパティが空でないときに実行できる `ReactiveCommand` を作成します。

```csharp
class ViewModel
{
    public ReactiveProperty<string> Input { get; }
    public ReactiveProperty<string> Output { get; }

    public ReactiveCommand ResetCommand { get; }

    public ViewModel()
    {
        Input = new ReactiveProperty("");
        // 上のサンプルと同じ
        Output = Input
            .Delay(TimeSpan.FromSeconds(1)) // Rx メソッドを使用。
            .Select(x => x.ToUpper()) // LINQ メソッドを使用。
            .ToReactiveProperty(); // ReactiveProperty に変換

        ResetCommand = Input.Select(x => !string.IsNullOrWhiteSpace(x)) // ReactiveProperty<string> を IObservable<bool> に変換
            .ToReactiveCommand() // IObservable<bool> から ReactiveCommand を作成できます。true 値が発行されると、コマンドを実行できます。
            .WithSubscribe(() => Input.Value = ""); // ResetCommand.Subscribe(() => ...) のショートカットです。
    }
}
```

クールです！本当に宣言的で分かりやすいです。

## 始めましょう！

ReactiveProperty は次のリンクから使い始めることができます。

- [Windows Presentation Foundation](getting-started/wpf.md)
- [Universal Windows Platform](getting-started/uwp.md)
- [Xamarin.Forms](getting-started/xf.md)
- [Uno Platform](getting-started/uno-platform.md)

コア機能については、次のリンクで学べます。

- [ReactiveProperty](features/ReactiveProperty.md)
- [コマンド](features/Commanding.md)
- [コレクション](features/Collections.md)


## NuGet パッケージ

|パッケージ ID|バージョンとダウンロード数|説明|
|----|----|----|
|ReactiveProperty|![](https://img.shields.io/nuget/v/ReactiveProperty.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.svg)|すべてのコア機能が含まれ、ターゲット プラットフォームは .NET Standard 2.0 です。ほぼすべての状況に適しています。|
|ReactiveProperty.Core|![](https://img.shields.io/nuget/v/ReactiveProperty.Core.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.Core.svg)|`ReactivePropertySlim<T>` や `ReadOnlyReactivePropertySlim<T>` など、最小限のクラスが含まれます。System.Reactive にも依存しません。Rx 機能が不要な場合に適しています。|
|ReactiveProperty.WPF|![](https://img.shields.io/nuget/v/ReactiveProperty.WPF.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.WPF.svg)|WPF 向けの EventToReactiveProperty と EventToReactiveCommand が含まれます。.NET Core 3.0 以降および .NET Framework 4.7.2 以降向けです。|
|ReactiveProperty.UWP|![](https://img.shields.io/nuget/v/ReactiveProperty.UWP.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.UWP.svg)|UWP 向けの EventToReactiveProperty と EventToReactiveCommand が含まれます。|
|ReactiveProperty.XamarinAndroid|![](https://img.shields.io/nuget/v/ReactiveProperty.XamarinAndroid.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.XamarinAndroid.svg)|Xamarin.Android ネイティブのイベントから IObservable インスタンスを作成するための多くの拡張メソッドが含まれます。|
|ReactiveProperty.XamariniOS|![](https://img.shields.io/nuget/v/ReactiveProperty.XamariniOS.svg)![](https://img.shields.io/nuget/dt/ReactiveProperty.XamariniOS.svg)|ReactiveProperty と ReactiveCommand を Xamarin.iOS ネイティブ コントロールにバインドするための多くの拡張メソッドが含まれます。|