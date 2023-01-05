using System;
using System.Collections.Generic;
using System.Linq;
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
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertyCoreSample.WPF.Messages;
using ReactivePropertyCoreSample.WPF.Views;

namespace ReactivePropertyCoreSample.WPF.Views;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly CompositeDisposable _disposables = new();
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        MessageBroker.Default.Subscribe<ShowWindowMessage>(x =>
        {
            var windowType = Type.GetType($"ReactivePropertyCoreSample.WPF.Views.{x.Name}Window");
            if (windowType is null)
            {
                MessageBox.Show($"{x.Name} was not found.");
                return;
            }

            var window = Activator.CreateInstance(windowType) as Window;
            if (window is null)
            {
                MessageBox.Show($"{windowType.FullName} instance couldn't be created.");
                return;
            }

            window.Show();
        }).AddTo(_disposables);
    }

    private void Window_Unloaded(object sender, RoutedEventArgs e)
    {
        _disposables.Dispose();
    }
}
