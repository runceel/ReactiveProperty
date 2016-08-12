using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
    public class ValidationViewModel
    {
        [Required(ErrorMessage = "Required")]
        [Range(0, 100, ErrorMessage = "Range 0...100")]
        public ReactiveProperty<string> ValidationAttr { get; }
        public ReactiveProperty<string> ValidationData { get; }
        [StringLength(5, ErrorMessage = "Length < 5")]
        public ReactiveProperty<string> ValidationBoth { get; }
        public ReactiveProperty<string> ErrorInfo { get; }
        public ReactiveCommand NextCommand { get; }
        public ReactiveProperty<string> AlertMessage { get; }

        public ValidationViewModel()
        {
            // DataAnnotation Attribute, call SetValidateAttribute and select self property
            // Note:error result dispatch to IDataErrorInfo, not exception.
            //      therefore, XAML is ValidatesOnDataErrors=True
            ValidationAttr = new ReactiveProperty<string>()
                .SetValidateAttribute(() => ValidationAttr);

            // null is success(have no error), string is error message
            ValidationData = new ReactiveProperty<string>()
                .SetValidateNotifyError((string s) => 
                    string.IsNullOrEmpty(s) ? 
                        "required" :
                        s.Cast<char>().All(Char.IsUpper) ? 
                            null : 
                            "not all uppercase");

            // Can set both validation
            ValidationBoth = new ReactiveProperty<string>()
                .SetValidateAttribute(() => ValidationBoth)
                .SetValidateNotifyError(s => string.IsNullOrEmpty(s) ?
                    "required" :
                    s.Cast<char>().All(Char.IsLower) ?
                        null :
                        "not all lowercase")
                .SetValidateNotifyError(async x =>
                {
                    await Task.Delay(2000);
                    if (x == null)          return null;
                    if (x.Contains("a"))    return "'a' shouldn't be contained";
                    return null;
                })
                .SetValidateNotifyError(xs =>
                {
                    return xs
					    .Throttle(TimeSpan.FromMilliseconds(500))
                        .Select(x =>
                        {
                            if (x == null)          return null;
                            if (x.Contains("b"))    return "'b' shouldn't be contained";
                            return null;
                        });
                });


            // Validation result is pushed to ObserveErrors
            var errors = new[]
                {
                    ValidationData.ObserveErrorChanged,
                    ValidationBoth.ObserveErrorChanged,
                    ValidationAttr.ObserveErrorChanged
                }
                .CombineLatest(x =>
                {
                    var result = x.Where(y => y != null)
                        .Select(y => y.OfType<string>())
                        .Where(y => y.Any())
                        .FirstOrDefault();
                    return result == null ? null : result.FirstOrDefault();
                });

            // Use OfType, choose error source
            ErrorInfo = errors.ToReactiveProperty();

            // Validation is view initialized not run in default.
            // If want to validate on view initialize,
            // use ReactivePropertyMode.RaiseLatestValueOnSubscribe to ReactiveProperty
            // that mode is validate values on initialize.
            NextCommand =
                new[]
                {
                    ValidationData.ObserveHasErrors,
                    ValidationBoth.ObserveHasErrors,
                    ValidationAttr.ObserveHasErrors
                }
                .CombineLatestValuesAreAllFalse()
                .ToReactiveCommand();
            this.AlertMessage = this.NextCommand.Select(_ => "Can go to next!")
                .ToReactiveProperty(mode: ReactivePropertyMode.None);
        }
    }
}