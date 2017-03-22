using Reactive.Bindings;
using Reactive.Bindings.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
    public class FilteredReadOnlyObservableCollectionBasicsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Person> PeopleSource { get; } = new ObservableCollection<Person>();

        private bool UseRemovedFilter { get; set; }
        private Func<Person, bool> RemovedFilter { get; } = x => x.IsRemoved;
        private Func<Person, bool> NotRemovedFilter { get; } = x => !x.IsRemoved;

        public IFilteredReadOnlyObservableCollection<Person> People { get; }

        public ReactiveCommand AddCommand { get; }
        public ReactiveCommand RefreshCommand { get; }

        public FilteredReadOnlyObservableCollectionBasicsViewModel()
        {
            this.UseRemovedFilter = false;
            this.People = this.PeopleSource.ToFilteredReadOnlyObservableCollection(this.NotRemovedFilter);

            this.AddCommand = new ReactiveCommand();
            this.AddCommand.Subscribe(_ => this.PeopleSource.Add(new Person()));

            this.RefreshCommand = new ReactiveCommand();
            this.RefreshCommand.Subscribe(Refresh);
        }

        private void Refresh()
        {
            this.People.Refresh(this.UseRemovedFilter ? this.NotRemovedFilter : this.RemovedFilter);
            this.UseRemovedFilter = !this.UseRemovedFilter;
        }
    }

    public class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }

            field = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        private string name = $"tanaka-{DateTime.Now}";

        public string Name
        {
            get { return this.name; }
            set { this.SetProperty(ref this.name, value); }
        }

        private bool isRemoved;

        public bool IsRemoved
        {
            get { return this.isRemoved; }
            set { this.SetProperty(ref this.isRemoved, value); }
        }

    }
}
