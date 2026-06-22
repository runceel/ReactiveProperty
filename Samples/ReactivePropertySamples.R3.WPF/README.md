# ReactiveProperty WPF sample migrated to R3 + ReactiveProperty.R3

This sample is a migrated copy of [`ReactivePropertySamples.WPF`](../ReactivePropertySamples.WPF)
(and its shared ViewModels in [`ReactivePropertySamples.Shared`](../ReactivePropertySamples.Shared)).
It shows what the existing WPF sample looks like after migrating from **ReactiveProperty** to
**[R3](https://github.com/Cysharp/R3)** plus the **ReactiveProperty.R3** bridge.

- **The UI is intentionally identical** to the original sample — only namespaces and a couple of
  platform-only features (see below) changed. Every window, label, and binding path matches the
  original so you can compare them side by side.
- The migrated ViewModels live in `ReactivePropertySamples.R3.Shared`
  (namespace `ReactivePropertySamples.Migrated.*`), the views in this project.

> **Build note:** this is a `net10.0-windows` WPF app, so it builds and runs **on Windows only**.

## How to map a ReactiveProperty type to R3

The single most important rule for WPF: **only `R3.BindableReactiveProperty<T>` implements
`INotifyPropertyChanged`** (and `INotifyDataErrorInfo`). Plain `R3.ReactiveProperty<T>` and
`R3.ReadOnlyReactiveProperty<T>` do **not** raise change notifications, so they cannot be bound in
XAML. Therefore every property that was bound in XAML migrates to `BindableReactiveProperty<T>`.

| Original (ReactiveProperty) | Migrated (R3 / ReactiveProperty.R3) |
| --- | --- |
| `ReactiveProperty<T>` / `ReactivePropertySlim<T>` (bound) | `BindableReactiveProperty<T>` |
| `ReadOnlyReactiveProperty<T>` / `…Slim<T>` (bound) | `…Observable…ToBindableReactiveProperty(initial)` |
| `IObservable<T>.ToReadOnlyReactiveProperty()` | `Observable<T>.ToBindableReactiveProperty(initial)` |
| `ReactiveCommand` / `ReactiveCommand<T>` | `Observable<bool>.ToReactiveCommand[<T>]()` or `new ReactiveCommand[<T>]()` |
| `AsyncReactiveCommand` | `Reactive.Bindings.R3.AsyncReactiveCommand` (+ `WithSubscribe`) |
| `.ToReactivePropertyAsSynchronized(x => x.P)` | bridge `.ToReactivePropertyAsSynchronized(x => x.P)` → `BindableReactiveProperty<T>` |
| `.ObserveProperty(x => x.P)` | R3 `.ObservePropertyChanged(x => x.P)` |
| `.AddTo(Disposables)` | R3 `.AddTo(Disposables)` (`R3.CompositeDisposable`) |
| DataAnnotations: `.SetValidateAttribute(...)` | `.EnableValidation(() => Prop)` / bridge `ValidatableReactiveProperty<T>` |
| `ObserveErrorChanged` / `ObserveHasErrors` | bridge `Reactive.Bindings.R3.Extensions` extensions |
| `ReadOnlyReactiveCollection` | bridge `.ToReadOnlyReactiveCollection(...)` |
| `IFilteredReadOnlyObservableCollection` | bridge `.ToFilteredReadOnlyObservableCollection(...)` |
| `ReactiveTimer` | bridge `Reactive.Bindings.R3.ReactiveTimer` |

### Operators that changed name or shape

- No `Throttle` → use R3 `Debounce`.
- No `Inverse()` → `.Select(x => !x)`.
- No `CombineLatestValuesAreAllTrue()` → `Observable.CombineLatest(sources).Select(xs => xs.All(x => x))`.
- No UI scheduler abstraction (`ReactivePropertyScheduler` / `ObserveOnUIDispatcher`) →
  `ObserveOnCurrentSynchronizationContext()`, captured on the UI thread. `App.xaml.cs` no longer
  configures a scheduler because WPF installs a `DispatcherSynchronizationContext` automatically.

## Things implemented naively (no bridge mapping)

These ReactiveProperty features are **not** part of the migration bridge, so they are re-implemented
with plain code, exactly as the task requires:

- **`EventToReactiveProperty` / `EventToReactiveCommand`** (the `ReactiveProperty.WPF`
  *Behaviors*/interactivity helpers and `ReactiveConverter`s). These are out of scope, so
  `EventToReactiveWindow` handles `MouseMove` and the Open-file button with ordinary WPF
  code-behind event handlers (`EventToReactiveWindow.xaml.cs`). The ViewModel stays
  platform-agnostic and just exposes the reactive surface they push into.
- **`ReactiveCollection<T>` / `ToReactiveCollection()` / `ClearOnScheduler`** — replaced with a plain
  `ObservableCollection<T>` mutated on the UI thread.
- **`ReactiveProperty.FromObject` (one-way-to-source)** — replaced with a `BindableReactiveProperty<T>`
  seeded from the POCO that writes its value back on change.
- **`DisposeViewModelWhenClosedBehavior`** uses `Microsoft.Xaml.Behaviors` (not a ReactiveProperty
  type), so it is kept as-is.

## Migration nuances worth knowing

- `ReactivePropertyMode.IgnoreInitialValidationError` → bridge
  `new ValidatableReactiveProperty<T>(ignoreInitialValidationError: true)`.
- The original `FirstName` used `ToReactivePropertyAsSynchronized(..., ignoreValidationErrorValue: true)`.
  The bridge overload has no `ignoreValidationErrorValue` parameter, so that nuance is dropped; the
  property is synchronized and validated with `.EnableValidation(() => FirstName)`.
- Cross-thread collection updates (`AddGuidToModelCommand` runs on the thread pool) are marshalled
  back to the UI by passing the captured `SynchronizationContext` as `raiseEventContext` to
  `ToReadOnlyReactiveCollection`.

See `dev-docs/r3-migration/design.md` and the
[`migrating-reactiveproperty-to-r3`](../../skills/migrating-reactiveproperty-to-r3) skill for the
authoritative rules behind these mappings.
