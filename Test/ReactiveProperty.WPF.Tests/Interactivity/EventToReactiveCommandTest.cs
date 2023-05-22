using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reactive.Bindings;
using Reactive.Bindings.Interactivity;

namespace ReactiveProperty.Tests.Interactivity;

[TestClass]
public class EventToReactiveCommandTest
{
    [STATestMethod]
    public void Test_CallExecuteOnScheduler_TrueCase()
    {
        var scheduler = new MockScheduler();
        ReactivePropertyScheduler.SetDefault(scheduler);

        var commandArgument = new object();
        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(x => x.Execute(It.Is<object>(y => y == commandArgument)))
            .Verifiable();
        mockCommand.Setup(x => x.CanExecute(It.IsAny<object>()))
            .Returns(true);

        var attachedTarget = new FrameworkElement();
        var target = new EventToReactiveCommand
        {
            Command = mockCommand.Object,
        };
        target.Attach(attachedTarget);

        target.AsDynamic().Invoke(commandArgument);

        scheduler.CallHistory.Count.Is(1);
        mockCommand.VerifyAll();
    }

    [STATestMethod]
    public void Test_CallExecuteOnScheduler_FalseCase()
    {
        var scheduler = new MockScheduler();
        ReactivePropertyScheduler.SetDefault(scheduler);

        var commandArgument = new object();
        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(x => x.Execute(It.Is<object>(y => y == commandArgument)))
            .Verifiable();
        mockCommand.Setup(x => x.CanExecute(It.IsAny<object>()))
            .Returns(true);

        var attachedTarget = new FrameworkElement();
        var target = new EventToReactiveCommand
        {
            CallExecuteOnScheduler = false,
            Command = mockCommand.Object,
        };
        target.Attach(attachedTarget);

        target.AsDynamic().Invoke(commandArgument);

        scheduler.CallHistory.Any().Is(false);
        mockCommand.VerifyAll();
    }

    [STATestMethod]
    public void Test_CallExecuteOnScheduler_ChangePropertyAfterInitialized()
    {
        var scheduler = new MockScheduler();
        ReactivePropertyScheduler.SetDefault(scheduler);

        var commandArgument = new object();
        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(x => x.Execute(It.Is<object>(y => y == commandArgument)))
            .Verifiable();
        mockCommand.Setup(x => x.CanExecute(It.IsAny<object>()))
            .Returns(true);

        var attachedTarget = new FrameworkElement();
        var target = new EventToReactiveCommand
        {
            CallExecuteOnScheduler = false,
            Command = mockCommand.Object,
        };
        target.Attach(attachedTarget);

        target.CallExecuteOnScheduler = true;

        target.AsDynamic().Invoke(commandArgument);

        scheduler.CallHistory.Count.Is(1);
        mockCommand.VerifyAll();
    }

    private class MockScheduler : IScheduler
    {
        private readonly ImmediateScheduler _innerScheduler = ImmediateScheduler.Instance;
        public DateTimeOffset Now => _innerScheduler.Now;

        public List<object> CallHistory { get; } = new List<object>();

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            CallHistory.Add(state);
            return _innerScheduler.Schedule(state, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            CallHistory.Add(state);
            return _innerScheduler.Schedule(state, dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            CallHistory.Add(state);
            return _innerScheduler.Schedule(state, dueTime, action);
        }
    }
}
