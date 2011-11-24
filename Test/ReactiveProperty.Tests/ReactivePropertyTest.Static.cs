using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Codeplex.Reactive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Concurrency;
using System;

namespace RxPropTest
{
    [TestClass]
    public class ReactivePropertyStaticTest
    {
        [TestMethod]
        public void FromObject()
        {
            var homuhomu = new ToaruClass { Name = "homuhomu", Age = 13 };
            var rxName = Codeplex.Reactive.ReactiveProperty.FromObject(homuhomu, x => x.Name);
            rxName.Value.Is("homuhomu");
            rxName.Value = "mami";
            homuhomu.Name.Is("mami");

            var rxAge = Codeplex.Reactive.ReactiveProperty.FromObject(homuhomu, x => x.Age);
            rxAge.Value.Is(13);
            rxAge.Value = 20;
            homuhomu.Age.Is(20);
        }

        [TestMethod]
        public void FromObjectConverter()
        {
            var homuhomu = new ToaruClass { Name = "homuhomu", Age = 13 };
            var rxAge = Codeplex.Reactive.ReactiveProperty.FromObject(homuhomu, x => x.Age,
                x => Convert.ToString(x, 16), x => Convert.ToInt32(x, 16));

            rxAge.Value.Is("d");
            rxAge.Value = "3f";
            homuhomu.Age.Is(63);
        }

        class ToaruClass
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}