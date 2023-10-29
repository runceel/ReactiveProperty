using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive;

namespace ReactiveProperty.WPF.ManualTests;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void DeadlockTestButton_Click(object sender, RoutedEventArgs e)
    {
        deadlockTestResult.Text = "Running...";
        var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), ReactivePropertyScheduler.Default);
        timer.Start();
        timer.Dispose();
        deadlockTestResult.Text = "Passed";
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
        {
            UseShellExecute = true
        });
    }

    private async void StackOverflowTestButton_Click(object sender, RoutedEventArgs e)
    {
        const int Max = 10000;
        stackOverflowTestResult.Text = "Running...";
        var list = new List<string>();
        var source = new ObservableCollection<string>();
        var readOnlyReactiveCollection = source.ToReadOnlyReactiveCollection();

        await Task.Run(() =>
        {
            for (int i = 0; i < Max; i++)
            {
                source.Add(i.ToString());
            }
        });

        int loopCount = 0;
        while(readOnlyReactiveCollection.Count < Max)
        {
            await Task.Delay(1000);
            stackOverflowTestResult.Text = $"Running... waiting for finishing... {loopCount++}";
            if (loopCount > 10)
            {
                stackOverflowTestResult.Text = "Failed";
                return;
            }
        }

        stackOverflowTestResult.Text = "Passed";
    }

    private void ValidationUITestButton_Click(object sender, RoutedEventArgs e)
    {
        new ValidationWindow().ShowDialog();
    }
}
