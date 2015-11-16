using Microsoft.Practices.Unity;
using Prism.Unity.Windows;
using System.Threading.Tasks;
using UWP.TodoMVVM.Models;
using UWP.TodoMVVM.Services;
using Windows.ApplicationModel.Activation;

namespace UWP.TodoMVVM
{
    sealed partial class App : PrismUnityApplication
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.RegisterType<TodoManager>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<TodoService>(new ContainerControlledLifetimeManager());
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            this.Container.Resolve<TodoService>().Load();
            return Task.CompletedTask;
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            this.NavigationService.Navigate("Main", args.Arguments);
            return Task.CompletedTask;
        }
    }
}
