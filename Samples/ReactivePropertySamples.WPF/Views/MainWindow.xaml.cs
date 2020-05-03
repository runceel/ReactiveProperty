using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReactivePropertySamples.WPF.Views
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

        private void BasicUsagesButton_Click(object sender, RoutedEventArgs e) => new BasicUsagesWindow().Show();

        private void ReactiveCommandButton_Click(object sender, RoutedEventArgs e) => new ReactiveCommandWindow().Show();

        private void CollectionsButton_Click(object sender, RoutedEventArgs e) => new CollectionsWindow().Show();
    }
}
