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

        public ReactiveCommand<string> OpenFileCommand { get; }
        public ReadOnlyReactivePropertySlim<string> OpenedFile { get; }

        public EventToReactiveViewModel()
        {
            MousePosition = new ReactivePropertySlim<string>();

            OpenFileCommand = new ReactiveCommand<string>();
            OpenedFile = OpenFileCommand.Select(x => $"You selected {x}")
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }
    }
}
