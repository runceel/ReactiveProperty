using Reactive.Bindings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReactivePropertyTest
    {
        [TestMethod]
        public void ObserveErrors()
        {
            var rp = new ReactiveProperty<string>()
                .SetValidateNotifyError(x => x == null ? "Error" : null);
            var results = new List<IEnumerable>();

            rp.ObserveErrorChanged.Subscribe(x => results.Add(x));
            rp.Value = "OK";

            results.Count.Is(2);
            results[0].OfType<string>().Is("Error");
            results[1].IsNull();
        }

        [TestMethod]
        public void ObserveHasError()
        {
            var rp = new ReactiveProperty<string>()
                .SetValidateNotifyError(x => x == null ? "Error" : null);
            var results = new List<bool>();

            rp.ObserveHasError.Subscribe(x => results.Add(x));
            rp.Value = "OK";

            results.Count.Is(2);
            results[0].IsTrue();
            results[1].IsFalse();
        }

        [TestMethod]
        public void ObserveHasNoError()
        {
            var rp = new ReactiveProperty<string>()
                .SetValidateNotifyError(x => x == null ? "Error" : null);
            var results = new List<bool>();

            rp.ObserveHasError.Select(x => !x).Subscribe(x => results.Add(x));
            rp.Value = "OK";

            results.Count.Is(2);
            results[0].IsFalse();
            results[1].IsTrue();
        }
    }
}
