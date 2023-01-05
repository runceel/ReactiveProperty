using System;
using System.ComponentModel.DataAnnotations;
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
    public ReactivePropertyVM ReactivePropertyValidationFromPoco()
    {
        var vm = new ReactivePropertyVM();
        vm.Name.Value = "valid";
        vm.Name.Value = "";
        vm.Dispose();
        return vm;
    }

    [Benchmark]
    public ValidatableReactivePropertyVM ValidatableReactivePropertyValidationFromPoco()
    {
        var vm = new ValidatableReactivePropertyVM();
        vm.Name.Value = "valid";
        vm.Name.Value = "";
        vm.Dispose();
        return vm;
    }

    [Benchmark]
    public IObservable<string> ObservePropertyLegacy()
    {
        var p = new Person();
        return p.ObservePropertyLegacy(x => x.Name);
    }

    [Benchmark]
    public ReactivePropertySlim<string> ToReactivePropertyAsSynchronizedSlim()
    {
        var p = new Person();
        return p.ToReactivePropertySlimAsSynchronized(x => x.Name);
    }
}

public class ReactivePropertyVM : IDisposable
{
    private Person _model = new();

    [Required]
    public ReactiveProperty<string> Name { get; }

    public ReactivePropertyVM()
    {
        Name = _model.ToReactivePropertyAsSynchronized(x => x.Name,
            ignoreValidationErrorValue: true)
            .SetValidateAttribute(() => Name);
    }

    public void Dispose()
    {
        Name.Dispose();
    }
}

public class ValidatableReactivePropertyVM : IDisposable
{
    private Person _model = new();

    [Required]
    public ValidatableReactiveProperty<string> Name { get; }

    public ValidatableReactivePropertyVM()
    {
        Name = _model.ToReactivePropertySlimAsSynchronized(x => x.Name)
            .ToValidatableReactiveProperty(() => Name, disposeSource: true);
    }

    public void Dispose()
    {
        Name.Dispose();
    }
}
