using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Threading;
using Reactive.Bindings;

namespace MultiUIThreadApp
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ReactivePropertyScheduler.SetDefaultSchedulerFactory(() =>
                new DispatcherScheduler(Dispatcher.CurrentDispatcher));
        }
    }
}
