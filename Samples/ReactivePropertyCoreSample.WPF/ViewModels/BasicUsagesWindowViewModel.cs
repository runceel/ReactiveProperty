using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;
using ReactivePropertyCoreSample.WPF.Models;

namespace ReactivePropertyCoreSample.WPF.ViewModels;
public class BasicUsagesViewModel : ViewModelBase
{
    public ReactivePropertySlim<string> Input { get; }
    public ReadOnlyReactivePropertySlim<string> Output { get; }

    public BasicUsagesViewModel()
    {
        Input = new ReactivePropertySlim<string>("")
            .AddTo(Disposables);
        Output = Input.Select(x => x.ToUpper())
            .ToReadOnlyReactivePropertySlim("")
            .AddTo(Disposables);
    }
}
