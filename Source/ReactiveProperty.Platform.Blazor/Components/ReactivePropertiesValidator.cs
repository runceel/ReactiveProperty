using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Reactive.Bindings.Components;

/// <summary>
/// Component to integrate ReactiveProperty validation feature to EditForm.
/// </summary>
public class ReactivePropertiesValidator : ComponentBase, IDisposable
{
    private readonly SingleAssignmentDisposable _disposables = new();

    /// <summary>
    /// EditContext from EditForm
    /// </summary>
    [CascadingParameter]
    public EditContext? CurrentEditContext { get; set; }

    private EditContext _originalEditContext = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
            throw new InvalidOperationException("EditContext of CascadingParameter is null.");

        _originalEditContext = CurrentEditContext;
        if (_originalEditContext.Model is not { }) return;
        _disposables.Disposable = _originalEditContext.EnableReactivePropertiesValidation();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (CurrentEditContext != _originalEditContext)
            throw new InvalidOperationException($"{nameof(ReactivePropertiesValidator)} does not support changing EditContext.");
    }

    /// <inheritdoc />
    public void Dispose() => _disposables.Dispose();
}
