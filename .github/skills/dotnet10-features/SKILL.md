---
name: dotnet10-features
description: 'Reference for the .NET 10 / C# 14 features that are relevant to the ReactiveProperty repository. Use this when adopting or evaluating .NET 10 / C# 14 language and Blazor features in this codebase: modernizing INotifyPropertyChanged property accessors, refactoring the many `Reactive.Bindings` extension methods, raising LangVersion or adding a net10.0 target framework, or working on ReactiveProperty.Blazor / Blazor samples. Triggers include mentions of ".NET 10", "C# 14", "field keyword", "extension members", "net10.0", "LangVersion", or Blazor net10 work in this repo.'
---

# .NET 10 / C# 14 features relevant to ReactiveProperty

This skill curates the **.NET 10** (and its bundled **C# 14**) features that actually
matter for this repository, and maps each one to concrete ReactiveProperty scenarios.
It is intentionally scoped: it skips .NET 10 features with no bearing on a reactive MVVM
library.

## Repository context (read this first)

Target frameworks and language version are **not uniform** across the solution, so a C# 14
feature may be unavailable in a given project:

| Project | Target framework(s) | C# 14 available? |
| --- | --- | --- |
| `ReactiveProperty.Core` | `netstandard2.0;net8.0;net9.0;net472` | Only if `LangVersion` is raised; runtime-dependent features stay off on `netstandard2.0`/`net472` |
| `ReactiveProperty` (main, `ReactiveProperty.NETStandard`) | `netstandard2.0;net8.0;net9.0;net472` | Same as above |
| `ReactiveProperty.Platform.WPF` | `net8.0-windows;net9.0-windows;net472` | Same as above |
| `ReactiveProperty.Platform.UWP` | (UWP TFMs) | Same as above |
| `ReactiveProperty.Platform.Blazor` | `net8.0;net9.0;net10.0` | Yes on the `net10.0` leg |
| `Samples/**`, Blazor samples | include `net10.0` | Yes |

Key facts:

- `Source/Directory.Build.props` currently pins **`<LangVersion>12.0</LangVersion>`**, so the
  library compiles as **C# 12** today. Adopting any C# 14 feature in the library requires
  raising `LangVersion` (to `14` / `latest` / `preview`).
- `net10.0` is currently used only by **`ReactiveProperty.Platform.Blazor`** and the
  **samples**. The core/main/WPF/UWP libraries do **not** target `net10.0`.
- The library is **multi-targeted down to `netstandard2.0` and `net472`**. C# 14 *language*
  features that the compiler can lower (e.g. `field`, extension members, null-conditional
  assignment, partial members) work across all TFMs once `LangVersion` is raised, but
  features that depend on the **.NET 10 runtime/BCL** (e.g. first-class `Span<T>` conversions
  that rely on new BCL APIs) will not light up on the older TFMs. Guard those with
  `#if NET10_0_OR_GREATER` when needed.

## How to enable C# 14 in a project here

```xml
<!-- Raise once in Source/Directory.Build.props, or per-project. -->
<LangVersion>14.0</LangVersion>
```

For runtime/BCL-gated features, multi-target and branch with the framework constant:

```csharp
#if NET10_0_OR_GREATER
    // .NET 10-only path
#else
    // fallback for net8.0 / net9.0 / netstandard2.0 / net472
#endif
```

---

## C# 14 features, ranked by relevance to this repo

### 1. The `field` keyword — HIGH relevance

`field` exposes the compiler-synthesized backing field inside a property accessor, so you can
add validation/notification without declaring an explicit field. ReactiveProperty exists to
serve MVVM/`INotifyPropertyChanged`, and both the library internals and consumer ViewModels
are full of hand-written backing fields.

Classic INPC property:

```csharp
private string _name = "";
public string Name
{
    get => _name;
    set
    {
        if (_name == value) return;
        _name = value;
        PropertyChanged?.Invoke(this, new(nameof(Name)));
    }
}
```

With `field`:

```csharp
public string Name
{
    get;
    set
    {
        if (field == value) return;
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(Name)));
    }
}
```

Notes for this repo:
- Watch for the naming collision: identifiers literally named `field` must be disambiguated
  with `@field` or `this.field`.
- Works on all TFMs once `LangVersion >= 14`; this is the lowest-risk C# 14 adoption here.

### 2. Extension members — HIGH relevance

ReactiveProperty's public surface is heavily built on **extension methods** in the
`Reactive.Bindings` namespace (`ToReactiveProperty`, `ToReadOnlyReactivePropertySlim`,
`ObserveProperty`, `CombineLatest` helpers, etc.). C# 14 adds an `extension` block syntax that
also enables **extension properties**, **static extension members**, and **extension
operators** — grouped by receiver instead of repeating `this T source` on every method.

```csharp
public static class ObservableExtensions
{
    extension<T>(IObservable<T> source)
    {
        // extension method (same call site as today)
        public ReactiveProperty<T> ToReactiveProperty(T initialValue = default!)
            => new ReactiveProperty<T>(source, initialValue);

        // NEW: extension property
        public IObservable<T> Shared => source.Publish().RefCount();
    }
}
```

Notes for this repo:
- Lets related operators on `IObservable<T>` / `IReactiveProperty<T>` be grouped in one
  `extension(...)` block, improving discoverability and enabling property-style helpers.
- This is a **language** feature (compiler-lowered), so it can apply across all TFMs once
  `LangVersion` is raised — but it is an API-shape decision; keep existing extension method
  signatures for source/binary compatibility and only add new members.

### 3. Null-conditional assignment (`?.`/`?[]` on the left side) — MEDIUM relevance

You can now assign through `?.`/`?[]`:

```csharp
// before
if (viewModel is not null) viewModel.SelectedItem = item;
// C# 14
viewModel?.SelectedItem = item;
```

Useful in ViewModel/event-handler glue and in sample apps. Increment/decrement
(`++`/`--`) are not allowed on the conditional left side; compound assignments (`+=`, etc.)
are.

### 4. First-class `Span<T>` / `ReadOnlySpan<T>` conversions — MEDIUM (performance)

C# 14 adds implicit conversions among `Span<T>`, `ReadOnlySpan<T>`, and `T[]`, improving
overload resolution and generic inference for span-based, allocation-free code paths. Relevant
only in hot paths (e.g. buffer/array handling inside operators).

Caveat: the most useful conversions rely on **.NET 10 BCL** support, so gate span-optimized
paths with `#if NET10_0_OR_GREATER` and keep an allocating fallback for `net8.0`/`net9.0`/
`netstandard2.0`/`net472`.

### 5. Simple lambda parameters with modifiers — MEDIUM/LOW relevance

Modifiers (`ref`, `in`, `out`, `scoped`, `ref readonly`) can now be applied to lambda
parameters **without** restating the type:

```csharp
delegate bool TryParse<T>(string text, out T result);
TryParse<int> parse = (text, out result) => int.TryParse(text, out result);
```

Handy for Rx/LINQ-style lambdas and `TryParse`-shaped converters in samples. `params` still
requires an explicit type.

### 6. Partial events and constructors — MEDIUM relevance

Instance constructors and events can now be `partial` (one defining + one implementing
declaration). This is primarily a **source-generator** enabler. If any INPC/event plumbing in
this repo (or a consumer toolkit pattern) is generated, partial events let a generator provide
`add`/`remove` accessors while the field-like event is declared by hand.

### 7. `nameof` with unbound generic types — LOW relevance

`nameof(ReactiveProperty<>)` now evaluates to `"ReactiveProperty"`. Minor convenience for
diagnostics, logging, and tests that reference open generic types.

### 8. User-defined compound assignment operators — LOW relevance

You can define `+=`, `-=`, etc. directly (instead of relying on the binary operator plus an
assignment). Niche for this library; mentioned for completeness.

---

## ASP.NET Core / Blazor 10 (for `ReactiveProperty.Platform.Blazor` and Blazor samples)

`ReactiveProperty.Platform.Blazor` targets `net10.0` and centers on **EditForm validation**
(`ReactivePropertiesValidator`, `EditContext.EnableReactivePropertiesValidation()`) plus
command helpers. The most relevant .NET 10 Blazor changes:

- **Improved form validation** — new opt-in validation (`builder.Services.AddValidation()` +
  `[ValidatableType]`) that validates nested objects and collection items. This overlaps with
  ReactiveProperty's `EditContext` validation integration; be aware of how the two interact
  when both are enabled on the same `EditForm`, and document the recommended combination.
- **`OwningComponentBase` now implements `IAsyncDisposable`** — relevant to the disposal
  pattern in `ReactivePropertiesValidator` (currently `IDisposable` wrapping a
  `SingleAssignmentDisposable`). Async disposal can be useful where reactive subscriptions own
  async resources.
- **Declarative state persistence (`[PersistentState]`)** — public properties marked
  `[PersistentState]` are auto-persisted via `PersistentComponentState` during prerendering,
  removing the manual `RegisterOnPersisting` boilerplate. Useful for ViewModel-held state in
  prerendered Blazor Web App samples.
- **Not Found handling** (`Router` `NotFoundPage` parameter and
  `NavigationManager`-based Not Found responses) and **Blazor script served as a static web
  asset** — general improvements worth knowing when updating the Blazor samples to `net10.0`.

When bumping the Blazor sample/template projects, also review the version-conditioned
`Microsoft.AspNetCore.Components.*` package versions in `Directory.Packages.props`
(`10.0.0` entries are gated on `'$(TargetFramework)' == 'net10.0'`).

---

## Adoption caveats (summary)

- The core libraries are **C# 12 today** (`LangVersion 12.0`) and do **not** target `net10.0`.
  Raising `LangVersion` is required before any C# 14 syntax compiles.
- Multi-targeting to `netstandard2.0`/`net472` means **runtime/BCL-gated** features must be
  `#if NET10_0_OR_GREATER`-guarded; language-only features are fine across TFMs.
- This is a published NuGet library — preserve existing public/extension API signatures for
  source and binary compatibility; prefer **adding** members over changing them.

## References (official docs)

- What's new in C# 14: <https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-14>
- Extension members tutorial: <https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods>
- The `field` keyword: <https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/field>
- Null-conditional assignment: <https://learn.microsoft.com/dotnet/csharp/language-reference/operators/member-access-operators>
- First-class spans: <https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-14.0/first-class-span-types>
- What's new in .NET 10: <https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/overview>
- What's new in ASP.NET Core in .NET 10: <https://learn.microsoft.com/aspnet/core/release-notes/aspnetcore-10.0>
