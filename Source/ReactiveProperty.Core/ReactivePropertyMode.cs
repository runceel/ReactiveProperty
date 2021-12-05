using System;

namespace Reactive.Bindings;

/// <summary>
/// Mode of ReactiveProperty
/// </summary>
[Flags]
public enum ReactivePropertyMode
{
    /// <summary>
    /// None
    /// </summary>
    None = 0x00,

    /// <summary>
    /// If next value is same as current, not set and not notify.
    /// </summary>
    DistinctUntilChanged = 0x01,

    /// <summary>
    /// Push notify on instance created and subscribed.
    /// </summary>
    RaiseLatestValueOnSubscribe = 0x02,

    /// <summary>
    /// Ignore initial validation error
    /// </summary>
    IgnoreInitialValidationError = 0x04,

    /// <summary>
    /// Ignore exception
    /// </summary>
    IgnoreException = 0x08,

    /// <summary>
    /// Default mode value. It is same as DistinctUntilChanged | RaiseLatestValueOnSubscribe.
    /// </summary>
    Default = DistinctUntilChanged | RaiseLatestValueOnSubscribe,
}
