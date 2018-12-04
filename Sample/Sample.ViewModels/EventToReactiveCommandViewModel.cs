using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;

namespace Sample.ViewModels
{
    public class EventToReactiveCommandViewModel
    {
        public ReactiveCommand<string> SelectFileCommand { get; }

        public ReactiveProperty<string> Message { get; }

        public EventToReactiveCommandViewModel()
        {
            // command called, after converter
            this.SelectFileCommand = new ReactiveCommand<string>();
            // create ReactiveProperty from ReactiveCommand
            this.Message = this.SelectFileCommand
                .Select(x => x + " selected.")
                .ToReactiveProperty();
        }
    }
}
