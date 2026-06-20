# dev-docs — Implementer documentation

This folder holds documentation for people working **on** ReactiveProperty
(maintainers and contributors). It is intentionally separate from the user-facing site.

| Folder | Audience | Published? |
|--------|----------|------------|
| [`../docs/`](../docs) | Library **users** (consumers) | Yes — built and published with VuePress |
| `dev-docs/` (this folder) | **Implementers / contributors** | No |

Put "how does a consumer use this feature?" content in `docs/`. Put "how/why is this
implemented, and how do I contribute?" content here.

## Contents

- [`adr/`](./adr) — Architecture Decision Records: the log of design/architecture decisions.

## Development workflow (summary)

The full, authoritative policy is the `development-workflow` skill
(`.agents/skills/development-workflow/SKILL.md`). In short:

1. **TDD, always Red → Green → Refactor.** Write a failing test first, make it pass with the
   minimum code, then refactor with tests green. A change isn't done until
   `dotnet test ReactiveProperty.slnx` passes (on Windows).
2. **Record design decisions as ADRs** under [`adr/`](./adr).
3. **Docs by audience:** `docs/` for users, `dev-docs/` for implementers.

See the repository [`AGENTS.md`](../AGENTS.md) for build/test commands, target frameworks,
central package management, and code style.
