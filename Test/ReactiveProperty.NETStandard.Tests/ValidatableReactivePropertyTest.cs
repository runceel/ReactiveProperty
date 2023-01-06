using System;
using System.ComponentModel;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reactive.Bindings;

namespace ReactiveProperty.Tests;

[TestClass]
public class ValidatableReactivePropertyTest : ReactiveTest
{
    [TestMethod]
    public void BasicUsage()
    {
        var target = new ValidatableReactiveProperty<string>(
            "",
            x => x == "valid" ? null : "invalid value");

        var errorChangedCount = 0;
        target.ErrorsChanged += (_, _) => errorChangedCount++;

        errorChangedCount.Is(0);
        target.Value.Is("");
        target.HasErrors.IsTrue();
        target.GetErrors().Is("invalid value");

        target.Value = "still invalid";
        target.Value.Is("still invalid");
        errorChangedCount.Is(0);
        target.HasErrors.IsTrue();
        target.GetErrors().Is("invalid value");

        target.Value = "valid";
        target.Value.Is("valid");
        errorChangedCount.Is(1);
        target.HasErrors.IsFalse();
        target.GetErrors().Length.Is(0);
    }

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
    public void ValidationLogicsCase()
    {
        var target = new ValidatableReactiveProperty<string>(
            "",
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

        target.Value = "valid";
        errorChangedCount.Is(2);
        target.HasErrors.IsFalse();
        target.GetErrors().Length.Is(0);
        target.ErrorMessage.Is("");
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

    [TestMethod]
    public void DisposeSourceFalseCase()
    {
        var mockSource = new Mock<IReactiveProperty<string>>();

        var target = mockSource.Object.ToValidatableReactiveProperty(
            x => string.IsNullOrEmpty(x) ? "invalid" : null);
        target.Dispose();
        mockSource.VerifyAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once());
        mockSource.VerifyRemove(x => x.PropertyChanged -= It.IsAny<PropertyChangedEventHandler>(), Times.Once());
        mockSource.Verify(x => x.Dispose(), Times.Never());
        mockSource.Verify(x => x.Subscribe(It.IsAny<IObserver<string>>()), Times.Never());
    }

    [TestMethod]
    public void DisposeSourceTrueCase()
    {
        var mockSource = new Mock<IReactiveProperty<string>>();

        var target = mockSource.Object.ToValidatableReactiveProperty(
            x => string.IsNullOrEmpty(x) ? "invalid" : null,
            disposeSource: true);

        target.Dispose();
        mockSource.VerifyAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once());
        mockSource.VerifyRemove(x => x.PropertyChanged -= It.IsAny<PropertyChangedEventHandler>(), Times.Once());
        mockSource.Verify(x => x.Dispose(), Times.Once());
        mockSource.Verify(x => x.Subscribe(It.IsAny<IObserver<string>>()), Times.Never());
    }

    [TestMethod]
    public void DistinctUntilChangedTest()
    {
        var target = new ValidatableReactiveProperty<string>("",
            x => string.IsNullOrEmpty(x) ? "invalid" : null);
        var testScheduler = new TestScheduler();
        var recorder = testScheduler.CreateObserver<string>();
        target.Subscribe(recorder); // OnNext

        target.Value = ""; // no OnNext
        target.Value = "changed"; // OnNext
        target.Value = "changed"; // no OnNext
        target.Value = ""; // OnNext
        target.Dispose(); // OnCompleted

        recorder.Messages.Is(
            OnNext(0, ""),
            OnNext(0, "changed"),
            OnNext(0, ""),
            OnCompleted<string>(0));
    }

    [TestMethod]
    public void NotDistinctUntilChangedTest()
    {
        var target = new ValidatableReactiveProperty<string>("",
            x => string.IsNullOrEmpty(x) ? "invalid" : null,
            mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);
        var testScheduler = new TestScheduler();
        var recorder = testScheduler.CreateObserver<string>();
        target.Subscribe(recorder); // OnNext

        target.Value = ""; // OnNext
        target.Value = "changed"; // OnNext
        target.Value = "changed"; // OnNext
        target.Value = ""; // OnNext
        target.Dispose(); // OnCompleted

        recorder.Messages.Is(
            OnNext(0, ""),
            OnNext(0, ""),
            OnNext(0, "changed"),
            OnNext(0, "changed"),
            OnNext(0, ""),
            OnCompleted<string>(0));
    }

    [TestMethod]
    public void RaiseLatestValueOnSubscribeTest()
    {
        var target = new ValidatableReactiveProperty<string>("",
            x => string.IsNullOrEmpty(x) ? "invalid" : null,
            mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);
        var testScheduler = new TestScheduler();
        var recorder = testScheduler.CreateObserver<string>();
        target.Subscribe(recorder); // OnNext
        target.Dispose(); // OnCompleted

        recorder.Messages.Is(
            OnNext(0, ""),
            OnCompleted<string>(0));
    }

    [TestMethod]
    public void NotRaiseLatestValueOnSubscribeTest()
    {
        var target = new ValidatableReactiveProperty<string>("",
            x => string.IsNullOrEmpty(x) ? "invalid" : null,
            mode: ReactivePropertyMode.None);
        var testScheduler = new TestScheduler();
        var recorder = testScheduler.CreateObserver<string>();
        target.Subscribe(recorder); // no OnNext
        target.Value = "changed"; // OnNext
        target.Dispose(); // OnCompleted

        recorder.Messages.Is(
            OnNext(0, "changed"),
            OnCompleted<string>(0));
    }
}
