using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Codeplex.Reactive;
using Codeplex.Reactive.Extensions;
using System.Collections;

namespace Silverlight.ViewModels
{
    public class ValidationViewModel
    {
        [Required]
        [Range(0, 100)]
        public ReactiveProperty<string> ValidationAttr { get; private set; }
        public ReactiveProperty<string> ValidationData { get; private set; }
        [StringLength(5)]
        public ReactiveProperty<string> ValidationBoth { get; private set; }
        public ReactiveProperty<string> ValidationNotify { get; private set; }
        public ReactiveProperty<string> ErrorInfo { get; private set; }
        public ReactiveCommand NextCommand { get; private set; }

        public ValidationViewModel()
        {
            // DataAnnotation Attribute, call SetValidateAttribute and select self property
            // Note:error result dispatch to IDataErrorInfo, not exception.
            //      therefore, XAML is ValidatesOnDataErrors=True
            ValidationAttr = new ReactiveProperty<string>()
                .SetValidateAttribute(() => ValidationAttr);

            // IDataErrorInfo, call SetValidateError and set validate condition
            // null is success(have no error), string is error message
            ValidationData = new ReactiveProperty<string>()
                .SetValidateError(s => s.All(Char.IsUpper) ? null : "not all uppercase");

            // Can set both validation
            ValidationBoth = new ReactiveProperty<string>()
                .SetValidateAttribute(() => ValidationBoth)
                .SetValidateError(s => s.All(Char.IsLower) ? null : "not all lowercase");

            // INotifyDataErrorInfo, call SetValidateNotifyErro and set validate condition
            // first argument is self observable sequence
            // null is success(have no error), IEnumerable is error messages
            ValidationNotify = new ReactiveProperty<string>("foo!", ReactivePropertyMode.RaiseLatestValueOnSubscribe)
                .SetValidateNotifyError(self => self
                    .Delay(TimeSpan.FromSeconds(3)) // asynchronous validation...
                    .Select(s => string.IsNullOrEmpty(s) ? null : new[] { "not empty string" }));

            // Validation result is pushed to ObserveErrorChanged
            var errors = Observable.Merge(
                ValidationAttr.ObserveErrorChanged,
                ValidationData.ObserveErrorChanged,
                ValidationBoth.ObserveErrorChanged,
                ValidationNotify.ObserveErrorChanged);

            // Use OfType, choose error source
            ErrorInfo = Observable.Merge(
                    errors.Where(o => o == null).Select(_ => ""), // success
                    errors.OfType<Exception>().Select(e => e.Message), // from attribute
                    errors.OfType<string>(), // from IDataErrorInfo
                    errors.OfType<string[]>().Select(xs => xs[0]))  // from INotifyDataErrorInfo
                .ToReactiveProperty();

            // Validation is view initialized not run in default.
            // If want to validate on view initialize,
            // use ReactivePropertyMode.RaiseLatestValueOnSubscribe to ReactiveProperty
            // that mode is validate values on initialize.
            NextCommand = ValidationAttr.ObserveErrorChanged
                .CombineLatest(
                    ValidationData.ObserveErrorChanged,
                    ValidationBoth.ObserveErrorChanged,
                    ValidationNotify.ObserveErrorChanged,
                    (a, b, c, d) => new[] { a, b, c, d }.All(x => x == null))
                .ToReactiveCommand(initialValue: false);
            NextCommand.Subscribe(_ => MessageBox.Show("Can go to next!"));
        }
    }
}