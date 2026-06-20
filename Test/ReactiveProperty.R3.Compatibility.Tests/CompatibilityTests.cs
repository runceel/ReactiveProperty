using System;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3Compat;
using Reactive.Bindings.R3Compat.Collections;
using Reactive.Bindings.R3Compat.Commands;
using Reactive.Bindings.R3Compat.Validation;

namespace ReactiveProperty.R3.Compatibility.Tests;

[TestClass]
public sealed class CompatibilityTests
{
    [TestMethod]
    public void IgnoreInitialValidationErrorSuppressesInitialError()
    {
        using var property = new CompatValidatableProperty<string>("", new CompatibilityOptions
        {
            Mode = CompatibilityMode.Default | CompatibilityMode.IgnoreInitialValidationError,
        }).SetValidateNotifyError(x => string.IsNullOrEmpty(x) ? "required" : null);

        Assert.IsFalse(property.HasErrors);
        property.ForceValidate();
        CollectionAssert.AreEqual(new[] { "required" }, property.GetErrors("Value").Cast<string>().ToArray());
    }

    [TestMethod]
    public void StreamValidationAggregatesMultipleErrors()
    {
        using var property = new CompatValidatableProperty<string>("")
            .SetValidateNotifyError(values => values.Select(_ => (IEnumerable?)new[] { "required", "too short" }));

        property.Value = "x";

        CollectionAssert.AreEqual(new[] { "required", "too short" }, property.GetErrors("Value").Cast<string>().ToArray());
    }

    [TestMethod]
    public void AttributeValidationUsesValueMemberContext()
    {
        var target = new AttributeValidationTarget();
        using var property = target.Name.SetValidateAttribute(() => target.Name);

        Assert.IsTrue(property.HasErrors);
        CollectionAssert.AreEqual(new[] { "Name is required" }, property.GetErrors("Value").Cast<string>().ToArray());
    }

    [TestMethod]
    public async Task AsyncCommandDisablesCanExecuteWhileRunning()
    {
        var gate = new TaskCompletionSource();
        using var command = new AsyncReactiveCommandCompat<int>().WithSubscribe(async _ => await gate.Task);

        var executing = command.ExecuteAsync(1);

        Assert.IsFalse(((ICommand)command).CanExecute(1));
        gate.SetResult();
        await executing;
        Assert.IsTrue(((ICommand)command).CanExecute(1));
    }

    [TestMethod]
    public async Task SharedCanExecuteLinksTwoAsyncCommands()
    {
        var shared = new CompatReactiveProperty<bool>(true);
        var gate = new TaskCompletionSource();
        using var command1 = new AsyncReactiveCommandCompat<int>(shared).WithSubscribe(async _ => await gate.Task);
        using var command2 = new AsyncReactiveCommandCompat<int>(shared).WithSubscribe(_ => ValueTask.CompletedTask);

        var executing = command1.ExecuteAsync(1);

        Assert.IsFalse(((ICommand)command1).CanExecute(1));
        Assert.IsFalse(((ICommand)command2).CanExecute(1));
        gate.SetResult();
        await executing;
        Assert.IsTrue(((ICommand)command1).CanExecute(1));
        Assert.IsTrue(((ICommand)command2).CanExecute(1));
    }

    [TestMethod]
    public async Task DisposedAsyncCommandIgnoresExecute()
    {
        var count = 0;
        var command = new AsyncReactiveCommandCompat<int>().WithSubscribe(_ =>
        {
            count++;
            return ValueTask.CompletedTask;
        });
        command.Dispose();

        await command.ExecuteAsync(1);

        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void CanExecuteSourceRaisesCanExecuteChanged()
    {
        var source = new global::R3.ReactiveProperty<bool>(false);
        using var command = new ReactiveCommandSlimCompat<int>(source).WithSubscribe(_ => { });
        var raised = 0;
        command.CanExecuteChanged += (_, _) => raised++;

        source.Value = true;

        Assert.IsTrue(command.CanExecute(1));
        Assert.AreEqual(1, raised);
    }

    [TestMethod]
    public void CollectionAppliesAddAndResetProjection()
    {
        using var source = new Subject<CollectionChanged<int>>();
        using var collection = ReadOnlyReactiveCollectionCompat<string>.Create(source, x => $"#{x}", disposeElement: false);

        source.OnNext(CollectionChanged<int>.Add(0, 1));
        source.OnNext(CollectionChanged<int>.ResetWithSource(new[] { 2, 3 }));

        CollectionAssert.AreEqual(new[] { "#2", "#3" }, collection.ToArray());
    }

    [TestMethod]
    public void CollectionDisposesElementsWhenEnabled()
    {
        using var source = new Subject<CollectionChanged<DisposableItem>>();
        using var collection = ReadOnlyReactiveCollectionCompat<DisposableItem>.Create(source, x => x);
        var item = new DisposableItem();

        source.OnNext(CollectionChanged<DisposableItem>.Add(0, item));
        collection.Dispose();

        Assert.IsTrue(item.IsDisposed);
    }

    [TestMethod]
    public void RuleEngineDowngradesWhenPredicateMatches()
    {
        var rule = new MigrationRule(
            "RP-VAL-001",
            "ReactiveProperty.SetValidateNotifyError(stream)",
            MigrationCategory.Compat,
            RequiresCompatibilityLayer: true,
            DowngradeWhen: "validator is synchronous and single-value",
            DowngradeTarget: "BindableReactiveProperty<T>.EnableValidation",
            Deprecation: "remove when stream-validator usage == 0");

        var result = MigrationRuleEngine.Classify(rule, new MigrationContext(DowngradeWhenSatisfied: true));

        Assert.AreEqual(MigrationCategory.Auto, result.Category);
        Assert.IsFalse(result.RequiresCompatibilityLayer);
        Assert.AreEqual("BindableReactiveProperty<T>.EnableValidation", result.Replacement);
    }

    private sealed class AttributeValidationTarget
    {
        [Required(ErrorMessage = "Name is required")]
        public CompatValidatableProperty<string> Name { get; } = new("");
    }

    private sealed class DisposableItem : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Dispose() => IsDisposed = true;
    }
}
