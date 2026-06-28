---
name: migrating-reactiveproperty-to-r3
description: 'Migrate an application from ReactiveProperty (namespace Reactive.Bindings) to R3 plus the ReactiveProperty.R3 bridge package. Use this when asked to "migrate to R3", "move off ReactiveProperty", "replace ReactiveProperty with R3", "swap Reactive.Bindings for R3", or to rewrite ViewModels/commands/validation/notifiers/collections that use ReactiveProperty so they run on R3. Drives the rewrite from a mapping table (references/rules.json): everything R3 already provides becomes native R3, every genuine gap becomes a ReactiveProperty.R3 type, and the few cases that cannot be rewritten mechanically are flagged for manual review.'
---
# Migrating ReactiveProperty to R3

Rewrite a project that uses **ReactiveProperty** (root namespace `Reactive.Bindings`) so it runs on
**[R3](https://github.com/Cysharp/R3)**, using the **`ReactiveProperty.R3`** bridge package for the
features R3 does not provide.

This skill is **execution-centric**: rewrite the code from the mapping table first, then surface only
the spots that still need a human decision. R3 is the destination; `ReactiveProperty.R3` is a single,
permanent bridge package that fills the genuine gaps under familiar names. Code may keep depending on
`ReactiveProperty.R3` indefinitely.

## When to use
- A project references `ReactiveProperty`, `ReactiveProperty.Core`, `ReactiveProperty.WPF`, or
  `ReactiveProperty.Blazor` and you are asked to move it to R3.

## When not to use
- Brand-new code with no ReactiveProperty usage — just use R3 directly.
- Working *on* the ReactiveProperty repository itself (see that repo's `AGENTS.md`).

## The mapping table
All rewrites are driven by [`references/rules.json`](references/rules.json) (`schemaVersion` 2). Each
rule has these fields and nothing else:

| Field | Meaning |
|---|---|
| `ruleId` | Stable identifier for the rule. |
| `symbol` | The ReactiveProperty type or member the rule matches (fully qualified). |
| `matchKind` | `TypeReference`, `MethodInvocation`, `PropertyReference`, or `ObjectCreation`. |
| `target` | `r3-direct`, `reactiveproperty-r3`, or `manualReview`. |
| `replacement` | The fully-qualified replacement symbol (`null` for `manualReview`). |
| `usingAdd` | Namespaces to add. |
| `usingRemove` | Namespaces to remove (only when nothing else in the file still needs them). |
| `manualReview` | A note to surface to the user (string), or `null`. |
| `notes` | Short guidance for applying the rule. |

> **Reading `references/rules.json`:** this file ships with the skill and is **not** secret or
> policy-excluded. If your structured file-view tool denies it because the skill is installed outside
> the current project (e.g. under `~/.copilot/skills/`), read it another way — a shell/Node/Python
> file read, or by adding its directory to the allowed paths. Do **not** report it as a
> content-exclusion or policy block, and do not skip the table.

`target` decides the rewrite:
- **`r3-direct`** — R3 already provides this (often under a different name). Rewrite to the native R3
  API and switch the `using` to `R3`. Examples: `ReactivePropertySlim<T>` → R3 `ReactiveProperty<T>`,
  `ObserveProperty(x => x.P)` → R3 `ObservePropertyChanged(x => x.P)`.
- **`reactiveproperty-r3`** — a genuine gap. Rewrite to the `ReactiveProperty.R3` type, which mirrors
  ReactiveProperty's name and behavior but exposes R3 shapes (returns R3 `Observable<T>`, uses
  `TimeProvider`/`SynchronizationContext` instead of `IScheduler`). Examples: `BusyNotifier`,
  `ReactiveTimer`, `AsyncReactiveCommand`, `ReadOnlyReactiveCollection<T>`, the collection-changed and
  `INotifyDataErrorInfo` extensions, `ValidatableReactiveProperty<T>`.
- **`manualReview`** — cannot be rewritten mechanically. Leave the code as-is and report it with file,
  line, and the rule's `manualReview` note. Do **not** guess.

## Steps

### 1. Fix the package references
- **Add** `R3` and `ReactiveProperty.R3`.
- If the project uses the WPF `EventToReactiveProperty` / `EventToReactiveCommand` trigger actions
  (or custom `ReactiveConverter<T, U>` converters for them), also **add** `ReactiveProperty.R3.WPF`.
  Their R3 versions live in that package, **not** in `ReactiveProperty.R3`.
- **Remove** `ReactiveProperty`, `ReactiveProperty.Core`, `ReactiveProperty.WPF`,
  `ReactiveProperty.Blazor`, and `ReactiveProperty.UWP`.
- Use the project's existing mechanism (NuGet Central Package Management if present, otherwise
  per-project `PackageReference`). Do not pin a version that conflicts with the repo's convention.

### 2. Rewrite symbols from the rules
For every ReactiveProperty symbol in the project, find its rule in `references/rules.json` and apply it:
- `r3-direct` → rewrite to the R3 `replacement`.
- `reactiveproperty-r3` → rewrite to the `ReactiveProperty.R3` `replacement`.
- `manualReview` → leave it and record it for step 5.

Generic type rules apply to the closed forms too — e.g. the rule for
`Reactive.Bindings.ReactivePropertySlim<T>` covers `ReactivePropertySlim<int>`.

### 3. Fix the `using` directives
- Apply each applied rule's `usingAdd` / `usingRemove`. Only remove a namespace when no remaining code
  in the file still needs it.
- Because the `reactiveproperty-r3` gap types expose **R3** observables, add `using R3;` wherever the
  rewritten code subscribes or chains R3 operators (`Subscribe`, `Where`, `Select`, `CombineLatest`,
  …). When in doubt, add `using R3;` to files that touch any rewritten symbol.
- `Reactive.Bindings*` namespaces map to `R3` and/or `Reactive.Bindings.R3*` per the rules.

### 4. Audit WPF/XAML bindings
- In WPF projects, search XAML for `{Binding Xxx.Value}` and `ItemsSource="{Binding Xxx.Value}"` after
  the mechanical rewrite. Any `Xxx` property that is bound through `.Value` must raise WPF
  `PropertyChanged` for `Value` changes.
- For UI-bound read/write properties, prefer `BindableReactiveProperty<T>` instead of
  `ReactiveProperty<T>` even when the original type was `ReactivePropertySlim<T>`.
- For UI-bound read-only properties, create them with `ToReadOnlyBindableReactiveProperty(...)` and
  expose them as `IReadOnlyBindableReactiveProperty<T>`; do **not** expose the internal concrete
  `ReadOnlyBindableReactiveProperty<T>` type. Use `ReadOnlyReactiveProperty<T>` only for non-XAML
  code paths.
- Build success alone is not enough for WPF migration: verify every XAML `.Value` binding is backed by
  a bindable R3 property or interface.

### 5. Build, test, and report
- Build the project and run its **existing** tests.
- Report: build status, test status, and the **manual-review list** — each `manualReview` hit with its
  file, line, and the rule's `manualReview` note. Keep this list concise; it is the only narrative
  output. Common manual-review cases are narrow: a custom `IScheduler` argument that is neither the UI
  dispatcher nor the default (`RP-MANUAL-SCHEDULER`), a `ReactivePropertyMode` argument that has no R3
  equivalent (`RP-MANUAL-MODE`), a `System.IObservable<T>` boundary that must not be retyped
  mechanically (`RP-OBSERVABLE-INTEROP`), and any `Reactive.Bindings*` API not covered by
  a rule (default fallback — flag it with file/line and a one-line description; do **not** guess a
  replacement; e.g. WPF/Blazor platform helpers like `BindTo`, `ReactiveCollection<T>`, or the
  Blazor `ValidationMessage` integration).

## Notes
- Prefer the R3 shape end-to-end. Use `ToObservable()` / `AsSystemObservable()` only at boundaries
  that still require `System.Reactive`.
- Element-property observation (`ObserveElementProperty`) is a `reactiveproperty-r3` rewrite, not manual
  review. Nested property paths are **not** supported as a single selector: the bridge's
  `ToReactivePropertyAsSynchronized` rejects `x => x.A.B` at runtime (`ArgumentException`) and only
  accepts chained single-hop selectors (`x => x.A, a => a.B`). Rewrite any nested path into that chain form.
- DataAnnotations validation on a single property is a direct `EnableValidation()` rewrite; only
  stream/async validation and observable error streams need `ValidatableReactiveProperty<T>` from
  `ReactiveProperty.R3`.
- The WPF `EventToReactiveProperty` / `EventToReactiveCommand` trigger actions have R3 versions in
  the **`ReactiveProperty.R3.WPF`** package (`Reactive.Bindings.R3.Interactivity`). They are normally
  referenced from XAML inside a `Microsoft.Xaml.Behaviors` `EventTrigger`, so the rewrite is mainly an
  xmlns swap: `clr-namespace:Reactive.Bindings.Interactivity;assembly=ReactiveProperty.WPF` →
  `clr-namespace:Reactive.Bindings.R3.Interactivity;assembly=ReactiveProperty.R3.WPF`. The bound
  `ReactiveProperty`/`Command` are the already-migrated R3 `BindableReactiveProperty<T>` / R3
  `ReactiveCommand`. Custom `ReactiveConverter<T, U>` subclasses keep the same shape, but `OnConvert`
  now takes/returns R3 `Observable<T?>`/`Observable<U?>` instead of `System.Reactive` observables —
  rewrite the body with R3 operators and add `using R3;`.
