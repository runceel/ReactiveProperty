using System;
using System.Reflection;

namespace Reactive.Bindings.R3.Extensions;

/// <summary>
/// Represents property and instance package.
/// </summary>
/// <typeparam name="TInstance">Type of instance.</typeparam>
/// <typeparam name="TValue">Type of property value.</typeparam>
public sealed class PropertyPack<TInstance, TValue>
{
    internal PropertyPack(TInstance instance, PropertyInfo property, TValue value)
    {
        Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        Property = property ?? throw new ArgumentNullException(nameof(property));
        Value = value;
    }

    /// <summary>
    /// Gets the instance that has the property.
    /// </summary>
    public TInstance Instance { get; }

    /// <summary>
    /// Gets the target property.
    /// </summary>
    public PropertyInfo Property { get; }

    /// <summary>
    /// Gets the property value.
    /// </summary>
    public TValue Value { get; }
}
