using System;
using System.Windows;
using Codeplex.Reactive;
using WPF.Views;

namespace WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class MainWindowViewModel
    {
        public ReactiveCommand NavigateBasics{ get; private set; }

        public MainWindowViewModel()
        {

            NavigateBasics = new ReactiveCommand();
            NavigateBasics.Subscribe(_ =>
            {
                new ReactivePropertyBasics().Show();
            });
        }
    }
}