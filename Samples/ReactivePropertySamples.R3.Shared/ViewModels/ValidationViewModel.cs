using System.ComponentModel.DataAnnotations;
using System.Linq;
using R3;
using Reactive.Bindings.R3;
using Reactive.Bindings.R3.Extensions;
using ReactivePropertySamples.Migrated.Models;

namespace ReactivePropertySamples.Migrated.ViewModels
{
    public class ValidationViewModel : ViewModelBase
    {
        // DataAnnotations validation on a single bound property -> BindableReactiveProperty + EnableValidation.
        [Required(ErrorMessage = "Required property")]
        public BindableReactiveProperty<string> WithDataAnnotations { get; }
        public BindableReactiveProperty<string> WithDataAnnotationsErrorMessage { get; }

        // Custom/stream validation -> ValidatableReactiveProperty from the ReactiveProperty.R3 bridge.
        public ValidatableReactiveProperty<string> WithCustomValidationLogic { get; }
        public BindableReactiveProperty<string> WithCustomValidationLogicErrorMessage { get; }

        public BindableReactiveProperty<bool> HasValidationErrors { get; }

        public ReactiveCommand SubmitCommand { get; }

        public BindableReactiveProperty<string> Message { get; }

        // IgnoreInitialValidationError mode is provided by the bridge's ValidatableReactiveProperty.
        [Required]
        public ValidatableReactiveProperty<string> IgnoreInitialValidationError { get; }

        public Poco Poco { get; } = new Poco { FirstName = "Kazuki" };
        [Required]
        public BindableReactiveProperty<string> FirstName { get; }

        public ValidationViewModel()
        {
            WithDataAnnotations = new BindableReactiveProperty<string>("")
                .EnableValidation(() => WithDataAnnotations)
                .AddTo(Disposables);
            WithDataAnnotationsErrorMessage = WithDataAnnotations.ObserveErrorChanged()
                .Select(x => x.Count > 0 ? x[0] : "")
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);

            WithCustomValidationLogic = new ValidatableReactiveProperty<string>("")
                .SetValidateNotifyError(x => !string.IsNullOrEmpty(x) && x.Contains("-") ? null : "Require '-'")
                .AddTo(Disposables);
            WithCustomValidationLogicErrorMessage = WithCustomValidationLogic.ObserveErrorChanged
                .Select(x => x.Count > 0 ? x[0] : "")
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);

            HasValidationErrors = Observable.CombineLatest(new[]
                {
                    WithDataAnnotations.ObserveHasErrors(),
                    WithCustomValidationLogic.ObserveHasErrors,
                })
                .Select(x => x.Any(y => y))
                .ToBindableReactiveProperty(false)
                .AddTo(Disposables);

            SubmitCommand = Observable.CombineLatest(new[]
                {
                    WithDataAnnotations.ObserveHasErrors().Select(x => !x),
                    WithCustomValidationLogic.ObserveHasErrors.Select(x => !x),
                })
                .Select(x => x.All(y => y))
                .ToReactiveCommand()
                .AddTo(Disposables);
            Message = SubmitCommand
                .Select(_ => $"You submitted '{WithDataAnnotations.Value} & {WithCustomValidationLogic.Value}'")
                .ToBindableReactiveProperty("")
                .AddTo(Disposables);

            IgnoreInitialValidationError = new ValidatableReactiveProperty<string>("", ignoreInitialValidationError: true)
                .SetValidateAttribute(() => IgnoreInitialValidationError)
                .AddTo(Disposables);

            FirstName = Poco.ToReactivePropertyAsSynchronized(x => x.FirstName)
                .EnableValidation(() => FirstName)
                .AddTo(Disposables);
        }
    }
}
