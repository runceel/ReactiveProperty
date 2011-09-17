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


    public class MainWindowViewModel
    {
        public ReactiveProperty<string> FirstName { get; set; }
        public ReactiveProperty<string> LastName { get; set; }
        public ReactiveProperty<string> FullName { get; set; }

        public MainWindowViewModel()
        {
            FirstName = new ReactiveProperty<string>();
            LastName = new ReactiveProperty<string>();

            FullName = FirstName.CombineLatest(LastName, (f, l) => f + " " + l)
                .ToReactiveProperty();
        }
    }

}
