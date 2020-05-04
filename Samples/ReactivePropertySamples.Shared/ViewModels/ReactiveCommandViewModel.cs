using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace ReactivePropertySamples.ViewModels
{
    public class ReactiveCommandViewModel : ViewModelBase
    {
        public ReactiveCommand CreateAndSubscribeCommand { get; }
        public ReactivePropertySlim<string> InvokedDateTime { get; }
        public ReadOnlyReactivePropertySlim<string> InvokedDateTimeFromCommand { get; }

        public ReactivePropertySlim<bool> IsChecked { get; }
        public ReactiveCommand CreateFromIObservableBoolCommand { get; }
        public ReadOnlyReactivePropertySlim<string> InvokedDateTimeForCreateFromIObservableBoolCommand { get; }

        public ReactiveCommand<string> ReactiveCommandWithParameter { get; }
        public ReadOnlyReactivePropertySlim<string> ReactiveCommandWithParameterOutput { get; }

        public AsyncReactiveCommand LongRunningCommand { get; }
        public ReactivePropertySlim<string> LongRunningCommandOutput { get; }

        public ReactivePropertySlim<bool> IsCheckedForAsyncReactiveCommand { get; }
        public AsyncReactiveCommand CreateFromIObservableBoolAsyncReactiveCommand { get; }
        public ReactivePropertySlim<string> CreateFromIObservableBoolAsyncReactiveCommandOutput { get; }


        public ReactivePropertySlim<bool> IsCheckedForSharedStatus { get; }
        public AsyncReactiveCommand ACommand { get; }
        public AsyncReactiveCommand BCommand { get; }
        public ReactivePropertySlim<string> SharedStatusOutput { get; }

        public ReactiveCommandViewModel()
        {
            InvokedDateTime = new ReactivePropertySlim<string>();
            CreateAndSubscribeCommand = new ReactiveCommand()
                .WithSubscribe(() => InvokedDateTime.Value = $"The command was invoked at {DateTime.Now}.", Disposables.Add);

            InvokedDateTimeFromCommand = CreateAndSubscribeCommand
                .Select(_ => $"The command was invoked at {DateTime.Now}.")
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            IsChecked = new ReactivePropertySlim<bool>()
                .AddTo(Disposables);
            CreateFromIObservableBoolCommand = IsChecked.ToReactiveCommand()
                .AddTo(Disposables);
            InvokedDateTimeForCreateFromIObservableBoolCommand = CreateFromIObservableBoolCommand
                .Select(_ => $"The command was invoked at {DateTime.Now}.")
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            ReactiveCommandWithParameter = new ReactiveCommand<string>()
                .AddTo(Disposables);
            ReactiveCommandWithParameterOutput = ReactiveCommandWithParameter
                .Select(x => $"The command was invoked with {x}.")
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            LongRunningCommandOutput = new ReactivePropertySlim<string>();
            LongRunningCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    LongRunningCommandOutput.Value = "Long running command was started.";
                    await Task.Delay(5000);
                    LongRunningCommandOutput.Value = "Long running command was finished.";
                })
                .AddTo(Disposables);

            IsCheckedForAsyncReactiveCommand = new ReactivePropertySlim<bool>()
                .AddTo(Disposables);
            CreateFromIObservableBoolAsyncReactiveCommandOutput = new ReactivePropertySlim<string>()
                .AddTo(Disposables);
            CreateFromIObservableBoolAsyncReactiveCommand = IsCheckedForAsyncReactiveCommand
                .ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    CreateFromIObservableBoolAsyncReactiveCommandOutput.Value = "Long running command was started.";
                    await Task.Delay(5000);
                    CreateFromIObservableBoolAsyncReactiveCommandOutput.Value = "Long running command was finished.";
                })
                .AddTo(Disposables);

            var sharedStatus = new ReactivePropertySlim<bool>(true);
            IsCheckedForSharedStatus = new ReactivePropertySlim<bool>()
                .AddTo(Disposables);
            SharedStatusOutput = new ReactivePropertySlim<string>()
                .AddTo(Disposables);
            ACommand = IsCheckedForSharedStatus
                .ToAsyncReactiveCommand(sharedStatus)
                .WithSubscribe(async () =>
                {
                    SharedStatusOutput.Value = "A command was started.";
                    await Task.Delay(5000);
                    SharedStatusOutput.Value = "A command was finished.";
                })
                .AddTo(Disposables);
            BCommand = IsCheckedForSharedStatus
                .ToAsyncReactiveCommand(sharedStatus)
                .WithSubscribe(async () =>
                {
                    SharedStatusOutput.Value = "B command was started.";
                    await Task.Delay(5000);
                    SharedStatusOutput.Value = "B command was finished.";
                })
                .AddTo(Disposables);
        }
    }
}
