using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using Reactive.Todo.Main.Events;
using Reactive.Todo.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Reactive.Todo.Main.ViewModels
{
    public class TodoItemsViewModel : ViewModelBase
    {
        private readonly TodoApp _todoApp;
        private readonly IMessageBroker _messageBroker;

        public ReadOnlyReactivePropertySlim<IEnumerable<TodoItem>> TodoItems { get; }
        public TodoItemsViewModel(TodoApp todoApp, IMessageBroker messageBroker)
        {
            _todoApp = todoApp;
            _messageBroker = messageBroker;
            TodoItems = _messageBroker.ToObservable<TargetViewChangedEvent>()
                .Select(x => x.TargetViewType)
                .Select(x => (IEnumerable<TodoItem>)(x switch
                {
                    TargetViewType.All => _todoApp.AllTodoItems,
                    TargetViewType.Active => _todoApp.ActiveTodoItems,
                    TargetViewType.Completed => _todoApp.CompletedTodoitems,
                    _ => throw new InvalidOperationException(),
                }))
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim(_todoApp.AllTodoItems)
                .AddTo(Disposables);
        }
    }
}
