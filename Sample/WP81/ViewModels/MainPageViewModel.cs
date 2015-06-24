using Reactive.Bindings;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace WP81.ViewModels
{
    public class MainPageViewModel
    {
        [Dependency]
        public INavigationService NavigationService { get; set; }

        public ReactiveCommand NavigateReactivePropertyBasicsCommand { get; }

        public MainPageViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            this.NavigateReactivePropertyBasicsCommand = new ReactiveCommand();
            this.NavigateReactivePropertyBasicsCommand.Subscribe(_ =>
            {
                this.NavigationService.Navigate("ReactivePropertyBasics", null);
            });
        }
    }
}
