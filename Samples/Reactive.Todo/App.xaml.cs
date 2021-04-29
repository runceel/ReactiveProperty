using Prism.Ioc;
using Prism.Modularity;
using Reactive.Bindings;
using Reactive.Bindings.Notifiers;
using Reactive.Bindings.Schedulers;
using Reactive.Todo.Main;
using Reactive.Todo.Views;
using System.Windows;

namespace Reactive.Todo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void Initialize()
        {
            ReactivePropertyScheduler.SetDefault(new ReactivePropertyWpfScheduler(Dispatcher));
            base.Initialize();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageBroker, MessageBroker>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<MainModule>();
        }
    }
}
