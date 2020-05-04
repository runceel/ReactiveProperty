using Reactive.Todo.Main.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Reactive.Todo.Main.Models;

namespace Reactive.Todo.Main
{
    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("InputRegion", typeof(InputView));
            regionManager.RegisterViewWithRegion("ListRegion", typeof(TodoItemsView));
            regionManager.RegisterViewWithRegion("CommandsRegion", typeof(CommandsView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<TodoApp>();
        }
    }
}
