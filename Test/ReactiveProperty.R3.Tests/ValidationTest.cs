#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
    public void RevalidatingUnchangedErrorsAvoidsExcessAllocation()
    {
        using var property = new ValidatableReactiveProperty<int>();
        for (var i = 0; i < 8; i++)
        {
            property.SetValidateNotifyError(_ => (IEnumerable<string>?)null);
        }

        for (var i = 0; i < 32; i++)
        {
            property.ForceValidate();
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 64; i++)
        {
            property.ForceValidate();
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.IsTrue(allocated < 64 * 256, $"Allocated {allocated} bytes for 64 validations.");
    }

    [TestMethod]
    public void RegisteringValidatorDuringValidationUsesStableSnapshot()
    {
        using var property = new ValidatableReactiveProperty<int>(0, ignoreInitialValidationError: true);
        var calls = new List<string>();
        var registered = false;
        property.SetValidateNotifyError(_ =>
        {
            calls.Add("first");
            if (!registered)
            {
                registered = true;
                property.SetValidateNotifyError(_ =>
                {
                    calls.Add("second");
                    return (string?)null;
                });
            }

            return (string?)null;
        });

        property.ForceValidate();
        calls.Is("first");
        calls.Clear();
        property.ForceValidate();
        calls.Is("first", "second");
    }

    [TestMethod]
    public void ReentrantValidationPreservesErrorNotificationOrder()
    {
        var invalid = false;
        using var property = new ValidatableReactiveProperty<int>()
            .SetValidateNotifyError(_ => invalid ? "error" : null);
        var notifications = new List<string>();
        using var subscription = property.ObserveErrorChanged.Subscribe(errors =>
        {
            notifications.Add(string.Join(",", errors));
            if (invalid)
            {
                invalid = false;
                property.ForceValidate();
            }
        });
        notifications.Clear();

        invalid = true;
        property.ForceValidate();

        notifications.Is("error", "");
        property.GetErrors().Is();
    }

    [TestMethod]
    public void RevalidationRestoresMutatedObservedErrors()
    {
        using var property = new ValidatableReactiveProperty<int>()
            .SetValidateNotifyError(_ => "error");
        IReadOnlyList<string>? observed = null;
        using var subscription = property.ObserveErrorChanged.Subscribe(errors => observed = errors);
        ((string[])observed!)[0] = "changed";

        property.ForceValidate();

        property.GetErrors().Is("error");
    }

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
    public void RevalidatingUnchangedErrorsRunsValidatorsWithoutRepeatingNotifications()
    {
        var validatorOrder = new List<string>();
        using var property = new ValidatableReactiveProperty<int>(0)
            .SetValidateNotifyError(_ =>
            {
                validatorOrder.Add("first");
                return new[] { "first", "detail" };
            })
            .SetValidateNotifyError(_ =>
            {
                validatorOrder.Add("second");
                return "second";
            });
        var errors = new List<IReadOnlyList<string>>();
        var errorsChangedCount = 0;
        var propertyChangedNames = new List<string?>();

        using var d1 = property.ObserveErrorChanged.Subscribe(x => errors.Add([.. x]));
        property.ErrorsChanged += (_, _) => errorsChangedCount++;
        property.PropertyChanged += (_, e) => propertyChangedNames.Add(e.PropertyName);
        errors.Clear();
        validatorOrder.Clear();

        property.ForceValidate();

        validatorOrder.Is("first", "second");
        errors.Count.Is(0);
        errorsChangedCount.Is(0);
        propertyChangedNames.Count.Is(0);
        property.GetErrors().Is("first", "detail", "second");
    }

    [TestMethod]
    public void ChangedErrorsRetainAggregationAndNotificationOrder()
    {
        using var property = new ValidatableReactiveProperty<int>(0)
            .SetValidateNotifyError(x => x == 0 ? "first" : null)
            .SetValidateNotifyError(x => x == 0 ? "second" : null);
        var notifications = new List<string>();

        using var d1 = property.ObserveErrorChanged.Subscribe(x => notifications.Add($"Errors:{string.Join(",", x)}"));
        using var d2 = property.ObserveHasErrors.Subscribe(x => notifications.Add($"HasErrors:{x}"));
        property.ErrorsChanged += (_, _) => notifications.Add("ErrorsChanged");
        property.PropertyChanged += (_, e) => notifications.Add($"PropertyChanged:{e.PropertyName}");
        notifications.Clear();

        property.Value = 1;

        notifications.Is(
            "Errors:second",
            "ErrorsChanged",
            "PropertyChanged:ErrorMessage",
            "Errors:",
            "HasErrors:False",
            "ErrorsChanged",
            "PropertyChanged:ErrorMessage",
            "PropertyChanged:HasErrors",
            "PropertyChanged:Value");
        property.GetErrors().Is();
    }

    [TestMethod]
    public void RevalidatingStreamErrorsDoesNotRepeatNotifications()
    {
        using var property = new ValidatableReactiveProperty<int>(-1)
            .SetValidateNotifyError(values => values.Select(x => x < 0 ? (IEnumerable?)new[] { "negative" } : null));
        var errorsChangedCount = 0;
        property.ErrorsChanged += (_, _) => errorsChangedCount++;

        property.Value = -2;

        property.GetErrors().Is("negative");
        errorsChangedCount.Is(0);
    }

    [TestMethod]
    public void RevalidatingAsyncErrorsDoesNotRepeatNotifications()
    {
        var validateCount = 0;
        using var property = new ValidatableReactiveProperty<int>(0)
            .SetValidateNotifyError(_ =>
            {
                validateCount++;
                return Task.FromResult<string?>("async error");
            });
        var errorsChangedCount = 0;
        property.ErrorsChanged += (_, _) => errorsChangedCount++;
        var initialValidateCount = validateCount;

        property.ForceValidate();

        validateCount.Is(initialValidateCount + 1);
        property.GetErrors().Is("async error");
        errorsChangedCount.Is(0);
    }

    [TestMethod]
    public void RevalidatingEnumerableErrorsEnumeratesInputOnce()
    {
        var enumerationCount = 0;
        using var property = new ValidatableReactiveProperty<int>(0)
            .SetValidateNotifyError(_ => GetErrors());
        var initialEnumerationCount = enumerationCount;

        property.ForceValidate();

        enumerationCount.Is(initialEnumerationCount + 1);
        property.GetErrors().Is("error");

        IEnumerable<string> GetErrors()
        {
            enumerationCount++;
            yield return "error";
        }
    }

    [TestMethod]
    public void StreamValidationUpdatesErrorStreams()
    {
        using var property = new ValidatableReactiveProperty<int>(0)
            .SetValidateNotifyError(values => values.Select(x => x < 0 ? (IEnumerable?)new[] { "negative" } : null));
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
    public void MultipleValidatorKindsAggregateErrors()
    {
        using var property = new ValidatableReactiveProperty<string>("")
            .SetValidateNotifyError(x => string.IsNullOrEmpty(x) ? "required" : null)
            .SetValidateNotifyError(x => x.Length < 3 ? new[] { "too short" } : null)
            .SetValidateNotifyError(values => values.Select(x => x.Contains('!') ? (IEnumerable?)new[] { "bang" } : null));

        property.Value = "!";

        property.GetErrors().Is("too short", "bang");
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

    [TestMethod]
    public void ValidatableReactivePropertySkipsValidationOnEqualValue()
    {
        var validateCount = 0;
        using var property = new ValidatableReactiveProperty<string>(
                "same",
                ignoreInitialValidationError: true)
            .SetValidateNotifyError(x =>
            {
                validateCount++;
                return x.Length == 0 ? "required" : null;
            });
        var valueChangedCount = 0;
        property.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ValidatableReactiveProperty<string>.Value))
            {
                valueChangedCount++;
            }
        };

        property.Value = "same";

        validateCount.Is(0);
        valueChangedCount.Is(0);
    }

    [TestMethod]
    public void ValidatableReactivePropertyReusesFixedPropertyChangedEventArgs()
    {
        using var property = new ValidatableReactiveProperty<string>("ok")
            .SetValidateNotifyError(x => x == "ng" ? "error" : null);
        var propertyChangedEvents = new List<PropertyChangedEventArgs>();
        property.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

        property.Value = "ng";
        property.Value = "ok";

        propertyChangedEvents.Select(static x => x.PropertyName).Is(
            nameof(ValidatableReactiveProperty<string>.ErrorMessage),
            nameof(ValidatableReactiveProperty<string>.HasErrors),
            nameof(ValidatableReactiveProperty<string>.Value),
            nameof(ValidatableReactiveProperty<string>.ErrorMessage),
            nameof(ValidatableReactiveProperty<string>.HasErrors),
            nameof(ValidatableReactiveProperty<string>.Value));
        Assert.AreSame(propertyChangedEvents[0], propertyChangedEvents[3]);
        Assert.AreSame(propertyChangedEvents[1], propertyChangedEvents[4]);
        Assert.AreSame(propertyChangedEvents[2], propertyChangedEvents[5]);
    }

    [TestMethod]
    public void ObserveErrorChangedIncludesEntityLevelNotifications()
    {
        var source = new TestNotifyDataErrorInfo();
        var values = new List<IReadOnlyList<string>>();

        using var subscription = source.ObserveErrorChanged("Name").Subscribe(values.Add);
        source.SetErrors("Name", "error-1");
        source.RaiseErrorsChanged(null);

        values.Count.Is(2);
        values[0].ToArray().Is();
        values[1].ToArray().Is("error-1");
    }

    private sealed class RequiredViewModel
    {
        [Required]
        public ValidatableReactiveProperty<string> Name { get; } = new("");
    }

    private sealed class TestNotifyDataErrorInfo : INotifyDataErrorInfo
    {
        private readonly Dictionary<string, string[]> _errors = new();

        public bool HasErrors => _errors.Count != 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return _errors.Values.SelectMany(static x => x).ToArray();
            }

            return _errors.TryGetValue(propertyName, out var errors) ? errors : Array.Empty<string>();
        }

        public void SetErrors(string propertyName, params string[] errors)
        {
            if (errors.Length == 0)
            {
                _errors.Remove(propertyName);
                return;
            }

            _errors[propertyName] = errors;
        }

        public void RaiseErrorsChanged(string? propertyName) =>
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
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
