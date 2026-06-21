#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3;
using Reactive.Bindings.R3.Extensions;

namespace ReactiveProperty.Tests;

[TestClass]
public sealed class ValidationTest
{
    [TestMethod]
    public void ValidatableReactivePropertyAggregatesMultipleErrors()
    {
        using var property = new ValidatableReactiveProperty<string>("")
            .SetValidateNotifyError(x => string.IsNullOrWhiteSpace(x) ? new[] { "required", "empty" } : null);
        var errors = new List<IReadOnlyList<string>>();
        var hasErrors = new List<bool>();
        var changedCount = 0;

        property.ErrorsChanged += (_, e) =>
        {
            e.PropertyName.Is(nameof(ValidatableReactiveProperty<string>.Value));
            changedCount++;
        };
        using var d1 = property.ObserveErrorChanged.Subscribe(errors.Add);
        using var d2 = property.ObserveHasErrors.Subscribe(hasErrors.Add);

        property.Value = "ok";
        property.Value = "";

        property.HasErrors.IsTrue();
        property.ErrorMessage.Is("required");
        property.GetErrors().Is("required", "empty");
        ((IEnumerable)property.GetErrors(nameof(ValidatableReactiveProperty<string>.Value))!).Cast<string>().ToArray().Is("required", "empty");
        errors.Count.Is(3);
        errors[0].ToArray().Is("required", "empty");
        errors[1].ToArray().Is();
        errors[2].ToArray().Is("required", "empty");
        hasErrors.Is(true, false, true);
        changedCount.Is(2);
    }

    [TestMethod]
    public void StreamValidationUpdatesErrorStreams()
    {
        using var property = new ValidatableReactiveProperty<int>(0)
            .SetValidateNotifyError(values => values.Select<int, IEnumerable?>(x => x < 0 ? new[] { "negative" } : null));
        var hasErrors = new List<bool>();

        using var subscription = property.ObserveHasErrors.Subscribe(hasErrors.Add);

        property.Value = -1;
        property.Value = 2;

        property.GetErrors().Is();
        hasErrors.Is(false, true, false);
    }

    [TestMethod]
    public async Task AsyncValidationUpdatesHasErrors()
    {
        using var property = new ValidatableReactiveProperty<string>("ok")
            .SetValidateNotifyError(async x =>
            {
                await Task.Yield();
                return x == "ng" ? "async error" : null;
            });

        property.Value = "ng";
        await SpinWaitUntil(() => property.HasErrors);

        property.ErrorMessage.Is("async error");
    }

    [TestMethod]
    public void DataErrorInfoExtensionsObserveErrors()
    {
        using var property = new ValidatableReactiveProperty<string>("ok")
            .SetValidateNotifyError(x => x == "ng" ? "error" : null);
        var changed = new List<DataErrorsChangedEventArgs>();
        var hasErrors = new List<bool>();
        var errorLists = new List<IReadOnlyList<string>>();

        using var d1 = property.ErrorsChangedAsObservable().Subscribe(changed.Add);
        using var d2 = property.ObserveHasErrors().Subscribe(hasErrors.Add);
        using var d3 = property.ObserveErrorChanged().Subscribe(errorLists.Add);

        property.Value = "ng";
        property.Value = "ok";

        changed.Count.Is(2);
        hasErrors.Is(false, true, false);
        errorLists.Count.Is(3);
        errorLists[0].ToArray().Is();
        errorLists[1].ToArray().Is("error");
        errorLists[2].ToArray().Is();
    }

    [TestMethod]
    public void SetValidateAttributeUsesValueMember()
    {
        using var property = new RequiredViewModel()
            .Name
            .SetValidateAttribute(() => new RequiredViewModel().Name);

        property.Value = "";

        property.HasErrors.IsTrue();
    }

    private sealed class RequiredViewModel
    {
        [Required]
        public ValidatableReactiveProperty<string> Name { get; } = new("");
    }

    private static async Task SpinWaitUntil(Func<bool> condition)
    {
        for (var i = 0; i < 100; i++)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(10);
        }

        Assert.Fail("Condition was not met.");
    }
}
