using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using Reactive.Bindings;

namespace Reactive.Todo.Main.Models
{
    public class TodoItem : BindableBase
    {
        public ReactivePropertySlim<string> Text { get; }
        public ReactivePropertySlim<bool> Done { get; }

        public TodoItem() : this(null, false)
        {
        }

        public TodoItem(string text, bool done)
        {
            Text = new ReactivePropertySlim<string>(text);
            Done = new ReactivePropertySlim<bool>(done);
            Text.Subscribe(_ => RaisePropertyChanged(nameof(Text)));
            Done.Subscribe(_ => RaisePropertyChanged(nameof(Done)));
        }
    }
}
