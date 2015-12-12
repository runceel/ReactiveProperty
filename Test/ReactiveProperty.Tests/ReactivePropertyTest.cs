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
        public void NormalCase()
        {
            var rp = new ReactiveProperty<string>();
            rp.Value.IsNull();
            rp.Subscribe(x => x.IsNull());
        }

        [TestMethod]
        public void InitialValue()
        {
            var rp = new ReactiveProperty<string>("Hello world");
            rp.Value.Is("Hello world");
            rp.Subscribe(x => x.Is("Hello world"));
        }

        [TestMethod]
        public void NoRaiseLatestValueOnSubscribe()
        {
            var rp = new ReactiveProperty<string>(mode: ReactivePropertyMode.DistinctUntilChanged);
            var called = false;
            rp.Subscribe(_ => called = true);
            called.Is(false);
        }

        [TestMethod]
        public void NoDistinctUntilChanged()
        {
            var rp = new ReactiveProperty<string>(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);
            var list = new List<string>();
            rp.Subscribe(list.Add);
            rp.Value = "Hello world";
            rp.Value = "Hello world";
            rp.Value = "Hello japan";
            list.Is(null, "Hello world", "Hello world", "Hello japan");
        }

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

            rp.ObserveHasErrors.Subscribe(x => results.Add(x));
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

            rp.ObserveHasErrors.Select(x => !x).Subscribe(x => results.Add(x));
            rp.Value = "OK";

            results.Count.Is(2);
            results[0].IsFalse();
            results[1].IsTrue();
        }

        [TestMethod]
        public void EnumCase()
        {
            var rp = new ReactiveProperty<TestEnum>();
            var results = new List<TestEnum>();
            rp.Subscribe(results.Add);
            results.Is(TestEnum.None);

            rp.Value = TestEnum.Enum1;
            results.Is(TestEnum.None, TestEnum.Enum1);

            rp.Value = TestEnum.Enum2;
            results.Is(TestEnum.None, TestEnum.Enum1, TestEnum.Enum2);
        }
    }

    enum TestEnum
    {
        None,
        Enum1,
        Enum2
    }
}
