using Microsoft.CodeAnalysis.Testing;
using Reactive.Bindings.Analyzer;
using Verify = ReactiveProperty.Analyzer.Tests.AnalyzerVerifier<Reactive.Bindings.Analyzer.ReactivePropertyNullableAnalyzer>;

namespace ReactiveProperty.Analyzer.Tests;

[TestClass]
public class ReactivePropertyNullableAnalyzerTest
{
    [TestMethod]
    public async Task ReactiveProperty_NonNullableReferenceType_WithoutInitialValue_Reports()
    {
        var source = """
            using Reactive.Bindings;
            class C
            {
                void M()
                {
                    var x = {|#0:new ReactiveProperty<string>()|};
                }
            }
            """;
        var expected = Verify.Diagnostic(ReactivePropertyNullableAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("ReactiveProperty", "string");
        await Verify.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task ReactivePropertySlim_NonNullableReferenceType_WithoutInitialValue_Reports()
    {
        var source = """
            using Reactive.Bindings;
            class C
            {
                void M()
                {
                    var x = {|#0:new ReactivePropertySlim<string>()|};
                }
            }
            """;
        var expected = Verify.Diagnostic(ReactivePropertyNullableAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("ReactivePropertySlim", "string");
        await Verify.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task ReactivePropertySlim_WithModeButNoInitialValue_Reports()
    {
        var source = """
            using Reactive.Bindings;
            class C
            {
                void M()
                {
                    var x = {|#0:new ReactivePropertySlim<string>(mode: ReactivePropertyMode.None)|};
                }
            }
            """;
        var expected = Verify.Diagnostic(ReactivePropertyNullableAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("ReactivePropertySlim", "string");
        await Verify.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task ReactivePropertySlim_WithInitialValue_DoesNotReport()
    {
        var source = """
            using Reactive.Bindings;
            class C
            {
                void M()
                {
                    var x = new ReactivePropertySlim<string>("initial");
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task ReactivePropertySlim_NullableReferenceType_DoesNotReport()
    {
        var source = """
            using Reactive.Bindings;
            class C
            {
                void M()
                {
                    var x = new ReactivePropertySlim<string?>();
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task ReactivePropertySlim_ValueType_DoesNotReport()
    {
        var source = """
            using Reactive.Bindings;
            class C
            {
                void M()
                {
                    var x = new ReactivePropertySlim<int>();
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task ReactivePropertySlim_NullableContextDisabled_DoesNotReport()
    {
        var source = """
            #nullable disable
            using Reactive.Bindings;
            class C
            {
                void M()
                {
                    var x = new ReactivePropertySlim<string>();
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task ReadOnlyReactivePropertySlim_Constructor_NonNullableReferenceType_Reports()
    {
        var source = """
            using System;
            using Reactive.Bindings;
            class C
            {
                void M(IObservable<string> source)
                {
                    var x = {|#0:new ReadOnlyReactivePropertySlim<string>(source)|};
                }
            }
            """;
        var expected = Verify.Diagnostic(ReactivePropertyNullableAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("ReadOnlyReactivePropertySlim", "string");
        await Verify.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task ToReadOnlyReactivePropertySlim_NonNullableReferenceType_WithoutInitialValue_Reports()
    {
        var source = """
            using System;
            using Reactive.Bindings;
            class C
            {
                void M(IObservable<string> source)
                {
                    var x = {|#0:source.ToReadOnlyReactivePropertySlim()|};
                }
            }
            """;
        var expected = Verify.Diagnostic(ReactivePropertyNullableAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("ReadOnlyReactivePropertySlim", "string");
        await Verify.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task ToReadOnlyReactivePropertySlim_WithInitialValue_DoesNotReport()
    {
        var source = """
            using System;
            using Reactive.Bindings;
            class C
            {
                void M(IObservable<string> source)
                {
                    var x = source.ToReadOnlyReactivePropertySlim("initial");
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task ToReadOnlyReactivePropertySlim_NullableReferenceTypeSource_DoesNotReport()
    {
        var source = """
            using System;
            using Reactive.Bindings;
            class C
            {
                void M(IObservable<string?> source)
                {
                    var x = source.ToReadOnlyReactivePropertySlim();
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task ToReadOnlyReactiveProperty_NonNullableReferenceType_WithoutInitialValue_Reports()
    {
        var source = """
            using System;
            using Reactive.Bindings;
            class C
            {
                void M(IObservable<string> source)
                {
                    var x = {|#0:source.ToReadOnlyReactiveProperty()|};
                }
            }
            """;
        var expected = Verify.Diagnostic(ReactivePropertyNullableAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("ReadOnlyReactiveProperty", "string");
        await Verify.VerifyAnalyzerAsync(source, expected);
    }
}
