using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReadOnlyReactivePropertyTest
    {
        [TestMethod]
        public void NormalPattern()
        {
            var s = new Subject<string>();

            var rp = s.ToReadOnlyReactiveProperty(eventScheduler: Scheduler.CurrentThread);
            var buffer = new List<string>();
            rp.Subscribe(buffer.Add);

            rp.Value.IsNull();
            buffer.Count.Is(1);
            buffer[0].IsNull();

            s.OnNext("Hello");
            rp.Value.Is("Hello");
            buffer.Count.Is(2);
            buffer.Is(default(string), "Hello");
        }
    }
}
