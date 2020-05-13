using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace ReactivePropertySamples.ViewModels
{
    public class BasicUsagesViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Input { get; }
        public ReadOnlyReactiveProperty<string> Output { get; }

        public ReactivePropertySlim<string> InputSlim { get; }

        public ReadOnlyReactivePropertySlim<string> OutputSlim { get; }

        public BasicUsagesViewModel()
        {
            Input = new ReactiveProperty<string>();
            Output = Input.Delay(TimeSpan.FromSeconds(2))
                .Select(x => x?.ToUpper())
                .ToReadOnlyReactiveProperty()
                .AddTo(Disposables);

            InputSlim = new ReactivePropertySlim<string>();
            OutputSlim = InputSlim.Delay(TimeSpan.FromSeconds(2))
                .Select(x => x?.ToUpper())
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }
    }
}
