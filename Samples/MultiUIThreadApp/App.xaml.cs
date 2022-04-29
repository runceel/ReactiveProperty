using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Threading;
using Reactive.Bindings;
using Reactive.Bindings.Schedulers;

namespace MultiUIThreadApp
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ReactivePropertyScheduler.SetDefaultSchedulerFactory(() =>
                new ReactivePropertyWpfScheduler(Dispatcher.CurrentDispatcher));
        }
    }
}
