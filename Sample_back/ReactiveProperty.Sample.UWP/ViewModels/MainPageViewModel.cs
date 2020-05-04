using ReactiveProperty.Sample.UWP.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ReactiveProperty.Sample.UWP.ViewModels
{
    public class MainPageViewModel
    {
        private readonly INavigationService _navigationService;

        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void Navigate(object invokedItem)
        {

        }
    }
}
