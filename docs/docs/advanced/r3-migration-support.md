# R3 migration support

ReactiveProperty.R3 is a bridge package for applications that are migrating from ReactiveProperty to [R3](https://github.com/Cysharp/R3). It is intended for codebases that still rely on ReactiveProperty concepts but want to adopt R3 gradually.

ReactiveProperty and R3 share the same general goal of composing reactive code, but they do not have a one-to-one API mapping. The bridge package is intentionally small. It provides the compatibility layer that is missing from R3 and ObservableCollections.R3 so you can keep moving without rewriting everything at once.

The migration work is also supported by a dedicated migration skill that helps guide the transition from ReactiveProperty-style code to R3-friendly patterns. In practice, that skill is useful for namespace updates, API replacements, and an incremental migration strategy that avoids a large rewrite.

## When to use it

Use ReactiveProperty.R3 when:

- you are maintaining an existing ReactiveProperty-based application and want to introduce R3 gradually
- you need to keep some existing ViewModel or command code intact while new code is written with R3
- you want a stable migration path instead of a big-bang rewrite

## What it provides

The bridge package is designed to fill the gaps between ReactiveProperty and R3. It is not meant to be a full compatibility layer or a replacement for ReactiveProperty. Instead, it gives you a pragmatic way to move a codebase forward while keeping your migration manageable.

It is also intended to be a permanent support package for migration work, rather than a temporary experiment.

The bridge focuses on the same core types that developers already know from ReactiveProperty:

- `ReactiveProperty<T>`: a reactive value container for state that changes over time.
- `ReadOnlyReactiveProperty<T>`: a read-only view over an observable source.
- `ReactiveCommand` and `AsyncReactiveCommand`: commands that can be enabled or disabled based on observable state and used from UI code.
- `BindableReactiveProperty<T>`: a bindable variant for UI frameworks that depend on change notifications.

## Limitations

ReactiveProperty.R3 is useful, but it is not a drop-in replacement for ReactiveProperty. Keep the following in mind:

- You still need to understand the R3 programming model and its APIs.
- Some code will still need to be adjusted as the migration progresses.
- For brand-new applications, R3 is the recommended starting point.

## Migration guidance

A practical migration usually looks like this:

1. Add ReactiveProperty.R3 to the projects that still depend on ReactiveProperty.
2. Migrate one feature, ViewModel, or module at a time.
3. Keep the bridge package in place while you replace the parts that still depend on ReactiveProperty semantics.
4. Once a module is fully moved to R3, remove the remaining ReactiveProperty usage from that module.
5. Prefer a gradual migration over a big-bang rewrite if the application is already in production.

## Summary

Use ReactiveProperty.R3 when you want to move to R3 without stopping the whole project. It is a bridge for migration work, not a reason to keep using ReactiveProperty in new code.
