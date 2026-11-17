# 0008. Reuse R3 validator snapshots

- **Status:** Proposed
- **Date:** 2026-11-17
- **Deciders:** ReactiveProperty maintainers

## Context

Each R3 validation currently copies the registered synchronous validators and
rebuilds the aggregated errors even when a validator returns the same errors.
Registration is less frequent than validation, and validators may register
additional validators or initiate validation recursively.

## Decision

Replace the validator array when registering a synchronous validator, and capture
the current array under the existing validation lock before invoking validators.
Keep each snapshot immutable and preserve registration and invocation order.
Skip aggregation only when the newly materialized errors for a slot equal its
previous errors; validators and their returned sequences are still executed.

### Alternatives considered

- Copy the validator list on every validation: simpler registration, but repeated
  allocations on the validation path.
- Read the snapshot without the lock: avoids lock acquisition but changes the
  synchronization point with concurrent registration.

## Consequences

- Validation with unchanged errors avoids repeated snapshot and aggregation work.
- Registration copies the existing validator array and therefore costs more as the
  number of validators increases.
- Sync, async and stream validation retain the existing slot and notification order.
