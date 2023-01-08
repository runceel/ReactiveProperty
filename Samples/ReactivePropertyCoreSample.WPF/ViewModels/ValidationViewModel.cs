using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;
using ReactivePropertyCoreSample.WPF.Models;

namespace ReactivePropertyCoreSample.WPF.ViewModels
{
    public class ValidationViewModel : ViewModelBase
    {
        [Required(ErrorMessage = "Required property")]
        public ValidatableReactiveProperty<string> WithDataAnnotations { get; }

        public ValidatableReactiveProperty<string> WithCustomValidationLogic { get; }

        public ReadOnlyReactivePropertySlim<bool> HasValidationErrors { get; }

        public ReactiveCommandSlim SubmitCommand { get; }

        public ReadOnlyReactivePropertySlim<string> Message { get; }

        [Required]
        public ValidatableReactiveProperty<string> IgnoreInitialValidationError { get; }

        public Poco Poco { get; } = new Poco { FirstName = "Kazuki" };
        [Required]
        public ValidatableReactiveProperty<string> FirstName { get; }

        public ValidationViewModel()
        {
            WithDataAnnotations = ValidatableReactiveProperty.CreateFromDataAnnotations(
                "",
                () => WithDataAnnotations)
                .AddTo(Disposables);
                
            WithCustomValidationLogic = new ValidatableReactiveProperty<string>(
                "",
                x => !string.IsNullOrEmpty(x) && x.Contains("-") ? null : "Require '-'")
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
                    WithDataAnnotations.ObserveHasErrors,
                    WithCustomValidationLogic.ObserveHasErrors,
                }.CombineLatest(x => x.All(x => x is false))
                .ToReactiveCommandSlim(false)
                .AddTo(Disposables);
            Message = SubmitCommand
                .Select(_ => $"You submittted \'{WithDataAnnotations.Value} & {WithCustomValidationLogic.Value}\'")
                .ToReadOnlyReactivePropertySlim("")
                .AddTo(Disposables);

            IgnoreInitialValidationError = ValidatableReactiveProperty.CreateFromDataAnnotations(
                "",
                () => IgnoreInitialValidationError,
                mode: ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError)
                .AddTo(Disposables);

            FirstName = Poco.ToReactivePropertySlimAsSynchronized(x => x.FirstName)
                .ToValidatableReactiveProperty(
                    () => FirstName,
                    mode: ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError,
                    disposeSource: true)
                .AddTo(Disposables);
        }
    }
}
