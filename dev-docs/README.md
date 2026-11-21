# dev-docs — Implementer documentation

This folder holds documentation for people working **on** ReactiveProperty
(maintainers and contributors). It is intentionally separate from the user-facing site.

| Folder | Audience | Published? |
|--------|----------|------------|
| [`../docs/`](../docs) | Library **users** (consumers) | Plain Markdown, browsable on GitHub |
| `dev-docs/` (this folder) | **Implementers / contributors** | No |

Put "how does a consumer use this feature?" content in `docs/`. Put "how/why is this
implemented, and how do I contribute?" content here.

## Contents

- [`adr/`](./adr) — Architecture Decision Records: the log of design/architecture decisions.

## Development workflow (summary)

The full, authoritative policy is the `development-workflow` skill
(`.agents/skills/development-workflow/SKILL.md`). In short:

1. **TDD, always Red → Green → Refactor.** Write a failing test first, make it pass with the
   minimum code, then refactor with tests green. A change isn't done until
   `dotnet test ReactiveProperty.slnx` passes (on Windows).
2. **Record design decisions as ADRs** under [`adr/`](./adr).
3. **Docs by audience:** `docs/` for users, `dev-docs/` for implementers.

See the repository [`AGENTS.md`](../AGENTS.md) for build/test commands, target frameworks,
central package management, and code style.

## Executable WPF documentation examples

The WPF test project extracts the complete C# and Page XAML examples from both
languages of Collections, Commanding, ReactiveProperty, Event transfer, and POCO
synchronization. It supplies omitted imports, preserves the snippets' nullable-oblivious
context, and isolates namespaces, but compiles the documented models, bindings, and converters.
When changing the example layout, update the explicit block map in
`Test/ReactiveProperty.WPF.Tests/Documentation/ExtractExamples.ps1`.

```powershell
dotnet test Test\ReactiveProperty.WPF.Tests\ReactiveProperty.WPF.Tests.csproj --filter FullyQualifiedName~DocumentationExamplesTest
```

These tests display real WPF Pages on a dedicated STA dispatcher, exercise controls,
and reject binding diagnostics. Twelve tests cover both languages (24 Pages):
collection start/clear, parent-command removal, reset, filtering and item text,
commands, asynchronous/shared CanExecute, validation, and POCO synchronization.
Periodic item subscriptions in the filtering example must be disposed when closing
the view; the test host disposes its models, properties, and collection items.

The other two tests exercise the four event-transfer Pages noninteractively.
They retain the documented ViewModels and XAML binding expressions, clone each
trigger action, and install a deterministic converter before attaching it.
Real Button Click events must produce converted values, observable notifications,
and matching displayed text. These tests do **not** exercise `OpenFileDialog` or
the documented file-picker converter. No environment variable or special test
filter is required: ordinary Visual Studio **Run All** and `dotnet test` run all
14 documentation tests without Inconclusive results or native dialogs.

### Manual file-picker verification

Use the existing **ReactiveProperty.WPF.ManualTests** startup project in Visual
Studio (or `dotnet run --project Test\ReactiveProperty.WPF.ManualTests`).
Choose **Start test** under **Documentation event-transfer / file-picker test**.
The four tabs load the actual English/Japanese command/property Pages and their
unmodified converters, extracted from the same Markdown during the build.

1. In each tab, click **OpenFile...**, select an existing `.snippet` file (for example
   one under `Snippet`), and verify that its full path is displayed.
2. Open the dialog again and cancel; the displayed path must stay unchanged.
3. Select a different `.snippet` file and verify that the displayed path updates.

This is manual verification, not automated test coverage. Ordinary tests and CI
never drive OS dialogs. Automatic tests use observable UI-state conditions with
watchdog timeouts, not performance thresholds or fixed sleeps to assert completion.
