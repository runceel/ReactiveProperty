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
synchronization. It supplies omitted imports and isolates namespaces, but compiles
and runs the documented models, bindings, and converters without replacement models.
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

The two event-transfer tests cover the remaining four Pages using the actual
`OpenFileDialog` converter, selecting a temporary `.snippet` file and then cancelling.
They are skipped unless explicitly enabled on an interactive Windows desktop:

```powershell
$env:REACTIVEPROPERTY_TEST_FILE_DIALOGS = '1'
try {
    dotnet test Test\ReactiveProperty.WPF.Tests\ReactiveProperty.WPF.Tests.csproj --filter FullyQualifiedName~DocumentationExamplesTest
}
finally {
    Remove-Item Env:\REACTIVEPROPERTY_TEST_FILE_DIALOGS
}
```

Dialog automation targets only native dialogs belonging to the test dispatcher
thread. Ordinary CI verifies the other Pages; it does not claim to verify the real
file picker. The tests use observable UI-state conditions with watchdog timeouts,
not performance thresholds or fixed sleeps to assert completion.
