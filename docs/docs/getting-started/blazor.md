# Getting started with Blazor

Blazor is a development framework for single-page applications using C#.

See the following:

[ASP.NET Core Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/)

ReactiveProperty works with both Blazor Server and Blazor WebAssembly, but Blazor WebAssembly doesn't support all Reactive Extensions operations. For example, the `Delay` extension method doesn't work on Blazor WASM.
If you want to use ReactiveProperty on Blazor WASM, be careful not to use unsupported features.

## Create a project

- Create a Blazor Server or WebAssembly project.
- Install ReactiveProperty.Blazor package from NuGet.

## Edit the code

- Create a class named `IndexViewModel`.
- Edit the class as follows:

```csharp
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BlazorApp1;

public class IndexViewModel : IDisposable
{
    private CompositeDisposable _disposable = new();

    public ReactivePropertySlim<string> Input { get; }
    public ReadOnlyReactivePropertySlim<string> Output { get; }

    public IndexViewModel()
    {
        Input = new ReactivePropertySlim<string>("")
            .AddTo(_disposable);
        Output = Input
            .Delay(TimeSpan.FromSeconds(2)) // Important! The Delay method doesn't work on Blazor WASM. If you are working on WASM, remove this line.
            .Select(x => x.ToUpperInvariant())
            .ToReadOnlyReactivePropertySlim("")
            .AddTo(_disposable);
    }

    public void Dispose() => _disposable.Dispose();
}
```

- Edit Index.razor as follows:

```csharp
@page "/"
@using System.Reactive.Disposables
@using Reactive.Bindings.Extensions
@implements IDisposable

<PageTitle>Index</PageTitle>

<h1>Hello, world!</h1>

<input type="text" @bind="_viewModel.Input.Value" />
<br/>
@_viewModel.Output.Value
<br/>

@code {
    private readonly CompositeDisposable _disposable = new();
    private IndexViewModel _viewModel = default!;

    protected override void OnInitialized()
    {
        _viewModel = new IndexViewModel()
            .AddTo(_disposable);

        // Observe changes to the Output property, and call StateHasChanged on the UI thread.
        _viewModel.Output
            .Subscribe(x => InvokeAsync(StateHasChanged))
            .AddTo(_disposable);
    }

    public void Dispose() => _disposable.Dispose();
}
```

- Launch the app.

You can see the following result:

![Launch the app](./images/blazor-helloworld.png)

## Other topics for Blazor

### Dependency Injection

If you want to inject ViewModels into a page, register them in the DI container in Program.cs as follows:

```csharp
builder.Services.AddTransient<IndexViewModel>();
```

Then inject the ViewModel into the page with `@inject IndexViewModel _viewModel`.


### Integrate validation feature

If you want to use ReactiveProperty validation features with Blazor's EditForm component, you can use the `Reactive.Bindings.Components.ReactivePropertiesValidator` component.

`ReactivePropertiesValidator` can be used in the same way as the `DataAnnotationsValidator` component, as shown below:

```csharp
<EditForm Model="_validationViewModel" OnInvalidSubmit="InvalidSubmit" OnValidSubmit="ValidSubmit">
    <ReactivePropertiesValidator /> @* needs @using Reactive.Bindings.Components *@

    <ValidationSummary />

    <div class="mb-3">
        <label for="firstName">First name</label>
        <InputText @bind-Value="_validationViewModel.FirstName.Value" class="form-control" />
        <ValidationMessage For="() => _validationViewModel.FirstName.Value" />
    </div>
```

See the Blazor sample app under the Samples/Blazor folder for more details. The page that uses it is Pages/Index.razor in the BlazorSample.Shared project.
