using System;

namespace Reactive.Bindings.R3Compat;

/// <summary>
/// Options for temporary ReactiveProperty-to-R3 compatibility APIs.
/// </summary>
public sealed class CompatibilityOptions
{
    /// <summary>Gets the compatibility mode flags.</summary>
    public CompatibilityMode Mode { get; init; } = CompatibilityMode.Default;

    /// <summary>Gets the time provider used when an R3 bridge needs scheduler-like timing.</summary>
    public TimeProvider TimeProvider { get; init; } = TimeProvider.System;

    /// <summary>Gets a value indicating whether the initial validation error should be suppressed.</summary>
    public bool IgnoreInitialValidationError => Mode.HasFlag(CompatibilityMode.IgnoreInitialValidationError);
}
