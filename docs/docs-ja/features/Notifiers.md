# 通知クラス

`Reactive.Bindings.Notifiers` 名前空間は、`IObservable` インターフェイスを実装する多くの便利なクラスを提供します。

## `BooleanNotifier`

`BooleanNotifier` クラスは `IObservable<bool>` インターフェイスを実装しています。
いくつかのメソッドとプロパティを持っています。

- `TurnOn` メソッド
    - 状態を true に変更します。
- `TurnOff` メソッド
    - 状態を false に変更します。
- `SwitchValue` メソッド
    - 状態を切り替えます。
- `Value` プロパティ
    - 状態を設定します。

初期状態はコンストラクターで設定できます。既定値は false です。


```csharp
var n = new BooleanNotifier();
n.Subscribe(x => Debug.WriteLine(x));

n.TurnOn(); // true
n.TurnOff(); // false
n.Value = true; // true
n.Value = false; // false
```

次のように `ReactiveCommand` のソースとして使えます。

```csharp
var n = new BooleanNotifier(); // 既定値は false です。

// ReactiveCommand の CanExecute メソッドは既定で true を返すため、initialValue に `n.Value` を明示的に設定します。
var command = n.ToReactiveCommand(initialValue: n.Value);

// または、ToReactiveCommand を呼び出す前に Select などの演算子で何かに変換したい場合は、StartWith を使えます。
var command2 = n.StartWith(n.Value).Select(x => Something(x)).ToReactiveCommand();
```

## `CountNotifier`

`CountNotifier` クラスは `IObservable<CountChangedStatus>` インターフェイスを実装しています。インクリメントとデクリメント機能を提供し、状態が変わると `CountChangedStatus` 値を発行します。

CountChangedStatus enum は次のように定義されています。

```csharp
/// <summary>CountNotifier のイベント種別です。</summary>
public enum CountChangedStatus
{
    /// <summary>カウントがインクリメントされました。</summary>
    Increment,
    /// <summary>カウントがデクリメントされました。</summary>
    Decrement,
    /// <summary>カウントは 0 です。</summary>
    Empty,
    /// <summary>カウントが最大値に達しました。</summary>
    Max
}
```

`CountNotifier` の最大値はコンストラクター引数から設定できます。

```csharp
var c = new CountNotifier(); // 既定の最大値は int.MaxValue です
// 状態を出力します。
c.Subscribe(x => Debug.WriteLine(x));
// 現在の値を出力します。
c.Select(_ => c.Count).Subscribe(x => Debug.WriteLine(x));
// インクリメント
var d = c.Increment(10);
// インクリメントを元に戻します
d.Dispose();
// インクリメントとデクリメント
c.Increment(10);
c.Decrement(5);
// 現在の値を出力します。
Debug.WriteLine(c.Count);
```

出力は次のとおりです。

```
Increment
10
Decrement
0
Empty
0
Increment
10
Decrement
5
5
```

## `ScheduledNotifier`

このクラスはスケジューラー上で値を発行します。既定のスケジューラーは `Scheduler.Immediate` です。コンストラクター引数を使ってスケジューラーを設定します。

```csharp
var n = new ScheduledNotifier<string>();
n.Subscribe(x => Debug.WriteLine(x));
// 値をすぐに出力します
n.Report("Hello world");
// 2 秒後に値を出力します。
n.Report("After 2 seconds.", TimeSpan.FromSeconds(2));
```

## `BusyNotifier`

このクラスは `IObservable<bool>` インターフェイスを実装しています。
処理の実行中は `true` を発行し、すべての処理が終了すると `false` を発行します。

`ProcessStart` メソッドは `IDisposable` インスタンスを返します。処理が終了したら、Dispose メソッドを呼び出します。


```csharp
using Reactive.Bindings.Notifiers;
using System;
using System.Threading.Tasks;

namespace ReactivePropertyEduApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var b = new BusyNotifier();
            b.Subscribe(x => Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: OnNext: {x}"));

            await Task.WhenAll(
                Task.Run(async () =>
                {
                    using (b.ProcessStart())
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: Process1 started.");
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: Process1 finished.");
                    }
                }),
                Task.Run(async () =>
                {
                    using (b.ProcessStart())
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: Process2 started.");
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: Process2 finished.");
                    }
                }));
        }
    }
}
```

出力は次のとおりです。

```
15:07:45: OnNext: False
15:07:45: OnNext: True
15:07:45: Process1 started.
15:07:45: Process2 started.
15:07:46: Process1 finished.
15:07:47: Process2 finished.
15:07:47: OnNext: False
```


## `MessageBroker`

`MessageBroker` はインメモリの pub-sub notifier です。`EventAggregator` や `MessageBus` に似ており、Rx と async に適しています。messenger パターンに利用できます。

```csharp
using Reactive.Bindings.Notifiers;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

public class MyClass
{
    public int MyProperty { get; set; }

    public override string ToString()
    {
        return "MP:" + MyProperty;
    }
}
class Program
{
    static void RunMessageBroker()
    {
        // グローバル スコープの pub-sub メッセージング
        MessageBroker.Default.Subscribe<MyClass>(x =>
        {
            Console.WriteLine("A:" + x);
        });

        var d = MessageBroker.Default.Subscribe<MyClass>(x =>
        {
            Console.WriteLine("B:" + x);
        });

        // IObservable<T> への変換をサポート
        MessageBroker.Default.ToObservable<MyClass>().Subscribe(x =>
        {
            Console.WriteLine("C:" + x);
        });

        MessageBroker.Default.Publish(new MyClass { MyProperty = 100 });
        MessageBroker.Default.Publish(new MyClass { MyProperty = 200 });
        MessageBroker.Default.Publish(new MyClass { MyProperty = 300 });

        d.Dispose(); // 購読解除
        MessageBroker.Default.Publish(new MyClass { MyProperty = 400 });
    }

    static async Task RunAsyncMessageBroker()
    {
        // 非同期の message pub-sub
        AsyncMessageBroker.Default.Subscribe<MyClass>(async x =>
        {
            Console.WriteLine("A:" + x);
            await Task.Delay(TimeSpan.FromSeconds(1));
        });

        var d = AsyncMessageBroker.Default.Subscribe<MyClass>(async x =>
        {
            Console.WriteLine("B:" + x);
            await Task.Delay(TimeSpan.FromSeconds(2));
        });

        // すべての subscriber の完了を待ちます
        await AsyncMessageBroker.Default.PublishAsync(new MyClass { MyProperty = 100 });
        await AsyncMessageBroker.Default.PublishAsync(new MyClass { MyProperty = 200 });
        await AsyncMessageBroker.Default.PublishAsync(new MyClass { MyProperty = 300 });

        d.Dispose(); // 購読解除
        await AsyncMessageBroker.Default.PublishAsync(new MyClass { MyProperty = 400 });
    }

    static void Main(string[] args)
    {
        Console.WriteLine("MessageBroker");
        RunMessageBroker();

        Console.WriteLine("AsyncMessageBroker");
        RunAsyncMessageBroker().Wait();
    }
}
```

messenger パターンのマルチスレッド ディスパッチは、Rx で簡単に扱えます。

```csharp
MessageBroker.Default.ToObservable<MyClass>()
    .ObserveOn(Dispatcher) // Rx の魔法!
    .Subscribe(x =>
    {
        Console.WriteLine(x);
    });
```



