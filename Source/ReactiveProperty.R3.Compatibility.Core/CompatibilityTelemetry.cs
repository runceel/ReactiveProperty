using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Reactive.Bindings.R3Compat;

/// <summary>
/// Records temporary compatibility API usage so migration CI can aggregate it.
/// </summary>
public static class CompatibilityTelemetry
{
    private static readonly object Gate = new();
    private static readonly Dictionary<string, int> Usage = new();

    /// <summary>
    /// Records one compatibility API usage.
    /// </summary>
    /// <param name="compatApiId">The stable compatibility API identifier.</param>
    /// <param name="callSite">A caller-provided call-site label for reports.</param>
    public static void Track(string compatApiId, string callSite)
    {
        if (string.IsNullOrWhiteSpace(compatApiId))
        {
            throw new ArgumentException("Compatibility API id must be specified.", nameof(compatApiId));
        }

        var key = string.IsNullOrWhiteSpace(callSite) ? compatApiId : compatApiId + "@" + callSite;
        lock (Gate)
        {
            Usage.TryGetValue(key, out var count);
            Usage[key] = count + 1;
        }
    }

    /// <summary>
    /// Returns a stable snapshot of compatibility usage counts.
    /// </summary>
    public static IReadOnlyDictionary<string, int> Snapshot()
    {
        lock (Gate)
        {
            return new ReadOnlyDictionary<string, int>(Usage.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value));
        }
    }

    /// <summary>
    /// Writes the usage report in JSON format.
    /// </summary>
    /// <param name="path">The destination path, typically compat-usage.json.</param>
    public static void WriteUsageReport(string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var snapshot = Snapshot();
        var directory = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var builder = new StringBuilder();
        builder.AppendLine("{");
        builder.AppendLine("  \"usage\": {");
        var index = 0;
        foreach (var item in snapshot)
        {
            builder.Append("    \"").Append(Escape(item.Key)).Append("\": ").Append(item.Value);
            builder.AppendLine(index++ == snapshot.Count - 1 ? string.Empty : ",");
        }

        builder.AppendLine("  }");
        builder.AppendLine("}");
        File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
    }

    internal static void ResetForTest()
    {
        lock (Gate)
        {
            Usage.Clear();
        }
    }

    private static string Escape(string value) => value.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
