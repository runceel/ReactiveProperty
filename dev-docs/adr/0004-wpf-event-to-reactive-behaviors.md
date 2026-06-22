# 0004. Add WPF behavior-based EventToReactive helpers

- **Status:** Accepted
- **Date:** 2026-06-22
- **Deciders:** runceel, Copilot agent

## Context

ReactiveProperty.WPF already exposes `EventToReactiveCommand` and `EventToReactiveProperty` as
`TriggerAction<FrameworkElement>` implementations. They work with
`Interaction.Triggers`/`EventTrigger`, but WPF migration scenarios for the R3 bridge also need a
syntax that can be placed directly under `Interaction.Behaviors` while keeping the existing event
conversion semantics.

## Decision

Add `EventToReactiveCommandBehavior` and `EventToReactivePropertyBehavior` to the existing
`ReactiveProperty.WPF` package. The new helpers listen to an `EventName` on their associated WPF
`FrameworkElement`, feed the event args through the same `IEventToReactiveConverter` chain, and then
execute the bound `ICommand` or update the bound `IReactiveProperty`. The command behavior keeps the
existing `IgnoreEventArgs`, `AutoEnable`, and `CallExecuteOnScheduler` options.

No new package is introduced; UWP keeps using the existing trigger-action helpers.

## Consequences

**Positive:**

- WPF XAML can use behavior-based event bridging without re-implementing code-behind during
  migration work.
- The implementation stays in `ReactiveProperty.WPF` and reuses the existing converter and scheduler
  abstractions.

**Trade-off / risk:**

- The new behavior helpers are WPF-only and do not change the UWP interactivity surface.
- R3-native property types still need migration review unless they are exposed through a compatible
  property/command shape.
