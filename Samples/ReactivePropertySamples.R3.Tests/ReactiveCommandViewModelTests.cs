using System.Windows.Input;
using ReactivePropertySamples.Migrated.ViewModels;

namespace ReactivePropertySamples.R3.Tests;

[TestClass]
public class ReactiveCommandViewModelTests
{
    [TestMethod]
    public void CreateAndSubscribeCommand_Execute_SetsInvokedOutputs()
    {
        using var vm = new ReactiveCommandViewModel();
        Assert.AreEqual("", vm.InvokedDateTime.Value);

        ((ICommand)vm.CreateAndSubscribeCommand).Execute(null);

        StringAssert.Contains(vm.InvokedDateTime.Value, "command was invoked");
        StringAssert.Contains(vm.InvokedDateTimeFromCommand.Value, "command was invoked");
    }

    [TestMethod]
    public void CreateFromIObservableBoolCommand_CanExecuteFollowsIsChecked()
    {
        using var vm = new ReactiveCommandViewModel();
        var command = (ICommand)vm.CreateFromIObservableBoolCommand;

        Assert.IsFalse(command.CanExecute(null), "IsChecked starts false.");

        vm.IsChecked.Value = true;
        Assert.IsTrue(command.CanExecute(null), "IsChecked true -> command can execute.");
    }

    [TestMethod]
    public void ReactiveCommandWithParameter_Execute_PassesParameter()
    {
        using var vm = new ReactiveCommandViewModel();

        ((ICommand)vm.ReactiveCommandWithParameter).Execute("abc");

        Assert.AreEqual("The command was invoked with abc.", vm.ReactiveCommandWithParameterOutput.Value);
    }
}
