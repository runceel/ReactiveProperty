using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeplex.Reactive;
using Codeplex.Reactive.Serialization;
using System.Runtime.Serialization;

namespace ReactiveProperty.Tests.Serialization
{
    [TestClass]
    public class SerializeHelperTest
    {
        [Ignore] // currently not supported recursive serialize
        [TestMethod]
        public void SerializeTest()
        {
            var p = new Parent();
            p.Child.Value = new Child();
            p.Child.Value.Name.Value = "Tanaka";

            var data = SerializeHelper.PackReactivePropertyValue(p);
            data.IsNotNull();
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
}
