using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Reactive.Bindings.Internals
{
    internal static class ExpressionTreeUtils
    {
        public static string GetPropertyPath<TType, TProperty>(Expression<Func<TType, TProperty>> propertySelector)
        {
            var memberExpression = GetMemberExpressionFromPropertySelector(propertySelector);
            var tokens = new LinkedList<string>();
            while (memberExpression != null)
            {
                tokens.AddFirst(memberExpression.Member.Name);
                if (memberExpression is not MemberExpression nextToken) { break; }
                memberExpression = nextToken;
            }

            return string.Join(".", tokens);
        }


        public static string GetPropertyName<TType, TProperty>(Expression<Func<TType, TProperty>> propertySelector) =>
            GetMemberExpressionFromPropertySelector(propertySelector).Member.Name;

        private static MemberExpression GetMemberExpressionFromPropertySelector<TType, TProperty>(Expression<Func<TType, TProperty>> propertySelector)
        {
            if (propertySelector.Body is not MemberExpression memberExpression)
            {
                if (propertySelector.Body is not UnaryExpression unaryExpression) { throw new ArgumentException(nameof(propertySelector)); }
                if (unaryExpression.Operand is not MemberExpression memberExpressionForUnary) { throw new ArgumentException(nameof(propertySelector)); }
                memberExpression = memberExpressionForUnary;
            }

            return memberExpression;
        }

        public static bool IsNestedPropertyPath<TSubject, TProperty>(Expression<Func<TSubject, TProperty>> propertySelector)
        {
            if (propertySelector.Body is MemberExpression member)
            {
                return member.Expression is not ParameterExpression;
            };

            if (propertySelector.Body is UnaryExpression unary)
            {
                if (unary.Operand is MemberExpression unaryMember)
                {
                    return unaryMember.Expression is not ParameterExpression;
                }
            }

            throw new ArgumentException();
        }

    }
}
