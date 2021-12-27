using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Reactive.Bindings.Internals;

internal static class ExpressionTreeUtils
{
    public static string GetPropertyPath<TType, TProperty>(Expression<Func<TType, TProperty>> propertySelector)
    {
        var memberExpression = propertySelector.Body as MemberExpression;
        if (memberExpression == null)
        {
            if (propertySelector.Body is not UnaryExpression unaryExpression) { throw new ArgumentException(nameof(propertySelector)); }
            memberExpression = unaryExpression.Operand as MemberExpression;
            if (memberExpression == null) { throw new ArgumentException(nameof(propertySelector)); }
        }

        var tokens = new LinkedList<string>();
        while (memberExpression != null)
        {
            tokens.AddFirst(memberExpression.Member.Name);
            memberExpression = memberExpression.Expression as MemberExpression;
        }

        return string.Join(".", tokens);
    }


    public static string GetPropertyName<TType, TProperty>(Expression<Func<TType, TProperty>> propertySelector)
    {
        var memberExpression = propertySelector.Body as MemberExpression;
        if (memberExpression == null)
        {
            if (!(propertySelector.Body is UnaryExpression unaryExpression)) { throw new ArgumentException(nameof(propertySelector)); }
            memberExpression = unaryExpression.Operand as MemberExpression;
            if (memberExpression == null) { throw new ArgumentException(nameof(propertySelector)); }
        }

        return memberExpression.Member.Name;
    }

    public static bool IsNestedPropertyPath<TSubject, TProperty>(Expression<Func<TSubject, TProperty>> propertySelector)
    {
        if (propertySelector.Body is MemberExpression member)
        {
            return !(member.Expression is ParameterExpression);
        };

        if (propertySelector.Body is UnaryExpression unary)
        {
            if (unary.Operand is MemberExpression unaryMember)
            {
                return !(unaryMember.Expression is ParameterExpression);
            }
        }

        throw new ArgumentException();
    }

}
