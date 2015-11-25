using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System;
using System.Linq;
using System.Reactive.Linq;
using UWP.TodoMVVM.Models;
using UWP.TodoMVVM.Services;

namespace UWP.TodoMVVM.ViewModels
{
    class MainPageViewModel
    {
        private TodoManager TodoManager { get; }

        private TodoService TodoService { get; }

        public ReactiveProperty<TodoItemViewModel> InputTodoItem { get; } = new ReactiveProperty<TodoItemViewModel>(new TodoItemViewModel());

        public ReactiveCommand AddTodoItemCommand { get; }

        public ReactiveProperty<ReadOnlyReactiveCollection<TodoItemViewModel>> TodoItems { get; }

        public ReactiveCommand ShowAllCommand { get; }
        public ReactiveCommand ShowActiveCommand { get; }
        public ReactiveCommand ShowCompletedCommand { get; }

        public ReactiveCommand ClearCompletedTodoItemCommand { get; }

        public MainPageViewModel(TodoManager todoManager, TodoService todoService)
        {
            this.TodoManager = todoManager;
            this.TodoService = todoService;

            this.AddTodoItemCommand = this.InputTodoItem
                .Select(x => x.HasErrors)
                .Switch()
                .Select(x => !x)
                .ToReactiveCommand();
            this.AddTodoItemCommand
                .Select(_ => this.InputTodoItem.Value)
                .Subscribe(x =>
                {
                    this.TodoService.AddTodoItem(x);
                    this.InputTodoItem.Value = new TodoItemViewModel();
                });

            this.TodoItems = this.TodoManager
                .ObserveProperty(x => x.CurrentView)
                .Do(_ => this.TodoItems?.Value?.Dispose())
                .Select(x => x.ToReadOnlyReactiveCollection(y => new TodoItemViewModel(y)))
                .ToReactiveProperty();

            this.ShowAllCommand = this.TodoManager.ObserveProperty(x => x.ViewType)
                .Select(x => x != ViewType.All)
                .ToReactiveCommand();
            this.ShowAllCommand.Subscribe(_ => this.TodoService.SetAllCurrentView());

            this.ShowActiveCommand = this.TodoManager.ObserveProperty(x => x.ViewType)
                .Select(x => x != ViewType.Active)
                .ToReactiveCommand();
            this.ShowActiveCommand.Subscribe(_ => this.TodoService.SetActiveCurrentView());

            this.ShowCompletedCommand = this.TodoManager.ObserveProperty(x => x.ViewType)
                .Select(x => x != ViewType.Completed)
                .ToReactiveCommand();
            this.ShowCompletedCommand.Subscribe(_ => this.TodoService.SetCompletedCurrentView());

            this.ClearCompletedTodoItemCommand = this.TodoManager
                .ChangedAsObservable
                .Select(x => x.Any(y => y.Completed))
                .ToReactiveCommand();
            this.ClearCompletedTodoItemCommand.Subscribe(_ => this.TodoService.ClearCompletedTodoItem());
        }

        public void ChangeStateAll(bool completed)
        {
            this.TodoService.ChangeStateAll(completed);
        }
    }
}
