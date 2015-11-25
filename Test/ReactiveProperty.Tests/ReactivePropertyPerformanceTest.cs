using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReactivePropertyPerformanceTest
    {
        [TestMethod]
        public void PerformanceCheck()
        {
            var source = Observable.Return(1);
            var properties = new List<ReactiveProperty<int>>();
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                properties.Add(source.ToReactiveProperty());
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }
}
