using System;
using System.Reactive.Linq;
using System.Windows;
using Codeplex.Reactive; // using Namespace

namespace WPF.ViewModels
{
    // ReactiveProperty and ReactiveCommand simple example.
    public class ReactivePropertyBasicsViewModel
    {
        public ReactiveProperty<string> InputText { get; private set; }
        public ReactiveProperty<string> DisplayText { get; private set; }
        public ReactiveCommand ShowMessageBox { get; private set; }

        public ReactivePropertyBasicsViewModel()
        {
            // mode is Flags.
            var allMode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe;

            // binding value from UI Control
            InputText = new ReactiveProperty<string>(initialValue: "", mode: allMode);

            // send value to UI Control
            DisplayText = InputText
                .Select(s => s.ToUpper())       // rx query1
                .Delay(TimeSpan.FromSeconds(1)) // rx query2
                .ToReactiveProperty();          // convert to ReactiveProperty

            ShowMessageBox = InputText
                .Select(s => !string.IsNullOrEmpty(s))   // condition sequence of CanExecute
                .ToReactiveCommand(); // Convert to ReactiveCommand

            // ReactiveCommand's Subscribe is set ICommand's Execute
            ShowMessageBox.Subscribe(_ => MessageBox.Show("Hello, Reactive Property!"));
        }
    }
}