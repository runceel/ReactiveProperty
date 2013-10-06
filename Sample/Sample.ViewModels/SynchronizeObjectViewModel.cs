using System;
using System.Reactive.Linq;
using System.Windows;
using Codeplex.Reactive;
using System.ComponentModel;
using Codeplex.Reactive.Extensions; // using namespace

namespace Sample.ViewModels
{
    // Synchroinize exsiting models.
    public class SynchronizeObjectViewModel
    {
        public ReactiveProperty<string> TwoWay { get; private set; }
        public ReactiveProperty<string> OneWay { get; private set; }
        public ReactiveProperty<string> OneWayToSource { get; private set; }
        public ReactiveCommand CheckCommand { get; private set; }
        public ReactiveProperty<string> AlertMessage { get; private set; }

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