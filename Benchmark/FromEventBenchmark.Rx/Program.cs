using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Reactive.Bindings.Extensions;

/*
FromEventBenchmark.Rx
|          Method |  Count |              Mean |           Error |          StdDev |
|---------------- |------- |------------------:|----------------:|----------------:|
| ObserveProperty |      1 |          4.662 us |       0.0403 us |       0.0377 us |
| ObserveProperty |     10 |         46.159 us |       0.2482 us |       0.2322 us |
| ObserveProperty |    100 |        488.395 us |       3.9745 us |       3.7177 us |
| ObserveProperty |   1000 |      7,299.851 us |      49.5364 us |      41.3651 us |
| ObserveProperty |  10000 |    184,803.556 us |   2,443.4350 us |   2,285.5906 us |
| ObserveProperty | 100000 | 21,848,008.562 us | 183,192.5623 us | 152,974.0864 us |

FromEventBenchmark.Rp
|          Method |  Count |           Mean |         Error |        StdDev |
|---------------- |------- |---------------:|--------------:|--------------:|
| ObserveProperty |      1 |       3.313 us |     0.0226 us |     0.0200 us |
| ObserveProperty |     10 |      32.927 us |     0.2260 us |     0.2114 us |
| ObserveProperty |    100 |     335.572 us |     2.0898 us |     1.9548 us |
| ObserveProperty |   1000 |   4,245.384 us |    82.0820 us |    91.2340 us |
| ObserveProperty |  10000 |  79,851.266 us | 1,057.3668 us |   937.3281 us |
| ObserveProperty | 100000 | 779,906.493 us | 8,120.6971 us | 7,596.1051 us |
*/

BenchmarkRunner.Run(typeof(FromEventBenchmark).Assembly);

public class FromEventBenchmark
{
    [Params(1, 10, 100, 1000, 10000, 100000)]
    public int Count { get; set; }

    public FromEventBenchmark()
    {
        SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
    }

    [Benchmark]
    public IDisposable ObserveProperty()
    {
        var d = new CompositeDisposable();
        foreach (var _ in Enumerable.Range(0, Count))
        {
            var obj = new ObservableObject();
            obj.ObserveProperty(x => x.Prop)
                .Subscribe()
                .AddTo(d);
            obj.Prop = "Trigger an event";
        }

        return d;
    }
}

class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private string _prop;
    public string Prop
    {
        get => _prop;
        set
        {
            _prop = value;
            PropertyChanged?.Invoke(this, new(nameof(Prop)));
        }
    }
}
