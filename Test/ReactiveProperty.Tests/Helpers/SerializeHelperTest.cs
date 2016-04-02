using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using Reactive.Bindings.Helpers;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;

namespace ReactiveProperty.Tests.Serialization
{
    [TestClass]
    public class SerializeHelperTest
    {
        [Ignore] // currently not supported recursive serialize
        [TestMethod]
        public void RecursiveSerializeTest()
        {
            var p = new Parent();
            p.Child.Value = new Child();
            p.Child.Value.Name.Value = "Tanaka";

            var data = SerializeHelper.PackReactivePropertyValue(p);
            data.IsNotNull();
        }

        [TestMethod]
        public void FlatClassSerializeTest()
        {
            var p = new Person();
            p.Name = "tanaka";
            p.Age.Value = 10;
            p.Is(o => o.Name == "tanaka" && o.Age.Value == 10 && o.Profile.Value == "tanaka 10");

            var s = new XmlSerializer(typeof(Person));

            var ms = new MemoryStream();
            // serialize
            s.Serialize(ms, p);
            ms.Seek(0, SeekOrigin.Begin);
            // deserialize
            var restored = s.Deserialize(ms) as Person;
            restored.Is(o => o.Name == "tanaka" && o.Age.Value == 10 && o.Profile.Value == "tanaka 10");
        }
    }

    [DataContract]
    public class Parent
    {
        public Parent()
        {
            this.Child = new ReactiveProperty<Child>();
        }

        public ReactiveProperty<Child> Child { get; private set; }
    }

    [DataContract]
    public class Child
    {
        public Child()
        {
            this.Name = new ReactiveProperty<string>();
        }

        public ReactiveProperty<string> Name { get; private set; }
    }

    public class Person
    {
        public string Name { get; set; }

        [XmlIgnore]
        public ReactiveProperty<int> Age { get; private set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public ReactiveProperty<string> Profile { get; private set; }

        public Person()
        {
            this.Age = new ReactiveProperty<int>();
            this.Profile = this.Age
                .Select(i => string.Format("{0} {1}", this.Name, i))
                .ToReactiveProperty();
        }

        // use serialize only
        public string PackValue
        {
            get
            {
                return SerializeHelper.PackReactivePropertyValue(this);
            }
            set
            {
                SerializeHelper.UnpackReactivePropertyValue(this, value);
            }
        }
    }
}
