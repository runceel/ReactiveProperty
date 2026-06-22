using System;
using System.Threading.Tasks;
using R3;
using Reactive.Bindings.R3;

namespace ReactivePropertySamples.Migrated.ViewModels
{
    public class ReactiveCommandViewModel : ViewModelBase
    {
        public ReactiveCommand CreateAndSubscribeCommand { get; }
        public BindableReactiveProperty<string> InvokedDateTime { get; }
        public BindableReactiveProperty<string> InvokedDateTimeFromCommand { get; }

        public BindableReactiveProperty<bool> IsChecked { get; }
        public ReactiveCommand CreateFromIObservableBoolCommand { get; }
        public BindableReactiveProperty<string> InvokedDateTimeForCreateFromIObservableBoolCommand { get; }

        public ReactiveCommand<string> ReactiveCommandWithParameter { get; }
        public BindableReactiveProperty<string> ReactiveCommandWithParameterOutput { get; }

        public AsyncReactiveCommand LongRunningCommand { get; }
        public BindableReactiveProperty<string> LongRunningCommandOutput { get; }

        public BindableReactiveProperty<bool> IsCheckedForAsyncReactiveCommand { get; }
        public AsyncReactiveCommand CreateFromIObservableBoolAsyncReactiveCommand { get; }
        public BindableReactiveProperty<string> CreateFromIObservableBoolAsyncReactiveCommandOutput { get; }

        public BindableReactiveProperty<bool> IsCheckedForSharedStatus { get; }
        public AsyncReactiveCommand ACommand { get; }
        public AsyncReactiveCommand BCommand { get; }
        public BindableReactiveProperty<string> SharedStatusOutput { get; }

        public ReactiveCommandViewModel()
        {
            InvokedDateTime = new BindableReactiveProperty<string>("");
            // R3 ReactiveCommand is ReactiveCommand<Unit>; subscribe directly to keep the
            // non-generic command type (the bridge WithSubscribe is generic only).
            CreateAndSubscribeCommand = new ReactiveCommand().AddTo(Disposables);
            CreateAndSubscribeCommand
                .Subscribe(_ => InvokedDateTime.Value = $"The command was invoked at {DateTime.Now}.")
                .AddTo(Disposables);

            InvokedDateTimeFromCommand = CreateAndSubscribeCommand
                .Select(_ => $"The command was invoked at {DateTime.Now}.")
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);

            IsChecked = new BindableReactiveProperty<bool>()
                .AddTo(Disposables);
            CreateFromIObservableBoolCommand = IsChecked.ToReactiveCommand()
                .AddTo(Disposables);
            InvokedDateTimeForCreateFromIObservableBoolCommand = CreateFromIObservableBoolCommand
                .Select(_ => $"The command was invoked at {DateTime.Now}.")
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);

            ReactiveCommandWithParameter = new ReactiveCommand<string>()
                .AddTo(Disposables);
            ReactiveCommandWithParameterOutput = ReactiveCommandWithParameter
                .Select(x => $"The command was invoked with {x}.")
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);

            LongRunningCommandOutput = new BindableReactiveProperty<string>("");
            LongRunningCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    LongRunningCommandOutput.Value = "Long running command was started.";
                    await Task.Delay(5000);
                    LongRunningCommandOutput.Value = "Long running command was finished.";
                })
                .AddTo(Disposables);

            IsCheckedForAsyncReactiveCommand = new BindableReactiveProperty<bool>()
                .AddTo(Disposables);
            CreateFromIObservableBoolAsyncReactiveCommandOutput = new BindableReactiveProperty<string>("")
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

            // The shared can-execute gate is an R3 ReactiveProperty<bool> (not bound to XAML).
            var sharedStatus = new ReactiveProperty<bool>(true)
                .AddTo(Disposables);
            IsCheckedForSharedStatus = new BindableReactiveProperty<bool>()
                .AddTo(Disposables);
            SharedStatusOutput = new BindableReactiveProperty<string>("")
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
