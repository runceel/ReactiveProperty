# Awaitable

`ReactiveProperty`（`ReactivePropertySlim` を含む）、`ReadOnlyReactiveProperty`（`ReadOnlyReactivePropertySlim` を含む）、`ReactiveCommand` では `await` 演算子を使用できます。
`await` 演算子を使用すると、次の値が発行されるまでプログラムは待機します。

## 例:

```csharp
// CancellationTokenSource を持つ View
public partial class SampleWindow : Window
{
    CancellationTokenSource cts;
    SampleViewModel viewModel;

    public SampleWindow()
    {
        InitializeComponent();
        cts = new CancellationTokenSource();
        viewModel = new SampleViewModel(cts.Token);
    }

    protected override void OnClosed(EventArgs e)
    {
        // 終了時に、すべての await をキャンセルします。
        cts.Cancel();
        cts.Dispose();

        base.OnClosed(e);
    }
}

// CancellationToken を持つ ViewModel
public class SampleViewModel
{
    public ReactiveCommand MyCommand { get; private set; }
    public ReactiveProperty<int> ClickCount { get; private set; }

    public SampleViewModel(CancellationToken closeToken)
    {
        MyCommand = new ReactiveCommand();
        ClickCount = new ReactiveProperty<int>();

        // async/await でイベントを処理します。
        SubscribeAsync(closeToken);
    }

    async void SubscribeAsync(CancellationToken closeToken)
    {
        using (var handler = MyCommand.GetAsyncHandler(closeToken))
        {
            while (true)
            {
                await handler; // クリックされるまで await します。
                ClickCount.Value += 1;
            }
        }
    }
}
```

複数回 await する場合は、`GetAsyncHandler` から `ObservableAsyncHandler<T>` を取得してください。これは割り当てなしで複数回 await できます。1 回だけ await する場合は、`await command.WaitUntilValueChangedAsync(token)` を使用できます。

> 注: `ReactiveProperty` を直接 await することもできますが、`GetAsyncHandler`（複数回の await）または `WaitUntilValueChangedAsync`（1 回限り）を使用し、`CancellationToken` を渡すことを推奨します。
