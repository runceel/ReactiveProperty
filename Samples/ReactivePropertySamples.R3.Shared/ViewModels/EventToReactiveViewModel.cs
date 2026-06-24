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
            // MousePosition / OpenedFile are updated through the EventToReactiveProperty and
            // EventToReactiveCommand trigger actions from ReactiveProperty.R3.WPF (see the View).
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
