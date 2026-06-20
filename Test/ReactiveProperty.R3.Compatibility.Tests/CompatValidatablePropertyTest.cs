using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3Compat;
using Reactive.Bindings.R3Compat.Validation;

namespace ReactiveProperty.R3.Compatibility.Tests;

[TestClass]
public class CompatValidatablePropertyTest
{
    [TestMethod]
    public void InitialValueValidationIsSuppressedByOption()
    {
        using var property = new CompatValidatableProperty<string>("", new CompatibilityOptions
        {
            Mode = CompatibilityMode.Default | CompatibilityMode.IgnoreInitialValidationError,
        });

        property.SetValidateNotifyError(x => string.IsNullOrEmpty(x) ? "required" : null);

        Assert.IsFalse(property.HasErrors);
        property.ForceValidate();
        Assert.IsTrue(property.HasErrors);
        CollectionAssert.AreEqual(new[] { "required" }, property.GetErrors("Value").Cast<string>().ToArray());
    }

    [TestMethod]
    public async Task HasErrorsTransitionsWithAsyncValidation()
    {
        using var property = new CompatValidatableProperty<string>("");
        var results = new Subject<IEnumerable?>();
        var transitions = new List<bool>();
        using var subscription = property.ObserveHasErrors.Subscribe(transitions.Add);
        property.SetValidateNotifyError(_ => results);

        property.Value = "invalid";
        await Task.Run(async () =>
        {
            await Task.Delay(10);
            results.OnNext(new[] { "async error" });
        });

        Assert.IsTrue(property.HasErrors);
        CollectionAssert.AreEqual(new[] { false, true }, transitions);

        results.OnNext(null);
        Assert.IsFalse(property.HasErrors);
        CollectionAssert.AreEqual(new[] { false, true, false }, transitions);
    }

    [TestMethod]
    public void MultipleErrorsAreAggregatedWithStreamValidation()
    {
        using var property = new CompatValidatableProperty<string>("");
        var results = new Subject<IEnumerable?>();
        property.SetValidateNotifyError(x => string.IsNullOrEmpty(x) ? "required" : null)
            .SetValidateNotifyError(_ => results);

        results.OnNext(new[] { "remote-1", "remote-2" });

        CollectionAssert.AreEqual(
            new[] { "required", "remote-1", "remote-2" },
            property.GetErrors("Value").Cast<string>().ToArray());
    }

    [TestMethod]
    public void DataAnnotationsUsesValueMemberName()
    {
        var viewModel = new ValidationViewModel();
        viewModel.Name.SetValidateAttribute(() => viewModel.Name);
        DataErrorsChangedEventArgs? args = null;
        viewModel.Name.ErrorsChanged += (_, e) => args = e;

        viewModel.Name.Value = string.Empty;

        Assert.IsTrue(viewModel.Name.HasErrors);
        Assert.AreEqual("Value", args?.PropertyName);
        CollectionAssert.AreEqual(new[] { "Name is required" }, viewModel.Name.GetErrors("Value").Cast<string>().ToArray());
    }

    private sealed class ValidationViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public CompatValidatableProperty<string> Name { get; } = new("ok");
    }
}
