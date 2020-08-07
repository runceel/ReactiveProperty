using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReactivePropertyPerformanceTest
    {
        [TestMethod]
        [Timeout(1500)]
        public void PerformanceCheck()
        {
            var source = Observable.Return(1);
            var properties = new List<ReactiveProperty<int>>();
            for (var i = 0; i < 100000; i++)
            {
                properties.Add(source.ToReactiveProperty());
            }
        }
    }
}
