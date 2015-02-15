using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XamarinAndroid.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace XamarinAndroid.Views
{
    [Activity(Label = "XamarinAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private MainActivityViewModel viewModel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            this.viewModel = new MainActivityViewModel(this);

            this.FindViewById<Button>(Resource.Id.ButtonReactivePropertyBasics)
                .ClickAsObservable()
                .SetCommand(this.viewModel.NavigateReactivePropertyBasicsCommand);

            this.FindViewById<Button>(Resource.Id.ButtonListAdapter)
                .ClickAsObservable()
                .SetCommand(this.viewModel.NavigateListAdapterCommand);
        }
    }
}

