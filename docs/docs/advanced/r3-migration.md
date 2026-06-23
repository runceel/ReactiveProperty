# Migrate to R3 with ReactiveProperty.R3

ReactiveProperty now recommends [R3](https://github.com/Cysharp/R3) for new applications, but many existing apps still rely on features that R3 does not provide out of the box. `ReactiveProperty.R3` is a permanent bridge package for those cases. It keeps the familiar ReactiveProperty experience while exposing R3-friendly observables and using `TimeProvider` / `SynchronizationContext` instead of `System.Reactive` schedulers.

## When to use it

Use `ReactiveProperty.R3` when you are moving a project from `Reactive.Bindings` to R3 and you still need one of the higher-level MVVM helpers that R3 intentionally leaves out.

The bridge covers the common gaps that appear during migration:

- notifiers such as `BooleanNotifier`, `BusyNotifier`, `CountNotifier`, `ScheduledNotifier<T>` and message brokers
- `ReactiveTimer`
- `AsyncReactiveCommand`
- `ReadOnlyReactiveCollection<T>` and collection helpers
- `ValidatableReactiveProperty<T>` and validation helpers
- `ToReactivePropertyAsSynchronized`

## Install the packages

Use the .NET CLI or Visual Studio NuGet Package Manager to add the packages to your project.

If you are using the .NET CLI, run:

```powershell
dotnet add package R3
dotnet add package ReactiveProperty.R3
```

If you are using Visual Studio, right-click the project, choose Manage NuGet Packages, search for `R3` and `ReactiveProperty.R3`, and install them from there.

If the project uses WPF trigger actions or converters, also add the WPF bridge package:

```powershell
dotnet add package ReactiveProperty.R3.WPF
```

Visual Studio users can install `ReactiveProperty.R3.WPF` the same way from the NuGet package manager.

## Migration approach

1. Replace the old ReactiveProperty packages with `R3` and `ReactiveProperty.R3`.
2. Rewrite the parts that R3 already provides directly to the native R3 APIs.
3. Keep the bridge types only for the genuine gaps listed above.
4. Add `using R3;` in files that compose observables, because the bridge types return R3 observables.

### Namespace changes

The bridge uses a dedicated namespace so it can coexist with the original ReactiveProperty package during an incremental migration:

- `Reactive.Bindings` -> `R3` for direct conversions
- `Reactive.Bindings.Extensions` -> `R3` and `Reactive.Bindings.R3.Extensions`
- `Reactive.Bindings.Notifiers` -> `Reactive.Bindings.R3.Notifiers`

The following table shows the most common migrations:

| ReactiveProperty API | R3 / bridge replacement |
|---|---|
| `ReactivePropertySlim<T>` | `R3.ReactiveProperty<T>` |
| `AsyncReactiveCommand` | `Reactive.Bindings.R3.AsyncReactiveCommand` |
| `BusyNotifier` | `Reactive.Bindings.R3.Notifiers.BusyNotifier` |
| `ReadOnlyReactiveCollection<T>` | `Reactive.Bindings.R3.ReadOnlyReactiveCollection<T>` |
| `ValidatableReactiveProperty<T>` | `Reactive.Bindings.R3.ValidatableReactiveProperty<T>` |
| `ToReactivePropertyAsSynchronized` | `Reactive.Bindings.R3.Extensions.ToReactivePropertyAsSynchronized` |

A typical migration looks like this:

```csharp
using R3;
using Reactive.Bindings.R3.Notifiers;

public sealed class ViewModel
{
    public ReactiveProperty<int> Count { get; } = new(0);
    public BusyNotifier IsBusy { get; } = new();
}
```

## How to use the migration skill

The repository also includes a published migration skill at `skills/migrating-reactiveproperty-to-r3/SKILL.md`. Use it when you want an agent or a human to migrate an app from `Reactive.Bindings` to R3 while keeping the features that R3 does not provide.

The skill is intended for projects that already use ReactiveProperty and want a repeatable migration flow. It works by:

1. changing the package references to `R3` and `ReactiveProperty.R3`
2. rewriting ReactiveProperty symbols that already have direct R3 equivalents
3. rewriting the genuine gap types to the bridge package types
4. reporting any cases that need manual review instead of guessing a replacement

In practice, you can invoke the skill when a request says things like:

- "migrate this app to R3"
- "replace ReactiveProperty with R3"
- "move this ViewModel off ReactiveProperty"

The skill is driven by the mapping table in `skills/migrating-reactiveproperty-to-r3/references/rules.json`, so the migration is systematic rather than a one-off rewrite.

## Notes for large migrations

The bridge is intended to be a long-term migration path, not a temporary shim. It is a good fit for projects that want to move to R3 gradually while keeping the existing MVVM patterns and behaviors they already rely on.
