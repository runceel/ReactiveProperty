using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Text;
using Reactive.Bindings.Helpers;
using Reactive.Bindings.Extensions;
using System.Linq;
using Reactive.Bindings;
using System.Reactive.Linq;

namespace Reactive.Todo.Main.Models
{
    public class TodoApp : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ObservableCollection<TodoItem> _todoItems;
        public ReadOnlyObservableCollection<TodoItem> AllTodoItems { get; }
        public IFilteredReadOnlyObservableCollection<TodoItem> ActiveTodoItems { get; }
        public IFilteredReadOnlyObservableCollection<TodoItem> CompletedTodoitems { get; }

        public ReadOnlyReactivePropertySlim<bool> IsCompletedAllItems { get; }

        public TodoApp()
        {
            _todoItems = new ObservableCollection<TodoItem>();
            AllTodoItems = new ReadOnlyObservableCollection<TodoItem>(_todoItems);
            ActiveTodoItems = AllTodoItems.ToFilteredReadOnlyObservableCollection(x => !x.Completed.Value).AddTo(_disposables);
            CompletedTodoitems = AllTodoItems.ToFilteredReadOnlyObservableCollection(x => x.Completed.Value).AddTo(_disposables);

            IsCompletedAllItems = AllTodoItems.ObserveElementProperty(x => x.Completed)
                .ToUnit()
                .Merge(AllTodoItems.CollectionChangedAsObservable().ToUnit())
                .Throttle(TimeSpan.FromMilliseconds(10))
                .Select(_ => AllTodoItems.Count != 0 && AllTodoItems.All(x => x.Completed.Value))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(_disposables);
        }

        public void AddTodoItem(TodoItem newTodoItem) => _todoItems.Add(newTodoItem);

        public void CompleteAll()
        {
            var status = _todoItems.Any(x => !x.Completed.Value);
            foreach (var item in _todoItems)
            {
                item.Completed.Value = status;
            }
        }

        public void ClearCompletedItems()
        {
            var targets = _todoItems.Where(x => x.Completed.Value).ToArray();
            foreach (var target in targets)
            {
                _todoItems.Remove(target);
            }
        }

        public void Dispose() => _disposables.Dispose();
    }
}
