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

> **Bridge availability:** `R3` and `ObservableCollections.R3` are already on NuGet. The
> `ReactiveProperty.R3` and `ReactiveProperty.R3.WPF` bridge packages are **not published yet**.
> Until they ship, reference the bridge from source with a `ProjectReference` to the
> `Source/ReactiveProperty.R3` and `Source/ReactiveProperty.R3.WPF` projects in this repository. The
> `dotnet add package` commands below describe the intended experience once the bridge is released.

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

## Runnable sample

This repository ships a **before/after pair** of WPF apps you can open side by side to see a full
migration in context:

- **Before** — `Samples/ReactivePropertySamples.WPF` (with `Samples/ReactivePropertySamples.Shared`):
  the original app built on `Reactive.Bindings` (ReactiveProperty).
- **After** — `Samples/ReactivePropertySamples.R3.WPF` (with
  `Samples/ReactivePropertySamples.R3.Shared`): the same app migrated to R3 + `ReactiveProperty.R3`.

The ViewModels live in the `.Shared` projects, so diffing the two `.Shared` folders shows exactly
which symbols changed and how. The migrated ViewModels are additionally covered by
`Samples/ReactivePropertySamples.R3.Tests`, which locks in the behaviors described below.

## Migrate with the GitHub Copilot CLI

The repository ships a migration **skill** at `skills/migrating-reactiveproperty-to-r3/`. It turns
the steps above into a repeatable, agent-driven flow: it changes the package references, rewrites
every ReactiveProperty symbol from a mapping table, and reports the few cases that need a human
decision instead of guessing. The flow below was validated end to end on a sample WPF app that
exercises ViewModels, DataAnnotations validation, notifiers, a `ReadOnlyReactiveCollection<T>`, and
a XAML `EventToReactiveCommand`.

### 1. Install the skill

**Recommended — install it into your project so the CLI loads it automatically and the agent can
read the skill's mapping table with no friction.** Copy the skill folder into your project's
`.agents/skills/` directory (this is what the repo's `skills/README.md` recommends first):

```powershell
# from a clone of this repository, run at the root of YOUR app's repo
Copy-Item -Recurse <path-to-clone>/skills/migrating-reactiveproperty-to-r3 `
  ".agents/skills/migrating-reactiveproperty-to-r3"
```

Because the skill now lives **inside your project tree**, the agent reads its bundled
`references/rules.json` directly with its structured file reader — no read-scoping workaround needed.

**Alternative — install it once for every project** in your personal Copilot skills directory:

```powershell
Copy-Item -Recurse skills/migrating-reactiveproperty-to-r3 `
  "$HOME/.copilot/skills/migrating-reactiveproperty-to-r3"
```

On macOS/Linux the personal destination is `~/.copilot/skills/migrating-reactiveproperty-to-r3`.
This is handy when you migrate several projects, but the skill folder is then *outside* your project,
which triggers the read-scoping note at the end of this guide. Either way, after the copy start
`copilot` in your project; the skill loads on its own and activates when you ask to migrate to R3.
(Installing it as a local plugin/marketplace also works, but the folder copy is simplest.)

### 2. Ask for a plan before changing code

Open the CLI in your project and ask for an inventory and plan first. A scoped, realistic prompt
works best — do not ask the agent to "migrate everything" in one shot:

```text
I want to migrate this WPF app from ReactiveProperty to R3. Before changing any code, inventory
every ReactiveProperty symbol in the project and tell me, for each one, the matching rule, its
target (r3-direct / reactiveproperty-r3 / manualReview), and the R3 replacement. List the
manual-review items separately.
```

The agent reads the mapping table and produces a per-symbol plan: direct R3 replacements
(`ReactivePropertySlim<T>` → `R3.ReactiveProperty<T>`, `ReactiveProperty<T>` →
`R3.BindableReactiveProperty<T>`, `ObserveProperty` → `ObservePropertyChanged`, …), bridge
replacements for the genuine gaps (`AsyncReactiveCommand`, `BusyNotifier`,
`ReadOnlyReactiveCollection<T>`, `ValidatableReactiveProperty<T>`,
`ToReactivePropertyAsSynchronized`), and a short manual-review list.

### 3. Migrate incrementally

Migrate in small, reviewable chunks and keep the same session with `--continue`:

```text
Migrate the CounterViewModel and PeopleViewModel to R3 now. Use R3 from NuGet and the
ReactiveProperty.R3 bridge for the gap types.
```

```text
Now migrate the remaining ViewModels, fix the package and using directives, and swap the XAML
EventToReactiveCommand xmlns to the R3.WPF bridge.
```

For XAML trigger actions the rewrite is mostly an xmlns swap:

```xml
<!-- before -->
xmlns:i="clr-namespace:Reactive.Bindings.Interactivity;assembly=ReactiveProperty.WPF"
<!-- after -->
xmlns:i="clr-namespace:Reactive.Bindings.R3.Interactivity;assembly=ReactiveProperty.R3.WPF"
```

### 4. Build, test, and fix

Ask the agent to build and run your existing tests, then drive any failures with concrete prompts:

```text
Build the solution and run the tests, then report build status, test status, and the manual-review
list.
```

```text
The build fails with <paste the error>. Fix it.
```

Because the bridge gap types expose **R3** observables, make sure `using R3;` is present in files
that subscribe or chain operators (`Subscribe`, `Where`, `Select`, …).

> **Validation timing:** DataAnnotations on a single property map to R3's
> `BindableReactiveProperty<T>.EnableValidation()`, but R3 validates **lazily** — it does not flag
> the initial value until a binding or subscriber attaches. If you have headless tests (or logic)
> that expect the original eager validation, use `ReactiveProperty.R3`'s
> `ValidatableReactiveProperty<T>` instead, which validates immediately.

### 5. Resolve the manual-review items

The skill never guesses the cases that cannot be rewritten mechanically — it flags them with the
file, line, and a note. Expect a small list, typically:

- **A `ReactivePropertyMode` argument** (for example on `ToReactivePropertyAsSynchronized` or a
  `ReactiveProperty<T>` constructor). It has no R3 equivalent, so the agent removes it and explains
  how to reproduce the intent: `DistinctUntilChanged` matches R3's default distinct behavior, and
  `RaiseLatestValueOnSubscribe` maps to whether the property replays its current value. Confirm the
  behavior you actually relied on.
- **A custom `IScheduler` argument.** R3 uses `TimeProvider` for time and `SynchronizationContext`
  for dispatch, so a non-default scheduler cannot be translated automatically. The agent reports it
  and you choose the right provider for that call site.
- **`System.IObservable<T>` boundaries** that must stay on `System.Reactive` (public APIs,
  third-party Rx). Bridge them with R3's `ToObservable()` / `AsSystemObservable()` rather than
  retyping the declarations.
- **DataAnnotations validation that must stay eager.** The skill rewrites single-property
  DataAnnotations to `EnableValidation()`, which validates lazily — see the **Validation timing**
  note above. If you relied on eager validation (for example in headless tests), switch that
  property to `ValidatableReactiveProperty<T>`.

You can invoke the whole flow with requests as simple as "migrate this app to R3", "replace
ReactiveProperty with R3", or "move this ViewModel off ReactiveProperty".

> **Note (only if you used the personal `~/.copilot/skills/` install):** The skill is driven by its
> bundled mapping table at `migrating-reactiveproperty-to-r3/references/rules.json`. The CLI's
> structured file reader is scoped to your project directory, so when the skill lives under
> `~/.copilot/skills/` (outside your project) the agent may report that it "cannot read" that file —
> sometimes even mislabeling it as a content/policy block. That is a read-scoping quirk, not a real
> block: the agent can still read it (for example with a shell command) and otherwise falls back to
> the guidance carried in the skill itself. **Installing the skill in your project's
> `.agents/skills/` (the recommended path in step 1) avoids this entirely**, because the mapping
> table is then inside your working tree.

## Notes for large migrations

The bridge is intended to be a long-term migration path, not a temporary shim. It is a good fit for projects that want to move to R3 gradually while keeping the existing MVVM patterns and behaviors they already rely on.
