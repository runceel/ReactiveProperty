using ReactiveProperty.Sample.UWP.Mvvm;
using ReactiveProperty.Sample.UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ReactiveProperty.Sample.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel ViewModel { get; }
        public MainPage()
        {
            NavigationService.RegisteryViewType(ViewNames.BasicUsage, typeof(BasicUsagePage));

            ViewModel = new MainPageViewModel(new NavigationService(RootFrame));
            this.InitializeComponent();
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            ViewModel.Navigate(args.InvokedItem);
        }
    }
}
