using BenchmarkDotNet.Running;

namespace ReactivePropertyBenchmark
{
    class Program
    {
        static void Main(string[] args) =>
            BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}
