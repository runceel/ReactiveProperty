# skills — Agent skills for ReactiveProperty users

This folder holds **published agent skills for people who *use* ReactiveProperty** in their
own applications. They help an AI coding agent write app code with ReactiveProperty —
ViewModels, `ReactiveProperty` / `ReactivePropertySlim`, `ReactiveCommand`,
`ValidatableReactiveProperty`, and Rx composition — following the library's idioms.

> Not to be confused with [`../.agents/skills/`](../.agents/skills), which holds skills for
> working **on** this repository (contributors/maintainers). Skills here are for **consumers**
> of the library and are intentionally **not** auto-loaded when working in this repo.

## How consumers use these

Copy (or install) a skill folder into the agent's skills location for their own project —
for example the project-level `.agents/skills/` of the consuming repository, or a personal
skills directory. Each skill is self-contained.

## Layout

One folder per skill, each with a `SKILL.md`:

```
skills/
  using-reactiveproperty/
    SKILL.md            # name + description frontmatter, then the guidance body
    references/         # optional supporting docs the skill links to
```

## Authoring a skill

`SKILL.md` starts with YAML frontmatter, then Markdown guidance:

```markdown
---
name: using-reactiveproperty
description: <when an agent should use this skill — be specific; this is what triggers it>
---
# Using ReactiveProperty
...guidance, patterns, and examples...
```

Keep the content focused on **consuming** the library (app/ViewModel code), not on building
ReactiveProperty itself. For repo-internal conventions, see [`../AGENTS.md`](../AGENTS.md).
