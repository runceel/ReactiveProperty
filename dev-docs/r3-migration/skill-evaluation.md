# Skill evaluation — `migrating-reactiveproperty-to-r3`

Maintainer-facing record of how the `migrating-reactiveproperty-to-r3` skill was validated
end-to-end, what it got right, and the three issues the evaluation surfaced. This distills a
longer evaluation run; the full CLI transcripts are **not** committed (they live outside the
repo as evaluation evidence) — one trimmed excerpt is embedded below.

## Method

A real GitHub Copilot CLI (`copilot` v1.0.64) migration of a purpose-built `net10.0-windows`
WPF lab app was driven the way an end user would: scoped, incremental, realistic prompts, no
blanket delegation. The lab kept three snapshots (`01-before`, `02-in-progress`,
`03-completed`) to prove the before / partway / after states, and the migrated output was
checked rule-by-rule against `skills/migrating-reactiveproperty-to-r3/references/rules.json`.

The committed sample under `Samples/ReactivePropertySamples.R3.*` is the shipped equivalent of
the lab's `03-completed`, and `Samples/ReactivePropertySamples.R3.Tests` is the regression net
that locks in the behaviors this evaluation verified by hand.

## Bottom line

A regular user **can** complete the migration with this skill and the output is correct: the
`03-completed` snapshot built clean (0 warnings, 0 errors, including the WPF/XAML app) and all
existing tests passed. In a no-special-flags plan run the agent produced a correct rule-by-rule
plan citing **17 distinct ruleIds** (`RP-PROP-SLIM`, `RP-PROP-BINDING`,
`RP-TO-PROP-AS-SYNCHRONIZED`, `RP-VALIDATABLE-PROP`, `RP-ASYNC-COMMAND`, `RP-NOTIFY-BUSY`,
`RP-MANUAL-MODE`, `RP-MANUAL-SCHEDULER`, `RP-EVENT-TO-COMMAND`, …). Those ruleIds exist **only**
in `references/rules.json` — `SKILL.md` contains none — which is hard proof the rules table, not
just the prose, drove the result. Both genuine manual-review cases were flagged (not guessed).

## Findings

### Finding 1 (shipped bug — FIXED) — invalid YAML frontmatter silently dropped the skill

`SKILL.md` had an **unquoted** YAML `description:` plain scalar whose text contained `: `
(colon-space) in `...(references/rules.json): everything R3 already provides...`. A YAML plain
scalar cannot contain `": "`, so the frontmatter failed to parse and the CLI **silently dropped
the skill** from `<available_skills>` — nothing logged, the skill simply never appeared. Impact:
every consumer of the published skill. The `winui` skill is immune only because it wraps its
whole description in double quotes.

**Fix applied:** single-quote the description (the text has no apostrophes, so the embedded
`"..."` examples are preserved verbatim; only the quoting changed). Verified via the request
payload log (`~/.copilot/logs/process-*.log`): occurrences of `migrating-reactiveproperty` went
**0 → present** after the fix.

**Follow-up worth doing:** lint skill frontmatter with a real YAML parser at publish time, or
document "always quote `description`."

### Finding 2 (mostly install-guidance) — an out-of-tree install makes `view` of `rules.json` fail and get mislabeled

The skill's whole design is "drive the rewrite from `references/rules.json`," so the agent must
be able to read that file. The structured `view`/read tool is scoped to the working tree (plus
any `--add-dir` paths). When the skill is installed in the **personal** `~/.copilot/skills/`, it
lives **outside** the project, so a naïve `view` of its `references/rules.json` is denied — and
the agent narrated that denial as an org "content-exclusion policy," which is simply wrong and
nearly derailed the evaluation.

Controlled probes pinned the cause to install location, not content policy. The decisive one
(Probe E): install the **same** skill in-tree under the project's `.agents/skills/<name>/` —
exactly what `skills/README.md` recommends first — and the `view` returns `VIEW=ok` with **zero
friction**, no shell fallback, no false "content-exclusion." So this is **first** a
documentation/install-guidance issue and only secondarily a skill-robustness gap.

Remediation, in recommended order:

1. **(Primary) Install in-tree under `.agents/skills/<name>/`.** `rules.json` is then inside the
   working tree and `view` reads it directly. Matches `skills/README.md`. *Downside:* per-project
   copy.
2. **(If a personal/global install is preferred) Harden `SKILL.md`.** Tell the agent that
   `references/rules.json` is part of the installed skill and, if a structured `view` is denied
   because the skill is out-of-tree, to read it via a shell/Node/Python read (or request
   `--add-dir`) and **never** report it as "content-exclusion." *Downside:* relies on the agent
   honoring the instruction; still a shell workaround.
3. **(Last resort) Inline the mapping into `SKILL.md`.** Always in context, never file-scoped, but
   the full table is ≈30 KB / 51 rules and bloats every activation. Inline only a compact subset,
   consciously.

Trimmed transcript excerpt (personal `~/.copilot/skills/` install; the friction this is about):

```text
I'll read the rules file first, then inventory the ViewModels project.
The rules.json read was denied. Let me retry it ...
The skills path is blocked for my direct tools in non-interactive mode.
Now trying PowerShell: ... Now trying Node.js:
Great! The file was successfully read via Node.js.
The agent copied rules.json to an accessible location ... place it inside my working directory
```

### Finding 3 (skill-accuracy nuance) — `RP-VALIDATE-ATTRIBUTE` → `EnableValidation` changes validation timing

`RP-VALIDATE-ATTRIBUTE` maps single-property DataAnnotations
(`prop.SetValidateAttribute(() => x.Name)`) to the `r3-direct`
`BindableReactiveProperty<T>.EnableValidation(() => x.Name)`. Functionally reasonable, but R3's
`EnableValidation` **defers validation until a subscriber / WPF binding attaches**, whereas
ReactiveProperty's `SetValidateAttribute` validates eagerly. A headless test that asserts an
empty value is invalid *immediately* therefore fails after the mechanical rewrite. The agent
detected this with a runtime probe and deviated to the eager-validating
`ValidatableReactiveProperty<T>` (a `reactiveproperty-r3` type), flagging the deviation for human
confirmation.

This is also why `ReactivePropertySamples.R3.Tests/ValidationViewModelTests` asserts validation
on **transitions** (valid → cleared, empty → "Required property") rather than at construction.

**Recommendation:** add a note to `RP-VALIDATE-ATTRIBUTE` (and/or `SKILL.md`) that
`EnableValidation` is lazy, so code/tests relying on eager validation should trigger validation
explicitly or use `ValidatableReactiveProperty<T>`.

## Rule conformance (51 rules)

All 51 rules in `references/rules.json` were read and every migrated symbol in the completed
sample was compared against them. Every symbol is rules-conformant; the only deviation is the
justified `RP-VALIDATE-ATTRIBUTE` → `ValidatableReactiveProperty<T>` swap above. Representative
rows:

| Symbol (before) | Rule | Target | Migrated to |
|---|---|---|---|
| `ReactivePropertySlim<T>` | RP-PROP-SLIM | r3-direct | `R3.ReactiveProperty<T>` |
| `ReactiveProperty<T>` (bound) | RP-PROP-BINDING | r3-direct | `R3.BindableReactiveProperty<T>` |
| `ReactiveCommand` / `<T>` | RP-COMMAND | r3-direct | `R3.ReactiveCommand` / `<T>` |
| `ObserveProperty` | RP-OBSERVE-PROPERTY | r3-direct | `ObservePropertyChanged` |
| `ValidatableReactiveProperty<T>` | RP-VALIDATABLE-PROP | reactiveproperty-r3 | `Reactive.Bindings.R3.ValidatableReactiveProperty<T>` |
| `AsyncReactiveCommand` | RP-ASYNC-COMMAND | reactiveproperty-r3 | `Reactive.Bindings.R3.AsyncReactiveCommand` |
| `ToReactivePropertyAsSynchronized` | RP-TO-PROP-AS-SYNCHRONIZED | reactiveproperty-r3 | bridge `ToReactivePropertyAsSynchronized` |
| `EventToReactiveCommand` (XAML) | RP-EVENT-TO-COMMAND | reactiveproperty-r3 | `Reactive.Bindings.R3.Interactivity` (ReactiveProperty.R3.WPF) |
| `ReactivePropertyMode` arg | RP-MANUAL-MODE | manualReview | dropped + documented (flagged) |
| custom `IScheduler` arg | RP-MANUAL-SCHEDULER | manualReview | dropped + documented (flagged) |
| `SetValidateAttribute` | RP-VALIDATE-ATTRIBUTE | r3-direct | `ValidatableReactiveProperty<T>` (Finding 3) |

## See also

- [ADR-0003 — ReactiveProperty.R3 migration bridge + skill](../adr/0003-reactiveproperty-r3-migration-bridge.md)
- [Detailed design](design.md) — §5 covers the skill and `rules.json` schema.
- User-facing guide: `docs/docs/advanced/r3-migration.md`.
