using System.Windows.Input;
using ReactivePropertySamples.Migrated.ViewModels;

namespace ReactivePropertySamples.R3.Tests;

// CollectionsViewModel also exposes GuidViewModels (updated via ObserveOnThreadPool + a captured
// SynchronizationContext) and FilteredItems (driven by a per-item ReactiveTimer with random values).
// Those paths are time- and thread-dependent and cannot be asserted deterministically in a headless
// unit test without a SynchronizationContext pump / TimeProvider seam the sample does not expose, so
// they are intentionally not covered here. The synchronous ReactiveCommand + ObservableCollection
// path is the deterministic surface and is covered below.
[TestClass]
public class CollectionsViewModelTests
{
    [TestMethod]
    public void AddToReactiveCollectionCommand_AddsItem()
    {
        using var vm = new CollectionsViewModel();
        Assert.AreEqual(0, vm.ReactiveCollection.Count);

        ((ICommand)vm.AddToReactiveCollectionCommand).Execute(null);

        Assert.AreEqual(1, vm.ReactiveCollection.Count);
    }

    [TestMethod]
    public void ClearReactiveCollectionCommand_EmptiesCollection()
    {
        using var vm = new CollectionsViewModel();
        ((ICommand)vm.AddToReactiveCollectionCommand).Execute(null);
        ((ICommand)vm.AddToReactiveCollectionCommand).Execute(null);
        Assert.AreEqual(2, vm.ReactiveCollection.Count);

        ((ICommand)vm.ClearReactiveCollectionCommand).Execute(null);

        Assert.AreEqual(0, vm.ReactiveCollection.Count);
    }
}
