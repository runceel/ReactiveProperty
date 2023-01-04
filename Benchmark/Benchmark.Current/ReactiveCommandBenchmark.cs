using System;
using System.Reactive.Subjects;
using BenchmarkDotNet.Attributes;
using Reactive.Bindings;

namespace Benchmark.Current;

public class ReactiveCommandBenchmarks
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
}
