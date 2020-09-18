using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Reactive.Bindings;

namespace MultiUIThreadApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ReactivePropertyScheduler.SetDefaultSchedulerFactory(() =>
                new DispatcherScheduler(Dispatcher.CurrentDispatcher));
        }
    }
}
