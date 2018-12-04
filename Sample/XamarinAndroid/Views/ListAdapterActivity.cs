
using System.Reactive.Disposables;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using XamarinAndroid.ViewModels;

namespace XamarinAndroid.Views
{
    [Activity(Label = "ListAdapterActivity")]
    public class ListAdapterActivity : ListActivity
    {
        private ListAdapterActivityViewModel viewModel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var inflater = LayoutInflater.FromContext(this);
            var header = inflater.Inflate(Resource.Layout.ListAdapterHeader, null);
            this.ListView.AddHeaderView(header);

            this.viewModel = new ListAdapterActivityViewModel(this);
            header.FindViewById<Button>(Resource.Id.ButtonAdd)
                .ClickAsObservable()
                .SetCommand(this.viewModel.AddPersonCommand);
            this.ListAdapter = this.viewModel.People
                .ToAdapter(
                    (index, value) => inflater.Inflate(Resource.Layout.ListAdapterPersonTemplate, null),
                    (index, value, view) =>
                    {
                        if (view.Tag != null)
                        {
                            view.Tag.Dispose(); // release databinding.
                        }

                        var d = new CompositeDisposable();
                        var textViewName = view.FindViewById<TextView>(Resource.Id.TextViewName);
                        textViewName.SetBinding(
                            x => x.Text,
                            value.Name)
                            .AddTo(d);
                        var textViewAge = view.FindViewById<TextView>(Resource.Id.TextViewAge);
                        textViewAge.SetBinding(
                            x => x.Text,
                            value.Age)
                            .AddTo(d);
                        view.Tag = new DisposableHolder(d);
                    },
                    (index, value) => value.Id.Value);
        }
    }
}
