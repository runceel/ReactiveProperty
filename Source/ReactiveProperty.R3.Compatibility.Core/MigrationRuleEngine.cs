using System;

namespace Reactive.Bindings.R3Compat;

public enum MigrationCategory
{
    Auto,
    Compat,
    Manual,
}

public sealed record MigrationRule(
    string RuleId,
    string Symbol,
    MigrationCategory Category,
    bool RequiresCompatibilityLayer,
    string DowngradeWhen,
    string DowngradeTarget,
    string Deprecation,
    bool AutoFix = false,
    string? Replacement = null);

public sealed record MigrationContext(bool DowngradeWhenSatisfied);

public sealed record MigrationClassification(
    string RuleId,
    MigrationCategory Category,
    bool RequiresCompatibilityLayer,
    string? Replacement,
    string DowngradeWhen,
    string Deprecation);

public static class MigrationRuleEngine
{
    public static MigrationClassification Classify(MigrationRule rule, MigrationContext context)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (context.DowngradeWhenSatisfied)
        {
            return new MigrationClassification(
                rule.RuleId,
                MigrationCategory.Auto,
                RequiresCompatibilityLayer: false,
                rule.DowngradeTarget,
                rule.DowngradeWhen,
                rule.Deprecation);
        }

        return new MigrationClassification(
            rule.RuleId,
            rule.Category,
            rule.RequiresCompatibilityLayer,
            rule.Replacement,
            rule.DowngradeWhen,
            rule.Deprecation);
    }
}
