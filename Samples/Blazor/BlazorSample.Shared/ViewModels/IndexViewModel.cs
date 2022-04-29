using System.ComponentModel.DataAnnotations;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace BlazorSample.Shared.ViewModels;

public class IndexViewModel : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(10, ErrorMessage = "First name is under 10 letters.")]
    public ReactiveProperty<string> FirstName { get; }
    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(10, ErrorMessage = "Last name is under 10 letters.")]
    public ReactiveProperty<string> LastName { get; }
    public ReadOnlyReactivePropertySlim<string> FullName { get; }

    public IndexViewModel()
    {
        var reactivePropertyMode = ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError;
        FirstName = new ReactiveProperty<string>("", mode: reactivePropertyMode)
            .SetValidateAttribute(() => FirstName)
            .AddTo(_disposables);
        LastName = new ReactiveProperty<string>("", mode: reactivePropertyMode)
            .SetValidateAttribute(() => LastName)
            .AddTo(_disposables);

        FullName = Observable.CombineLatest(new[] { FirstName, LastName }, x => $"{x[0]} {x[1]}")
            .ToReadOnlyReactivePropertySlim("")
            .AddTo(_disposables);
    }

    public void Dispose() => _disposables.Dispose();
}
