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

namespace MultiUIThreadApp.Views
{
    /// <summary>
    /// Interaction logic for ListBoxWindow.xaml
    /// </summary>
    public partial class ListBoxWindow : Window
    {
        public ListBoxWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e) => (DataContext as IDisposable)?.Dispose();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = $"Thread ID: {Thread.CurrentThread.ManagedThreadId}";
        }
    }
}
