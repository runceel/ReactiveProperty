using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace Reactive.Bindings
{
    /// <summary>
    /// Reactive Property Extensions
    /// </summary>
    public static class ReactivePropertyExtensions
    {
        /// <summary>
        /// Set validation logic from DataAnnotations attributes.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="self">Target ReactiveProperty</param>
        /// <param name="selfSelector">Target property as expression</param>
        /// <returns>Self</returns>
        public static ReactiveProperty<T> SetValidateAttribute<T>(this ReactiveProperty<T> self, Expression<Func<ReactiveProperty<T>>> selfSelector)
        {
            var memberExpression = (MemberExpression)selfSelector.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var display = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            var attrs = propertyInfo.GetCustomAttributes<ValidationAttribute>().ToArray();
            var context = new ValidationContext(self)
            {
                DisplayName = display?.GetName() ?? propertyInfo.Name,
                MemberName = nameof(ReactiveProperty<T>.Value),
            };

            if (attrs.Length != 0)
            {
                self.SetValidateNotifyError(x =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (Validator.TryValidateValue(x, context, validationResults, attrs))
                    {
                        return null;
                    }

                    return validationResults[0].ErrorMessage;
                });
            }

            return self;
        }

        /// <summary>
        /// Create an IObservable instance to observe validation error messages of ReactiveProperty.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="self">Target ReactiveProperty</param>
        public static IObservable<string> ObserveValidationErrorMessage<T>(this ReactiveProperty<T> self) =>
            self.ObserveErrorChanged
                .Select(x => x?.OfType<string>()?.FirstOrDefault());
    }
}
