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
        public ReactivePropertySlim<bool> Completed { get; }

        public TodoItem() : this(null, false)
        {
        }

        public TodoItem(string text, bool completed)
        {
            Text = new ReactivePropertySlim<string>(text);
            Completed = new ReactivePropertySlim<bool>(completed);
            Text.Subscribe(_ => RaisePropertyChanged(nameof(Text)));
            Completed.Subscribe(_ => RaisePropertyChanged(nameof(Completed)));
        }
    }
}
