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
            // Initialize UIDispatcherScheduler
            Codeplex.Reactive.UIDispatcherScheduler.Initialize();
        }
    }
}