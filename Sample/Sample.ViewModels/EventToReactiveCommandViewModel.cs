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
        public ReactiveCommand<string> SelectFileCommand { get; private set; }

        public ReactiveProperty<string> Message { get; private set; }

        public EventToReactiveCommandViewModel()
        {
            this.SelectFileCommand = new ReactiveCommand<string>();
            this.Message = this.SelectFileCommand
                .Select(x => x + " selected.")
                .ToReactiveProperty();
        }
    }
}
