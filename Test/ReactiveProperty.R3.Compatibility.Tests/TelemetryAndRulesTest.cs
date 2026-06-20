using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.R3Compat;

namespace ReactiveProperty.R3.Compatibility.Tests;

[TestClass]
public class TelemetryAndRulesTest
{
    [TestMethod]
    public void TelemetryWritesUsageReport()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "compat-usage.json");
        CompatibilityTelemetry.Track("test-api", "test-callsite");

        CompatibilityTelemetry.WriteUsageReport(path);

        var json = File.ReadAllText(path);
        Assert.IsTrue(json.Contains("test-api@test-callsite"));
    }

    [TestMethod]
    public void RuleEngineDowngradeRulesDocumentCompatExitPath()
    {
        var rulesPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../..", "skills/migrating-reactiveproperty-to-r3/references/rules.json"));
        using var document = JsonDocument.Parse(File.ReadAllText(rulesPath));
        var streamRule = document.RootElement.GetProperty("rules").EnumerateArray().Single(x => x.GetProperty("ruleId").GetString() == "RP-VAL-001");

        Assert.IsTrue(streamRule.GetProperty("requiresCompatibilityLayer").GetBoolean());
        Assert.IsFalse(string.IsNullOrWhiteSpace(streamRule.GetProperty("downgradeWhen").GetString()));
        Assert.AreEqual("BindableReactiveProperty<T>.EnableValidation", streamRule.GetProperty("downgradeTarget").GetString());
    }
}
