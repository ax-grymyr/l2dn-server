using System.Collections.Frozen;
using System.Linq.Expressions;
using System.Reflection;
using L2Dn.GameServer.Handlers;

namespace L2Dn.GameServer.Utilities;

internal static class FactoryHelper
{
    internal static FrozenDictionary<TKey, Func<TArg, THandler>> CreateFactories<TKey, TArg, THandler>(Assembly assembly)
        where TKey: notnull
    {
        return GetAllHandlerTypes<TKey>(assembly, typeof(THandler)).Select(pair =>
                new KeyValuePair<TKey, Func<TArg, THandler>>(pair.Key, CreateFactory<TArg, THandler>(pair.Type))).
            ToFrozenDictionary();
    }

    private static Func<TArg, THandler> CreateFactory<TArg, THandler>(Type type)
    {
        ConstructorInfo? constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            [typeof(TArg)]);

        ParameterExpression parameters = Expression.Parameter(typeof(TArg), "arg");

        if (constructor != null)
        {
            NewExpression newExpression = Expression.New(constructor, parameters);
            return Expression.Lambda<Func<TArg, THandler>>(newExpression, parameters).Compile();
        }

        constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, []);
        if (constructor != null)
        {
            NewExpression newExpression = Expression.New(constructor);
            return Expression.Lambda<Func<TArg, THandler>>(newExpression, parameters).Compile();
        }

        throw new InvalidOperationException($"Handler type {type} does not have supported constructor.");
    }

    private static IEnumerable<TypeHandlerKeyPair<TKey>> GetAllHandlerTypes<TKey>(Assembly assembly, Type baseType)
        where TKey: notnull
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsClass)
                continue;

            bool isOfBaseType;
            if (baseType.IsInterface)
            {
                isOfBaseType = type.GetInterfaces().Contains(baseType);
            }
            else
            {
                Type? typeBaseType = type.BaseType;
                while (typeBaseType != null && typeBaseType != baseType)
                    typeBaseType = typeBaseType.BaseType;

                isOfBaseType = typeBaseType == baseType;
            }

            if (!isOfBaseType)
                continue;

            HandlerKeyAttribute<TKey>? attribute = type.GetCustomAttribute<HandlerKeyAttribute<TKey>>();
            if (attribute is null)
                continue;

            yield return new TypeHandlerKeyPair<TKey>(attribute.Key, type);
        }
    }

    private readonly record struct TypeHandlerKeyPair<TKey>(TKey Key, Type Type);
}