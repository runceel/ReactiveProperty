using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Reactive.Bindings
{
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
            var attrs = propertyInfo.GetCustomAttributes<ValidationAttribute>().ToArray();
            var context = new ValidationContext(self)
            {
                MemberName = nameof(ReactiveProperty<T>.Value)
            };

            if (attrs.Length != 0)
            {
                self.SetValidateNotifyError(x =>
                {
                    try
                    {
                        Validator.ValidateValue(x, context, attrs);
                        return null;
                    }
                    catch (ValidationException ex)
                    {
                        return ex.ValidationResult.ErrorMessage;
                    }
                });
            }

            return self;
        }
    }
}