# Awaitable

You can use the `await` operator on `ReactiveProperty` (including `ReactivePropertySlim`), `ReadOnlyReactiveProperty` (including `ReadOnlyReactivePropertySlim`), and `ReactiveCommand`.
When using the `await` operator, the program will wait until the next value is published.

## Example:

```csharp
// View with CancellationTokenSource
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
        // On finish, cancel all awaits.
        cts.Cancel();
        cts.Dispose();

        base.OnClosed(e);
    }
}

// ViewModel with CancellationToken
public class SampleViewModel
{
    public ReactiveCommand MyCommand { get; private set; }
    public ReactiveProperty<int> ClickCount { get; private set; }

    public SampleViewModel(CancellationToken closeToken)
    {
        MyCommand = new ReactiveCommand();
        ClickCount = new ReactiveProperty<int>();

        // Handle events with async/await.
        SubscribeAsync(closeToken);
    }

    async void SubscribeAsync(CancellationToken closeToken)
    {
        using (var handler = MyCommand.GetAsyncHandler(closeToken))
        {
            while (true)
            {
                await handler; // await when clicked.
                ClickCount.Value += 1;
            }
        }
    }
}
```

If you await multiple times, get `ObservableAsyncHandler<T>` from `GetAsyncHandler`. It can await multiple times with zero allocation. If you await only once, you can use `await command.WaitUntilValueChangedAsync(token)`.

> Note: you can await `ReactiveProperty` directly, but we recommend using `GetAsyncHandler` (multiple awaits) or `WaitUntilValueChangedAsync` (one shot) and passing a `CancellationToken`.
