# スレッド

ReactiveProperty は実行スレッドを制御する機能を提供します。
ReactiveProperty は `PropertyChanged` イベントを UI スレッドで自動的に発生させます。

## スケジューラーを変更する

この動作は `IScheduler` を使って変更できます。
インスタンスを作成するときに、`raiseEventScheduler` 引数に `IScheduler` インスタンスを設定します。

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReactiveProperty(raiseEventScheduler: ImmediateScheduler.Instance);
```

`ReactiveCollection` と `ReadOnlyReactiveCollection` は、`ReactiveProperty` と同様に UI スレッドで `CollectionChanged` イベントを発生させます。
この動作は、コンストラクターやファクトリー メソッドの scheduler 引数を使用して変更できます。

```csharp
var collection = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReactiveCollection(scheduler: ImmediateScheduler.Instance);

var readOnlyCollection = Observable.Interval(TimeSpan.FromSeconds(1))
    .ToReadOnlyReactiveProperty(scheduler: ImmediateScheduler.Instance);
```

## グローバル スケジューラーを変更する

`ReactivePropertyScheduler.SetDefault` メソッドを使用すると、ReactiveProperty の既定のスケジューラーを変更できます。

```csharp
ReactivePropertyScheduler.SetDefault(TaskPoolScheduler.Default);
var taskPoolRp = new ReactiveProperty<string>();
ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
var immediateRp = new ReactiveProperty<string>();

taskPoolRp.Value = "changed"; // TaskPoolScheduler スレッドでイベントを発生させます。
immediateRp.Value = "changed"; // ImmediateScheduler スレッドでイベントを発生させます。
```

## グローバル スケジューラー ファクトリーを変更する

`ReactivePropertyScheduler.SetDefaultSchedulerFactory` メソッドを使用すると、ReactiveProperty の既定のスケジューラー インスタンスを作成するファクトリー メソッドを変更できます。

```csharp
using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Threading;
using Reactive.Bindings;

namespace MultiUIThreadApp
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 各インスタンスの作成時に DispatcherScheduler インスタンスを作成するように設定します
            // 対象は ReactiveProperty、ReadOnlyReactiveProperty、ReactiveCollection、ReadOnlyReactiveProperty です。
            ReactivePropertyScheduler.SetDefaultSchedulerFactory(() =>
                new DispatcherScheduler(Dispatcher.CurrentDispatcher));
        }
    }
}
```

## Rx オペレーター

もちろん、`ObserveOn` 拡張メソッドを使用できます。

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ObserveOn(someScheduler)
    .ToReactiveProperty();
```

`ObserveOnUIDispatcher` 拡張メソッドも提供しています。
これは `ObserveOn(ReactivePropertyScheduler.Default)` のショートカットです。

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ObserveOnUIDispatcher()
    .ToReactiveProperty();
```

## 注意

既定では、ReactiveProperty は単一 UI スレッドのプラットフォーム向けに設計されています。
つまり、UWP などの複数 UI スレッドを持つプラットフォームでは、一部の機能が動作しません。

UWP では、複数のウィンドウを作成すると、単一プロセス内に複数の UI スレッドが存在します。
UWP で複数のウィンドウを作成する場合は、`ReactivePropertyScheduler.SetDefault` メソッドで `ImmediateScheduler` を設定して UI スレッドへの自動イベント ディスパッチを無効にするか、`ReactivePropertyScheduler.SetDefaultSchedulerFactory` メソッドを使用して UI スレッドごとに異なるスケジューラー インスタンスを作成してください。または、ReactiveProperty / ReadOnlyReactiveProperty クラスの代わりに `ReactivePropertySlim` / `ReadOnlyReactivePropertySlim` クラスを使用してください。
