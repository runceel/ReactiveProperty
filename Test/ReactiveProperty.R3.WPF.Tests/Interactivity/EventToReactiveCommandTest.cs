#nullable enable
using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using R3;
using Reactive.Bindings.R3.Interactivity;

namespace ReactiveProperty.R3.Tests.Interactivity;

[TestClass]
public class EventToReactiveCommandTest
{
    [STATestMethod]
    public void Invoke_CallExecuteOnSchedulerFalse_ExecutesCommandWithConvertedValue()
    {
        var command = new Mock<ICommand>();
        command.Setup(x => x.CanExecute(It.IsAny<object>())).Returns(true);
        command.Setup(x => x.Execute("converted")).Verifiable();

        var attachedTarget = new FrameworkElement();
        var target = new EventToReactiveCommand
        {
            CallExecuteOnScheduler = false,
            Command = command.Object,
        };
        target.Converters.Add(new EventArgsToStringConverter());
        target.Attach(attachedTarget);

        Invoke(target, EventArgs.Empty);

        command.Verify(x => x.Execute("converted"), Times.Once);
    }

    [STATestMethod]
    public void Invoke_CallExecuteOnSchedulerTrue_ExecutesCommandWithConvertedValue()
    {
        var previous = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new ImmediateSynchronizationContext());
        try
        {
            var command = new Mock<ICommand>();
            command.Setup(x => x.CanExecute(It.IsAny<object>())).Returns(true);
            command.Setup(x => x.Execute("converted")).Verifiable();

            var attachedTarget = new FrameworkElement();
            var target = new EventToReactiveCommand
            {
                Command = command.Object,
            };
            target.Converters.Add(new EventArgsToStringConverter());
            target.Attach(attachedTarget);

            Invoke(target, EventArgs.Empty);

            command.Verify(x => x.Execute("converted"), Times.Once);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
        }
    }

    [STATestMethod]
    public void Attach_AutoEnable_DisablesElementWhenCommandCannotExecute()
    {
        var command = new Mock<ICommand>();
        command.Setup(x => x.CanExecute(It.IsAny<object>())).Returns(false);

        var attachedTarget = new FrameworkElement();
        var target = new EventToReactiveCommand
        {
            Command = command.Object,
        };
        target.Attach(attachedTarget);

        attachedTarget.IsEnabled.Is(false);
    }

    private sealed class EventArgsToStringConverter : ReactiveConverter<EventArgs, string>
    {
        protected override Observable<string?> OnConvert(Observable<EventArgs?> source) =>
            source.Select(static _ => (string?)"converted");
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
