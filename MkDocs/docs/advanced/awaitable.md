# Awaitable

You can use `await` operator to `ReactiveProperty`(includes Slim), `ReadOnlyReactiveProperty`(includes Slim), `ReactiveCommand` and `AsyncReactiveCommand`.
When use `await` operator to them, then wait a next value will be published.

## For example:

```cs
// Create a ReactiveProperty with initial value that is 999.
var rp = new ReactiveProperty<int>(999);

// Value will be setted after 1 sec.
var _ = Task.Delay(1000).Subscribe(_ => rp.Value = 1000);

// Wait the value will be changed.
var result = await rp;
Console.WriteLine(result); // Output is '1000'. Not '999'.
```

```cs
var cmd = new ReactiveCommand<int>();
cmd.Execute(999); // Call the Execute method before use await operator.

// The Execute method will be called with an argument that value is 1000 after 1 sec.
var _ = Task.Delay(1000).Subscribe(_ => cmd.Execute(1000));

// Wait the Execute method will be called.
var result = await cmd;
Console.WriteLine(result); // Output is '1000'. Not '999'.
```
