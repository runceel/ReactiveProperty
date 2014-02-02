using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeplex.Reactive;

namespace ReactiveProperty.Tests.Extensions
{
    [TestClass]
    public class ReadOnlyReactiveCollectionTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var c = new ReactiveCollection<string>();
        }
    }
}
