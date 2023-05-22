using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Reactive.Bindings.Internals;

internal static class ExpressionTreeUtils
{
    public static string GetPropertyPath<TType, TProperty>(Expression<Func<TType, TProperty>> propertySelector)
    {
        var memberExpression = GetMemberExpressionFromPropertySelector(propertySelector);

        var tokens = new LinkedList<string>();
        MemberExpression? currentMemberExpression = memberExpression;
        while (currentMemberExpression != null)
        {
            tokens.AddFirst(currentMemberExpression.Member.Name);
            currentMemberExpression = currentMemberExpression!.Expression as MemberExpression;
        }

        return string.Join(".", tokens);
    }

    private static MemberExpression GetMemberExpressionFromPropertySelector<TType, TProperty>(Expression<Func<TType, TProperty>> propertySelector)
    {
        var memberExpression = propertySelector.Body switch
        {
            MemberExpression me => me,
            UnaryExpression ue => ue.Operand as MemberExpression,
            _ => null,
        };

        if (memberExpression is null) throw new ArgumentException(nameof(propertySelector));
        return memberExpression;
    }

    public static string GetPropertyName<TType, TProperty>(Expression<Func<TType, TProperty>> propertySelector) => 
        GetMemberExpressionFromPropertySelector(propertySelector).Member.Name;

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

        throw new ArgumentException(nameof(propertySelector));
    }

}
