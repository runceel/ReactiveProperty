using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Reactive.Bindings.Extensions;

namespace Reactive.Bindings.Components;

/// <summary>
/// Component to integrate ReactiveProperty validation feature to EditForm.
/// </summary>
public class ReactivePropertiesValidator : ComponentBase, IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    /// <summary>
    /// EditContext from EditForm
    /// </summary>
    [CascadingParameter]
    public EditContext? CurrentEditContext { get; set; }

    private EditContext _originalEditContext = default!;

    private IReadOnlyCollection<IReactiveProperty>? _properties;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
            throw new InvalidOperationException("EditContext of CascadingParameter is null.");

        _originalEditContext = CurrentEditContext;
        if (_originalEditContext.Model is not { } model) return;
        var messages = new ValidationMessageStore(_originalEditContext);

        // Collect IReactiveProperty<T> from Model's properties.
        _properties = model.GetType()
            .GetProperties()
            .Where(x => x.PropertyType.IsAssignableTo(typeof(IReactiveProperty)))
            .Where(x => x.CanRead)
            .Where(x => x.PropertyType.GenericTypeArguments.Length == 1)
            .Where(x => x.PropertyType.IsAssignableTo(typeof(ReactiveProperty<>).MakeGenericType(x.PropertyType.GenericTypeArguments[0])))
            .Select(x => (IReactiveProperty?)x.GetValue(model))
            .Where(x => x != null)
            .Select(x => x!)
            .ToList();

        foreach (var property in _properties)
        {
            // When validation state of ReactiveProperty changed, then notify it to ValidationMessageStore
            var identifier = new FieldIdentifier(property, nameof(property.Value));
            property.ObserveErrorChanged
                .Subscribe(x =>
                {
                    messages.Clear(identifier);
                    if (x is not null)
                    {
                        foreach (var message in x.OfType<string>())
                        {
                            messages.Add(identifier, message);
                        }
                    }

                    _originalEditContext.NotifyValidationStateChanged();
                })
                .AddTo(_disposables);
        }

        // When validation process was requested, then ReactiveProperty#ForceNotify call to trigger validation process.
        _originalEditContext.OnValidationRequested += ValidateAll;
        _disposables.Add(
            Disposable.Create<(EditContext EditContext, EventHandler<ValidationRequestedEventArgs> EventHandler)>(
                (_originalEditContext, ValidateAll), 
                state => state.EditContext.OnValidationRequested -= state.EventHandler));
    }

    private void ValidateAll(object? sender, ValidationRequestedEventArgs e)
    {
        if (_properties is null) return;

        foreach (var property in _properties)
        {
            property.ForceNotify();
        }
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
