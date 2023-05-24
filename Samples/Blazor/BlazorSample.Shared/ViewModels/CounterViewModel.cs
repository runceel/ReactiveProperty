using System.Reactive.Disposables;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;

namespace BlazorSample.Shared.ViewModels;
public class CounterViewModel : IDisposable
{
    private CompositeDisposable _disposables = new();
    public ReactivePropertySlim<int> Counter { get; }

    public ReactiveCommandSlim IncrementCommand { get; }

    public CounterViewModel()
    {
        Counter = new ReactivePropertySlim<int>(0)
            .AddTo(_disposables);

        IncrementCommand = Counter.Select(x => x < 10)
            .ToReactiveCommandSlim()
            .WithSubscribe(Increment, _disposables.Add)
            .AddTo(_disposables);
    }

    private void Increment()
    {
        Counter.Value++;
    }

    public void Dispose() => _disposables.Dispose();
}
