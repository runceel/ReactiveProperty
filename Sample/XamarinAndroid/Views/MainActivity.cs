using Android.App;
using Android.OS;
using Android.Widget;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using XamarinAndroid.ViewModels;

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

