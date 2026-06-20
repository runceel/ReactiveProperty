# 0001. Document architecture decisions and agent skills

- Status: Accepted
- Date: 2026-06-20

## Context

ReactiveProperty has documentation for users who consume the NuGet packages, but the repository did not have a place to record project-level architecture decisions. The repository also did not contain guidance that an AI agent can load as a reusable skill before changing the library, documentation, samples, or release artifacts.

Without these files, future contributors and agents must infer decisions from source code, pull requests, and release history. That makes repeated maintenance tasks slower and increases the chance of inconsistent changes.

## Decision

Use Architecture Decision Records (ADRs) under `docs/docs/adr/` to capture project-level decisions that affect implementation, packaging, compatibility, or maintenance. Each ADR should include the decision status, date, context, decision, and consequences.

Add repository documentation under `docs/docs/development/` that explains how to use ADRs and Agent Skills. Add a ReactiveProperty maintenance Agent Skill under `.github/skills/reactiveproperty-maintainer/SKILL.md` so agents have repository-specific guidance for future work.

## Consequences

- Contributors have a stable location for architecture and maintenance decisions.
- Agent tasks can reuse repository-specific guidance instead of rediscovering the same context.
- Documentation navigation must include the new development and ADR pages so they are discoverable.
- ADRs should be updated only by adding new records or superseding existing records; accepted ADRs should not be rewritten except to correct mistakes.
