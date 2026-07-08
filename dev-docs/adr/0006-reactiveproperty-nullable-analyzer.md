# 0006. Ship a Roslyn analyzer (RP0001) for nullable reference type usage

- **Status:** Accepted
- **Date:** 2026-07-08
- **Deciders:** ReactiveProperty maintainers

## Context

ReactiveProperty enables nullable reference types (`<Nullable>enable</Nullable>`), but the C#
compiler cannot detect one important source of `null`: the `Value` of a `ReactiveProperty<T>`,
`ReactivePropertySlim<T>`, or `ReadOnlyReactivePropertySlim<T>` is initialized to `default(T)`
when no initial value is supplied. For a non-nullable reference type such as `string`, this
leaves `Value` as `null` even though its declared type says it can never be `null`.

For backward compatibility the parameterless / `initialValue = default` constructors must stay,
so the type system alone cannot express "you got a null here". This was tracked in issue #241,
where the community converged on a Roslyn analyzer as the way to add nullability support without a
breaking change. The compiler additionally infers the return type of
`ToReadOnlyReactivePropertySlim` / `ToReadOnlyReactiveProperty` as nullable (because of the
`initialValue = default` parameter), so the analyzer cannot rely on the inferred return type and
must inspect the source `IObservable<T>` element type instead.

## Decision

We will add a `Reactive.Bindings.Analyzer.ReactivePropertyNullableAnalyzer` in a new
`Source/ReactiveProperty.Analyzer` project (netstandard2.0). It reports diagnostic **RP0001**
(category `Usage`, severity `Warning`) when:

- `new ReactiveProperty<T>(...)`, `new ReactivePropertySlim<T>(...)`, or
  `new ReadOnlyReactivePropertySlim<T>(...)` is created without an `initialValue`, or
- `source.ToReadOnlyReactivePropertySlim(...)` / `source.ToReadOnlyReactiveProperty(...)` is
  called without an `initialValue`,

**and** the relevant type argument `T` is a non-nullable reference type (nullable context enabled,
annotation `NotAnnotated`). Value types, nullable annotated types (`T?`), open type parameters,
and code where the nullable context is disabled are ignored.

The analyzer is packed into both the `ReactiveProperty.Core` and `ReactiveProperty` NuGet packages
under `analyzers/dotnet/cs`, so consumers of either package get it. It is only referenced with
`ReferenceOutputAssembly="false"` so it is not a runtime dependency.

### Alternatives considered

- **Do nothing / wait for the compiler** — the compiler still cannot detect this pattern and the
  request in #241 remained open for years; rejected.
- **Add `[Obsolete]` to the default constructor** — rejected in #241 because it forces
  `new ReactiveProperty<string?>(null)` and hurts the developer experience for the legitimate
  `new ReactiveProperty<string?>()` usage.
- **Trust the inferred return type of the `ToReadOnly*` extension methods** — rejected because the
  compiler infers it as nullable (`string?`) due to `initialValue = default`; the source
  observable's element type is used instead.

## Consequences

- Consumers using nullable reference types get a clear, actionable warning at the exact call site,
  with the fix spelled out (provide an initial value or use `T?`).
- No public API or runtime behavior changes; the analyzer only produces diagnostics.
- Two new projects are added: the analyzer and its MSTest test project (using
  `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.MSTest`).
- Future rules (for example, dedicated codefixes) can be added to the same analyzer package and
  tracked via `AnalyzerReleases.Unshipped.md` / `AnalyzerReleases.Shipped.md`.
