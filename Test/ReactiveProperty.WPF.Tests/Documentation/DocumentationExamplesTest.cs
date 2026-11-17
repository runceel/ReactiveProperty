using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xaml.Behaviors;
using Reactive.Bindings.Interactivity;

namespace ReactiveProperty.Tests.Documentation;

[TestClass]
[DoNotParallelize]
public sealed class DocumentationExamplesTest
{
    private static readonly TaskCompletionSource<Dispatcher> s_dispatcher = new();
    private static Thread s_thread;
    private const string EventChapter = "Event-transfer-to-ViewModel-from-View";

    [AssemblyInitialize]
    public static void Initialize(TestContext _)
    {
        // ObserveOnUIDispatcher caches its first UI context for the process.
        // Keep that dispatcher alive until all tests in this assembly finish.
        s_thread = new Thread(() =>
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(dispatcher));
            s_dispatcher.SetResult(dispatcher);
            Dispatcher.Run();
        })
        {
            IsBackground = true,
            Name = "Documentation examples"
        };
        s_thread.SetApartmentState(ApartmentState.STA);
        s_thread.Start();
    }

    [AssemblyCleanup]
    public static void Cleanup()
    {
        var dispatcher = s_dispatcher.Task.GetAwaiter().GetResult();
        dispatcher.Invoke(dispatcher.InvokeShutdown);
        s_thread.Join();
    }

    private static void ForBothLanguages(string chapter, int page, Action<DocumentationWindow> verify)
    {
        s_dispatcher.Task.GetAwaiter().GetResult().Invoke(() =>
        {
            foreach (var locale in new[] { "docs", "docs-ja" })
            {
                using var example = new DocumentationWindow(locale, chapter, page);
                verify(example);
                example.AssertNoBindingErrors();
            }
        });
    }

    [TestMethod]
    public void ReactiveCollection_StartButton_PopulatesTheList()
    {
        ForBothLanguages("Collections", 0, example =>
        {
            var list = example.Controls<ListView>().Single();
            Assert.AreEqual(0, list.Items.Count);
            example.Click(example.Controls<Button>().Single());
            DocumentationWindow.Until(() => list.Items.Count > 0);
            Assert.IsInstanceOfType<DateTime>(list.Items[0]);
        });
    }

    [TestMethod]
    public void ReactiveCollection_ClearButton_ClearsTheDisplayedCollection()
    {
        ForBothLanguages("Collections", 1, example =>
        {
            var list = example.Controls<ListView>().Single();
            var buttons = example.Controls<Button>();
            example.Click(buttons[0]);
            DocumentationWindow.Until(() => list.Items.Count > 0);
            example.Click(buttons[1]);
            DocumentationWindow.Until(() => list.Items.Count == 0);
        });
    }

    [TestMethod]
    public void ReadOnlyCollection_ItemTemplate_RemovesThroughTheParentCommand()
    {
        ForBothLanguages("Collections", 2, example =>
        {
            var list = example.Controls<ListView>().Single();
            example.Click(example.Controls<Button>().Single());
            DocumentationWindow.Until(() => list.Items.Count == 1);
            DocumentationWindow.Until(() => example.Controls<Button>().Length == 2);
            DocumentationWindow.Until(() => example.Controls<TextBlock>().Any(x => x.Text.Contains("1")));
            var item = list.Items[0];
            var remove = example.Controls<Button>().Single(x => Equals(x.Content, "Remove"));
            Assert.AreSame(item, remove.CommandParameter);
            Assert.IsNotNull(remove.Command);
            example.Click(remove);
            DocumentationWindow.Until(() => list.Items.Count == 0);
        });
    }

    [TestMethod]
    public void ReadOnlyCollection_ResetButton_ClearsTheDisplayedMessages()
    {
        ForBothLanguages("Collections", 3, example =>
        {
            var list = example.Controls<ListView>().Single();
            DocumentationWindow.Until(() => list.Items.Count > 0);
            example.Click(example.Controls<Button>().Single());
            Assert.AreEqual(0, list.Items.Count);
        });
    }

    [TestMethod]
    public void FilteredCollection_SourceChanges_UpdateBothListsAndItemText()
    {
        ForBothLanguages("Collections", 4, example =>
        {
            example.Click(example.Controls<Button>().Single());
            var lists = example.Controls<ListView>();
            DocumentationWindow.Until(() => lists[0].Items.Count == 1);
            var item = lists[0].Items[0];
            // The documented model owns a periodic subscription; the host must
            // be able to stop it when the example is closed.
            Assert.IsInstanceOfType<IDisposable>(item);
            var value = item.GetType().GetProperty("Value");
            Assert.IsNotNull(value);
            value.SetValue(item, 9);
            DocumentationWindow.Drain();
            Assert.AreEqual(1, lists[1].Items.Count);
            Assert.AreSame(item, lists[1].Items[0]);
            Assert.IsTrue(example.Controls<TextBlock>().Any(x => x.Text.Contains("9")));
            value.SetValue(item, 2);
            DocumentationWindow.Drain();
            Assert.AreEqual(0, lists[1].Items.Count);
            Assert.AreEqual(1, lists[0].Items.Count);
        });
    }

    [TestMethod]
    public void ReactiveCommand_Button_UpdatesTheDisplayedTime()
    {
        ForBothLanguages("Commanding", 0, example =>
        {
            var output = example.Controls<TextBlock>().Last();
            Assert.AreEqual("", output.Text);
            example.Click(example.Controls<Button>().Single());
            Assert.IsTrue(DateTime.TryParse(output.Text, out _));
        });
    }

    [TestMethod]
    public void AsyncCommand_Running_DisablesThenReenablesTheButton()
    {
        ForBothLanguages("Commanding", 1, example =>
        {
            var button = example.Controls<Button>().Single();
            var output = example.Controls<TextBlock>().Last();
            example.Click(button);
            Assert.IsFalse(button.IsEnabled);
            Assert.AreEqual("Heavy command started.", output.Text);
            DocumentationWindow.Until(() => button.IsEnabled);
            Assert.AreEqual("Heavy command finished.", output.Text);
        });
    }

    [TestMethod]
    public void AsyncCommands_SharedState_DisablesBothUntilEachCommandFinishes()
    {
        ForBothLanguages("Commanding", 2, example =>
        {
            var buttons = example.Controls<Button>();
            var output = example.Controls<TextBlock>().Last();
            for (var i = 0; i < buttons.Length; i++)
            {
                example.Click(buttons[i]);
                Assert.IsTrue(buttons.All(x => !x.IsEnabled));
                Assert.AreEqual($"Heavy command {i + 1} started.", output.Text);
                DocumentationWindow.Until(() => buttons.All(x => x.IsEnabled));
                Assert.AreEqual($"Heavy command {i + 1} finished.", output.Text);
            }
        });
    }

    [TestMethod]
    public void AsyncCommands_ValidatedInput_ControlsSharedCanExecute()
    {
        ForBothLanguages("Commanding", 3, example =>
        {
            var buttons = example.Controls<Button>();
            var input = example.Controls<TextBox>().Single();
            Assert.IsTrue(buttons.All(x => !x.IsEnabled));
            DocumentationWindow.Enter(input, "valid");
            Assert.IsTrue(buttons.All(x => x.IsEnabled));
            foreach (var button in buttons)
            {
                example.Click(button);
                Assert.IsTrue(buttons.All(x => !x.IsEnabled));
                DocumentationWindow.Until(() => buttons.All(x => x.IsEnabled));
            }
            DocumentationWindow.Enter(input, "");
            Assert.IsTrue(buttons.All(x => !x.IsEnabled));
        });
    }

    [TestMethod]
    public void Validation_Input_DisplaysAndClearsTheErrorMessage()
    {
        ForBothLanguages("ReactiveProperty", 0, example =>
        {
            var input = example.Controls<TextBox>().Single();
            var error = example.Controls<TextBlock>().Last();
            Assert.IsFalse(string.IsNullOrWhiteSpace(error.Text));
            Assert.IsTrue(Validation.GetHasError(input));
            DocumentationWindow.Enter(input, "valid");
            Assert.AreEqual("", error.Text);
            Assert.IsFalse(Validation.GetHasError(input));
            DocumentationWindow.Enter(input, "");
            Assert.IsFalse(string.IsNullOrWhiteSpace(error.Text));
            Assert.IsTrue(Validation.GetHasError(input));
        });
    }

    [TestMethod]
    public void Validation_CombinedErrors_UpdatesTheReadOnlyCheckBox()
    {
        ForBothLanguages("ReactiveProperty", 1, example =>
        {
            var inputs = example.Controls<TextBox>();
            var errors = example.Controls<CheckBox>().Single();
            Assert.AreEqual(true, errors.IsChecked);
            Assert.IsFalse(errors.IsEnabled);
            DocumentationWindow.Enter(inputs[0], "name");
            Assert.AreEqual(true, errors.IsChecked);
            DocumentationWindow.Enter(inputs[1], "memo");
            Assert.AreEqual(false, errors.IsChecked);
            DocumentationWindow.Enter(inputs[0], "");
            Assert.AreEqual(true, errors.IsChecked);
        });
    }

    [TestMethod]
    public void Poco_TwoWayBinding_SynchronizesBothInputsAndModel()
    {
        ForBothLanguages("Work-together-with-plane-model-layer-objects", 0, example =>
        {
            var inputs = example.Controls<TextBox>();
            DocumentationWindow.Enter(inputs[0], "from ReactiveProperty");
            Assert.AreEqual("from ReactiveProperty", inputs[1].Text);
            DocumentationWindow.Enter(inputs[1], "from POCO");
            Assert.AreEqual("from POCO", inputs[0].Text);
            var person = DocumentationWindow.Property<object>(example.Page.DataContext, "Person");
            Assert.AreEqual("from POCO", DocumentationWindow.Property<string>(person, "Name"));
        });
    }

    [TestMethod]
    public void EventToCommand_Click_ConvertsAndNotifiesTheDisplayedValue()
    {
        VerifyEventTransfer(0);
    }

    [TestMethod]
    public void EventToProperty_Click_ConvertsAndNotifiesTheDisplayedValue()
    {
        VerifyEventTransfer(1);
    }

    private static void VerifyEventTransfer(int page)
    {
        ForBothLanguages(EventChapter, page, example =>
        {
            var button = example.Controls<Button>().Single();
            var trigger = Interaction.GetTriggers(button).Single();
            var original = trigger.Actions.Single();
            Assert.IsInstanceOfType<Microsoft.Xaml.Behaviors.TriggerAction>(original.Clone(), out var action);
            var converter = new ClickValueConverter();

            // Command actions capture their converter pipeline when attached.
            // Clone the XAML bindings, then set the test converter before attaching.
            // The unmodified file-picker converter is exercised in ManualTests.
            if (action is EventToReactiveCommand command)
            {
                Assert.IsNotNull(BindingOperations.GetBinding(command, EventToReactiveCommand.CommandProperty));
                Assert.AreEqual(0, command.Converters.Count);
                command.Converters.Add(converter);
            }
            else
            {
                Assert.IsInstanceOfType<EventToReactiveProperty>(action, out var property);
                Assert.IsNotNull(BindingOperations.GetBinding(property, EventToReactiveProperty.ReactivePropertyProperty));
                Assert.AreEqual(0, property.Converters.Count);
                property.Converters.Add(converter);
            }

            trigger.Actions.Remove(original);
            trigger.Actions.Add(action);
            try
            {
                var values = new List<string>();
                var fileName = DocumentationWindow.Property<IObservable<string>>(example.Page.DataContext, "FileName");
                using var subscription = fileName.Subscribe(values.Add);
                var initialCount = values.Count;
                var output = example.Controls<TextBlock>().Last();

                for (var i = 1; i <= 2; i++)
                {
                    example.Click(button);
                    var expected = $"converted Click {i}";
                    Assert.AreEqual(expected, output.Text);
                    Assert.AreEqual(initialCount + i, values.Count);
                    Assert.AreEqual(expected, values.Last());
                    Assert.AreEqual(i, converter.Invocations);
                    Assert.AreSame(button, converter.AssociateObject);
                }
            }
            finally
            {
                trigger.Actions.Remove(action);
            }
        });
    }

    private sealed class ClickValueConverter : ReactiveConverter<RoutedEventArgs, string>
    {
        public int Invocations { get; private set; }

        protected override IObservable<string> OnConvert(IObservable<RoutedEventArgs> source)
        {
            return source.Select(args =>
            {
                Assert.AreSame(AssociateObject, args.Source);
                return $"converted {args.RoutedEvent.Name} {++Invocations}";
            });
        }
    }
}
