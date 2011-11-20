using System;
using System.Windows;
using Codeplex.Reactive;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Codeplex.Reactive.Notifiers;

namespace WP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }

    public class MainPageViewModel
    {
        public ReactiveCommand NavigateBasics { get; private set; }
        public ReactiveCommand NavigateAsync { get; private set; }
        public ReactiveCommand NavigateValidation { get; private set; }
        public ReactiveCommand NavigateSerialization { get; private set; }
        public ReactiveCommand NavigateEventToReactive { get; private set; }
        public ReactiveCommand NavigateSynchronizeObject { get; private set; }

        public MainPageViewModel()
        {
            NavigateBasics = new ReactiveCommand();
            NavigateBasics.Subscribe(_ => Root.Navigate(new Uri("/Views/ReactivePropertyBasics.xaml", UriKind.Relative)));
            NavigateAsync = new ReactiveCommand();
            NavigateAsync.Subscribe(_ => Root.Navigate(new Uri("/Views/Asynchronous.xaml", UriKind.Relative)));
            NavigateValidation = new ReactiveCommand();
            NavigateValidation.Subscribe(_ => Root.Navigate(new Uri("/Views/Validation.xaml", UriKind.Relative)));
            NavigateSerialization = new ReactiveCommand();
            NavigateSerialization.Subscribe(_ => Root.Navigate(new Uri("/Views/Serialization.xaml", UriKind.Relative)));
            NavigateEventToReactive = new ReactiveCommand();
            NavigateEventToReactive.Subscribe(_ => Root.Navigate(new Uri("/Views/EventToReactive.xaml", UriKind.Relative)));
            NavigateSynchronizeObject = new ReactiveCommand();
            NavigateSynchronizeObject.Subscribe(_ => Root.Navigate(new Uri("/Views/SynchronizeObject.xaml", UriKind.Relative)));
        }

        PhoneApplicationFrame Root
        {
            get { return Application.Current.RootVisual as PhoneApplicationFrame; }
        }
    }
}