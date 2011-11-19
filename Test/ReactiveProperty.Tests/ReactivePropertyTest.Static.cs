using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Codeplex.Reactive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Concurrency;

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

        class ToaruClass
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}