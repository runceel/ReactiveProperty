using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Models;

namespace ReactivePropertySamples.ViewModels
{
    public class ValidationViewModel : ViewModelBase
    {
        [Required(ErrorMessage = "Required property")]
        public ReactiveProperty<string> WithDataAnnotations { get; }
        public ReadOnlyReactivePropertySlim<string> WithDataAnnotationsErrorMessage { get; }

        public ReactiveProperty<string> WithCustomValidationLogic { get; }
        public ReadOnlyReactivePropertySlim<string> WithCustomValidationLogicErrorMessage { get; }

        public ReadOnlyReactivePropertySlim<bool> HasValidationErrors { get; }

        public ReactiveCommand SubmitCommand { get; }

        public ReadOnlyReactivePropertySlim<string> Message { get; }

        [Required]
        public ReactiveProperty<string> IgnoreInitialValidationError { get; }

        public Poco Poco { get; } = new Poco { FirstName = "Kazuki" };
        [Required]
        public ReactiveProperty<string> FirstName { get; }

        public ValidationViewModel()
        {
            WithDataAnnotations = new ReactiveProperty<string>()
                .SetValidateAttribute(() => WithDataAnnotations)
                .AddTo(Disposables);
            WithDataAnnotationsErrorMessage = WithDataAnnotations
                .ObserveValidationErrorMessage()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            WithCustomValidationLogic = new ReactiveProperty<string>()
                .SetValidateNotifyError(x => !string.IsNullOrEmpty(x) && x.Contains("-") ? null : "Require '-'")
                .AddTo(Disposables);
            WithCustomValidationLogicErrorMessage = WithCustomValidationLogic
                .ObserveValidationErrorMessage()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            HasValidationErrors = new[]
                {
                    WithDataAnnotations.ObserveHasErrors,
                    WithCustomValidationLogic.ObserveHasErrors,
                }.CombineLatest(x => x.Any(y => y))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SubmitCommand = new[]
                {
                    WithDataAnnotations.ObserveHasErrors.Inverse(),
                    WithCustomValidationLogic.ObserveHasErrors.Inverse(),
                }.CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .AddTo(Disposables);
            Message = WithDataAnnotations.CombineLatest(WithCustomValidationLogic, (x, y) => $"You submitted \'{x} & {y}\'")
                .Throttle(_ => SubmitCommand)
                .ToReadOnlyReactivePropertySlim();

            IgnoreInitialValidationError = new ReactiveProperty<string>(mode: ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError)
                .SetValidateAttribute(() => IgnoreInitialValidationError)
                .AddTo(Disposables);


            FirstName = Poco.ToReactivePropertyAsSynchronized(x => x.FirstName, ignoreValidationErrorValue: true)
                .SetValidateAttribute(() => FirstName)
                .AddTo(Disposables);
        }
    }
}
