using System;
using System.Reactive.Subjects;
using BenchmarkDotNet.Attributes;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace ReactivePropertyBenchmark;

public partial class BasicUsages
{
    [Benchmark]
    public ReactiveCommand CreateReactiveCommand() => new ReactiveCommand();

    [Benchmark]
    public ReactiveCommandSlim CreateReactiveCommandSlim() => new ReactiveCommandSlim();



    [Benchmark]
    public ReactiveCommand BasicUsecaseForReactiveCommand()
    {
        var subject = new Subject<bool>();
        var cmd = subject.ToReactiveCommand();
        cmd.Subscribe(x => { });
        cmd.Execute();
        subject.OnNext(true);
        cmd.Execute();
        return cmd;
    }

    [Benchmark]
    public ReactiveCommandSlim BasicUsecaseForReactiveCommandSlim()
    {
        var subject = new Subject<bool>();
        var cmd = subject.ToReactiveCommandSlim();
        cmd.Subscribe(x => { });
        cmd.Execute();
        subject.OnNext(true);
        cmd.Execute();
        return cmd;
    }

    [Benchmark]
    public ReactiveProperty<string> ReactivePropertyValidation()
    {
        var rp = new ReactiveProperty<string>("")
            .SetValidateNotifyError(x => string.IsNullOrEmpty(x) ? "invalid" : null);
        rp.Value = "xxx"; // valid
        rp.Value = ""; // invalid
        return rp;
    }

    [Benchmark]
    public ValidatableReactiveProperty<string> ValidatableReactivePropertyValidation()
    {
        var rp = ValidatableReactiveProperty.CreateFromValidationLogic(
            "",
            x => string.IsNullOrEmpty(x) ? "invalid" : null);
        rp.Value = "xxx"; // valid
        rp.Value = ""; // invalid
        return rp;
    }

    [Benchmark]
    public IObservable<string> ObservePropertyLegacy()
    {
        var p = new Person();
        return p.ObservePropertyLegacy(x => x.Name);
    }

}
