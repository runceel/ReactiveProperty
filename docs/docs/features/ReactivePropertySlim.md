# ReactivePropertySlim

`ReactivePropertySlim` is a lightweight version of `ReactiveProperty`.
`ReactivePropertySlim` is five times faster than `ReactiveProperty`.

`ReactivePropertySlim` provides the following features:

- Implements `INotifyPropertyChanged` interface.
- Implements `IObservable<T>` interface.
- Provides a `Value` property.
- Provides a `ForceNotify` method.

`ReactivePropertySlim` is high performance.
The following table shows benchmark results between `ReactiveProperty` and `ReactivePropertySlim`.
ReactivePropertySlim is 16 times faster to create an instance and 36 times faster in the primary use case.

```
|                             Method |         Mean |     Error |    StdDev |
|----------------------------------- |-------------:|----------:|----------:|
|     CreateReactivePropertyInstance |    87.146 ns | 0.8331 ns | 0.7385 ns |
| CreateReactivePropertySlimInstance |     5.460 ns | 0.0537 ns | 0.0502 ns |
|           BasicForReactiveProperty | 2,470.957 ns | 9.1934 ns | 8.1497 ns |
|       BasicForReactivePropertySlim |    68.773 ns | 1.3841 ns | 1.8478 ns |
```

This class can be used like `ReactiveProperty`.

```csharp
var rp = new ReactivePropertySlim<string>("neuecc");
rp.Select(x => $"{x}-san").Subscribe(x => Console.WriteLine(x));
rp.Value = "xin9le";
rp.Value = "okazuki";
```

The output is below.

```
neuecc-san
xin9le-san
okazuki-san
```

One difference from `ReactiveProperty` is that `ReactivePropertySlim` can't be created from `IObservable<T>`.

```csharp
// It isn't valid code.
var rp = Observable.Interval(TimeSpan.FromSeconds(1)).ToReactivePropertySlim();
```

If you want to create an instance of a Slim class from `IObservable<T>`, use the `ToReadOnlyReactivePropertySlim` extension method.

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1)).ToReadOnlyReactivePropertySlim();
```

## Dispatch to UI thread

`ReactivePropertySlim` class doesn't dispatch to the UI thread automatically.
If you need this, use `ReactiveProperty` or dispatch to the UI thread explicitly.

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ObserveOnUIDispatcher() // dispatch to UI thread
    .ToReadOnlyReactivePropertySlim();
```

## Validation

`ValidatableReactiveProperty<T>` is a lightweight implementation of `IReactiveProperty<T>` with validation capabilities. It is designed to provide high performance while offering validation features.

#### Example

Here is an example of how to use `ValidatableReactiveProperty<T>` with a simple validation logic:

```csharp
var validatableProperty = new ValidatableReactiveProperty<string>(
    initialValue: "",
    validate: value => string.IsNullOrEmpty(value) ? "Value cannot be empty" : null
);

validatableProperty.Value = "valid"; // No validation error
validatableProperty.Value = ""; // Validation error: "Value cannot be empty"
```

You can also use `ValidatableReactiveProperty<T>` with `DataAnnotations` for more complex validation scenarios:

```csharp
public class Person
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
}

public class PersonViewModel : IDisposable
{
    private Person _person = new Person();

    public ValidatableReactiveProperty<string> Name { get; }

    public PersonViewModel()
    {
        Name = _person.ToReactivePropertySlimAsSynchronized(x => x.Name)
                      .ToValidatableReactiveProperty(() => Name, disposeSource: true);
    }

    public void Dispose()
    {
        Name.Dispose();
    }
}
```

In this example, the `Name` property of the `Person` class is validated using `DataAnnotations`. The `PersonViewModel` class synchronizes the `Name` property with a `ValidatableReactiveProperty<string>` instance, ensuring that validation rules are applied.

#### Performance

`ValidatableReactiveProperty<T>` offers significant performance improvements over traditional `ReactiveProperty<T>` with validation. The following benchmark results demonstrate the performance benefits:

|                                        Method |         Mean |       Error |      StdDev |
|---------------------------------------------- |-------------:|------------:|------------:|
|                    ReactivePropertyValidation | 4,954.138 ns |  93.2171 ns | 107.3490 ns |
|         ValidatableReactivePropertyValidation |   704.852 ns |  12.8322 ns |  10.7155 ns |

By using `ValidatableReactiveProperty<T>`, you can achieve high performance while maintaining robust validation logic in your applications.