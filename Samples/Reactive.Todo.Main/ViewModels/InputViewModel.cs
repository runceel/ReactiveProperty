﻿using Prism.Commands;
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

        [Required]
        public ReactiveProperty<string> Input { get; }

        public ReactiveCommand AddCommand { get; }
        public ReactiveCommand CheckAllCommand { get; }
        public ReadOnlyReactivePropertySlim<bool> IsCompletedAllItems => _todoApp.IsCompletedAllItems;

        public InputViewModel(TodoApp todoApp)
        {
            _todoApp = todoApp;

            Input = new ReactiveProperty<string>();
            AddCommand = Input.ObserveHasErrors
                .Inverse()
                .ToReactiveCommand()
                .WithSubscribe(AddTodoItem, Disposables.Add)
                .AddTo(Disposables);

            CheckAllCommand = new ReactiveCommand()
                .WithSubscribe(() => _todoApp.CheckAll(), Disposables.Add)
                .AddTo(Disposables);
        }

        private void AddTodoItem()
        {
            _todoApp.AddTodoItem(new TodoItem(Input.Value, false));
            Input.Value = "";
        }
    }
}