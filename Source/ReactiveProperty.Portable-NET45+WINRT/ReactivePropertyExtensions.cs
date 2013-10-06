using Codeplex.Reactive;
using System;
using System.Reactive;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
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
                self.SetValidateNotifyError(src =>
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
                            catch (Exception ex)
                            {
                                o.OnNext(new[] { ex });
                            }
                        });
                    });
                });
            }

            return self;
        }
    }
}
