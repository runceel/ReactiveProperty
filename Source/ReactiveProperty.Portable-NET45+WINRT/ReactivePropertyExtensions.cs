using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace Codeplex.Reactive
{
    public static class ReactivePropertyExtensions
    {
        public static ReactiveProperty<T> SetValidateAttribute<T>(this ReactiveProperty<T> self, Expression<Func<ReactiveProperty<T>>> selfSelector)
        {
            var memberExpression = (MemberExpression)selfSelector.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var attrs = propertyInfo.GetCustomAttributes<ValidationAttribute>();
            var context = new ValidationContext(self)
            {
                MemberName = "Value"
            };

            if (attrs.Any())
            {
                self.SetValidateNotifyError(src => ValidateAttribute(src, context, attrs));
            }

            return self;
        }

        public static ReactiveProperty<T> AddValidateAttribute<T>(this ReactiveProperty<T> self, Expression<Func<ReactiveProperty<T>>> selfSelector)
        {
            var memberExpression = (MemberExpression)selfSelector.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var attrs = propertyInfo.GetCustomAttributes<ValidationAttribute>();
            var context = new ValidationContext(self)
            {
                MemberName = "Value"
            };

            if (attrs.Any())
            {
                self.AddValidateNotifyError(src => ValidateAttribute(src, context, attrs));
            }

            return self;
        }


        private static IObservable<IEnumerable> ValidateAttribute<T>(IObservable<T> src, ValidationContext context, IEnumerable<ValidationAttribute> attrs)
        {
            return Observable.Create<IEnumerable>(o =>
            {
                return src.Subscribe(value =>
                {
                    try
                    {
                        Validator.ValidateValue(value, context, attrs);
                        o.OnNext(null);
                    }
                    catch (ValidationException ex)
                    {
                        o.OnNext(ex.ValidationResult.ErrorMessage);
                    }
                });
            });
        }
    }
}
