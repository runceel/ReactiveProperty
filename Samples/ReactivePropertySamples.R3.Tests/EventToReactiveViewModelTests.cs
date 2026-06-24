using System.Windows.Input;
using ReactivePropertySamples.Migrated.ViewModels;

namespace ReactivePropertySamples.R3.Tests;

[TestClass]
public class EventToReactiveViewModelTests
{
    [TestMethod]
    public void OpenFileCommand_Execute_SetsOpenedFile()
    {
        using var vm = new EventToReactiveViewModel();

        ((ICommand)vm.OpenFileCommand).Execute("C:\\sample.txt");

        Assert.AreEqual("You selected C:\\sample.txt", vm.OpenedFile.Value);
    }

    [TestMethod]
    public void OpenFileCommand_CanExecuteFollowsCanExecuteProperty()
    {
        using var vm = new EventToReactiveViewModel();
        var command = (ICommand)vm.OpenFileCommand;

        Assert.IsTrue(command.CanExecute(null), "CanExecute starts true.");

        vm.CanExecute.Value = false;
        Assert.IsFalse(command.CanExecute(null), "CanExecute false -> command cannot execute.");
    }
}
