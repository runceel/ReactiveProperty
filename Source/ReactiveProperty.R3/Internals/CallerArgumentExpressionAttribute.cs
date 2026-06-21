#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices;

/// <summary>
/// Allows capturing the expression passed to another parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class CallerArgumentExpressionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CallerArgumentExpressionAttribute"/> class.
    /// </summary>
    /// <param name="parameterName">The target parameter name.</param>
    public CallerArgumentExpressionAttribute(string parameterName) => ParameterName = parameterName;

    /// <summary>
    /// Gets the target parameter name.
    /// </summary>
    public string ParameterName { get; }
}
#endif
