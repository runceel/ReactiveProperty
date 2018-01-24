## Create a project

- Create a Cross Platform app (Xamarin.Forms) project.
- Setting the `New Cross Platform App` dialog like following.
  Choice the .NET Standard project. Of couse you can select shared project.
  ![New Cross Platform App dialog](images/xf-create-project.png)
- Install ReactiveProperty to all projects from NuGet.

### Edit codes

- Create MainPageViewModel.cs to .NET Standard project.
- Edit file like following.

MainPageViewModel.cs
```cs
using Reactive.Bindings;
using System;
using System.Reactive.Linq;

namespace GettingStartedXF
{
    public class MainPageViewModel
    {
        public ReactiveProperty<string> Input { get; }
        public ReadOnlyReactiveProperty<string> Output { get; }

        public MainPageViewModel()
        {
            Input = new ReactiveProperty<string>("");
            Output = Input
                .Delay(TimeSpan.FromSeconds(1))
                .Select(x => x.ToUpper())
                .ToReadOnlyReactiveProperty();
        }
    }
}
```

MainPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:GettingStartedXF"
             x:Class="GettingStartedXF.MainPage">
    <ContentPage.BindingContext>
        <local:MainPageViewModel />
    </ContentPage.BindingContext>
    <StackLayout>
        <Entry Text="{Binding Input.Value, UpdateSourceEventName=TextChanged}" />
        <Label Text="{Binding Output.Value}" />
    </StackLayout>
</ContentPage>
```

# Launch the application.

After launch the app, you can see below window.
The output value was displayed to upper case, after 1sec from the input.

![Launch the app](images/launch-xf-app-android.gif)

![Launch the app](images/launch-xf-app-uwp.gif)

