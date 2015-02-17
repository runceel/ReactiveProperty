# ReactiveProperty.XamarinAndroid
ReactiveProperty.XamarinAndroid is MVVM support library for ReactiveProperty + Xamarin.Android environment.

- Xamarin is [here](https://xamarin.com/).

# Features

- OneWay and TwoWay databinding, between View's property and ReactiveProperty.
- IList, ObservableCollection<T> and ReadOnlyObservableCollectoion<T> convert to IListAdapter.
- View classes(ex, Button, TextView...) event provide extension method "EventName"AsObservable.

## DataBinding

This ViewModel class is here.
```cs
public class VM
{
  // ViewModel Property
  public ReactiveProperty<string> Output { get; set; }

  public VM() { this.Output = new ReactiveProperty(); }
}
```

### OneWay databinding.

```cs
var vm = new VM();

this.FindViewById<TextView>(Resource.Id.TextView)
  .SetBinding(
    x => x.Text, // select target property
    vm.Output); // source property
```

### TwoWay databinding

```cs
var vm = new VM();

this.FindViewById<EditText>(Resource.Id.EditText)
  .SetBinding(
    x => x.Text, // select target property
    vm.Output,  // source property
    x => x.TextChangedAsObservable().ToUnit()); // update source trigger
```

### Command binding
SetCommand extension methods can be found in IObservable <T>.
```cs
this.FindViewById<Button>(Resource.Id.Button)
  .ClickAsObservable()
  .SetCommand(vm.AlertCommand);
```

### Collection binding
Collection type can convert to IListAdapter.
```cs
public class VM
{
  public ReadOnlyReactiveCollection<string> Values { get; private set; }
}


var vm = new VM();
this.FindViewById<ListView>(Resource.Id.ListView)
    .Adapter = vm.Values.ToAdapter(
      (index, value) => LayoutInflator.FromContext(this).Inflate(Android.Resource.Layout.SimpleListItem1) // create view
      (index, value, view) => view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = value, // setup view
      (index, value) => value.GetHashCode()); // generate id
```

### many many many... extension methods
[here](https://github.com/runceel/ReactiveProperty/blob/vNext/Source/ReactiveProperty.Platform.Android/Extensions/ViewEventExtensions.cs)
