# Awaitable

You can use `await` operator on `ReactiveProperty`(includes `ReactivePropertySlim`), `ReadOnlyReactiveProperty`(includes `ReadOnlyReactivePropertySlim`), and `ReactiveCommand`.
When using the `await` operator, the program will wait until the next value is published.

## For example:

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
        // on finish, cancel all await.
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

        // handling event by async/await.
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

If you await multiple times, you should get `ObservableAsyncHandler<T>` from `GetAsyncHandler`. it can await multiple times on zero allocation. If you await single time, you can use `await command.WaitUntilValueChangedAsync(token)`.

> Note: you can await `ReactiveProperty` directly but we recommend use `GetAsyncHandler`(multiple) or `WaitUntilValueChangedAsync`(one shot) with pass over `CancellationToken`.