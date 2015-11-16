using Prism.Mvvm;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace UWP.TodoMVVM.Models
{
    class TodoManager : BindableBase
    {
        private ObservableCollection<TodoItem> AllTodoItems { get; } = new ObservableCollection<TodoItem>();

        private IFilteredReadOnlyObservableCollection<TodoItem> currentView;

        public IFilteredReadOnlyObservableCollection<TodoItem> CurrentView
        {
            get { return this.currentView; }
            set
            {
                this.currentView?.Dispose();
                this.SetProperty(ref this.currentView, value);
            }
        }

        public IObservable<IEnumerable<TodoItem>> ChangedAsObservable =>
            this.AllTodoItems.CollectionChangedAsObservable().ToUnit()
                .Merge(this.AllTodoItems.ObserveElementPropertyChanged().ToUnit())
                .Select(_ => this.AllTodoItems.ToArray());

        private ViewType viewType;

        public ViewType ViewType
        {
            get { return this.viewType; }
            set
            {
                if (this.SetProperty(ref this.viewType, value))
                {
                    this.ChangeCurrentView();
                }
            }
        }

        public TodoManager()
        {
            this.ChangeCurrentView();
        }

        public void SetTodoItems(IEnumerable<TodoItem> todoItems)
        {
            foreach (var item in todoItems)
            {
                this.AllTodoItems.Add(item);
            }
        }

        public void AddTodoItem(TodoItem todoItem)
        {
            this.AllTodoItems.Add(todoItem);
        }

        public void ChangeStateAll(bool completed)
        {
            foreach (var item in this.AllTodoItems)
            {
                item.Completed = completed;
            }
        }

        public void ClearCompletedTodoItem()
        {
            var targets = this.AllTodoItems.Where(x => x.Completed).ToArray();
            foreach (var item in targets)
            {
                this.AllTodoItems.Remove(item);
            }
        }

        private void ChangeCurrentView()
        {
            switch(this.ViewType)
            {
                case ViewType.All:
                    this.CurrentView = this.AllTodoItems.ToFilteredReadOnlyObservableCollection(_ => true);
                    break;
                case ViewType.Active:
                    this.CurrentView = this.AllTodoItems.ToFilteredReadOnlyObservableCollection(x => !x.Completed);
                    break;
                case ViewType.Completed:
                    this.CurrentView = this.AllTodoItems.ToFilteredReadOnlyObservableCollection(x => x.Completed);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
