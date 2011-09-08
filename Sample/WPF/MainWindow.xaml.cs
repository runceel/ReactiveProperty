using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Codeplex.Reactive.Notifier;
using Codeplex.Reactive;
using GalaSoft.MvvmLight;
using System.Reactive.Linq;
using Codeplex.Reactive.Extensions;
using System.ComponentModel;

namespace WPF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            DataContext = new MainWindowViewModel();


        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveProperty<string> In { get; set; }
        public ReactiveProperty<string> Out { get; set; }
        public ReactiveCollection<long> Collection { get; set; }

        public MainWindowViewModel()
        {
            In = new ReactiveProperty<string>(_=>RaisePropertyChanged("In"));

            Out = In.Delay(TimeSpan.FromSeconds(1))
                    .ToReactiveProperty();

            Collection = Observable.Interval(TimeSpan.FromSeconds(1))
                .ToReactiveCollection();



            
        }
    }
}
