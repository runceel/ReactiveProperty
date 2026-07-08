using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Reactive.Bindings.Analyzer;

/// <summary>
/// Reports usages of ReactiveProperty family types for a non-nullable reference type that are
/// created without an initial value. In that situation the <c>Value</c> property is initialized to
/// <c>null</c> even though its type is a non-nullable reference type, which the C# compiler cannot
/// detect on its own.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ReactivePropertyNullableAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The diagnostic id reported by this analyzer.
    /// </summary>
    public const string DiagnosticId = "RP0001";

    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor s_rule = new(
        DiagnosticId,
        "Provide an initial value or use a nullable type argument",
        "'{0}' is created for the non-nullable reference type '{1}' without an initial value, so its Value will be null. Provide an initial value or use a nullable type argument such as '{1}?'.",
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "ReactiveProperty family types initialize Value to default (null for reference types) when no initial value is supplied. The C# compiler cannot detect this, so this analyzer reports it for non-nullable reference types.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(s_rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var compilation = context.Compilation;

        var targetTypes = new[]
        {
            compilation.GetTypeByMetadataName("Reactive.Bindings.ReactiveProperty`1"),
            compilation.GetTypeByMetadataName("Reactive.Bindings.ReactivePropertySlim`1"),
            compilation.GetTypeByMetadataName("Reactive.Bindings.ReadOnlyReactivePropertySlim`1"),
            compilation.GetTypeByMetadataName("Reactive.Bindings.ReadOnlyReactiveProperty`1"),
        }
            .Where(x => x is not null)
            .Select(x => (ISymbol?)x!.OriginalDefinition)
            .ToImmutableHashSet(SymbolEqualityComparer.Default);

        if (targetTypes.IsEmpty)
        {
            return;
        }

        context.RegisterOperationAction(ctx => AnalyzeObjectCreation(ctx, targetTypes), OperationKind.ObjectCreation);
        context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
    }

    private static void AnalyzeObjectCreation(OperationAnalysisContext context, ImmutableHashSet<ISymbol?> targetTypes)
    {
        var operation = (IObjectCreationOperation)context.Operation;
        if (operation.Type is not INamedTypeSymbol createdType || createdType.TypeArguments.Length != 1)
        {
            return;
        }

        if (!targetTypes.Contains(createdType.OriginalDefinition))
        {
            return;
        }

        var typeArgument = createdType.TypeArguments[0];
        if (!IsNonNullableReferenceType(typeArgument) || HasInitialValue(operation.Arguments))
        {
            return;
        }

        Report(context, operation.Syntax.GetLocation(), createdType, typeArgument);
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
        var operation = (IInvocationOperation)context.Operation;
        var method = operation.TargetMethod;
        if (method.Name is not ("ToReadOnlyReactivePropertySlim" or "ToReadOnlyReactiveProperty"))
        {
            return;
        }

        if (method.ContainingNamespace?.ToDisplayString() != "Reactive.Bindings")
        {
            return;
        }

        if (operation.Type is not INamedTypeSymbol returnType)
        {
            return;
        }

        // The compiler infers the return type argument as nullable because of the
        // 'initialValue = default' parameter, so the element type is taken from the source
        // observable instead, which reflects what the caller actually provided.
        ITypeSymbol? typeArgument = null;
        foreach (var argument in operation.Arguments)
        {
            typeArgument = GetObservableElementType(argument.Value.Type);
            if (typeArgument is not null)
            {
                break;
            }
        }

        if (typeArgument is null || !IsNonNullableReferenceType(typeArgument) || HasInitialValue(operation.Arguments))
        {
            return;
        }

        Report(context, operation.Syntax.GetLocation(), returnType, typeArgument);
    }

    private static ITypeSymbol? GetObservableElementType(ITypeSymbol? type)
    {
        if (type is null)
        {
            return null;
        }

        if (type is INamedTypeSymbol named &&
            named.OriginalDefinition.SpecialType == SpecialType.None &&
            named.OriginalDefinition.ToDisplayString() == "System.IObservable<T>" &&
            named.TypeArguments.Length == 1)
        {
            return named.TypeArguments[0];
        }

        foreach (var @interface in type.AllInterfaces)
        {
            if (@interface.OriginalDefinition.ToDisplayString() == "System.IObservable<T>" &&
                @interface.TypeArguments.Length == 1)
            {
                return @interface.TypeArguments[0];
            }
        }

        return null;
    }

    private static void Report(OperationAnalysisContext context, Location location, INamedTypeSymbol type, ITypeSymbol typeArgument)
    {
        var diagnostic = Diagnostic.Create(
            s_rule,
            location,
            type.OriginalDefinition.Name,
            typeArgument.ToDisplayString());
        context.ReportDiagnostic(diagnostic);
    }

    private static bool HasInitialValue(ImmutableArray<IArgumentOperation> arguments)
    {
        foreach (var argument in arguments)
        {
            if (argument.Parameter?.Name == "initialValue")
            {
                return argument.ArgumentKind != ArgumentKind.DefaultValue;
            }
        }

        return false;
    }

    private static bool IsNonNullableReferenceType(ITypeSymbol typeSymbol)
    {
        // Skip open type parameters (e.g. usages inside generic code) to avoid false positives.
        if (typeSymbol.TypeKind == TypeKind.TypeParameter)
        {
            return false;
        }

        // Only reference types can be null.
        if (!typeSymbol.IsReferenceType)
        {
            return false;
        }

        // NotAnnotated: nullable context is enabled and the type is non-nullable (e.g. string).
        // Annotated (string?) and None (nullable context disabled) are intentionally ignored.
        return typeSymbol.NullableAnnotation == NullableAnnotation.NotAnnotated;
    }
}
