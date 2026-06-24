using System.ComponentModel;
using System.Windows.Input;
using ReactivePropertySamples.Migrated.ViewModels;

namespace ReactivePropertySamples.R3.Tests;

[TestClass]
public class ValidationViewModelTests
{
    [TestMethod]
    public void WithDataAnnotations_EmptyIsInvalid_NonEmptyClearsError()
    {
        using var vm = new ValidationViewModel();

        vm.WithDataAnnotations.Value = "valid";
        Assert.AreEqual("", vm.WithDataAnnotationsErrorMessage.Value);

        vm.WithDataAnnotations.Value = "";
        Assert.AreEqual("Required property", vm.WithDataAnnotationsErrorMessage.Value);
    }

    [TestMethod]
    public void WithCustomValidationLogic_RequiresHyphen()
    {
        using var vm = new ValidationViewModel();

        // The bridge ValidatableReactiveProperty validates eagerly, so the initial empty value fails.
        Assert.AreEqual("Require '-'", vm.WithCustomValidationLogicErrorMessage.Value);

        vm.WithCustomValidationLogic.Value = "a-b";
        Assert.AreEqual("", vm.WithCustomValidationLogicErrorMessage.Value);
    }

    [TestMethod]
    public void IgnoreInitialValidationError_StartsValid_ThenFlagsAfterEdit()
    {
        using var vm = new ValidationViewModel();

        var errorInfo = (INotifyDataErrorInfo)vm.IgnoreInitialValidationError;
        Assert.IsFalse(errorInfo.HasErrors, "Initial validation error should be suppressed.");

        vm.IgnoreInitialValidationError.Value = "x";
        vm.IgnoreInitialValidationError.Value = "";
        Assert.IsTrue(errorInfo.HasErrors, "Validation should flag the empty value after an edit.");
    }

    [TestMethod]
    public void FirstName_TwoWaySyncsWithPoco()
    {
        using var vm = new ValidationViewModel();

        Assert.AreEqual("Kazuki", vm.FirstName.Value);

        vm.FirstName.Value = "Taro";
        Assert.AreEqual("Taro", vm.Poco.FirstName);

        vm.Poco.FirstName = "Jiro";
        Assert.AreEqual("Jiro", vm.FirstName.Value);
    }

    [TestMethod]
    public void SubmitCommand_CanExecuteReflectsValidity()
    {
        using var vm = new ValidationViewModel();
        var command = (ICommand)vm.SubmitCommand;

        Assert.IsFalse(command.CanExecute(null), "Both fields start invalid.");

        vm.WithDataAnnotations.Value = "ok";
        vm.WithCustomValidationLogic.Value = "a-b";
        Assert.IsTrue(command.CanExecute(null), "Both fields valid -> command can execute.");
    }
}
