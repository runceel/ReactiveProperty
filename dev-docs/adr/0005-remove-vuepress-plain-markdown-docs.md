# 0005. Remove VuePress; publish docs as plain Markdown on GitHub

- **Status:** Accepted
- **Date:** 2026-07-05
- **Deciders:** ReactiveProperty maintainers

## Context

The user-facing documentation under `docs/` was built with [VuePress](https://vuepress.vuejs.org/)
1.5 and published to GitHub Pages (the `gh-pages` branch) by the
`.github/workflows/deploy-document.yml` workflow. This added a Node.js/npm toolchain
(`docs/package.json`, `docs/package-lock.json`), VuePress-specific configuration
(`docs/docs/.vuepress/config.js`), deploy scripts (`docs/deploy.ps1`, `docs/deploy.sh`), and a
CI job whose only purpose was to build and push the static site.

The documentation content is already ordinary Markdown with relative `.md` links and relative
image paths, so it renders correctly when browsed directly on GitHub. The VuePress layer mainly
provided a navigation sidebar and theming — nice-to-have, but not essential for a project that is
in maintenance/active-support mode and whose author recommends the successor library
[R3](https://github.com/Cysharp/R3) for new apps. Maintaining the Node toolchain and a bespoke
publish pipeline is ongoing overhead disproportionate to that value.

## Decision

We will remove the VuePress dependency and treat `docs/` as plain Markdown that is read by
browsing the folder on GitHub.

Concretely:

1. Delete the VuePress configuration and Node tooling: `docs/docs/.vuepress/config.js`,
   `docs/package.json`, `docs/package-lock.json`, `docs/deploy.ps1`, `docs/deploy.sh`, and the
   VuePress-only `docs/.gitignore`.
2. Delete the `deploy-document.yml` GitHub Actions workflow that built and pushed the site.
3. Preserve navigation for GitHub readers by adding a top-level `docs/README.md` index and a
   "Documentation contents" table of contents to `docs/docs/README.md` and
   `docs/docs-ja/README.md` (mirroring the old sidebar).
4. Update contributor docs (`AGENTS.md`, `dev-docs/README.md`, and the `development-workflow`
   skill) to describe `docs/` as plain Markdown rather than a published VuePress site.

Disabling GitHub Pages itself and deleting the `gh-pages` branch are repository settings that
cannot be changed from repository files; they are a manual follow-up for a maintainer.

### Alternatives considered

- **Keep VuePress** — rejected: ongoing Node/npm maintenance and a bespoke publish pipeline for
  marginal benefit on a maintenance-mode project.
- **Migrate to another static-site generator (DocFX, MkDocs, Docusaurus, VitePress)** — rejected:
  still introduces a toolchain and publish pipeline; the stated goal is to remove the site build
  entirely and rely on GitHub's Markdown rendering.

## Consequences

- No Node.js/npm toolchain or site-build step is required to work on the docs; edits are just
  Markdown.
- The hosted documentation site (GitHub Pages) will no longer be updated and should be disabled
  by a maintainer; existing deep links to the old site will break. Links from the repo now point
  at the Markdown files in `docs/`.
- Navigation is provided by README indexes rather than a generated sidebar; contributors must keep
  those indexes in sync when adding or renaming pages.
- Documentation links must stay relative (`.md`) so they resolve both on GitHub and locally.
