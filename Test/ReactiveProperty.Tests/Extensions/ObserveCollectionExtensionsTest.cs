using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Extensions;
using Reactive.Bindings;

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
            var buffer = new List<PropertyPack<Person, string>>();
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
            buffer[2].Instance.Is(neuecc);
            buffer[2].Property.Name.Is("Name");
            buffer[2].Value.Is(newName);

            //--- add element
            collection.Add(xin9le);
            collection.Count.Is(3);
            buffer.Count.Is(4);
            buffer[3].Instance.Is(xin9le);
            buffer[3].Property.Name.Is("Name");
            buffer[3].Value.Is("xin9le");

            //--- change added element's property
            newName = "xin9le_renamed";
            xin9le.Name = newName;
            buffer.Count.Is(5);
            buffer[4].Instance.Is(xin9le);
            buffer[4].Property.Name.Is("Name");
            buffer[4].Value.Is(newName);

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
            buffer[5].Instance.Is(anders);
            buffer[5].Property.Name.Is("Name");
            buffer[5].Value.Is("anders");

            //--- change replaced element's property
            newName = "anders_renamed";
            anders.Name = newName;
            buffer.Count.Is(7);
            buffer[6].Instance.Is(anders);
            buffer[6].Property.Name.Is("Name");
            buffer[6].Value.Is(newName);

            xin9le.Name = "replaced";
            buffer.Count.Is(7);

            //--- unsubscribe
            subscription.Dispose();
            collection.Count.Is(2);
            neuecc.Name = "neuecc_unsubscribed";
            anders.Name = "anders_unsubscribed";
            buffer.Count.Is(7);
        }


        [TestMethod]
        public void OvserveElementObservablePropertyReferenceTypeTest()
        {
            var neuecc  = new PersonViewModel("neuecc", 31);
            var okazuki = new PersonViewModel("okazuki", 33);
            var xin9le  = new PersonViewModel("xin9le", 30);
            var anders  = new PersonViewModel("anders", 54);
            var collection = new ObservableCollection<PersonViewModel>(new []{ neuecc, okazuki });

            //--- no data
            var buffer = new List<PropertyPack<PersonViewModel, string>>();
            buffer.Count.Is(0);

            //--- subscribe all
            var sequence = collection.ObserveElementObservableProperty(x => x.Name);
            var subscription = sequence.Subscribe(buffer.Add);
            buffer.Count.Is(2);

            //--- change element's property
            var newName = "neuecc_renamed";
            neuecc.Name.Value = newName;
            buffer.Count.Is(3);
            buffer[2].Instance.Is(neuecc);
            buffer[2].Property.Name.Is("Name");
            buffer[2].Value.Is(newName);

            //--- add element
            collection.Add(xin9le);
            collection.Count.Is(3);
            buffer.Count.Is(4);
            buffer[3].Instance.Is(xin9le);
            buffer[3].Property.Name.Is("Name");
            buffer[3].Value.Is("xin9le");

            //--- change added element's property
            newName = "xin9le_renamed";
            xin9le.Name.Value = newName;
            buffer.Count.Is(5);
            buffer[4].Instance.Is(xin9le);
            buffer[4].Property.Name.Is("Name");
            buffer[4].Value.Is(newName);

            //--- remove element
            collection.Remove(okazuki);
            collection.Count.Is(2);
            buffer.Count.Is(5);

            //--- change removed element's property
            okazuki.Name.Value = "okazuki_renamed";
            buffer.Count.Is(5);  //--- no push

            //--- replace element
            collection[1] = anders;
            collection.Count.Is(2);
            buffer.Count.Is(6);
            buffer[5].Instance.Is(anders);
            buffer[5].Property.Name.Is("Name");
            buffer[5].Value.Is("anders");

            //--- change replaced element's property
            newName = "anders_renamed";
            anders.Name.Value = newName;
            buffer.Count.Is(7);
            buffer[6].Instance.Is(anders);
            buffer[6].Property.Name.Is("Name");
            buffer[6].Value.Is(newName);

            xin9le.Name.Value = "replaced";
            buffer.Count.Is(7);

            //--- unsubscribe
            subscription.Dispose();
            collection.Count.Is(2);
            neuecc.Name.Value = "neuecc_unsubscribed";
            anders.Name.Value = "anders_unsubscribed";
            buffer.Count.Is(7);
        }


        [TestMethod]
        public void OvserveElementObservablePropertyValueTypeTest()
        {
            var neuecc  = new PersonViewModel("neuecc", 31);
            var okazuki = new PersonViewModel("okazuki", 33);
            var xin9le  = new PersonViewModel("xin9le", 30);
            var anders  = new PersonViewModel("anders", 54);
            var collection = new ObservableCollection<PersonViewModel>(new []{ neuecc, okazuki });

            //--- no data
            var buffer = new List<PropertyPack<PersonViewModel, int>>();
            buffer.Count.Is(0);

            //--- subscribe all
            var sequence = collection.ObserveElementObservableProperty(x => x.Age);
            var subscription = sequence.Subscribe(buffer.Add);
            buffer.Count.Is(2);

            //--- change element's property
            var newAge = 10;
            neuecc.Age.Value = newAge;
            buffer.Count.Is(3);
            buffer[2].Instance.Is(neuecc);
            buffer[2].Property.Name.Is("Age");
            buffer[2].Value.Is(newAge);

            //--- add element
            collection.Add(xin9le);
            collection.Count.Is(3);
            buffer.Count.Is(4);
            buffer[3].Instance.Is(xin9le);
            buffer[3].Property.Name.Is("Age");
            buffer[3].Value.Is(30);

            //--- change added element's property
            newAge = 20;
            xin9le.Age.Value = newAge;
            buffer.Count.Is(5);
            buffer[4].Instance.Is(xin9le);
            buffer[4].Property.Name.Is("Age");
            buffer[4].Value.Is(newAge);

            //--- remove element
            collection.Remove(okazuki);
            collection.Count.Is(2);
            buffer.Count.Is(5);

            //--- change removed element's property
            okazuki.Age.Value = 25;
            buffer.Count.Is(5);  //--- no push

            //--- replace element
            collection[1] = anders;
            collection.Count.Is(2);
            buffer.Count.Is(6);
            buffer[5].Instance.Is(anders);
            buffer[5].Property.Name.Is("Age");
            buffer[5].Value.Is(54);

            //--- change replaced element's property
            newAge = 60;
            anders.Age.Value = newAge;
            buffer.Count.Is(7);
            buffer[6].Instance.Is(anders);
            buffer[6].Property.Name.Is("Age");
            buffer[6].Value.Is(newAge);

            xin9le.Age.Value = 100;
            buffer.Count.Is(7);

            //--- unsubscribe
            subscription.Dispose();
            collection.Count.Is(2);
            neuecc.Age.Value = -1;
            anders.Age.Value = -2;
            buffer.Count.Is(7);
        }


        [TestMethod]
        public void ObserveElementPropertyChanged()
        {
            var source = new ObservableCollection<Person>(new[]
            {
                new Person { Name = "tanaka" },
                new Person { Name = "kimuta" }
            });
            var buffer = new List<SenderEventArgsPair<Person, PropertyChangedEventArgs>>();

            var subscription = source.ObserveElementPropertyChanged()
                .Subscribe(buffer.Add);

            buffer.Count.Is(0);

            source[0].Name = "okazuki";
            buffer.Count.Is(1);
            buffer[0].Is(x => x.Sender.Name == "okazuki" && x.EventArgs.PropertyName == "Name");

            source[1].Name = "xin9le";
            buffer.Count.Is(2);
            buffer[1].Is(x => x.Sender.Name == "xin9le" && x.EventArgs.PropertyName == "Name");

            source.Add(new Person { Name = "neuecc" });
            buffer.Count.Is(2);

            source[2].Age = 30;
            buffer.Count.Is(3);
            buffer[2].Is(x => x.Sender.Name == "neuecc" && x.Sender.Age == 30 && x.EventArgs.PropertyName == "Age");

            subscription.Dispose();

            source[1].Age = 10;
            source[0].Age = 100;
            buffer.Count.Is(3);
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

        private class PersonViewModel
        {
            public ReactiveProperty<string> Name { get; private set; }
            public ReactiveProperty<int> Age { get; private set; }

            public PersonViewModel(string name, int age)
            {
                this.Name = new ReactiveProperty<string>(name);
                this.Age = new ReactiveProperty<int>(age);
            }
        }
        #endregion
    }
}
