using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reactive.Bindings;
using Reactive.Bindings.Interactivity;

namespace ReactiveProperty.Tests.Interactivity;

[TestClass]
public class EventToReactiveBehaviorTest
{
    [STATestMethod]
    public void EventToReactiveCommandBehavior_EventRaised_ExecutesCommandWithConvertedValue()
    {
        var eventArgs = EventArgs.Empty;
        var command = new Mock<ICommand>();
        command.Setup(x => x.CanExecute(It.IsAny<object>())).Returns(true);
        command.Setup(x => x.Execute("converted")).Verifiable();
        var associatedObject = new TestFrameworkElement();
        var behavior = new EventToReactiveCommandBehavior
        {
            Command = command.Object,
            EventName = nameof(TestFrameworkElement.TestEvent),
            CallExecuteOnScheduler = false,
        };
        behavior.Converters.Add(new TestConverter());
        behavior.Attach(associatedObject);

        associatedObject.RaiseTestEvent(eventArgs);

        command.Verify(x => x.Execute("converted"), Times.Once);
    }

    [STATestMethod]
    public void EventToReactivePropertyBehavior_EventRaised_UpdatesReactivePropertyWithConvertedValue()
    {
        var associatedObject = new TestFrameworkElement();
        var property = new ReactiveProperty<string>();
        var behavior = new EventToReactivePropertyBehavior
        {
            EventName = nameof(TestFrameworkElement.TestEvent),
            ReactiveProperty = property,
        };
        behavior.Converters.Add(new TestConverter());
        behavior.Attach(associatedObject);

        associatedObject.RaiseTestEvent(EventArgs.Empty);

        property.Value.Is("converted");
    }

    private sealed class TestFrameworkElement : FrameworkElement
    {
        public event EventHandler<EventArgs> TestEvent;

        public void RaiseTestEvent(EventArgs eventArgs)
        {
            TestEvent?.Invoke(this, eventArgs);
        }
    }

    private sealed class TestConverter : ReactiveConverter<EventArgs, string>
    {
        protected override IObservable<string> OnConvert(IObservable<EventArgs> source)
        {
            return source.Select(_ => "converted");
        }
    }
}
