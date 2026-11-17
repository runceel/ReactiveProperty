# 0007. Support .NET 11 and retire .NET 8 and .NET 9 target legs

- **Status:** Accepted
- **Date:** 2026-11-17
- **Deciders:** ReactiveProperty maintainers

## Context

The maintainer approved adding .NET 11 support and retiring the .NET 8 and .NET 9
package target legs and test coverage in issue #570. Existing .NET Standard 2.0,
.NET Framework 4.7.2, and .NET 10 assets must remain available. Current contributor
and package guidance no longer matches the projects or the configured C# 14 compiler.

## Decision

We will ship explicit .NET 10 and .NET 11 targets in all six packages in the main
solution, retaining each package's existing .NET Standard 2.0 and .NET Framework
4.7.2 assets where present. WPF targets remain Windows-specific. We will not raise
`LangVersion` beyond C# 14 or change supported-platform runtime semantics.

Automated library and R3 sample tests will run on .NET 10 and .NET 11. The core
test project will run its target frameworks serially: concurrent test hosts
reproducibly exceed the existing large-collection test timeouts. CI will install
both runtimes, use the .NET 11 SDK, build all package targets in Release, and
retain the existing default Debug configuration for automated tests.

The migration assessment uses the migration skill's .NET 11 Preview 3 compiler,
core library, SDK/MSBuild, and ASP.NET Core references. The scanned code does not
use the affected compression/archive, pipe, calendar, cryptography, background
service, OpenAPI, EF Core, or Blazor Virtualize APIs. The new SDK's redundant
`Microsoft.CSharp` package warning is resolved in modern WPF test projects.

Retired platform removal is a separate issue (#571), not part of this decision.
Historical release notes, accepted ADRs, and existing platform code and tutorials
remain unchanged here.

### Alternatives considered

- Keep all old framework legs: rejected after the maintainer explicitly approved
  dropping the out-of-support .NET 8 and .NET 9 legs.
- Remove compatibility assets as well: rejected because preserving existing
  .NET Standard and .NET Framework consumers is required.
- Combine platform removal with this migration: rejected in favor of a separate
  issue and pull request.

## Consequences

Consumers receive native .NET 11 assets, and CI exercises both supported modern
runtimes without losing legacy compatibility builds. Existing .NET 8/9 consumers
must upgrade for a supported modern runtime; some may still resolve the retained
.NET Standard assets, but no dedicated .NET 8/9 binaries or tests are shipped.

C# 14 and compatibility targets still constrain which BCL APIs can be used in
shared code.

The supplied migration references cover changes through Preview 3 only, even
though validation uses the installed .NET 11.0.100 SDK. Passing builds and tests
cannot establish coverage of every later runtime change or external deployment's
hardware requirements.
