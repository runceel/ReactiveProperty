using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Reactive.Bindings.Internals;

internal static class PropertyPathAccessor
{
    private static readonly MethodInfo s_dynamicInvokeMethod = typeof(Delegate).GetMethod(
        nameof(Delegate.DynamicInvoke),
        new[] { typeof(object[]) })!;
    private static readonly ConditionalWeakTable<Delegate, Func<object, object?>> s_getters = new();
    private static readonly ConditionalWeakTable<Delegate, Action<object, object?>> s_setters = new();

    public static Func<object, object?> Get(Delegate accessor) => s_getters.GetValue(accessor, CreateGetter);

    public static Action<object, object?> Set(Delegate accessor) => s_setters.GetValue(accessor, CreateSetter);

    private static Func<object, object?> CreateGetter(Delegate accessor)
    {
        var typeArguments = accessor.GetType().GetGenericArguments();
        var source = Expression.Parameter(typeof(object), "source");
        var invoke = Expression.Invoke(Expression.Constant(accessor), Expression.Convert(source, typeArguments[0]));
        var fallback = Expression.Call(
            Expression.Constant(accessor),
            s_dynamicInvokeMethod,
            Expression.NewArrayInit(typeof(object), source));
        var body = Expression.Condition(
            Expression.TypeIs(source, typeArguments[0]),
            Expression.Convert(WrapTargetExceptions(invoke), typeof(object)),
            fallback);
        return Expression.Lambda<Func<object, object?>>(body, source).Compile();
    }

    private static Action<object, object?> CreateSetter(Delegate accessor)
    {
        var typeArguments = accessor.GetType().GetGenericArguments();
        var source = Expression.Parameter(typeof(object), "source");
        var value = Expression.Parameter(typeof(object), "value");
        var invoke = Expression.Invoke(
            Expression.Constant(accessor),
            Expression.Convert(source, typeArguments[0]),
            Expression.Convert(value, typeArguments[1]));
        var fallback = Expression.Call(
            Expression.Constant(accessor),
            s_dynamicInvokeMethod,
            Expression.NewArrayInit(typeof(object), source, value));
        var body = Expression.IfThenElse(
            Expression.AndAlso(Expression.TypeIs(source, typeArguments[0]), Expression.TypeIs(value, typeArguments[1])),
            WrapTargetExceptions(invoke),
            Expression.Block(fallback, Expression.Empty()));
        return Expression.Lambda<Action<object, object?>>(body, source, value).Compile();
    }

    private static Expression WrapTargetExceptions(Expression invoke)
    {
        var exception = Expression.Parameter(typeof(Exception), "exception");
        var constructor = typeof(TargetInvocationException).GetConstructor(new[] { typeof(Exception) })!;
        // DynamicInvoke wraps exceptions thrown by the target delegate.
        return Expression.TryCatch(
            invoke,
            Expression.Catch(exception, Expression.Throw(Expression.New(constructor, exception), invoke.Type)));
    }
}
