using R3;

namespace ReactivePropertySamples.Migrated.ViewModels
{
    public class EventToReactiveViewModel : ViewModelBase
    {
        public BindableReactiveProperty<string> MousePosition { get; }
        public BindableReactiveProperty<bool> CanExecute { get; }
        public ReactiveCommand<string> OpenFileCommand { get; }
        public BindableReactiveProperty<string> OpenedFile { get; }

        public EventToReactiveViewModel()
        {
            // EventToReactiveProperty / EventToReactiveCommand are WPF platform helpers that are
            // out of scope for the bridge, so the View raises these via plain code-behind handlers.
            // The ViewModel just exposes the reactive surface they push into.
            MousePosition = new BindableReactiveProperty<string>("")
                .AddTo(Disposables);
            CanExecute = new BindableReactiveProperty<bool>(true)
                .AddTo(Disposables);
            OpenFileCommand = CanExecute.ToReactiveCommand<string>()
                .AddTo(Disposables);
            OpenedFile = OpenFileCommand.Select(x => $"You selected {x}")
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);
        }
    }
}
