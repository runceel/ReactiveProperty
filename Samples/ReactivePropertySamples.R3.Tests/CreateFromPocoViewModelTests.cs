using ReactivePropertySamples.Migrated.ViewModels;

namespace ReactivePropertySamples.R3.Tests;

[TestClass]
public class CreateFromPocoViewModelTests
{
    [TestMethod]
    public void TwoWay_PropertyAndPocoStayInSync()
    {
        using var vm = new CreateFromPocoViewModel();

        Assert.AreEqual("Kazuki", vm.FirstNameTwoWay.Value);

        vm.FirstNameTwoWay.Value = "Taro";
        Assert.AreEqual("Taro", vm.Poco.FirstName);

        vm.Poco.FirstName = "Jiro";
        Assert.AreEqual("Jiro", vm.FirstNameTwoWay.Value);
    }

    [TestMethod]
    public void OneWay_PocoChangePropagatesToProperty()
    {
        using var vm = new CreateFromPocoViewModel();

        vm.Poco.LastName = "Yamada";
        Assert.AreEqual("Yamada", vm.LastNameOneWay.Value);
    }

    [TestMethod]
    public void OneWayToSource_PropertyChangeWritesBackToPoco()
    {
        using var vm = new CreateFromPocoViewModel();

        vm.FirstNameOneWayToSource.Value = "Hanako";
        Assert.AreEqual("Hanako", vm.Poco.FirstName);
    }
}
