# 0008. Remove UWP and Xamarin support from the maintained repository

- **Status:** Accepted
- **Date:** 2026-11-17
- **Deciders:** ReactiveProperty maintainers

## Context

The retired UWP package is no longer built by the main solution, but its project
and platform-specific branches remain in source. Current English and Japanese
documentation still recommends UWP and Xamarin packages and tutorials.
Issue [#571](https://github.com/runceel/ReactiveProperty/issues/571) approves removing
this unsupported surface while preserving all other supported platforms.

## Decision

Remove the UWP project and inactive platform branches, and remove UWP/Xamarin
package guidance, tutorials, recordings, and screenshots from current documentation.
Replace shared feature examples with WPF examples instead of dropping those features.
Remove the old Uno tutorial because it is based on the retired UWP app model;
this is not a decision to remove compatibility with current Uno applications.

Keep historical release notes and accepted ADRs unchanged. Do not remove or unlist
previously published NuGet packages. Keep framework targets and CI framework
settings unchanged; .NET 11 support and .NET 8/9 target retirement belong to #570.

Compile and execute the replacement English/Japanese Page examples in the existing
WPF test project, extracting the actual Markdown code instead of maintaining
duplicate models. Use real controls and binding diagnostics; keep native file-picker
selection/cancellation opt-in because it requires an interactive Windows desktop.

### Alternatives considered

- **Retain unsupported projects and tutorials** -- rejected because they continue
  to advertise integration paths that are no longer maintained.
- **Delete shared feature documentation** -- rejected because WPF still supports
  these features and should retain working examples in both languages.

## Consequences

The maintained source and documentation no longer offer UWP/Xamarin integrations.
WPF keeps its existing APIs and behavior, including the non-generic collection
interfaces used by data binding. Consumers of retired platform packages must use
previously published versions; those versions and historical records remain available.
