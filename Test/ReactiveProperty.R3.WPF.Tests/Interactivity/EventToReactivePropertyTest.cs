#nullable enable
using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3.Interactivity;

namespace ReactiveProperty.R3.Tests.Interactivity;

[TestClass]
public class EventToReactivePropertyTest
{
    [STATestMethod]
    public void Invoke_UpdatesReactivePropertyWithConvertedValue()
    {
        var previous = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new ImmediateSynchronizationContext());
        try
        {
            var property = new BindableReactiveProperty<string>();
            var attachedTarget = new FrameworkElement();
            var target = new EventToReactiveProperty
            {
                ReactiveProperty = property,
            };
            target.Converters.Add(new EventArgsToStringConverter());
            target.Attach(attachedTarget);

            Invoke(target, EventArgs.Empty);

            property.Value.Is("converted");
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
        }
    }

    [STATestMethod]
    public void Invoke_IgnoreEventArgs_PushesUnitThroughConverters()
    {
        var previous = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new ImmediateSynchronizationContext());
        try
        {
            var property = new BindableReactiveProperty<string>();
            var attachedTarget = new FrameworkElement();
            var target = new EventToReactiveProperty
            {
                ReactiveProperty = property,
                IgnoreEventArgs = true,
            };
            target.Converters.Add(new UnitToStringConverter());
            target.Attach(attachedTarget);

            Invoke(target, EventArgs.Empty);

            property.Value.Is("unit");
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
        }
    }

    private sealed class EventArgsToStringConverter : ReactiveConverter<EventArgs, string>
    {
        protected override Observable<string?> OnConvert(Observable<EventArgs?> source) =>
            source.Select(static _ => (string?)"converted");
    }

    private sealed class UnitToStringConverter : ReactiveConverter<Unit, string>
    {
        protected override Observable<string?> OnConvert(Observable<Unit> source) =>
            source.Select(static _ => (string?)"unit");
    }

    private static void Invoke(Microsoft.Xaml.Behaviors.TriggerAction action, object parameter) =>
        typeof(Microsoft.Xaml.Behaviors.TriggerAction)
            .GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(action, new[] { parameter });

    private sealed class ImmediateSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object? state) => d(state);

        public override void Send(SendOrPostCallback d, object? state) => d(state);
    }
}
