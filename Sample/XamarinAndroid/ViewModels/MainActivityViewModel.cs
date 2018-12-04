using System;

using Android.App;
using Reactive.Bindings;
using XamarinAndroid.Views;

namespace XamarinAndroid.ViewModels
{
    public class MainActivityViewModel
    {
        public ReactiveCommand NavigateReactivePropertyBasicsCommand { get; }

        public ReactiveCommand NavigateListAdapterCommand { get; }

        public MainActivityViewModel(Activity context)
        {
            this.NavigateReactivePropertyBasicsCommand = new ReactiveCommand();
            this.NavigateReactivePropertyBasicsCommand
                .Subscribe(_ => context.StartActivity(typeof(ReactivePropertyBasicsActivity)));

            this.NavigateListAdapterCommand = new ReactiveCommand();
            this.NavigateListAdapterCommand
                .Subscribe(_ => context.StartActivity(typeof(ListAdapterActivity)));
        }
    }
}
