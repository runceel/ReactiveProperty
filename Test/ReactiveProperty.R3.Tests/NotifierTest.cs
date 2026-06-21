#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3.Notifiers;

namespace ReactiveProperty.Tests;

[TestClass]
public sealed class NotifierTest
{
    [TestMethod]
    public void BooleanNotifierTurnOnTurnOffSwitchValue()
    {
        var notifier = new BooleanNotifier();
        var values = new List<bool>();
        var propertyChanges = new List<string?>();
        notifier.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
        using var subscription = notifier.Subscribe(values.Add);

        notifier.Value.IsFalse();

        notifier.TurnOn();
        notifier.Value.IsTrue();
        notifier.TurnOn(); // no-op, value already true

        notifier.TurnOff();
        notifier.Value.IsFalse();

        notifier.SwitchValue();
        notifier.Value.IsTrue();

        values.Is(true, false, true);
        propertyChanges.Count.Is(3);
        foreach (var name in propertyChanges)
        {
            name.Is(nameof(BooleanNotifier.Value));
        }
    }

    [TestMethod]
    public void BooleanNotifierInitialValue()
    {
        var notifier = new BooleanNotifier(true);
        notifier.Value.IsTrue();
    }

    [TestMethod]
    public void CountNotifierIncrementDecrementStatusSequence()
    {
        var notifier = new CountNotifier(max: 2);
        var statuses = new List<CountChangedStatus>();
        using var subscription = notifier.Subscribe(statuses.Add);

        notifier.Increment();
        notifier.Count.Is(1);
        notifier.Increment();
        notifier.Count.Is(2);
        notifier.Increment(); // already at max, no-op

        notifier.Decrement();
        notifier.Count.Is(1);
        notifier.Decrement();
        notifier.Count.Is(0);

        statuses.Is(
            CountChangedStatus.Increment,
            CountChangedStatus.Increment,
            CountChangedStatus.Max,
            CountChangedStatus.Decrement,
            CountChangedStatus.Decrement,
            CountChangedStatus.Empty);
    }

    [TestMethod]
    public void CountNotifierIncrementAutoDecrementsOnDispose()
    {
        var notifier = new CountNotifier();
        var handle = notifier.Increment(3);
        notifier.Count.Is(3);

        handle.Dispose();
        notifier.Count.Is(0);
    }

    [TestMethod]
    public void CountNotifierInvalidMaxThrows()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new CountNotifier(0));
    }

    [TestMethod]
    public void CountNotifierIncrementClampedAtMaxDisposableDecrementsFullAmount()
    {
        // When incrementCount would exceed Max, Count is clamped to Max,
        // but the returned disposable decrements by the full requested incrementCount.
        // This matches legacy behavior: the decrement can underflow to 0 (clamped there too).
        var notifier = new CountNotifier(max: 3);
        notifier.Increment(2); // Count = 2
        var handle = notifier.Increment(5); // would be 7, clamped to Max=3; disposable decrements by 5
        notifier.Count.Is(3);

        handle.Dispose(); // 3 - 5 = -2, clamped to 0
        notifier.Count.Is(0);
    }

    [TestMethod]
    public void BusyNotifierReferenceCounting()
    {
        var notifier = new BusyNotifier();
        var values = new List<bool>();
        using var subscription = notifier.Subscribe(values.Add);

        notifier.IsBusy.IsFalse();

        var first = notifier.ProcessStart();
        notifier.IsBusy.IsTrue();
        var second = notifier.ProcessStart();
        notifier.IsBusy.IsTrue();

        first.Dispose();
        notifier.IsBusy.IsTrue();
        second.Dispose();
        notifier.IsBusy.IsFalse();

        // First emission replays current value (false) on subscribe.
        values.Is(false, true, false);
    }

    [TestMethod]
    public void BusyNotifierReplaysCurrentValueOnSubscribe()
    {
        var notifier = new BusyNotifier();
        using var process = notifier.ProcessStart();

        var values = new List<bool>();
        using var subscription = notifier.Subscribe(values.Add);

        values.Is(true);
    }

    [TestMethod]
    public void ScheduledNotifierReportImmediate()
    {
        var notifier = new ScheduledNotifier<int>();
        var values = new List<int>();
        using var subscription = notifier.Subscribe(values.Add);

        notifier.Report(1);
        notifier.Report(2);

        values.Is(1, 2);
    }

    [TestMethod]
    public void ScheduledNotifierReportWithDueTime()
    {
        var timeProvider = new Microsoft.Extensions.Time.Testing.FakeTimeProvider();
        var notifier = new ScheduledNotifier<int>(timeProvider);
        var values = new List<int>();
        using var subscription = notifier.Subscribe(values.Add);

        notifier.Report(10, TimeSpan.FromSeconds(5));
        values.Count.Is(0);

        timeProvider.Advance(TimeSpan.FromSeconds(4));
        values.Count.Is(0);

        timeProvider.Advance(TimeSpan.FromSeconds(1));
        values.Is(10);
    }

    [TestMethod]
    public void ScheduledNotifierReportWithDueTimeCanBeCancelled()
    {
        var timeProvider = new Microsoft.Extensions.Time.Testing.FakeTimeProvider();
        var notifier = new ScheduledNotifier<int>(timeProvider);
        var values = new List<int>();
        using var subscription = notifier.Subscribe(values.Add);

        var handle = notifier.Report(10, TimeSpan.FromSeconds(5));
        handle.Dispose();

        timeProvider.Advance(TimeSpan.FromSeconds(10));
        values.Count.Is(0);
    }

    [TestMethod]
    public void ScheduledNotifierReportWithDateTimeOffset()
    {
        var timeProvider = new Microsoft.Extensions.Time.Testing.FakeTimeProvider();
        var notifier = new ScheduledNotifier<int>(timeProvider);
        var values = new List<int>();
        using var subscription = notifier.Subscribe(values.Add);

        notifier.Report(10, timeProvider.GetUtcNow().AddSeconds(3));
        timeProvider.Advance(TimeSpan.FromSeconds(2));
        values.Count.Is(0);
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        values.Is(10);
    }
}
