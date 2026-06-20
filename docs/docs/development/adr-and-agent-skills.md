# ADRs and Agent Skills

This repository keeps long-lived maintenance guidance in two places:

- Architecture Decision Records (ADRs) in `docs/docs/adr/`
- Agent Skills in `.github/skills/`

## Architecture Decision Records

Use an ADR when a decision affects project structure, public API compatibility, packaging, target frameworks, documentation strategy, or recurring maintenance work.

Each ADR should include:

- Status
- Date
- Context
- Decision
- Consequences

Accepted ADRs should remain stable. If a decision changes, create a new ADR that supersedes the old one instead of rewriting history.

## Agent Skills

Agent Skills provide reusable instructions for AI agents working in the repository. They should describe durable repository practices, validation expectations, and important paths.

Add or update an Agent Skill when repository-specific guidance would help repeated maintenance tasks, for example:

- choosing the right solution or package file
- updating documentation and samples consistently
- selecting validation commands
- avoiding unsupported target framework or API changes

The ReactiveProperty maintenance skill is located at `.github/skills/reactiveproperty-maintainer/SKILL.md`.
