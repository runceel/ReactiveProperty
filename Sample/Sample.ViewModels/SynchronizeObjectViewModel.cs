using System;
using System.Reactive.Linq;
using System.Windows;
using Reactive.Bindings;
using System.ComponentModel;
using Reactive.Bindings.Extensions; // using namespace

namespace Sample.ViewModels
{
    // Synchroinize exsiting models.
    public class SynchronizeObjectViewModel
    {
        public ReactiveProperty<string> TwoWay { get; }
        public ReactiveProperty<string> OneWay { get; }
        public ReactiveProperty<string> OneWayToSource { get; }
        public ReactiveCommand CheckCommand { get; }
        public ReactiveProperty<string> AlertMessage { get; }

        public SynchronizeObjectViewModel()
        {
            var inpc = new ObservableObject { Name = "Bill" };
            var poco = new PlainObject { Name = "Steve" };

            // TwoWay synchronize
            TwoWay = inpc.ToReactivePropertyAsSynchronized(x => x.Name);

            // OneWay synchronize
            OneWay = inpc.ObserveProperty(x => x.Name).ToReactiveProperty();

            // OneWayToSource synchronize
            OneWayToSource = ReactiveProperty.FromObject(poco, x => x.Name);

            // synchronization check
            CheckCommand = new ReactiveCommand();
            this.AlertMessage = CheckCommand.Select(_ => 
                "INPC Name:" + inpc.Name + Environment.NewLine
              + "POCO Name:" + poco.Name)
              .ToReactiveProperty(mode: ReactivePropertyMode.None);
        }
    }

    public class ObservableObject : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = (_, __) => { };
    }

    public class PlainObject
    {
        public string Name { get; set; }
    }
}