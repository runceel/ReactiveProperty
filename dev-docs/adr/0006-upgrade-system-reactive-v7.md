# 0006. Upgrade the System.Reactive dependency to v7

- **Status:** Accepted
- **Date:** 2026-08-14
- **Deciders:** ReactiveProperty maintainers

## Context

ReactiveProperty currently depends on System.Reactive 6.1.0 and Microsoft.Reactive.Testing
6.1.0. Rx.NET v7 is a major release that removes UI-framework-specific reference assemblies
from the core package, moves those integrations to platform-specific packages, and drops
out-of-support .NET 6 and .NET 7 targets. ReactiveProperty's supported targets are
netstandard2.0, net472, net8.0, net9.0, and net10.0, and its WPF scheduler uses the common
`IScheduler` and `SynchronizationContextScheduler` APIs rather than Rx.NET's WPF scheduler.

## Decision

We will upgrade the centrally managed `System.Reactive` and `Microsoft.Reactive.Testing`
packages to 7.0.0. We will keep the existing target frameworks and public ReactiveProperty
APIs. We will not add a platform-specific Rx.NET package unless a project uses a UI-specific
Rx.NET API; the current WPF integration does not do so. We will also remove the obsolete
`System.Reactive.PlatformServices` using directive from the main library.

### Alternatives considered

- **Remain on Rx.NET 6.1.0** — avoids migration work, but leaves the library on the previous
  major release and prevents consumers from using the v7 dependency graph consistently.
- **Add all Rx.NET platform packages** — unnecessary for the APIs currently used and would add
  dependencies without providing behavior.

## Consequences

The library and tests use Rx.NET v7 and benefit from its current supported package layout.
Existing target frameworks remain available because Rx.NET v7 supports netstandard2.0 and
net472 in addition to current .NET targets. Consumers using ReactiveProperty's existing WPF
scheduler do not need an extra Rx.NET UI package; applications that use Rx.NET UI-specific
types directly must reference the corresponding platform package themselves.
