using System;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

namespace ReactiveProperty.Tests;

[TestClass]
public class ValidatableReactivePropertyTest : ReactiveTest
{


    [TestMethod]
    public void CreateFromSourceBasicUsage()
    {
        var source = new ReactivePropertySlim<string>("");
        var target = source.ToValidatableReactiveProperty(
            x => x == "valid" ? null : "invalid value");

        var errorChangedCount = 0;
        target.ErrorsChanged += (_, _) => errorChangedCount++;

        errorChangedCount.Is(0);
        target.HasErrors.IsTrue();
        target.GetErrors().Is("invalid value");
        target.Value = "still invalid";
        errorChangedCount.Is(0);
        target.HasErrors.IsTrue();
        target.GetErrors().Is("invalid value");
        source.Value.Is("");

        target.Value = "valid";
        errorChangedCount.Is(1);
        target.HasErrors.IsFalse();
        target.GetErrors().Length.Is(0);
        source.Value.Is("valid");
    }

    [TestMethod]
    public void CreateFromSourceWithValidationLogicsCase()
    {
        var source = new ReactivePropertySlim<string>("");
        var target = source.ToValidatableReactiveProperty(
            new Func<string, string?>[]
            {
                x => string.IsNullOrWhiteSpace(x) ? "required" : null,
                x => x == "valid" ? null : "invalid value",
            });

        var errorChangedCount = 0;
        target.ErrorsChanged += (_, _) => errorChangedCount++;

        errorChangedCount.Is(0);
        target.HasErrors.IsTrue();
        target.GetErrors().Is("required", "invalid value");
        target.ErrorMessage.Is("required");
        target.Value = "still invalid";
        errorChangedCount.Is(1);
        target.HasErrors.IsTrue();
        target.GetErrors().Is("invalid value");
        target.ErrorMessage.Is("invalid value");
        source.Value.Is("");

        target.Value = "valid";
        errorChangedCount.Is(2);
        target.HasErrors.IsFalse();
        target.GetErrors().Length.Is(0);
        target.ErrorMessage.Is("");
        source.Value.Is("valid");
    }

    [TestMethod]
    public void CreateFromSourceDefaultModeTest()
    {
        var source = new ReactivePropertySlim<string>("initialValue");
        var target = source.ToValidatableReactiveProperty(
            x => string.IsNullOrWhiteSpace(x) ? "invalid" : null);

        string? raiseLatestValueOnSubscribed = null;
        target.Subscribe(x => raiseLatestValueOnSubscribed = x).Dispose();

        raiseLatestValueOnSubscribed.Is("initialValue");

        int subscribeCount = 0;
        target.Subscribe(x => subscribeCount++);
        subscribeCount.Is(1);
        target.Value = "initialValue";
        subscribeCount.Is(1);
        target.Value = "changed";
        subscribeCount.Is(2);
    }

    [TestMethod]
    public void CreateFromSourceIgnoreInitialValidationErrorTest()
    {
        var source = new ReactivePropertySlim<string>("");
        var target = source.ToValidatableReactiveProperty(
            x => string.IsNullOrWhiteSpace(x) ? "invalid" : null,
            mode: ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError);

        target.HasErrors.IsFalse();
        target.GetErrors().Length.Is(0);
        target.Value.Is("");

        target.Value = "valid";
        target.HasErrors.IsFalse();
        target.GetErrors().Length.Is(0);

        target.Value = "";
        target.HasErrors.IsTrue();
        target.GetErrors().Is("invalid");
    }

    [TestMethod]
    public void CreateFromSourceIgnoreExceptionDoesntSupportCase()
    {
        var source = new ReactivePropertySlim<string>("initialValue");

        Assert.ThrowsException<NotSupportedException>(() =>
        {
            source.ToValidatableReactiveProperty(
                x => string.IsNullOrWhiteSpace(x) ? "invalid" : null,
                mode: ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreException);
        });
    }

    [TestMethod]
    public void CreateFromSourceObserveErrorChangedTest()
    {
        var source = new ReactivePropertySlim<string>("");
        var target = source.ToValidatableReactiveProperty(
            x => string.IsNullOrWhiteSpace(x) ? "invalid" : null,
            mode: ReactivePropertyMode.Default);

        var scheduler = new TestScheduler();
        var recorder = scheduler.CreateObserver<string[]>();
        target.ObserveErrorChanged.Subscribe(recorder);

        recorder.Messages.Count.Is(0);

        target.Value = "valid";
        target.Value = "";
        target.Dispose();
        recorder.Messages.Is(
            OnNext(0, (string[] x) => x is []),
            OnNext(0, (string[] x) => x is ["invalid"]),
            OnCompleted<string[]>(0));
    }

    [TestMethod]
    public void CreateFromSourceObserveHasErrorsTest()
    {
        var source = new ReactivePropertySlim<string>("");
        var target = source.ToValidatableReactiveProperty(
            x => string.IsNullOrWhiteSpace(x) ? "invalid" : null,
            mode: ReactivePropertyMode.Default);

        var scheduler = new TestScheduler();
        var recorder = scheduler.CreateObserver<bool>();
        target.ObserveHasErrors.Subscribe(recorder);

        recorder.Messages.Count.Is(0);

        target.Value = "valid";
        target.Value = "";
        target.Dispose();
        recorder.Messages.Is(
            OnNext(0, false),
            OnNext(0, true),
            OnCompleted<bool>(0));
    }
}
