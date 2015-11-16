using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Linq;
using UWP.TodoMVVM.Models;
using UWP.TodoMVVM.ViewModels;

namespace UWP.TodoMVVM.Services
{
    class TodoService
    {
        private TodoManager TodoManager { get; }

        private TodoItemRepository TodoItemRepository { get; }

        public TodoService(TodoManager todoManager, TodoItemRepository todoItemRepository)
        {
            this.TodoManager = todoManager;
            this.TodoItemRepository = todoItemRepository;

            this.TodoManager.ChangedAsObservable
                .Throttle(TimeSpan.FromMilliseconds(10))
                .ObserveOnUIDispatcher()
                .Subscribe(x =>
                {
                    this.TodoItemRepository.Save(x);
                });
        }

        public void Load()
        {
            var todoItems = this.TodoItemRepository.Load();
            this.TodoManager.SetTodoItems(todoItems);
        }

        public void AddTodoItem(TodoItemViewModel todoItemViewModel)
        {
            if (todoItemViewModel.HasErrors.Value)
            {
                return;
            }

            this.TodoManager.AddTodoItem(todoItemViewModel.Model);
        }

        public void ClearCompletedTodoItem()
        {
            this.TodoManager.ClearCompletedTodoItem();
        }

        public void SetAllCurrentView()
        {
            this.TodoManager.ViewType = ViewType.All;
        }

        public void SetActiveCurrentView()
        {
            this.TodoManager.ViewType = ViewType.Active;
        }

        public void SetCompletedCurrentView()
        {
            this.TodoManager.ViewType = ViewType.Completed;
        }

        internal void ChangeStateAll(bool completed)
        {
            this.TodoManager.ChangeStateAll(completed);
        }
    }
}
