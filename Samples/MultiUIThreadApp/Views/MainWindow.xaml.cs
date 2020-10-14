using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MultiUIThreadApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e) => (DataContext as IDisposable)?.Dispose();

        private void OpenNewWindowButton_Click(object sender, RoutedEventArgs e)
        {
            // I'm stronglly not recommend multi UI threads.
            var t = new Thread(_ =>
            {
                var w = new ListBoxWindow();
                w.Closed += (_, __) =>
                {
                    Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);
                    Dispatcher.Run();
                };
                w.ShowDialog();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = $"Thread ID: {Thread.CurrentThread.ManagedThreadId}";
        }
    }
}
