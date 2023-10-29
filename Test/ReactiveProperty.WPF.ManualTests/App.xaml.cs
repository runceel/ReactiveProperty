using System.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Schedulers;

namespace ReactiveProperty.WPF.ManualTests;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        ReactivePropertyScheduler.SetDefault(new ReactivePropertyWpfScheduler(Dispatcher));
    }
}
