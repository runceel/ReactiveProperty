using Codeplex.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace WP8.ViewModels
{
    public class ReactivePropertyBasicsViewModel
    {
        public ReactiveProperty<string> InputText { get; private set; }
        public ReactiveProperty<string> DisplayText { get; private set; }
        public ReactiveCommand ReplaceTextCommand { get; private set; }

        public ReactivePropertyBasicsViewModel()
        {
            // mode is Flags. (default is all)
            // DistinctUtilChanged is no push value if next value is same as current
            // RaiseLatestValueOnSubscribe is push value when subsribed
            var allMode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe;

            // binding value from UI Control
            // if no set initialValue then initialValue is default(T). int:0, string:null...
            this.InputText = new ReactiveProperty<string>(initialValue: "", mode: allMode);

            // send value to UI Control
            this.DisplayText = this.InputText
                .Select(s => s.ToUpper())
                .Delay(TimeSpan.FromSeconds(1))
                .ToReactiveProperty();

            this.ReplaceTextCommand = this.InputText
                .Select(s => !string.IsNullOrEmpty(s))
                .ToReactiveCommand();

            // ReactiveCommand's Subscribe is set ICommand's Execute
            // ReactiveProperty.Value set is push(& set) value
            ReplaceTextCommand.Subscribe(_ => InputText.Value = "Hello, ReactiveProperty!");
        }
    }
}
