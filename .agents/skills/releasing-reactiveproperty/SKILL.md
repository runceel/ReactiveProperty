---
name: releasing-reactiveproperty
description: >
  Release the ReactiveProperty NuGet package set from this repository. Use this skill whenever the user asks to release, publish, ship, pre-release/prerelease, stable release, tag v*, push NuGet packages, or mentions "リリース", "pre リリース", "正式リリース", "NuGet 公開", or "パッケージ公開" for ReactiveProperty, even if they do not use the skill name. Handles tag-driven releases, mode/version confirmation, GitHub Actions monitoring, NuGet/GitHub Release verification, and failure guardrails. Do not use for setting up NuGet OIDC/trusted publishing; use nuget-trusted-publishing instead.
---
# Releasing ReactiveProperty

Run the tag-driven release procedure for the ReactiveProperty NuGet package set.

## Prerequisites

- **Repository access**: permission to push tags to `runceel/ReactiveProperty`.
- **GitHub CLI**: authenticated with access to Actions and Releases (`gh auth status`).
- **NuGet trusted publishing**: `.github/workflows/build-and-publish.yml` uses `NuGet/login@v1` and `secrets.NUGET_USER`; if this setup is broken or missing, use the `nuget-trusted-publishing` skill instead.
- **Local validation environment**: Windows with the .NET 10 SDK (`10.0.x`) when running the full solution locally.

## Critical Rules

- **Ask for release mode first when it is not specified.** Use `ask_user` with choices for `pre-release` and `stable release` before doing any release work.
- **The current release workflow is tag-driven.** Do not update `Source/Directory.Build.props` or create a version bump PR unless the maintainer explicitly asks for repository version record-keeping.
- **The tag is the package version source of truth.** `.github/workflows/build-and-publish.yml` rewrites `Source/Directory.Build.props` from the pushed `v*` tag inside the runner before build/test/package.
- **Never overwrite or recreate a release tag.** If `v{version}` already exists locally or remotely, stop and ask the maintainer.
- **Never publish manually as a fallback.** If OIDC/NuGet publish fails, diagnose the workflow; do not bypass it with a local `dotnet nuget push` or a long-lived API key.

## Release Modes and Version Rules

Normalize a requested version by removing one leading `v` for validation, then add `v` back for the tag.

| Mode | Required version shape | Example tag | Notes |
|---|---|---|---|
| Pre-release | SemVer prerelease containing `-pre` | `v9.4.0-pre.1` | The workflow marks GitHub Releases as prerelease only when the version contains `-pre`. Do not use `-alpha`, `-beta`, or `-rc` unless the maintainer accepts that the current workflow will not mark it as prerelease. |
| Stable release | Plain SemVer | `v9.4.0` | Must not contain a prerelease suffix. |

If the user supplies a version that does not match the selected mode, stop and ask for a corrected version.

## Package Set

The normal `ReactiveProperty.slnx` release publishes these package IDs:

| Package ID | Project |
|---|---|
| `ReactiveProperty.Core` | `Source/ReactiveProperty.Core/ReactiveProperty.Core.csproj` |
| `ReactiveProperty` | `Source/ReactiveProperty.NETStandard/ReactiveProperty.NETStandard.csproj` |
| `ReactiveProperty.Blazor` | `Source/ReactiveProperty.Platform.Blazor/ReactiveProperty.Platform.Blazor.csproj` |
| `ReactiveProperty.WPF` | `Source/ReactiveProperty.Platform.WPF/ReactiveProperty.Platform.WPF.csproj` |
| `ReactiveProperty.R3` | `Source/ReactiveProperty.R3/ReactiveProperty.R3.csproj` |
| `ReactiveProperty.R3.WPF` | `Source/ReactiveProperty.R3.WPF/ReactiveProperty.R3.WPF.csproj` |

`ReactiveProperty.UWP` is legacy and is not part of the normal `ReactiveProperty.slnx` release.

## Workflow

### 1. Resolve mode, version, and tag

1. Determine whether this is a **pre-release** or **stable release**.
   - If unspecified, call `ask_user` before proceeding.
2. Determine the target version.
   - If unspecified, ask the user for it.
3. Validate the version against the selected mode.
4. Normalize the tag to `v{version}`.

### 2. Preflight the target commit

Fetch `main` and tags, then inspect the intended release commit:

```pwsh
git fetch origin main --tags --prune
git rev-parse origin/main
git log --oneline -n 5 origin/main
git tag --list v{version}
```

Stop if the tag already exists. Confirm that `origin/main` is the intended release commit; do not tag a feature/worktree branch by accident.

### 3. Run local validation before tagging

Mirror the release workflow as closely as possible, passing the intended version without editing files:

```pwsh
dotnet restore ReactiveProperty.slnx
dotnet build ReactiveProperty.slnx -c Release --no-restore /p:Version={version} /p:ContinuousIntegrationBuild=True /p:EmbedUntrackedSources=True
dotnet test ReactiveProperty.slnx --no-restore --verbosity normal
```

If validation fails, stop and fix the failure before tagging. Only skip local validation when the maintainer explicitly asks to rely on GitHub Actions.

### 4. Optional version record PR

Do this only when the maintainer explicitly asks to record the release version in `main` before release.

1. Update `Source/Directory.Build.props` `<Version>` to the target version.
2. Create a PR and wait for CI/review.
3. Merge the PR.
4. Fetch `origin/main` again and tag the merged commit.

This PR is optional for the current workflow because the package version is produced from the tag during the release run.

### 5. Confirm and push the release tag

Before pushing, present a compact confirmation with:

- release mode;
- normalized version and tag;
- target `origin/main` commit SHA;
- package set;
- whether local validation passed or was explicitly skipped.

After explicit confirmation, create an annotated tag on `origin/main` and push it:

```pwsh
git tag -a v{version} origin/main -m "Release {version}"
git push origin v{version}
```

### 6. Monitor GitHub Actions

Find the tag-triggered run and watch it to completion:

```pwsh
gh run list --workflow build-and-publish.yml --limit 10 --json databaseId,headBranch,headSha,status,conclusion,event,createdAt
gh run watch {run-id} --exit-status
```

If the run fails, inspect failed logs:

```pwsh
gh run view {run-id} --log-failed
```

Do not delete/recreate the tag. If the failure is transient and no version correction is needed, rerun the same workflow instead.

### 7. Verify published outputs

Verify the GitHub Release:

```pwsh
gh release view v{version} --json tagName,isPrerelease,assets,url,publishedAt
```

For pre-releases, `isPrerelease` must be `true`. If it is false, check whether the version contains `-pre`; the current workflow only uses that substring to set prerelease status.

Verify NuGet indexing for every package ID. NuGet indexing can lag, so retry for a short period before reporting a delay:

```pwsh
$version = "{version}".ToLowerInvariant()
$packages = @(
  "ReactiveProperty",
  "ReactiveProperty.Core",
  "ReactiveProperty.Blazor",
  "ReactiveProperty.WPF",
  "ReactiveProperty.R3",
  "ReactiveProperty.R3.WPF"
)

foreach ($package in $packages) {
  $id = $package.ToLowerInvariant()
  $index = Invoke-RestMethod "https://api.nuget.org/v3-flatcontainer/$id/index.json"
  [PSCustomObject]@{
    Package = $package
    Published = $index.versions -contains $version
  }
}
```

## Error Handling

| Problem | Likely cause | Recovery |
|---|---|---|
| Release mode is unclear | User asked for "release" without pre/stable detail | Ask `pre-release` vs `stable release` with `ask_user` before proceeding. |
| Version does not match mode | Pre-release lacks `-pre`, or stable has a suffix | Stop and ask for a corrected version. |
| `v{version}` already exists | Release was already attempted or completed | Stop. Inspect existing tag/release; never force-push or recreate it. |
| Local validation fails | Build/test regression or environment issue | Fix the failure or ask whether to rely on Actions; do not tag by default. |
| `NuGet/login@v1` fails | Trusted publishing policy, `NUGET_USER`, or OIDC permission issue | Use `nuget-trusted-publishing`; do not use a local API key fallback. |
| Workflow fails after some packages publish | Partial publish or transient failure | Do not retag. Use the same workflow run/rerun; `--skip-duplicate` handles already-published packages. |
| NuGet package is not visible immediately | NuGet indexing delay | Recheck flat-container indexes before declaring failure. |
| GitHub Release is not marked prerelease | Version did not contain `-pre` or workflow changed | Report the mismatch; do not edit release metadata unless the maintainer asks. |

## Output Format

Before tagging, show:

```text
Release mode: pre-release|stable
Version: {version}
Tag: v{version}
Target commit: {short-sha} {subject}
Packages: ReactiveProperty, ReactiveProperty.Core, ReactiveProperty.Blazor, ReactiveProperty.WPF, ReactiveProperty.R3, ReactiveProperty.R3.WPF
Validation: passed|skipped by maintainer
```

After completion, show:

```text
Release v{version}: succeeded|failed
GitHub Actions: {run-url}
GitHub Release: {release-url and prerelease status}
NuGet: all package IDs visible|pending indexing|failed package list
```

## References

| Reference | Purpose |
|---|---|
| `.github/workflows/build-and-publish.yml` | Source of truth for tag trigger, package build, NuGet publish, and GitHub Release creation. |
| `Source/Directory.Build.props` | Repository version property that the workflow rewrites from the tag during release. |
| `ReactiveProperty.slnx` | Solution restored, built, tested, and packaged by the release workflow. |
| `.agents/skills/nuget-trusted-publishing/SKILL.md` | Use when OIDC/trusted publishing setup needs to be created or repaired. |

## Post-Run Reflection

After a release attempt, note any step that differed from this skill. If the repository workflow changed, update this skill in a follow-up PR so the next release runbook stays accurate.
