using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using Codeplex.Reactive;
using Codeplex.Reactive.Extensions;
using System.Collections;

namespace Sample.ViewModels
{
    public class ValidationViewModel
    {
        public ReactiveProperty<string> ValidationData { get; private set; }
        public ReactiveProperty<string> ValidationNotify { get; private set; }
        public ReactiveProperty<string> ValidationBoth { get; private set; }
        public ReactiveProperty<string> ErrorInfo { get; private set; }
        public ReactiveCommand NextCommand { get; private set; }
        public ReactiveProperty<string> AlertMessage { get; private set; }

        public ValidationViewModel()
        {
            // null is success(have no error), string is error message
            ValidationData = new ReactiveProperty<string>()
                .SetValidateNotifyError((string s) => !string.IsNullOrEmpty(s) && s.Cast<char>().All(Char.IsUpper) ? null : "not all uppercase");

            // async validation
            // first argument is self observable sequence
            // null is success(have no error), IEnumerable is error messages
            ValidationNotify = new ReactiveProperty<string>("foo!", ReactivePropertyMode.RaiseLatestValueOnSubscribe)
                .SetValidateNotifyError(self => self
                    .Delay(TimeSpan.FromSeconds(3)) // asynchronous validation...
                    .Select(s => string.IsNullOrEmpty(s) ? null : "not empty string"));

            // both validation
            ValidationBoth = new ReactiveProperty<string>()
                .SetValidateNotifyError(s => !string.IsNullOrEmpty(s) && s.Cast<char>().All(Char.IsLower) ? null : "not all lowercase")
                .SetValidateNotifyError(self => self
                    .Delay(TimeSpan.FromSeconds(1)) // asynchronous validation...
                    .Select(s => string.IsNullOrEmpty(s) || s.Length <= 5 ? null : (IEnumerable)new[] { "length 5" }));

            // Validation result is pushed to ObserveErrors
            var errors = new[]
                {
                    ValidationData.ObserveErrorChanged,
                    ValidationBoth.ObserveErrorChanged,
                    ValidationNotify.ObserveErrorChanged
                }
                .CombineLatest();

            // Use OfType, choose error source
            ErrorInfo = errors
                .SelectMany(x => x)
                .Where(x => x != null)
                .Select(x => x.OfType<string>())
                .Select(x => x.FirstOrDefault())
                .ToReactiveProperty();

            // Validation is view initialized not run in default.
            // If want to validate on view initialize,
            // use ReactivePropertyMode.RaiseLatestValueOnSubscribe to ReactiveProperty
            // that mode is validate values on initialize.
            NextCommand =
                new[]
                {
                    ValidationData.ObserveHasError,
                    ValidationBoth.ObserveHasError,
                    ValidationNotify.ObserveHasError
                }
                .CombineLatestValuesAreAllFalse()
                .ToReactiveCommand();
            this.AlertMessage = NextCommand.Select(_ => "Can go to next!").ToReactiveProperty(
                initialValue: "Can go to next!",
                mode: ReactivePropertyMode.None);
        }
    }
}