using System.ComponentModel.DataAnnotations;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace BlazorSample.Client.ViewModels;

public class ValidationViewModel : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(10, ErrorMessage = "First name is under 10 letters.")]
    public ValidatableReactiveProperty<string> FirstName { get; }
    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(10, ErrorMessage = "Last name is under 10 letters.")]
    public ReactiveProperty<string> LastName { get; }
    public ReadOnlyReactivePropertySlim<string> FullName { get; }

    public ReactivePropertySlim<string> Message { get; }

    public AsyncReactiveCommand SubmitCommand { get; }
    public AsyncReactiveCommand InvalidSubmitCommand { get; }

    private readonly ReactivePropertySlim<bool> _sharedCanExecute;

    public ValidationViewModel()
    {
        var reactivePropertyMode = ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError;
        FirstName = ValidatableReactiveProperty.CreateFromDataAnnotations(
            "",
            () => FirstName,
            mode: reactivePropertyMode)
            .AddTo(_disposables);
        LastName = new ReactiveProperty<string>("", mode: reactivePropertyMode)
            .SetValidateAttribute(() => LastName)
            .AddTo(_disposables);

        FullName = FirstName.CombineLatest(LastName, (x, y) => $"{x} {y}")
            .ToReadOnlyReactivePropertySlim("")
            .AddTo(_disposables);

        _sharedCanExecute = new ReactivePropertySlim<bool>(true).AddTo(_disposables);
        SubmitCommand = new AsyncReactiveCommand(_sharedCanExecute)
            .WithSubscribe(SubmitAsync, _disposables.Add)
            .AddTo(_disposables);

        InvalidSubmitCommand = new[]
            {
                FirstName.ObserveHasErrors,
                LastName.ObserveHasErrors,
            }.CombineLatest(x => x.Any(y => y))
            .ToAsyncReactiveCommand(_sharedCanExecute)
            .WithSubscribe(InvalidSubmitAsync, _disposables.Add)
            .AddTo(_disposables);

        Message = new ReactivePropertySlim<string>("")
            .AddTo(_disposables);
    }

    private async Task SubmitAsync()
    {
        Message.Value = $"Starting {nameof(SubmitAsync)}";
        await Task.Delay(3000);
        Message.Value = $"Finished {nameof(SubmitAsync)}";
    }

    private async Task InvalidSubmitAsync()
    {
        Message.Value = $"Starting {nameof(InvalidSubmitAsync)}";
        await Task.Delay(3000);
        Message.Value = $"Finished {nameof(InvalidSubmitAsync)}";
    }

    public void Dispose() => _disposables.Dispose();
}
