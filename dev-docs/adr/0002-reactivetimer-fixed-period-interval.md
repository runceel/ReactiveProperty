# 0002. ReactiveTimer uses a fixed-period ITimer; Interval changes take effect on next Start

- **Status:** Accepted
- **Date:** 2026-06-21
- **Deciders:** runceel, Copilot agent

## Context

The legacy `Reactive.Bindings.ReactiveTimer` (System.Reactive-based) implemented ticking via
a recursive `Observable.Timer` schedule:

```csharp
self(Interval)   // reads Interval fresh on every tick
```

This meant that changing `Interval` mid-run took effect immediately on the next tick.

The R3 bridge implementation (`Reactive.Bindings.R3.ReactiveTimer`) uses
`TimeProvider.CreateTimer(callback, null, dueTime, period)` where `period` is captured once
at `Start()`. Two competing design goals apply:

1. **Behavioral parity (§DesignPrinciple 5)** — changing `Interval` while running should
   ideally take effect on the next tick, matching the legacy behavior.
2. **Deterministic testability** — `TimeProvider`/`FakeTimeProvider` works best when the
   full tick schedule is fixed at `Start()`. Per-tick rescheduling requires disposing and
   recreating the `ITimer` inside the callback, which introduces synchronization complexity
   and makes `FakeTimeProvider`-based tests harder to reason about.

## Decision

We will use a **single periodic `ITimer` with a fixed `period = Interval` captured at
`Start()`**. Changing `Interval` after `Start()` is valid (it raises `PropertyChanged`) but
the new value only takes effect on the next `Start()` call.

This is a **known, intentional parity gap** documented here.

### Alternatives considered

- **Option A — re-schedule on every tick (parity-exact):** Inside the `OnTick` callback,
  dispose the old timer and create a new one with the current `Interval`. Achieves full
  parity but introduces extra complexity (callback ↔ timer lifecycle), increases allocation
  pressure on each tick, and makes `FakeTimeProvider`-based testing less deterministic.
  Rejected.

- **Option B — `ITimer.Change(...)` on Interval set:** Call `_timer.Change(dueTime,
  newInterval)` inside the `Interval` setter when a timer is active. Complex to implement
  safely (race between the setter and a concurrent `Start`/`Stop`), and not tested by
  `FakeTimeProvider`. Rejected.

- **Option C (chosen) — fixed period, next-`Start` semantics:** Simple, allocation-free,
  fully deterministic under `FakeTimeProvider`. Mid-run `Interval` change is uncommon in
  practice.

## Consequences

**Positive:**
- Implementation is simple and the tick schedule is fully deterministic under `FakeTimeProvider`.
- No per-tick allocations.

**Trade-off / risk:**
- Changing `Interval` while the timer is running has no immediate effect; callers who rely
  on per-tick interval adjustment must call `Stop()` + `Start()` to apply the new period.
- This is a subtle behavioral difference from the legacy `ReactiveTimer`. Consumers
  migrating from the legacy library should be aware (migration guide should mention this).
