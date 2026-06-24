using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReactiveProperty.Tests;

[TestClass]
public sealed class ScaffoldTest
{
    [TestMethod]
    public void TestDependenciesAreAvailable()
    {
        var timeProvider = new FakeTimeProvider();
        timeProvider.IsNotNull();
    }
}
