using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Extensions;

namespace ReactiveProperty.Tests
{
    [TestClass()]
    public class OldNewPairTest
    {
        [TestMethod()]
        public void ToStringTest()
        {
            // ToString result same as AnonymousType

            new OldNewPair<string>("old", "new")
                .ToString()
                .Is(new { Old = "old", New = "new" }.ToString());

            new OldNewPair<string>(null, null)
                .ToString()
                .Is(new { Old = (string)null, New = (string)null }.ToString());

            new OldNewPair<int>(10, 20)
                .ToString()
                .Is(new { Old = 10, New = 20 }.ToString());
        }
    }
}
