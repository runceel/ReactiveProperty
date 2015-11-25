using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWP.TodoMVVM.Models
{
    class TodoItemRepository
    {
        public IEnumerable<TodoItem> Load()
        {
            var json = ApplicationData.Current.LocalSettings.Values[nameof(TodoItemRepository)] as string;
            if (string.IsNullOrWhiteSpace(json))
            {
                return Enumerable.Empty<TodoItem>();
            }

            return JsonConvert.DeserializeObject<IEnumerable<TodoItem>>(json);
        }

        public void Save(IEnumerable<TodoItem> todoItems)
        {
            var json = JsonConvert.SerializeObject(todoItems);
            ApplicationData.Current.LocalSettings.Values[nameof(TodoItemRepository)] = json;
        }
    }
}
