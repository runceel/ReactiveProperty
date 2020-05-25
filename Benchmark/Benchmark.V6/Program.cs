using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Running;

namespace ReactivePropertyBenchmark
{
    class Program
    {
        static void Main(string[] args) =>
            BenchmarkRunner.Run(typeof(Program).Assembly);

    }
}
