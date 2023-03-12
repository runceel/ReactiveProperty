using System.Reactive.Disposables;
using System.Reflection;
using Microsoft.AspNetCore.Components.Forms;
using Reactive.Bindings.Extensions;

namespace Reactive.Bindings.Components;

/// <summary>
/// EditContext extensions for ReactivePropertiesValidator
/// </summary>
public static class ReactivePropertiesValidatorEditContextExtensions
{
    /// <summary>
    /// Enable ReactiveProperties validation support for EditContext
    /// </summary>
    /// <param name="editContext">The Microsoft.AspNetCore.Components.Forms.EditContext.</param>
    /// <returns>A disposable object whose disposal will remove ReactiveProperties validation support from the Microsoft.AspNetCore.Components.Forms.EditContext.</returns>
    public static IDisposable EnableReactivePropertiesValidation(this EditContext editContext)
    {
        bool isValidatableReactiveProperty(PropertyInfo x) =>
            x.PropertyType.IsAssignableTo(typeof(ValidatableReactiveProperty<>).MakeGenericType(x.PropertyType.GenericTypeArguments[0]));


        bool isReactiveProperty(PropertyInfo x) =>
            x.PropertyType.IsAssignableTo(typeof(ReactiveProperty<>).MakeGenericType(x.PropertyType.GenericTypeArguments[0]));

        var disposable = new CompositeDisposable();
        var messages = new ValidationMessageStore(editContext);
        var model = editContext.Model;
        // Collect IReactiveProperty<T> from Model's properties.
        var properties = editContext.Model.GetType()
            .GetProperties()
            .Where(x => x.PropertyType.IsAssignableTo(typeof(IReactiveProperty)))
            .Where(x => x.CanRead)
            .Where(x => x.PropertyType.GenericTypeArguments.Length == 1)
            .Where(x => isReactiveProperty(x) || isValidatableReactiveProperty(x))
            .Select(x => (IReactiveProperty?)x.GetValue(model))
            .Where(x => x != null)
            .Select(x => x!)
            .ToList();

        foreach (var property in properties)
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

                    editContext.NotifyValidationStateChanged();
                })
                .AddTo(disposable);
        }

        void validateAll(object? sender, ValidationRequestedEventArgs e)
        {
            if (properties is null) return;

            foreach (var property in properties)
            {
                property.ForceNotify();
            }
        }
        // When validation process was requested, then ReactiveProperty#ForceNotify call to trigger validation process.
        editContext.OnValidationRequested += validateAll;
        Disposable.Create<(EditContext EditContext, EventHandler<ValidationRequestedEventArgs> EventHandler)>(
            (editContext, validateAll),
            state => state.EditContext.OnValidationRequested -= state.EventHandler)
            .AddTo(disposable);

        return disposable;
    }
}
