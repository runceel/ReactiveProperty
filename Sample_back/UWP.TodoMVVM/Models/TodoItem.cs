using Prism.Mvvm;

namespace UWP.TodoMVVM.Models
{
    class TodoItem : BindableBase
    {
        private string title;

        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        private bool completed;

        public bool Completed
        {
            get { return this.completed; }
            set { this.SetProperty(ref this.completed, value); }
        }

    }
}
