# 0009. Cache property-path accessor adapters

- **Status:** Proposed
- **Date:** 2026-11-17
- **Deciders:** ReactiveProperty maintainers

## Context

Core and Legacy property paths invoke cached typed property delegates through
`DynamicInvoke`, allocating argument arrays on repeated reads and writes. These
paths also rely on reflection's argument handling and target-exception wrapping.

## Decision

Cache object-based getter and setter adapters per existing accessor delegate.
For compatible, non-null arguments, adapters invoke the typed delegate directly
and wrap target exceptions in `TargetInvocationException`. For other arguments,
use `DynamicInvoke` to preserve its conversions and failures. Both path
implementations share the adapters; public and protected APIs stay unchanged.

### Alternatives considered

- **Continue using `DynamicInvoke`** — preserves behavior but retains repeated
  reflection invocation and allocations.
- **Change accessor APIs to expose object delegates** — would affect existing
  extension points and require a wider compatibility change.

## Consequences

Steady-state invocations avoid reflection argument arrays for compatible values,
at the cost of compiling an adapter the first time each accessor is used.
The weak-key cache is thread-safe, although concurrent first use can compile
more than one adapter. Existing property discovery and expression compilation
already rely on reflection and dynamic code; trimming and Native AOT have not
been validated for this change.
