using System;
using R3;

namespace ReactivePropertySamples.Migrated.ViewModels
{
    public class BasicUsagesViewModel : ViewModelBase
    {
        // Bound to XAML with .Value, so the binding-friendly BindableReactiveProperty<T> is used.
        public BindableReactiveProperty<string> Input { get; }
        public BindableReactiveProperty<string> Output { get; }

        public BindableReactiveProperty<string> InputSlim { get; }
        public BindableReactiveProperty<string> OutputSlim { get; }

        public BasicUsagesViewModel()
        {
            Input = new BindableReactiveProperty<string>("");
            Output = Input.Delay(TimeSpan.FromSeconds(2))
                .Select(x => x?.ToUpper())
                // R3 has no UI scheduler abstraction; marshal back to the UI thread explicitly.
                .ObserveOnCurrentSynchronizationContext()
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);

            InputSlim = new BindableReactiveProperty<string>("");
            OutputSlim = InputSlim.Delay(TimeSpan.FromSeconds(2))
                .Select(x => x?.ToUpper())
                .ObserveOnCurrentSynchronizationContext()
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);
        }
    }
}
