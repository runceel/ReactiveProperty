#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3;

namespace ReactiveProperty.Tests;

[TestClass]
public sealed class ReactiveTimerTest
{
    [TestMethod]
    public void StartStopReset()
    {
        var timeProvider = new FakeTimeProvider();
        using var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), timeProvider);
        var values = new List<long>();
        using var subscription = timer.Subscribe(values.Add);

        timer.IsEnabled.IsFalse();
        timer.Start();
        timer.IsEnabled.IsTrue();

        timeProvider.Advance(TimeSpan.FromSeconds(1));
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        values.Is(0L, 1L, 2L);

        timer.Stop();
        timer.IsEnabled.IsFalse();
        timeProvider.Advance(TimeSpan.FromSeconds(5));
        values.Is(0L, 1L, 2L);

        // Restart continues the count.
        timer.Start();
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        values.Is(0L, 1L, 2L, 3L);

        // Reset stops and clears the count.
        timer.Reset();
        timer.IsEnabled.IsFalse();
        timer.Start();
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        values.Is(0L, 1L, 2L, 3L, 0L);
    }

    [TestMethod]
    public void StartWithDueTime()
    {
        var timeProvider = new FakeTimeProvider();
        using var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), timeProvider);
        var values = new List<long>();
        using var subscription = timer.Subscribe(values.Add);

        timer.Start(TimeSpan.FromSeconds(3));
        timeProvider.Advance(TimeSpan.FromSeconds(2));
        values.Count.Is(0);

        timeProvider.Advance(TimeSpan.FromSeconds(1));
        values.Is(0L);

        timeProvider.Advance(TimeSpan.FromSeconds(1));
        values.Is(0L, 1L);
    }

    [TestMethod]
    public void DisposeCompletesSubscribers()
    {
        var timeProvider = new FakeTimeProvider();
        var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), timeProvider);
        var completed = false;
        using var subscription = timer.Subscribe(_ => { }, _ => completed = true);

        timer.Dispose();
        completed.IsTrue();

        // Dispose is idempotent.
        timer.Dispose();
    }

    [TestMethod]
    public void DisposeResetsIsEnabled()
    {
        var timeProvider = new FakeTimeProvider();
        var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), timeProvider);
        var propertyChanges = new List<string?>();
        timer.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);

        timer.Start();
        timer.IsEnabled.IsTrue();

        timer.Dispose();
        timer.IsEnabled.IsFalse();

        // PropertyChanged should have fired for IsEnabled (true on Start, false on Dispose).
        propertyChanges.Contains(nameof(ReactiveTimer.IsEnabled)).IsTrue();
    }

    [TestMethod]
    public void IntervalChangeRaisesPropertyChanged()
    {
        var timeProvider = new FakeTimeProvider();
        using var timer = new ReactiveTimer(TimeSpan.FromSeconds(1), timeProvider);
        var changes = new List<string?>();
        timer.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

        timer.Interval = TimeSpan.FromSeconds(2);
        changes.Is(nameof(ReactiveTimer.Interval));

        timer.Interval = TimeSpan.FromSeconds(2); // no change
        changes.Is(nameof(ReactiveTimer.Interval));
    }
}
