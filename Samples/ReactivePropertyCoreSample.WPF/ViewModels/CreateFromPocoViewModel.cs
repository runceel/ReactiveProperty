using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertyCoreSample.WPF.Models;

namespace ReactivePropertyCoreSample.WPF.ViewModels;
public class CreateFromPocoViewModel : ViewModelBase
{
    public Poco Poco { get; } = new()
    {
        FirstName = "Kazuki",
        LastName = "Ota",
    };

    public ReactivePropertySlim<string> FirstNameTwoWay { get; }
    public ReactivePropertySlim<string> LastNameTwoWay { get; }
    public ReadOnlyReactivePropertySlim<string> FirstNameOneWay { get; }
    public ReadOnlyReactivePropertySlim<string> LastNameOneWay { get; }

    public CreateFromPocoViewModel()
    {
        FirstNameTwoWay = Poco.ToReactivePropertySlimAsSynchronized(x => x.FirstName)
            .AddTo(Disposables);
        LastNameTwoWay = Poco.ToReactivePropertySlimAsSynchronized(x => x.LastName)
            .AddTo(Disposables);

        FirstNameOneWay = Poco.ObserveProperty(x => x.FirstName)
            .ToReadOnlyReactivePropertySlim("")
            .AddTo(Disposables);
        LastNameOneWay = Poco.ObserveProperty(x => x.LastName)
            .ToReadOnlyReactivePropertySlim("")
            .AddTo(Disposables);
    }

}
