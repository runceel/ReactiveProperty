using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Reactive.Bindings.R3Compat;

[Flags]
public enum CompatibilityMode
{
    None = 0,
    DistinctUntilChanged = 1,
    RaiseLatestValueOnSubscribe = 2,
    IgnoreInitialValidationError = 4,
    IgnoreException = 8,
    Default = DistinctUntilChanged | RaiseLatestValueOnSubscribe,
}

public sealed class CompatibilityOptions
{
    public CompatibilityMode Mode { get; init; } = CompatibilityMode.Default;

    public TimeProvider TimeProvider { get; init; } = TimeProvider.System;

    public bool IgnoreInitialValidationError => Mode.HasFlag(CompatibilityMode.IgnoreInitialValidationError);
}

public static class CompatibilityTelemetry
{
    private static readonly ConcurrentDictionary<string, int> Usage = new();

    public static void Track(string compatApiId, string callSite)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(compatApiId);
        Usage.AddOrUpdate(compatApiId, 1, static (_, count) => count + 1);
    }

    public static IReadOnlyDictionary<string, int> Snapshot() => new ReadOnlyDictionary<string, int>(Usage.ToDictionary());

    public static void WriteUsageReport(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(path, JsonSerializer.Serialize(Snapshot(), new JsonSerializerOptions { WriteIndented = true }));
    }
}

public interface IReactiveProperty<T> : INotifyPropertyChanged
{
    T Value { get; set; }
}

public sealed class CompatReactiveProperty<T> : IReactiveProperty<T>, IDisposable
{
    private T _value;
    private bool _isDisposed;

    public CompatReactiveProperty(T value) => _value = value;

    public event PropertyChangedEventHandler? PropertyChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (_isDisposed || EqualityComparer<T>.Default.Equals(_value, value))
            {
                return;
            }

            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    public void Dispose() => _isDisposed = true;
}
