using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Internals;

namespace ReactiveProperty.Tests.Extensions
{
    [TestClass]
    public class AccessorCacheTest
    {
        [TestMethod]
        public void LookupGetAndSet()
        {
            var get = AccessorCache<Person>.LookupGet(p => p.Name, out var prop);
            Assert.AreEqual("Name", prop);

            var set = AccessorCache<Person>.LookupSet(p => p.Name, out prop);
            Assert.AreEqual("Name", prop);

            var person = new Person { Name = "tanaka" };
            get(person).Is("tanaka");
            set(person, "kimura");
            person.Name.Is("kimura");
        }

        private class Person
        {
            public string Name { get; set; }
        }
    }
}
