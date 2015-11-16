using Prism.Mvvm;

namespace UWP.TodoMVVM.Models
{
    class TodoItem : BindableBase
    {
        private string titl;

        public string Title
        {
            get { return this.titl; }
            set { this.SetProperty(ref this.titl, value); }
        }

        private bool completed;

        public bool Completed
        {
            get { return this.completed; }
            set { this.SetProperty(ref this.completed, value); }
        }

    }
}
