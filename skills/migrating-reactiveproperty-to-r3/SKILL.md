---
name: migrating-reactiveproperty-to-r3
description: Analyze and migrate applications from ReactiveProperty to R3 incrementally. Use when a user mentions migrating ReactiveProperty, ReactiveCommand, ValidatableReactiveProperty, SetValidateNotifyError, ReadOnlyReactiveCollection, or ReactivePropertyMode to R3.
---
# Migrating ReactiveProperty to R3

Use this skill to move ReactiveProperty applications toward native R3 while keeping temporary compatibility usage explicit and measurable.

## Phases

1. **Scan** target projects and produce `inventory.json` with ReactiveProperty symbol usage sites.
2. **Classify** each usage with `references/rules.json` into `auto`, `compat`, or `manual` and produce `classification.json`.
3. **Transform** only `autoFix=true` rules. Do not automatically rewrite public API signatures or validation/command/scheduler behavior.
4. **Validate** with the app's existing build and test commands and produce `verify.json`.
5. **Report** unhandled/manual differences, compatibility usage, downgrade opportunities, and R3 native adoption in `migration-report.md` plus `compat-usage.json`.

## Guardrails

- Prefer native R3 for simple renames or mechanical conversions.
- Use `Reactive.Bindings.R3Compat.*` only when a rule has `requiresCompatibilityLayer=true`.
- Every compatibility usage must retain its `downgradeWhen` and `deprecation` note in the migration report.
- Treat validation streams, async command re-entrancy/shared CanExecute, collection reset/dispose behavior, and scheduler mode differences as test-required areas.
- The migration goal is always to reduce compatibility references to zero.

## Compatibility packages

- `ReactiveProperty.R3.Compatibility.Core` — configuration, telemetry, and minimal shared state.
- `ReactiveProperty.R3.Compatibility.Validation` — `CompatValidatableProperty<T>` for stream/DataAnnotations validation blockers.
- `ReactiveProperty.R3.Compatibility.Commands` — `AsyncReactiveCommandCompat<T>` and `ReactiveCommandSlimCompat<T>`.
- `ReactiveProperty.R3.Compatibility.Collections` — minimal `ReadOnlyReactiveCollectionCompat<T>`.

## Reporting checklist

Include these sections in `migration-report.md`:

- R3 native adoption rate.
- Counts by category: auto, compat, manual.
- Compatibility APIs used, with `downgradeWhen`, `downgradeTarget`, and `deprecation`.
- Required tests that were generated or matched.
- Unhandled differences that need manual review.
