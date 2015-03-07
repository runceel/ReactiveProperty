using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings.Extensions;
using System.Reactive;

namespace ReactiveProperty.Tests.Extensions
{
    [TestClass]
    public class ObserveCollectionExtensionsTest
    {
        [TestMethod]
        public void ObserveAddChangedTest()
        {
            var l = new List<string>();
            var c = new ObservableCollection<string>();
            c.ObserveAddChanged()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);

            c.Add("a");
            l.Count.Is(1);

            c.Add("b");
            l.Count.Is(2);

            l.Is("a", "b");
        }

        [TestMethod]
        public void ReadOnlyCollection_ObserveAddChangedTest()
        {
            var l = new List<string>();
            var c = new ObservableCollection<string>();
            var r = new ReadOnlyObservableCollection<string>(c);
            r.ObserveAddChanged()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);

            c.Add("a");
            l.Count.Is(1);

            c.Add("b");
            l.Count.Is(2);

            l.Is("a", "b");
        }

        [TestMethod]
        public void ObserveAddChangedItemsTest()
        {
            var l = new List<string[]>();
            var c = new ObservableCollection<string>();
            c.ObserveAddChangedItems()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);

            c.Add("a");
            l.Count.Is(1);

            c.Add("b");
            l.Count.Is(2);

            l[0].Is("a");
            l[1].Is("b");
        }

        [TestMethod]
        public void ReadOnlyCollection_ObserveAddChangedItemsTest()
        {
            var l = new List<string[]>();
            var c = new ObservableCollection<string>();
            var r = new ReadOnlyObservableCollection<string>(c);
            r.ObserveAddChangedItems()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);

            c.Add("a");
            l.Count.Is(1);

            c.Add("b");
            l.Count.Is(2);

            l[0].Is("a");
            l[1].Is("b");
        }

        [TestMethod]
        public void ObserveRemoveChangedTest()
        {
            var l = new List<string>();
            var c = new ObservableCollection<string>(new[] { "a", "b", "c" });
            c.ObserveRemoveChanged()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);

            c.Remove("a");
            l.Is("a");

            c.RemoveAt(1);
            l.Is("a", "c");
        }

        [TestMethod]
        public void ReadOnlyObservableCollection_ObserveRemoveChangedTest()
        {
            var l = new List<string>();
            var c = new ObservableCollection<string>(new[] { "a", "b", "c" });
            var r = new ReadOnlyObservableCollection<string>(c);
            r.ObserveRemoveChanged()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);

            c.Remove("a");
            l.Is("a");

            c.RemoveAt(1);
            l.Is("a", "c");
        }

        [TestMethod]
        public void ObserveRemoveChangedItemsTest()
        {
            var l = new List<string[]>();
            var c = new ObservableCollection<string>(new[] { "a", "b", "c" });
            c.ObserveRemoveChangedItems()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);

            c.Remove("a");
            l[0].Is("a");

            c.RemoveAt(1);
            l[0].Is("a");
            l[1].Is("c");
        }

        [TestMethod]
        public void ReadOnlyObservableCollection_ObserveRemoveChangedItemsTest()
        {
            var l = new List<string[]>();
            var c = new ObservableCollection<string>(new[] { "a", "b", "c" });
            var r = new ReadOnlyObservableCollection<string>(c);
            r.ObserveRemoveChangedItems()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);

            c.Remove("a");
            l[0].Is("a");

            c.RemoveAt(1);
            l[0].Is("a");
            l[1].Is("c");
        }

        [TestMethod]
        public void ObserveReplaceChangedTest()
        {
            var l = new List<OldNewPair<string>>();
            var c = new ObservableCollection<string>(new[] { "a", "b", "c" });
            c.ObserveReplaceChanged()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);
            c[0] = "aaa";
            l.Count.Is(1);
            l[0].Is(x => x.OldItem == "a" && x.NewItem == "aaa");

            c[2] = "ccc";
            l.Count.Is(2);
            l[1].Is(x => x.OldItem == "c" && x.NewItem == "ccc");
        }

        [TestMethod]
        public void ReadOnlyObservableCollection_ObserveReplaceChangedTest()
        {
            var l = new List<OldNewPair<string>>();
            var c = new ObservableCollection<string>(new[] { "a", "b", "c" });
            var r = new ReadOnlyObservableCollection<string>(c);
            r.ObserveReplaceChanged()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);
            c[0] = "aaa";
            l.Count.Is(1);
            l[0].Is(x => x.OldItem == "a" && x.NewItem == "aaa");

            c[2] = "ccc";
            l.Count.Is(2);
            l[1].Is(x => x.OldItem == "c" && x.NewItem == "ccc");
        }

        [TestMethod]
        public void ObserveReplaceChangedItemsTest()
        {
            var l = new List<OldNewPair<string[]>>();
            var c = new ObservableCollection<string>(new[] { "a", "b", "c" });
            c.ObserveReplaceChangedItems()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);
            c[0] = "aaa";
            l.Count.Is(1);
            l[0].Is(x => x.OldItem[0] == "a" && x.NewItem[0] == "aaa");

            c[2] = "ccc";
            l.Count.Is(2);
            l[1].Is(x => x.OldItem[0] == "c" && x.NewItem[0] == "ccc");
        }

        [TestMethod]
        public void ReadOnlyObservableCollection_ObserveReplaceChangedItemsTest()
        {
            var l = new List<OldNewPair<string[]>>();
            var c = new ObservableCollection<string>(new[] { "a", "b", "c" });
            var r = new ReadOnlyObservableCollection<string>(c);
            r.ObserveReplaceChangedItems()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);
            c[0] = "aaa";
            l.Count.Is(1);
            l[0].Is(x => x.OldItem[0] == "a" && x.NewItem[0] == "aaa");

            c[2] = "ccc";
            l.Count.Is(2);
            l[1].Is(x => x.OldItem[0] == "c" && x.NewItem[0] == "ccc");
        }

        [TestMethod]
        public void ObserveResetChangedTest()
        {
            var l = new List<Unit>();
            var c = new ObservableCollection<string>(new[] { "a", "b", "c" });
            c.ObserveResetChanged()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);
            c.Clear();
            l.Count.Is(1);

            c.Add("a"); c.Add("b");
            c.Clear();
            l.Count.Is(2);
        }

        [TestMethod]
        public void ReadOnlyObservableCollection_ObserveResetChangedTest()
        {
            var l = new List<Unit>();
            var c = new ObservableCollection<string>(new[] { "a", "b", "c" });
            var r = new ReadOnlyObservableCollection<string>(c);
            r.ObserveResetChanged()
                .Subscribe(x => l.Add(x));

            l.Count.Is(0);
            c.Clear();
            l.Count.Is(1);

            c.Add("a"); c.Add("b");
            c.Clear();
            l.Count.Is(2);
        }
    }
}
