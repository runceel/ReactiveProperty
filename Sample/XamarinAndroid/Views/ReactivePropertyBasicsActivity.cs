using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sample.ViewModels;
using Android.Util;

namespace XamarinAndroid.Views
{
    [Activity(Label = "ReactivePropertyBasicsActivity")]
    public class ReactivePropertyBasicsActivity : Activity
    {
        private ReactivePropertyBasicsViewModel viewModel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.SetContentView(Resource.Layout.ReactivePropertyBasics);

            this.viewModel = new ReactivePropertyBasicsViewModel();

            this.FindViewById<EditText>(Resource.Id.EditTextInput)
                .SetBinding(
                    x => x.Text,
                    this.viewModel.InputText,
                    x => x.TextChangedAsObservable().ToUnit());

            this.FindViewById<TextView>(Resource.Id.TextViewOutput)
                .SetBinding(
                    x => x.Text,
                    this.viewModel.DisplayText);

            var buttonReplaceText = this.FindViewById<Button>(Resource.Id.ButtonReplaceText);
            buttonReplaceText
                .ClickAsObservable()
                .SetCommand(this.viewModel.ReplaceTextCommand);
            buttonReplaceText
                .SetBinding(
                    x => x.Enabled,
                    this.viewModel.ReplaceTextCommand
                        .CanExecuteChangedAsObservable()
                        .Select(_ => this.viewModel.ReplaceTextCommand.CanExecute())
                        .ToReactiveProperty());
            
        }
    }
}