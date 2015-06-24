using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Diagnostics;
using Reactive.Bindings.Extensions;

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
