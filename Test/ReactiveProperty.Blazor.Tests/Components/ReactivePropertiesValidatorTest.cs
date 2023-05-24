using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using Reactive.Bindings.Components;
using Reactive.Bindings.Extensions;
using Bunit;
using BunitTestContext = Bunit.TestContext;
using System.Linq;

namespace ReactiveProperty.Blazor.Tests.Components;

[TestClass]
public class ReactivePropertiesValidatorTest
{
    [TestMethod]
    public void InstanceCreationFailWithoutEditContextCascadingParameter()
    {
        using var testContext = new BunitTestContext();
        Assert.ThrowsException<InvalidOperationException>(() =>
            testContext.RenderComponent<ReactivePropertiesValidator>());
    }

    [TestMethod]
    public void InstanceCreationSuccessWithEditContextCascadingParameter()
    {
        using var testContext = new BunitTestContext();
        (_, _, var component) = Init(testContext);
        Assert.IsNotNull(component.Instance);
    }

    [TestMethod]
    public void NotifyEventWhenValidationStateChanged()
    {
        using var testContext = new BunitTestContext();
        var (editContext, viewModel, component) = Init(testContext);
        var numberOfValidationStateChangedCalled = 0;
        editContext.OnValidationStateChanged += (_, _) => numberOfValidationStateChangedCalled++;

        // Invalid to valid
        viewModel.Name.Value = "Tanaka";
        Assert.AreEqual(1, numberOfValidationStateChangedCalled);
    }

    [TestMethod]
    public void NotifyCorrectValidationMessages()
    {
        using var testContext = new BunitTestContext();
        var (editContext, viewModel, component) = Init(testContext);

        var validationMessagesWhenNamePropertyIsInvalid = editContext.GetValidationMessages().ToArray();
        // Invalid to valid
        viewModel.Name.Value = "Tanaka";
        var validationMessagesWhenNamePropertyIsValid = editContext.GetValidationMessages().ToArray();

        // Valid to invalid
        viewModel.Comment.Value = "01234567890";
        var validationMessagesWhenCommentPropertyIsInvalid = editContext.GetValidationMessages().ToArray();

        CollectionAssert.AreEqual(
            new[] { "Name is required" },
            validationMessagesWhenNamePropertyIsInvalid);
        Assert.IsFalse(validationMessagesWhenNamePropertyIsValid.Any());
        CollectionAssert.AreEqual(
            new[] { "Comment is under 10 letters" },
            validationMessagesWhenCommentPropertyIsInvalid);
    }

    private (EditContext EditContext, TestViewModel ViewModel, IRenderedComponent<ReactivePropertiesValidator> Validator) Init(BunitTestContext testContext)
    {
        var vm = new TestViewModel();
        var editContext = new EditContext(vm);
        var component = testContext.RenderComponent<ReactivePropertiesValidator>(builder =>
            builder.AddCascadingValue(editContext));
        return (editContext, vm, component);
    }
}

class TestViewModel : IDisposable
{
    private readonly CompositeDisposable _disposable = new();

    [Required(ErrorMessage = "Name is required")]
    public ReactiveProperty<string> Name { get; }
    [StringLength(10, ErrorMessage = "Comment is under 10 letters")]
    public ReactiveProperty<string> Comment { get; }

    public TestViewModel()
    {
        Name = new ReactiveProperty<string>("")
            .SetValidateAttribute(() => Name)
            .AddTo(_disposable);

        Comment = new ReactiveProperty<string>("")
            .SetValidateAttribute(() => Comment)
            .AddTo(_disposable);
    }

    public void Dispose() => _disposable.Dispose();
}
