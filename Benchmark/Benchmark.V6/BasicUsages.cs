using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using Reactive.Bindings;

namespace ReactivePropertyBenchmark
{
    public class BasicUsages
    {
        [Benchmark]
        public ReactiveProperty<string> CreateReactivePropertyInstance() => new ReactiveProperty<string>();

        [Benchmark]
        public ReactivePropertySlim<string> CreateReactivePropertySlimInstance() => new ReactivePropertySlim<string>();

        [Benchmark]
        public void BasicForReactiveProperty()
        {
            var rp = new ReactiveProperty<string>();
            var rrp = rp.ToReadOnlyReactiveProperty();

            rp.Value = "xxxx";
            rp.Dispose();
        }

        [Benchmark]
        public void BasicForReactivePropertySlim()
        {
            var rp = new ReactivePropertySlim<string>();
            var rrp = rp.ToReadOnlyReactivePropertySlim();

            rp.Value = "xxxx";
            rp.Dispose();
        }
    }
}
