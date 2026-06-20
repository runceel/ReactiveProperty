---
name: development-workflow
description: ReactiveProperty repository development policy. Use whenever you implement a feature or bug fix, change library or test code under Source/ or Test/, make a design or architecture decision, or write documentation in this repo. Enforces strict Test-Driven Development (Red -> Green -> Refactor) with MSTest and `dotnet test ReactiveProperty.slnx`, requires recording design decisions as ADRs under dev-docs/adr/, and defines the documentation split: docs/ is user-facing (published via VuePress) while dev-docs/ holds implementer/contributor documentation. Triggers include "implement", "add feature", "fix bug", "TDD", "red green refactor", "design decision", "architecture", "ADR", "where do docs go", and "contributor docs".
---
# Development Workflow (ReactiveProperty)

The required development workflow and documentation policy for this repository.

## When to Use
- Implementing any feature or bug fix in `Source/` or `Test/`.
- Making a design or architecture decision (public API shape, threading model, new package, dependency, breaking change).
- Deciding where a piece of documentation belongs.

## When Not to Use
- Pure release/versioning mechanics (see `AGENTS.md` and the build/publish workflow).
- One-off exploration or read-only investigation with no code change.

---

## 1. TDD is mandatory — Red → Green → Refactor

Every code change to `Source/` must be driven by a failing test first. Follow the cycle
strictly and do not skip steps:

1. **Red** — Write the smallest test that expresses the desired behavior, then run it and
   **watch it fail**. The failure must be for the right reason (asserting the new behavior,
   not a compile error you didn't expect).
   ```pwsh
   dotnet test ReactiveProperty.slnx
   ```
   Narrow the loop while iterating, e.g. a single project or filter:
   ```pwsh
   dotnet test Test/ReactiveProperty.NETStandard.Tests/ReactiveProperty.NETStandard.Tests.csproj --filter "FullyQualifiedName~YourNewTest"
   ```
2. **Green** — Write the **minimum** production code to make the test pass. Re-run the
   tests and confirm they are green. Don't add unrequested functionality.
3. **Refactor** — With tests green, clean up production and test code (naming, duplication,
   structure). Re-run tests after refactoring; they must stay green. Behavior must not change.

Rules:
- Never write production code without a failing test that requires it.
- Keep each Red→Green→Refactor cycle small and focused on one behavior.
- A change is not "done" until `dotnet test ReactiveProperty.slnx` passes on Windows
  (WPF/UWP projects require Windows).

### Test conventions (this repo)
- Framework: **MSTest** (`MSTest.TestAdapter` / `MSTest.TestFramework`).
- Helpers available: **Moq**, **Microsoft.Reactive.Testing** (`TestScheduler` for time-based
  Rx), the bundled `ChainingAssertion` (`actual.Is(expected)` fluent assertions); Blazor
  tests use **bunit**.
- Put tests in the matching `Test/*` project; the core test project uses namespace
  `ReactiveProperty.Tests`.
- Keep new `Source/` code compatible with `netstandard2.0` / `net472` (lowest TFMs).

> See the `writing-mstest-tests` and `run-tests` skills for MSTest authoring details and
> filter/runner syntax.

---

## 2. Record design decisions as ADRs

Going forward, every non-trivial **design / architecture decision** is recorded as an
Architecture Decision Record (ADR).

**Write an ADR when** you:
- add or remove a public API surface, or change its semantics;
- add a new package/project or a new external dependency;
- change threading/scheduling, validation, or serialization behavior;
- make a breaking change or deprecate something;
- choose between competing implementation approaches with long-term impact.

**How:**
- ADRs live in **`dev-docs/adr/`**, one file per decision.
- Copy `dev-docs/adr/template.md`, name it `NNNN-short-title.md` with the next zero-padded
  number (e.g. `0002-...`), and fill in **Status, Context, Decision, Consequences**.
- New ADRs start as `Status: Proposed`; mark `Accepted` once agreed. Don't edit accepted
  ADRs to reverse a decision — add a new ADR that supersedes the old one and update both
  `Status` lines.
- Add the new ADR to the index in `dev-docs/adr/README.md`.

Trivial, reversible choices (local refactors, naming) do **not** need an ADR.

---

## 3. Documentation split: `docs/` vs `dev-docs/`

- **`docs/` = user-facing** documentation, published as the VuePress site
  (`npm run docs:build`). Write here for library *consumers*: getting-started, API usage,
  samples. Available in English and Japanese (`*-ja`).
- **`dev-docs/` = implementer/contributor-facing** documentation, **not** published.
  Write here for people working *on* ReactiveProperty: architecture notes, internal design,
  contributor workflow, and ADRs (`dev-docs/adr/`).

When you add documentation, pick the audience first:
- "How does a consumer use this feature?" → `docs/`.
- "How/why is this implemented, or how do I contribute?" → `dev-docs/`.

---

## Checklist before finishing a change
- [ ] Behavior was driven by a failing test first (Red), then made to pass (Green), then refactored.
- [ ] `dotnet test ReactiveProperty.slnx` passes.
- [ ] New `Source/` code stays compatible with `netstandard2.0` / `net472`.
- [ ] Any design/architecture decision is captured as an ADR in `dev-docs/adr/` and indexed.
- [ ] New docs placed by audience (`docs/` for users, `dev-docs/` for implementers).
