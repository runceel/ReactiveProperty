---
name: reactiveproperty-maintainer
description: >-
  Repository-specific guidance for maintaining ReactiveProperty source, tests,
  documentation, samples, and release artifacts. Use before making changes in
  the runceel/ReactiveProperty repository.
user-invocable: true
---

# ReactiveProperty maintainer

Use this skill when changing ReactiveProperty source code, tests, samples, documentation, packaging, or release notes.

## Repository orientation

- Main source code is under `Source/`.
- Tests are under `Test/`.
- User documentation is under `docs/docs/` and is built with VuePress from the `docs/` directory.
- Samples are under `Samples/` and snippets are under `Snippet/`.
- The repository contains English and Japanese top-level README and release note files.

## Maintenance guidance

- Preserve public API compatibility unless the task explicitly requires a breaking change.
- Keep target framework and package changes aligned with existing project files.
- Update documentation when behavior, supported platforms, package usage, or samples change.
- For decisions that affect project structure, compatibility, packaging, or repeated maintenance work, add an ADR under `docs/docs/adr/`.
- Prefer small, focused changes and avoid unrelated cleanup.

## Validation guidance

- For documentation-only changes, build the documentation from `docs/` with `npm run docs:build` when practical.
- For source or test changes, run the relevant .NET build and tests for the affected solution or project.
- Scan changed files for secrets before committing.
