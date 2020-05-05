using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Todo.Main.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;

namespace Reactive.Todo.Main.ViewModels
{
    public class InputViewModel : ViewModelBase
    {
        private readonly TodoApp _todoApp;

        public ReactiveProperty<string> Input { get; }

        public ReactiveCommand AddCommand { get; }
        public ReactiveCommand CompleteAllCommand { get; }
        public ReadOnlyReactivePropertySlim<bool> IsCompletedAllItems => _todoApp.IsCompletedAllItems;

        public InputViewModel(TodoApp todoApp)
        {
            _todoApp = todoApp;

            Input = new ReactiveProperty<string>();
            AddCommand = new ReactiveCommand()
                .WithSubscribe(AddTodoItem)
                .AddTo(Disposables);

            CompleteAllCommand = new ReactiveCommand()
                .WithSubscribe(() => _todoApp.CompleteAll())
                .AddTo(Disposables);
        }

        private void AddTodoItem()
        {
            if (string.IsNullOrWhiteSpace(Input.Value))
            {
                return;
            }

            _todoApp.AddTodoItem(new TodoItem(Input.Value, false));
            Input.Value = "";
        }
    }
}
