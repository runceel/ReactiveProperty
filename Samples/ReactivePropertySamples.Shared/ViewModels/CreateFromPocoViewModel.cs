using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Models;

namespace ReactivePropertySamples.ViewModels
{
    public class CreateFromPocoViewModel : ViewModelBase
    {
        public Poco Poco { get; } = new Poco
        {
            FirstName = "Kazuki",
            LastName = "Ota",
        };

        public ReactiveProperty<string> FirstNameTwoWay { get; }

        public ReactiveProperty<string> LastNameTwoWay { get; }

        public ReadOnlyReactivePropertySlim<string> FirstNameOneWay { get; }
        public ReadOnlyReactivePropertySlim<string> LastNameOneWay { get; }

        public ReactiveProperty<string> FirstNameOneWayToSource { get; }
        public ReactiveProperty<string> LastNameOneWayToSource { get; }

        public CreateFromPocoViewModel()
        {
            FirstNameTwoWay = Poco.ToReactivePropertyAsSynchronized(x => x.FirstName)
                .AddTo(Disposables);
            LastNameTwoWay = Poco.ToReactivePropertyAsSynchronized(x => x.LastName)
                .AddTo(Disposables);

            FirstNameOneWay = Poco.ObserveProperty(x => x.FirstName)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
            LastNameOneWay = Poco.ObserveProperty(x => x.LastName)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            FirstNameOneWayToSource = ReactiveProperty.FromObject(Poco, x => x.FirstName);
            LastNameOneWayToSource = ReactiveProperty.FromObject(Poco, x => x.LastName);
        }
    }
}
