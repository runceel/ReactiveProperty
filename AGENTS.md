# AGENTS.md

Guidance for AI coding agents working in the **ReactiveProperty** repository.

## What this project is

ReactiveProperty provides MVVM and asynchronous support features built on top of
Reactive Extensions (`System.Reactive`). The core type is `ReactiveProperty[Slim]<T>`,
a value wrapper that implements `IObservable<T>` and `INotifyPropertyChanged` so it can
flow through Rx LINQ operators and bind to XAML/Blazor UIs.

- Library is consumed via NuGet packages (`ReactiveProperty`, `ReactiveProperty.Core`,
  `ReactiveProperty.WPF`, `ReactiveProperty.Blazor`).
- It deliberately ships **no ViewModel base class**, so it composes with other MVVM
  frameworks (Prism, CommunityToolkit.Mvvm, etc.).
- Note: for brand-new apps the author recommends the successor library
  [R3](https://github.com/Cysharp/R3). This repo is in maintenance/active-support mode.

> ⚠️ The root namespace is **`Reactive.Bindings`**, not `ReactiveProperty`.
> It is set centrally in `Source/Directory.Build.props`.

## Repository layout

```
Source/                         Library source (each folder is one package/assembly)
  Directory.Build.props         Shared metadata: Version, RootNamespace, LangVersion, Nullable, SourceLink
  ReactiveProperty.Core/        pkg ReactiveProperty.Core — minimal types, NO System.Reactive dependency
  ReactiveProperty.NETStandard/ pkg ReactiveProperty — the main package (depends on System.Reactive)
  ReactiveProperty.Platform.Blazor/  pkg ReactiveProperty.Blazor — EditForm validation support
  ReactiveProperty.Platform.WPF/     pkg ReactiveProperty.WPF — EventToReactiveCommand/Property
  ReactiveProperty.Platform.UWP/     pkg ReactiveProperty.UWP (legacy, no longer supported)
  ReactiveProperty.Platform.Shared/  Shared project (.shproj/.projitems) imported by platform projects
Test/                           Unit tests (MSTest)
  ReactiveProperty.NETStandard.Tests/  Core/main library tests (namespace ReactiveProperty.Tests)
  ReactiveProperty.Blazor.Tests/       Blazor tests (bunit)
  ReactiveProperty.WPF.Tests/          WPF tests
  ReactiveProperty.WPF.ManualTests/    Manual/interactive WPF test app
Samples/                        Example apps (WPF, Blazor, MAUI-style, Prism, etc.)
Benchmark/                      BenchmarkDotNet projects (separate solution)
docs/                           VuePress documentation site (user-facing, published)
dev-docs/                       Implementer/contributor docs + ADRs (not published)
skills/                         Published agent skills for ReactiveProperty *users* (consumers)
.agents/skills/                 Agent skills for working *on* this repo (contributors)
Snippet/                        Visual Studio code snippets
Directory.Packages.props        Central Package Management — all NuGet versions live here
ReactiveProperty.slnx           Main solution (library + tests)
ReactiveProperty-Samples.slnx   Samples solution
```

## Build, test, and run

The project uses the **.NET 10 SDK** (`10.0.x`) and CI runs on **Windows**
(the WPF/UWP projects require Windows to build the full solution).

```pwsh
# Restore, build, and test the library + tests
dotnet restore ReactiveProperty.slnx
dotnet build   ReactiveProperty.slnx -c Release --no-restore
dotnet test    ReactiveProperty.slnx --no-restore --verbosity normal
```

- **Run a single test project:** `dotnet test Test/ReactiveProperty.NETStandard.Tests/ReactiveProperty.NETStandard.Tests.csproj`
- **Samples:** open/build `ReactiveProperty-Samples.slnx`.
- **Benchmarks:** use `Benchmark/ReactiveProperty-Benchmark.sln` (separate from the main solution).

CI mirrors these commands:
- `.github/workflows/dotnet-core-unit-testing.yml` — restore + test on every PR to `main`.
- `.github/workflows/build-and-publish.yml` — runs on `v*` tags: bumps the version in
  `Source/Directory.Build.props`, builds Release, tests, and pushes packages to NuGet.

## Conventions you must follow

### NuGet — Central Package Management
Package versions are managed centrally (`ManagePackageVersionsCentrally=true`).
- Add/upgrade versions **only** in `Directory.Packages.props` using `<PackageVersion Include="..." Version="..." />`.
- In `.csproj`, reference packages **without** a version: `<PackageReference Include="..." />`.

### Target frameworks (don't change casually)
| Project | TargetFramework(s) |
|---|---|
| `ReactiveProperty.Core`, `ReactiveProperty.NETStandard` | `netstandard2.0;net8.0;net9.0;net472` |
| `ReactiveProperty.Platform.Blazor` | `net8.0;net9.0;net10.0` |
| `ReactiveProperty.Platform.WPF` | `net8.0-windows;net9.0-windows;net472` |
| `ReactiveProperty.NETStandard.Tests` | `net9.0` |

Because of `netstandard2.0`/`net472`, keep core code compatible with older frameworks
(avoid APIs that only exist on modern .NET unless guarded).

### Project settings (from `Source/Directory.Build.props`)
- `LangVersion` is **12.0**; `Nullable` is **enable** — keep new code null-annotated.
- Assemblies are **strong-name signed** (`key.snk`); don't break signing.
- XML documentation is generated (`GenerateDocumentationFile=true`) — public APIs need doc comments.
- The package **version** is `<Version>` in `Source/Directory.Build.props` (do not hand-edit for releases; the tag workflow updates it).

### Code style (enforced via `.editorconfig`)
- Indentation: **4 spaces** for `.cs`; **2 spaces** for `.csproj`/`.props`/`.targets`/XML.
- **File-scoped namespaces** (`namespace Foo;`).
- Prefer **`var`**; expression-bodied **properties/accessors** yes, expression-bodied **methods/constructors** no.
- **Allman braces** (open brace on a new line); always use braces.
- Naming: private instance fields `_camelCase`, private static fields `s_camelCase`,
  members/types PascalCase, locals/parameters camelCase.
- Files are **UTF-8 with BOM** and end with a final newline.

## Development workflow (must follow)
See the **`development-workflow`** skill (`.agents/skills/development-workflow/SKILL.md`) for the full policy. In short:
- **TDD is mandatory** — drive every `Source/` change with a failing test first, then
  Red → Green → Refactor. Not done until `dotnet test ReactiveProperty.slnx` passes.
- **Record design/architecture decisions as ADRs** in `dev-docs/adr/` (copy `template.md`,
  number it, add it to the index).
- **Docs by audience** — `docs/` for users, `dev-docs/` for implementers (see below).

## Testing notes
- Framework: **MSTest** (`MSTest.TestAdapter` / `MSTest.TestFramework`).
- Helpers: **Moq**, **Microsoft.Reactive.Testing** (`TestScheduler`), and the bundled
  `ChainingAssertion` (`.Is(...)` fluent assertions). Blazor tests use **bunit**.
- Add tests under the matching `Test/*` project; mirror the namespace `ReactiveProperty.Tests`.

## Documentation
- **`docs/` = user-facing**, built with **VuePress** (English + Japanese `*-ja` variants):
  ```pwsh
  cd docs
  npm install
  npm run docs:dev     # local dev server
  npm run docs:build   # static build
  ```
- **`dev-docs/` = implementer/contributor-facing** (not published): architecture notes,
  contributor workflow, and ADRs in `dev-docs/adr/`. See `dev-docs/README.md`.

## Agent skills
This repo has **two** skill locations, split by audience. Each skill is a folder containing a
`SKILL.md` with YAML frontmatter (`name`, `description`) plus the body.

- **`.agents/skills/` — for working *on* this repo (contributors).** Auto-loaded when an agent
  works in this repository. Holds the repo's `development-workflow` skill and vendored .NET
  build/test skills. Add a skill here only if it helps maintain ReactiveProperty itself.
- **`skills/` — published skills for ReactiveProperty *users* (consumers).** These help agents
  build apps that *use* ReactiveProperty (ViewModels, `ReactiveProperty`/`ReactiveCommand`,
  validation, Rx composition). They are meant to be discovered/installed by consumers, so they
  are **not** auto-loaded for repo work (working in this repo means building the library, not
  consuming it). See `skills/README.md`. Use one folder per skill, e.g.
  `skills/using-reactiveproperty/SKILL.md`.

When adding a user-facing skill, put it under **`skills/`**, not `.agents/skills/`.

## Quick reminders
- Follow **TDD (Red → Green → Refactor)**; write the failing test first (see the `development-workflow` skill).
- Record design decisions as **ADRs** in `dev-docs/adr/`.
- Don't add package versions to `.csproj` — use `Directory.Packages.props`.
- Keep `Source/` code compatible with `netstandard2.0` / `net472`.
- Use `Reactive.Bindings` as the namespace for library code.
- Run `dotnet test ReactiveProperty.slnx` (on Windows) before finishing changes.
