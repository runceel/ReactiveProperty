using Reactive.Bindings.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Reactive.Bindings;
using Microsoft.Reactive.Testing;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings.Interactivity;

namespace ReactiveProperty.Tests
{
    [TestClass()]
    public class PairwiseTest : ReactiveTest
    {
        [TestMethod()]
        public void Test()
        {
            var xs = Observable.Range(1, 5).Pairwise().ToEnumerable().ToArray();
            xs.Length.Is(4);
            xs[0].Is(x => x.OldItem == 1 && x.NewItem == 2);
            xs[1].Is(x => x.OldItem == 2 && x.NewItem == 3);
            xs[2].Is(x => x.OldItem == 3 && x.NewItem == 4);
            xs[3].Is(x => x.OldItem == 4 && x.NewItem == 5);

            Observable.Range(1, 5).Pairwise(Tuple.Create).ToEnumerable()
                .Is(
                    Tuple.Create(1, 2),
                    Tuple.Create(2, 3),
                    Tuple.Create(3, 4),
                    Tuple.Create(4, 5)
                );

            new TestScheduler().Start(() =>
                    Observable.Empty<int>().Pairwise(), 10, 20, 30)
                .Messages
                .Is(OnCompleted<OldNewPair<int>>(20));

            new TestScheduler().Start(() =>
                    Observable.Return(1000).Pairwise(), 10, 20, 30)
                .Messages
                .Is(OnCompleted<OldNewPair<int>>(20));
        }
    }
}