using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace ReactivePropertySamples.ViewModels
{
    public class EventToReactiveViewModel : ViewModelBase
    {
        public ReactivePropertySlim<string> MousePosition { get; }
        public ReactivePropertySlim<bool> CanExecute { get; }
        public ReactiveCommand<string> OpenFileCommand { get; }
        public ReadOnlyReactivePropertySlim<string> OpenedFile { get; }

        public EventToReactiveViewModel()
        {
            MousePosition = new ReactivePropertySlim<string>()
                .AddTo(Disposables);
            CanExecute = new ReactivePropertySlim<bool>(true)
                .AddTo(Disposables);
            OpenFileCommand = CanExecute.ToReactiveCommand<string>()
                .AddTo(Disposables);
            OpenedFile = OpenFileCommand.Select(x => $"You selected {x}")
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }
    }
}
