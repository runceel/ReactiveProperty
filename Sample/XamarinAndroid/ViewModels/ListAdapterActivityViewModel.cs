using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace XamarinAndroid.ViewModels
{
    public class ListAdapterActivityViewModel
    {
        private ObservableCollection<PersonViewModel> source = new ObservableCollection<PersonViewModel>();

        public ReadOnlyReactiveCollection<PersonViewModel> People { get; }

        public ReactiveCommand AddPersonCommand { get; }

        public ListAdapterActivityViewModel(Activity context)
        {
            this.People = source.ToReadOnlyReactiveCollection();

            this.AddPersonCommand = new ReactiveCommand();
            this.AddPersonCommand.Subscribe(_ => source.Add(new PersonViewModel(
                this.source.Count,
                "Person " + this.source.Count,
                30 + this.source.Count % 10)));
        }
    }

    public class PersonViewModel
    {
        public ReactiveProperty<long> Id { get; }
        public ReactiveProperty<string> Name { get; }
        public ReactiveProperty<string> Age { get; }

        private ReactiveTimer timer;

        public PersonViewModel(long id, string name, int age)
        {
            this.Id = new ReactiveProperty<long>(id);
            this.Name = new ReactiveProperty<string>(name);
            this.Age = new ReactiveProperty<string>(age.ToString());

            this.timer = new ReactiveTimer(TimeSpan.FromSeconds(10));
            this.timer
                .ObserveOnUIDispatcher()
                .Subscribe(_ =>
                {
                    this.Age.Value = (int.Parse(this.Age.Value) + 1).ToString();
                });
            this.timer.Start();
        }
    }
}