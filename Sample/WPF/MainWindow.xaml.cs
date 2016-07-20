using System;
using System.Windows;
using Reactive.Bindings;
using WPF.Views;

namespace WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class MainWindowViewModel
    {
        public ReactiveCommand NavigateBasics { get; private set; }
        public ReactiveCommand NavigateAsync { get; private set; }
        public ReactiveCommand NavigateValidation { get; private set; }
        public ReactiveCommand NavigateSerialization { get; private set; }
        public ReactiveCommand NavigateEventToReactive { get; private set; }
        public ReactiveCommand NavigateSynchronize { get; private set; }
        public ReactiveCommand NavigateEventToReactiveCommand { get; private set; }

        public ReactiveCommand NavigateAsyncReactiveCommand { get; }

        public MainWindowViewModel()
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
            NavigateEventToReactive.Subscribe(_ => new EventToReactiveProperty().Show());
            NavigateSynchronize = new ReactiveCommand();
            NavigateSynchronize.Subscribe(_ => new SynchronizeObject().Show());
            NavigateEventToReactiveCommand = new ReactiveCommand();
            NavigateEventToReactiveCommand.Subscribe(_ => new EventToReactiveCommandWindow().Show());
            NavigateAsyncReactiveCommand = new ReactiveCommand();
            NavigateAsyncReactiveCommand.Subscribe(_ => new Views.AsyncReactiveCommand().Show());
        }
    }
}