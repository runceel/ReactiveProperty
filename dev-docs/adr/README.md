# Architecture Decision Records (ADRs)

This directory records the significant design and architecture decisions for
ReactiveProperty. Each ADR is one Markdown file describing a single decision, its context,
and its consequences. ADRs are immutable history: once a decision changes, add a new ADR
that supersedes the old one rather than rewriting it.

We use a lightweight [Nygard-style](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)
format. See [`template.md`](./template.md).

## When to write an ADR

Write one when you:

- add or remove a public API surface, or change its semantics;
- add a new package/project or a new external dependency;
- change threading/scheduling, validation, or serialization behavior;
- make a breaking change or deprecate something;
- choose between competing implementation approaches with long-term impact.

Trivial, easily reversible choices (local refactors, naming) do not need an ADR.

## How to add a new ADR

1. Copy [`template.md`](./template.md) to `NNNN-short-title.md`, where `NNNN` is the next
   zero-padded number (e.g. `0002-...`).
2. Fill in **Status**, **Context**, **Decision**, and **Consequences**.
3. Start with `Status: Proposed`; change to `Accepted` once the decision is agreed.
4. To reverse an earlier decision, write a new ADR and set the old one's status to
   `Superseded by NNNN` (and the new one's `Context` should reference the old number).
5. Add the ADR to the index below.

## Index

| ADR | Title | Status |
|-----|-------|--------|
| [0001](./0001-adopt-tdd-adr-and-dev-docs.md) | Adopt TDD, ADRs, and a dev-docs location | Accepted |
