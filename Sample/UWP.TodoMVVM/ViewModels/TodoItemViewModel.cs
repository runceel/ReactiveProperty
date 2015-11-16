using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.TodoMVVM.Models;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace UWP.TodoMVVM.ViewModels
{
    class TodoItemViewModel
    {
        public TodoItem Model { get; }

        [Required]
        public ReactiveProperty<string> Title { get; }

        public ReactiveProperty<bool> Completed { get; }

        public ReadOnlyReactiveProperty<Brush> ForegroundBrush { get; }

        public ReadOnlyReactiveProperty<bool> HasErrors { get; }

        public TodoItemViewModel() : this(new TodoItem())
        {
        }

        public TodoItemViewModel(TodoItem todoItem)
        {
            this.Model = todoItem;
            this.Title = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Title)
                .SetValidateAttribute(() => this.Title);

            this.Completed = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Completed);

            this.ForegroundBrush = this.Model
                .ObserveProperty(x => x.Completed)
                .Select(x => x ? new SolidColorBrush(Colors.DarkGray) : new SolidColorBrush(Colors.Black))
                .Select(x => (Brush)x)
                .ToReadOnlyReactiveProperty();

            this.HasErrors = Observable.CombineLatest(
                this.Title.ObserveHasErrors,
                this.Completed.ObserveHasErrors)
                .Select(x => x.Any(y => y))
                .ToReadOnlyReactiveProperty();
        }
    }
}
