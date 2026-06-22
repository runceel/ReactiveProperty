using R3;
using Reactive.Bindings.R3.Extensions;
using ReactivePropertySamples.Migrated.Models;

namespace ReactivePropertySamples.Migrated.ViewModels
{
    public class CreateFromPocoViewModel : ViewModelBase
    {
        public Poco Poco { get; } = new Poco
        {
            FirstName = "Kazuki",
            LastName = "Ota",
        };

        public BindableReactiveProperty<string> FirstNameTwoWay { get; }
        public BindableReactiveProperty<string> LastNameTwoWay { get; }

        public BindableReactiveProperty<string> FirstNameOneWay { get; }
        public BindableReactiveProperty<string> LastNameOneWay { get; }

        public BindableReactiveProperty<string> FirstNameOneWayToSource { get; }
        public BindableReactiveProperty<string> LastNameOneWayToSource { get; }

        public CreateFromPocoViewModel()
        {
            FirstNameTwoWay = Poco.ToReactivePropertyAsSynchronized(x => x.FirstName)
                .AddTo(Disposables);
            LastNameTwoWay = Poco.ToReactivePropertyAsSynchronized(x => x.LastName)
                .AddTo(Disposables);

            FirstNameOneWay = Poco.ObservePropertyChanged(x => x.FirstName)
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);
            LastNameOneWay = Poco.ObservePropertyChanged(x => x.LastName)
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);

            // R3 has no ReactiveProperty.FromObject; a one-way-to-source binding is written
            // naively as a property that pushes its value back to the POCO on change.
            FirstNameOneWayToSource = new BindableReactiveProperty<string>(Poco.FirstName)
                .AddTo(Disposables);
            FirstNameOneWayToSource.Subscribe(x => Poco.FirstName = x)
                .AddTo(Disposables);
            LastNameOneWayToSource = new BindableReactiveProperty<string>(Poco.LastName)
                .AddTo(Disposables);
            LastNameOneWayToSource.Subscribe(x => Poco.LastName = x)
                .AddTo(Disposables);
        }
    }
}
