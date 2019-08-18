using PrismSampleModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace PrismSampleModule
{
    public class PrismSampleModuleModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public PrismSampleModuleModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate("ContentRegion", "ViewA");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>();
        }
    }
}
