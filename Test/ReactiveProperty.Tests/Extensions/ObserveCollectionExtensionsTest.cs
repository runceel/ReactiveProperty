using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Extensions;

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

        [TestMethod]
        public void ObserveElementPropertyTest()
        {
            this.ObserveElementPropertyTestCore(false);
        }

        [TestMethod]
        public void ReadOnlyObservableCollection_ObserveElementPropertyTest()
        {
            this.ObserveElementPropertyTestCore(true);
        }

        private void ObserveElementPropertyTestCore(bool wrapAsReadOnly)
        {
            var neuecc  = new Person { Name = "neuecc",  Age = 31 };
            var okazuki = new Person { Name = "okazuki", Age = 34 };
            var xin9le  = new Person { Name = "xin9le",  Age = 30 };
            var anders  = new Person { Name = "anders",  Age = 54 };
            var collection = new ObservableCollection<Person>(new []{ neuecc, okazuki });

            //--- no data
            var buffer = new List<string>();
            buffer.Count.Is(0);

            //--- subscribe all
            var sequence    = wrapAsReadOnly
                            ? new ReadOnlyObservableCollection<Person>(collection).ObserveElementProperty(x => x.Name)
                            : collection.ObserveElementProperty(x => x.Name);
            var subscription = sequence.Subscribe(buffer.Add);
            buffer.Count.Is(2);

            //--- change element's property
            var newName = "neuecc_renamed";
            neuecc.Name = newName;
            buffer.Count.Is(3);
            buffer[2].Is(newName);

            //--- add element
            collection.Add(xin9le);
            collection.Count.Is(3);
            buffer.Count.Is(4);
            buffer[3].Is("xin9le");

            //--- change added element's property
            newName = "xin9le_renamed";
            xin9le.Name = newName;
            buffer.Count.Is(5);
            buffer[4].Is(newName);

            //--- remove element
            collection.Remove(okazuki);
            collection.Count.Is(2);
            buffer.Count.Is(5);

            //--- change removed element's property
            okazuki.Name = "okazuki_renamed";
            buffer.Count.Is(5);  //--- no push

            //--- replace element
            collection[1] = anders;
            collection.Count.Is(2);
            buffer.Count.Is(6);
            buffer[5].Is("anders");

            //--- change replaced element's property
            newName = "anders_renamed";
            anders.Name = newName;
            buffer.Count.Is(7);
            buffer[6].Is(newName);

            xin9le.Name = "replaced";
            buffer.Count.Is(7);

            //--- unsubscribe
            subscription.Dispose();
            collection.Count.Is(2);
            neuecc.Name = "neuecc_unsubscribed";
            anders.Name = "anders_unsubscribed";
            buffer.Count.Is(7);
        }


        #region private classes
        private class Person : INotifyPropertyChanged
        {
            #region Properties
            public string Name
            {
                get { return this.name; }
                set
                {
                    if (this.name != value)
                    {
                        this.name = value;
                        this.RaisePropertyChanged();
                    }
                }
            }
            private string name;
 
            public int Age
            {
                get { return this.age; }
                set
                {
                    if (this.age != value)
                    {
                        this.age = value;
                        this.RaisePropertyChanged();
                    }
                }
            }
            private int age;
            #endregion


            #region INotifyPropertyChanged members
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion


            #region helpers
            private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            {
                var handler = this.PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
        #endregion
    }
}
