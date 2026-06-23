# 0004. Provide R3-targeting EventToReactive trigger actions in a new ReactiveProperty.R3.WPF package

- **Status:** Accepted
- **Date:** 2026-06-22
- **Deciders:** runceel, Copilot agent

## Context

`ReactiveProperty.WPF` exposes `EventToReactiveCommand` and `EventToReactiveProperty` as
`TriggerAction<FrameworkElement>` implementations. They route a routed event through an
`IEventToReactiveConverter` chain and then update a `Reactive.Bindings.IReactiveProperty` or execute an
`ICommand`. These trigger actions are a core part of the WPF MVVM story.

When migrating a WPF app to [R3](https://github.com/Cysharp/R3) (see
[ADR 0003](./0003-reactiveproperty-r3-migration-bridge.md)), the view model swaps
`ReactiveProperty<T>`/`ReactiveCommand` for R3's `BindableReactiveProperty<T>`/`ReactiveCommand`. The
existing trigger actions cannot bind to those: R3's `BindableReactiveProperty<T>` implements R3's own
`R3.IBindableReactiveProperty`, **not** `Reactive.Bindings.IReactiveProperty`, and the classic helpers
schedule on `ReactivePropertyScheduler.Default` rather than R3's synchronization-context model. As a
result the migrated WPF sample had to re-implement these features with plain code-behind handlers,
which is exactly the boilerplate the trigger actions exist to remove.

## Decision

Introduce a new package, **`ReactiveProperty.R3.WPF`** (namespace
`Reactive.Bindings.R3.Interactivity`, assembly `ReactiveProperty.R3.WPF`,
TFMs `net8.0-windows;net9.0-windows;net10.0-windows`), that provides R3-native equivalents of the
classic trigger actions:

- `EventToReactiveProperty : TriggerAction<FrameworkElement>` — updates an
  `R3.IBindableReactiveProperty` and keeps `IgnoreEventArgs`.
- `EventToReactiveCommand : TriggerAction<FrameworkElement>` — executes an `ICommand` and keeps
  `IgnoreEventArgs`, `AutoEnable`, and `CallExecuteOnScheduler`.
- `IEventToReactiveConverter` / `ReactiveConverter<T, U>` — converter contracts mirroring the classic
  shapes but built on R3's `Observable<object?>` instead of `System.Reactive`.

The helpers keep the **same class names and XAML usage** as the classic ones (under
`Interaction.Triggers`/`EventTrigger` with the converter as content), so migrating XAML is just
swapping the `xmlns` assembly/namespace. The package depends only on `R3` and
`Microsoft.Xaml.Behaviors.Wpf`; it does not reference `System.Reactive` or `ReactiveProperty.WPF`.
UI marshaling uses `ObserveOnCurrentSynchronizationContext()` (the WPF dispatcher context) instead of
a global scheduler. The migrated R3 WPF sample is updated to use these helpers and drops its
code-behind handlers.

The classic `ReactiveProperty.WPF` trigger actions are left unchanged; no `Behavior<T>`-based variants
are added.

## Consequences

**Positive:**

- WPF apps using R3 view models can keep the familiar EventToReactive XAML with no code-behind.
- Migration from `ReactiveProperty.WPF` to R3 becomes a near-mechanical `xmlns` swap.
- The R3 package stays free of `System.Reactive`, matching the `ReactiveProperty.R3` bridge direction.

**Trade-off / risk:**

- A new package must be built, packed, and maintained.
- The converter contracts are intentionally duplicated (R3 `Observable<>` vs `System.Reactive`),
  so the two packages share names but not types.
