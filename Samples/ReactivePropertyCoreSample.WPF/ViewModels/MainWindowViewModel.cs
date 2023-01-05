using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertyCoreSample.WPF.Messages;

namespace ReactivePropertyCoreSample.WPF.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    public ReactiveCommandSlim<string> ShowWindowCommand { get; }
    public MainWindowViewModel()
    {
        ShowWindowCommand = new ReactiveCommandSlim<string>()
            .WithSubscribe(x =>
            {
                MessageBroker.Default.Publish(new ShowWindowMessage(x));
            },
            Disposables.Add)
            .AddTo(Disposables);
    }
}
