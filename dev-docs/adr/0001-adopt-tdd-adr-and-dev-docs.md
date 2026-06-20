# 0001. Adopt TDD, ADRs, and a dev-docs location

- **Status:** Accepted
- **Date:** 2026-06-20
- **Deciders:** ReactiveProperty maintainers

## Context

ReactiveProperty is a long-lived MVVM/Rx library shipped as several NuGet packages and
targeting a wide range of frameworks (`netstandard2.0`, `net472`, and current .NET). To keep
quality high and the design history understandable as the project evolves, we want explicit,
written conventions for how changes are made and how decisions are captured.

Three gaps existed:

1. No stated testing discipline for changes to library code.
2. No durable record of *why* design/architecture decisions were made.
3. Only one documentation location (`docs/`), which is the user-facing site published with
   VuePress. There was nowhere obvious to put documentation aimed at implementers and
   contributors, and putting it under `docs/` risks publishing internal notes.

## Decision

We will adopt the following development conventions, codified in the
`development-workflow` skill (`.agents/skills/development-workflow/SKILL.md`):

1. **Test-Driven Development is mandatory.** All changes to `Source/` follow a strict
   Red → Green → Refactor cycle: write a failing test first, write the minimum production
   code to pass, then refactor with tests green. A change is not done until
   `dotnet test ReactiveProperty.slnx` passes.
2. **Design decisions are recorded as ADRs** in `dev-docs/adr/`, using a lightweight
   Nygard-style template, one file per decision, numbered sequentially.
3. **Documentation is split by audience.** `docs/` remains user-facing (published via
   VuePress). A new top-level **`dev-docs/`** folder holds implementer/contributor
   documentation (including ADRs) and is not published.

### Alternatives considered

- **Implementer docs under `docs/` (e.g. `docs/contributing/`)** — rejected: `docs/` is the
  VuePress build root, so internal docs could be published unintentionally and would mix
  audiences.
- **`eng/docs/` or `docs-dev/`** — viable, but `dev-docs/` was chosen as the clearest
  top-level, audience-named location that is unambiguously separate from the published site.
- **No formal decision log (rely on PR/commit history)** — rejected: decision rationale is
  hard to find later and easily lost in noise.

## Consequences

- Contributors and AI agents have a single, discoverable policy (the `development-workflow`
  skill) and a home for internal docs (`dev-docs/`).
- Every significant future decision should add an ADR; this is light overhead per decision
  but yields a durable, reviewable design history.
- TDD becomes the expected default for library changes, improving regression safety across
  the many target frameworks.
- `dev-docs/` is excluded from the published site by virtue of living outside `docs/`; no
  VuePress configuration change is required.
