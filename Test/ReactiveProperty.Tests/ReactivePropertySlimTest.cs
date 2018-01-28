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
using System.Reactive.Subjects;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReactivePropertySlimTest
    {
        [TestMethod]
        public void NormalCase()
        {
            var rp = new ReactivePropertySlim<string>();
            rp.Value.IsNull();
            rp.Subscribe(x => x.IsNull());
        }

        [TestMethod]
        public void InitialValue()
        {
            var rp = new ReactivePropertySlim<string>("Hello world");
            rp.Value.Is("Hello world");
            rp.Subscribe(x => x.Is("Hello world"));
        }

        [TestMethod]
        public void NoRaiseLatestValueOnSubscribe()
        {
            var rp = new ReactivePropertySlim<string>(mode: ReactivePropertyMode.DistinctUntilChanged);
            var called = false;
            rp.Subscribe(_ => called = true);
            called.Is(false);
        }

        [TestMethod]
        public void NoDistinctUntilChanged()
        {
            var rp = new ReactivePropertySlim<string>(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);
            var list = new List<string>();
            rp.Subscribe(list.Add);
            rp.Value = "Hello world";
            rp.Value = "Hello world";
            rp.Value = "Hello japan";
            list.Is(null, "Hello world", "Hello world", "Hello japan");
        }

        class IgnoreCaseComparer : EqualityComparer<string>
        {
            public override bool Equals(string x, string y)
                => x?.ToLower() == y?.ToLower();

            public override int GetHashCode(string obj)
                => (obj?.ToLower()).GetHashCode();
        }

        [TestMethod]
        public void CustomEqualityComparer()
        {
            var rp = new ReactivePropertySlim<string>(equalityComparer: new IgnoreCaseComparer());
            var list = new List<string>();
            rp.Subscribe(list.Add);
            rp.Value = "Hello world";
            rp.Value = "HELLO WORLD";
            rp.Value = "Hello japan";
            list.Is(null, "Hello world", "Hello japan");
        }

        [TestMethod]
        public void EnumCase()
        {
            var rp = new ReactivePropertySlim<TestEnum>();
            var results = new List<TestEnum>();
            rp.Subscribe(results.Add);
            results.Is(TestEnum.None);

            rp.Value = TestEnum.Enum1;
            results.Is(TestEnum.None, TestEnum.Enum1);

            rp.Value = TestEnum.Enum2;
            results.Is(TestEnum.None, TestEnum.Enum1, TestEnum.Enum2);
        }

        [TestMethod]
        public void ForceNotify()
        {
            var rp = new ReactivePropertySlim<int>(0);
            var collecter = new List<int>();
            rp.Subscribe(collecter.Add);

            collecter.Is(0);
            rp.ForceNotify();
            collecter.Is(0, 0);
        }

        [TestMethod]
        public void UnsubscribeTest()
        {
            var rp = new ReactivePropertySlim<int>(mode: ReactivePropertyMode.None);
            var collecter = new List<(string, int)>();
            var a = rp.Select(x => ("a", x)).Subscribe(collecter.Add);
            var b = rp.Select(x => ("b", x)).Subscribe(collecter.Add);
            var c = rp.Select(x => ("c", x)).Subscribe(collecter.Add);

            rp.Value = 99;
            collecter.Is(("a", 99), ("b", 99), ("c", 99));

            collecter.Clear();
            a.Dispose();

            rp.Value = 40;
            collecter.Is(("b", 40), ("c", 40));

            collecter.Clear();
            c.Dispose();

            rp.Value = 50;
            collecter.Is(("b", 50));

            collecter.Clear();
            b.Dispose();

            rp.Value = 9999;
            collecter.Count.Is(0);

            var d = rp.Select(x => ("d", x)).Subscribe(collecter.Add);

            rp.Value = 9;
            collecter.Is(("d", 9));

            rp.Dispose();
        }
    }
}
