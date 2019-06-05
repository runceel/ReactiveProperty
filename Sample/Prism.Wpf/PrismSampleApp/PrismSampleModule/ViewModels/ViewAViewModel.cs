using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace PrismSampleModule.ViewModels
{
    public class ViewAViewModel : BindableBase
    {
        public ReactiveProperty<string> Input { get; }
        public ReadOnlyReactiveProperty<string> Output { get; }

        public ReactiveCommand ResetCommand { get; }

        public ViewAViewModel()
        {
            Input = new ReactiveProperty<string>("");
            Output = Input.Delay(TimeSpan.FromSeconds(1))
                .Select(x => x.ToUpper())
                .ToReadOnlyReactiveProperty();

            ResetCommand = Input.Select(x => !string.IsNullOrWhiteSpace(x))
                .ToReactiveCommand()
                .WithSubscribe(() => Input.Value = "");
        }
    }
}
