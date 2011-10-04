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
using System.ComponentModel.DataAnnotations;
using System.Net;
using Codeplex.Reactive.Asynchronous;

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






        }

    }



    public class MainWindowViewModel
    {
        [Required]
        [RegularExpression("...")]
        public ReactiveCollection<string> TadanoText { get; private set; }



        public ReactiveCollection<string> PropertyName { get; private set; }
        public ReactiveCommand CommandName { get; private set; }


        

        public ReactiveCommand MessageBoxCommand { get; private set; }

        public MainWindowViewModel()
        {





            MessageBoxCommand = new ReactiveCommand();





        }
    }










}
