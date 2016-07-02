using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace WPF
{
    public partial class App : Application
    {
        public App()
        {
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Initialize UIDispatcherScheduler
            //Reactive.Bindings.UIDispatcherScheduler.Initialize();
        }
    }
}