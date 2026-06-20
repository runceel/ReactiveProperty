using System;

namespace Reactive.Bindings.R3Compat;

/// <summary>
/// Temporary configuration flags that bridge ReactiveProperty behavior to R3 behavior.
/// </summary>
[Flags]
public enum CompatibilityMode
{
    /// <summary>No compatibility behavior.</summary>
    None = 0,
    /// <summary>Suppress duplicated values when the default equality comparer judges them equal.</summary>
    DistinctUntilChanged = 1,
    /// <summary>Raise the latest value to new subscribers.</summary>
    RaiseLatestValueOnSubscribe = 2,
    /// <summary>Skip validation for the constructor-supplied initial value.</summary>
    IgnoreInitialValidationError = 4,
    /// <summary>Preserve the ReactiveProperty mode flag for callers that still need to classify it.</summary>
    IgnoreException = 8,
    /// <summary>The ReactiveProperty default behavior used by most migrations.</summary>
    Default = DistinctUntilChanged | RaiseLatestValueOnSubscribe,
}
