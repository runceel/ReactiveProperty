# 0003. Ship a permanent minimal `ReactiveProperty.R3` migration bridge and a migration skill

- **Status:** Accepted
- **Date:** 2026-06-21
- **Deciders:** runceel, Copilot agent

## Context

The author now recommends [R3](https://github.com/Cysharp/R3) for brand-new applications, and
ReactiveProperty is in maintenance/active-support mode. Teams with existing ReactiveProperty
(root namespace `Reactive.Bindings`) code want a low-friction path to R3, but R3 deliberately
does not provide some of ReactiveProperty's higher-level MVVM helpers.

The gap analysis (`dev-docs/r3-migration/design.md` §2, against R3 1.3.1 and
ObservableCollections.R3 3.3.4) found two categories of ReactiveProperty surface:

1. **Already covered by R3** under a different name (e.g. `ReactivePropertySlim<T>` →
   R3 `ReactiveProperty<T>`, `ObserveProperty(x => x.P)` → `ObservePropertyChanged(x => x.P)`).
2. **Genuine gaps** with no R3 equivalent: the notifiers (`BooleanNotifier`, `CountNotifier`,
   `BusyNotifier`, `ScheduledNotifier<T>`, `MessageBroker`/`AsyncMessageBroker`),
   `ReactiveTimer`, `AsyncReactiveCommand`, `ReadOnlyReactiveCollection<T>` and the
   collection-changed / filtered-collection extensions, `ValidatableReactiveProperty<T>` and the
   `INotifyDataErrorInfo` extensions, and `ToReactivePropertyAsSynchronized`.

A migration needs both a place for the gap types to live and a repeatable, mechanical procedure
an agent (or a human) can follow.

## Decision

We will ship a single, **permanent** bridge package, **`ReactiveProperty.R3`**, that fills the
genuine R3 gaps under familiar ReactiveProperty names while exposing **R3 shapes** (returning
`R3.Observable<T>`, and using `TimeProvider` / `SynchronizationContext` instead of
`System.Reactive`'s `IScheduler`). Migrated code may depend on `ReactiveProperty.R3`
indefinitely; it is not a temporary shim. We will also ship a published migration skill,
`skills/migrating-reactiveproperty-to-r3`, that drives the rewrite from a mapping table
(`references/rules.json`): every R3-covered symbol becomes native R3, every gap symbol becomes a
`ReactiveProperty.R3` type, and the few cases that cannot be rewritten mechanically are flagged
for manual review.

Three sub-decisions are confirmed here (see `design.md` §9):

- **Namespace (§3.1).** The bridge uses the root namespace **`Reactive.Bindings.R3`** (with
  `.Notifiers` and `.Extensions` sub-namespaces), *not* the original `Reactive.Bindings`. The
  `.R3` suffix is self-documenting and avoids `CS0433` ambiguous-type errors if a project
  transiently references both `ReactiveProperty` and `ReactiveProperty.R3`.
- **Build target (§3.0).** The repository moves to **.NET 10 + C# 14**: `LangVersion` is raised
  to `14.0` centrally in `Source/Directory.Build.props` (explicit, so C# 14 syntax compiles even
  on `netstandard2.0` / `net472`). `ReactiveProperty.R3` targets
  `netstandard2.0;net8.0;net9.0;net10.0` and depends on `R3` (+ `ObservableCollections.R3`) via
  Central Package Management.
- **Collection UI dispatch (§4.4).** Every `ReadOnlyReactiveCollection` factory and constructor
  takes an optional `SynchronizationContext? raiseEventContext = null`; a non-null context
  `Post`s collection-changed events to it, and `null` raises them synchronously. This replaces
  ReactiveProperty's `IScheduler`-based UI dispatch.

### Alternatives considered

- **Reuse the original namespaces** (`Reactive.Bindings`, `…Notifiers`, `…Extensions`) so
  gap-type migration needs zero `using` changes. Rejected: types still return R3
  `Observable<T>` (so downstream operator calls need `using R3;` anyway), and a project that
  transiently references both packages gets `CS0433` ambiguous-type errors.
- **No bridge — rewrite the gaps inline per project.** Rejected: it duplicates non-trivial,
  bug-prone logic (validation aggregation, collection synchronization, shared-can-execute
  commands) in every consuming codebase, with no shared test coverage.
- **A temporary "shim" framed as throwaway.** Rejected: several gaps (notifiers, validation,
  collections) are genuine missing features, not transitional scaffolding; framing the package
  as permanent sets correct expectations.

## Consequences

**Positive:**
- Existing ReactiveProperty apps have a mechanical, mostly-automatable path to R3, with the
  genuine gaps covered by a maintained, tested package instead of hand-rolled per-project code.
- The `.R3` namespace keeps both libraries usable side-by-side during an incremental migration.
- The skill's `rules.json` gives agents a single source of truth for the mapping, and the
  behavior-template test matrix (§6) protects parity.

**Trade-off / risk:**
- `ReactiveProperty.R3` is a new package and assembly to maintain and ship alongside the
  existing packages.
- Gap types expose R3 shapes rather than `System.Reactive`, so migrated code that crosses a
  `System.Reactive` boundary must convert explicitly (`ToObservable()` / `AsSystemObservable()`).
- Raising `LangVersion` to `14.0` repo-wide means C# 14 syntax can appear on older target
  frameworks; runtime-dependent C# 14 features must still be guarded per the
  `dotnet10-features` guidance.
- A few constructs cannot be rewritten mechanically (custom `IScheduler`, `ReactivePropertyMode`,
  `System.IObservable<T>` interop boundaries, and platform helpers such as
  `EventToReactiveCommand`); these are surfaced as manual-review items rather than guessed.

## See also

- [Skill evaluation](../r3-migration/skill-evaluation.md) — end-to-end validation of the
  migration skill (3 findings incl. the fixed YAML frontmatter bug, plus 51-rule conformance).
