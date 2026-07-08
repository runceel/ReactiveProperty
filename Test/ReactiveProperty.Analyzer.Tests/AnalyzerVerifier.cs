using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace ReactiveProperty.Analyzer.Tests;

internal static class AnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic(string diagnosticId) =>
        CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(diagnosticId);

    public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new Test
        {
            TestCode = source,
        };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }

    private sealed class Test : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    {
        public Test()
        {
            ReferenceAssemblies = new ReferenceAssemblies(
                "net9.0",
                new PackageIdentity("Microsoft.NETCore.App.Ref", "9.0.0"),
                Path.Combine("ref", "net9.0"));
            SolutionTransforms.Add((solution, projectId) =>
            {
                var project = solution.GetProject(projectId)!;
                var parseOptions = ((CSharpParseOptions)project.ParseOptions!).WithLanguageVersion(LanguageVersion.Latest);
                var compilationOptions = ((CSharpCompilationOptions)project.CompilationOptions!)
                    .WithNullableContextOptions(NullableContextOptions.Enable);

                solution = solution
                    .WithProjectParseOptions(projectId, parseOptions)
                    .WithProjectCompilationOptions(projectId, compilationOptions);

                foreach (var reference in ReactivePropertyReferences.Value)
                {
                    solution = solution.AddMetadataReference(projectId, reference);
                }

                return solution;
            });
        }
    }

    private static readonly Lazy<ImmutableArray<MetadataReference>> ReactivePropertyReferences = new(() =>
    {
        var assemblies = new[]
        {
            typeof(Reactive.Bindings.ReactivePropertySlim<>).Assembly,
            typeof(Reactive.Bindings.ReactiveProperty<>).Assembly,
            typeof(System.Reactive.Linq.Observable).Assembly,
        };

        return assemblies
            .Select(x => (MetadataReference)MetadataReference.CreateFromFile(x.Location))
            .ToImmutableArray();
    });
}
