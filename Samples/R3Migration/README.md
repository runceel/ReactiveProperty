# R3 migration sample — ReactiveProperty → R3 + `ReactiveProperty.R3`

This is the **end-to-end migration sample** for the
[`migrating-reactiveproperty-to-r3`](../../skills/migrating-reactiveproperty-to-r3/SKILL.md)
skill and the `ReactiveProperty.R3` bridge package. It takes a small registration ViewModel
that originally used **ReactiveProperty** (root namespace `Reactive.Bindings`) and shows the
result of migrating it to **[R3](https://github.com/Cysharp/R3)** plus the `ReactiveProperty.R3`
bridge for the features R3 does not provide.

The ViewModel deliberately exercises every gap category from the acceptance criteria:
**notifiers**, **validation**, **async commands**, and **collections**.

## Run it

```pwsh
cd Samples/R3Migration
dotnet run -c Release
```

`Program.cs` drives the migrated ViewModel through a full scenario and prints a `PASS`/`FAIL`
line for each behavior, returning a non-zero exit code if anything regresses — so it doubles as
a smoke test.

## Before — ReactiveProperty (`Reactive.Bindings`)

```csharp
using System.Collections.ObjectModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

public sealed class RegistrationViewModel : IDisposable
{
    private readonly ObservableCollection<string> _registrations = new();
    private readonly CompositeDisposable _disposables = new();

    public ReactiveProperty<string> Name { get; }
    public ReactivePropertySlim<int> Age { get; }
    public BusyNotifier Busy { get; } = new();
    public ReadOnlyReactiveCollection<RegisteredUser> RegisteredUsers { get; }
    public AsyncReactiveCommand RegisterCommand { get; }

    public RegistrationViewModel()
    {
        Name = new ReactiveProperty<string>("")
            .SetValidateNotifyError(x => string.IsNullOrWhiteSpace(x) ? "Name is required." : null)
            .AddTo(_disposables);
        Age = new ReactivePropertySlim<int>(0).AddTo(_disposables);

        RegisteredUsers = _registrations
            .ToReadOnlyReactiveCollection(name => new RegisteredUser(name))
            .AddTo(_disposables);

        var canRegister = Busy.CombineLatest(
            Name.ObserveHasErrors,
            (isBusy, hasErrors) => !isBusy && !hasErrors);

        RegisterCommand = canRegister
            .ToAsyncReactiveCommand()
            .WithSubscribe(RegisterAsync)
            .AddTo(_disposables);
    }

    private async Task RegisterAsync()
    {
        using (Busy.ProcessStart())
        {
            await Task.Delay(50);
            _registrations.Add($"{Name.Value} ({Age.Value})");
        }

        Name.Value = "";
        Age.Value = 0;
    }

    public void Dispose() => _disposables.Dispose();
}
```

## After — R3 + `ReactiveProperty.R3`

See [`RegistrationViewModel.cs`](./RegistrationViewModel.cs) for the migrated code. The key
moves, each driven by a rule in
[`references/rules.json`](../../skills/migrating-reactiveproperty-to-r3/references/rules.json):

| Original (`Reactive.Bindings`) | Migrated | `target` | Rule |
|---|---|---|---|
| `ReactivePropertySlim<int>` | R3 `ReactiveProperty<int>` (`using R3;`) | `r3-direct` | `RP-PROP-SLIM` |
| `ReactiveProperty<string>` + `SetValidateNotifyError` (stream/error-aware) | `ValidatableReactiveProperty<string>` | `reactiveproperty-r3` | validation rules |
| `BusyNotifier` | `Reactive.Bindings.R3.Notifiers.BusyNotifier` | `reactiveproperty-r3` | notifier rules |
| `ReadOnlyReactiveCollection<T>` + `ToReadOnlyReactiveCollection` | `Reactive.Bindings.R3.ReadOnlyReactiveCollection<T>` | `reactiveproperty-r3` | collection rules |
| `AsyncReactiveCommand` + `ToAsyncReactiveCommand` / `WithSubscribe` | `Reactive.Bindings.R3.AsyncReactiveCommand` | `reactiveproperty-r3` | command rules |
| `Observable.CombineLatest` / `Subscribe` | R3 `CombineLatest` / `Subscribe` (`using R3;`) | `r3-direct` | Rx operators |

Notes that fall out of the migration:

- The gap types return **R3** observables (`Observable<bool> ObserveHasErrors`, `BusyNotifier`
  is an `Observable<bool>`), so the Rx composition stays R3-native — only `using R3;` is needed,
  no `System.Reactive`.
- `using Reactive.Bindings;` / `Reactive.Bindings.Extensions` / `Reactive.Bindings.Notifiers`
  become `R3` and `Reactive.Bindings.R3[.Notifiers|.Extensions]`.
- Package references change from `ReactiveProperty` to `R3` + `ReactiveProperty.R3`.
