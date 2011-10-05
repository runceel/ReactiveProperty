using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Codeplex.Reactive;
using System.Reactive.Linq;
using System.Windows;

namespace WPF.ViewModels
{
    public class ValidationViewModel
    {
        [Required]
        [Range(0, 100)]
        public ReactiveProperty<string> Validation1 { get; private set; }
        public ReactiveProperty<string> Validation2 { get; private set; }
        public ReactiveProperty<string> ErrorInfo { get; private set; }
        public ReactiveCommand NextCommand { get; private set; }

        public ValidationViewModel()
        {
            Validation1 = new ReactiveProperty<string>()
                .SetValidateAttribute(() => Validation1);

            Validation2 = new ReactiveProperty<string>()
                .SetValidateError(s => s.All(Char.IsUpper) ? null : "not all uppercase");

            var errors = Validation1.ObserveErrorChanged.Merge(Validation2.ObserveErrorChanged);

            ErrorInfo = Observable.Merge(
                    errors.Where(o => o == null).Select(_ => ""), // success
                    errors.OfType<Exception>().Select(e => e.Message), // from attribute
                    errors.OfType<string>()) // from IDataErrorInfo
                .ToReactiveProperty();

            NextCommand = errors.Select(x => x == null).ToReactiveCommand(false);
            NextCommand.Subscribe(_ => MessageBox.Show("Can go to next!"));
        }
    }
}