using System;
using Codeplex.Reactive;
using Silverlight.Views;

namespace Silverlight
{
    public partial class MainPage : System.Windows.Controls.UserControl
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

        public MainPageViewModel()
        {
            NavigateBasics = new ReactiveCommand();
            NavigateBasics.Subscribe(_ => new ReactivePropertyBasics().Show());
            NavigateAsync = new ReactiveCommand();
            NavigateAsync.Subscribe(_ => new Asynchronous().Show());
            NavigateValidation = new ReactiveCommand();
            NavigateValidation.Subscribe(_ => new Validation().Show());
            NavigateSerialization = new ReactiveCommand();
            NavigateSerialization.Subscribe(_ => new Serialization().Show());
            NavigateEventToReactive = new ReactiveCommand();
            NavigateEventToReactive.Subscribe(_ => new EventToReactive().Show());
        }
    }
}