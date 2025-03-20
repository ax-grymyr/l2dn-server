using System.Collections.Frozen;
using System.Linq.Expressions;
using System.Reflection;
using NLog;

namespace L2Dn.GameServer.Handlers;

public sealed class AbstractEffectFactory
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AbstractEffectFactory));

    private FrozenDictionary<string, Func<EffectParameterSet, IAbstractEffect>> _factories =
        FrozenDictionary<string, Func<EffectParameterSet, IAbstractEffect>>.Empty;

    private AbstractEffectFactory()
    {
    }

    public static AbstractEffectFactory Instance { get; } = new();

    public void Register(Assembly assembly)
    {
        _factories = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IAbstractEffect))).
            Select(type => (type, type.GetCustomAttribute<AbstractEffectNameAttribute>()?.Name ?? string.Empty)).
            Where(t => !string.IsNullOrEmpty(t.Item2)).
            Select(t => (t.Item2, CreateFactory(t.Item1))).
            ToFrozenDictionary(t => t.Item1, t => t.Item2, StringComparer.OrdinalIgnoreCase);

        _logger.Info($"{GetType().Name}: Registered {_factories.Count} effect handlers.");
    }

    public IAbstractEffect? Create(string name, EffectParameterSet parameters) =>
        _factories.TryGetValue(name, out Func<EffectParameterSet, IAbstractEffect>? factory)
            ? factory(parameters)
            : null;

    private static Func<EffectParameterSet, IAbstractEffect> CreateFactory(Type type)
    {
        ConstructorInfo? constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            [typeof(EffectParameterSet)]);

        ParameterExpression parameters = Expression.Parameter(typeof(EffectParameterSet), "parameters");

        if (constructor != null)
        {
            NewExpression newExpression = Expression.New(constructor, parameters);
            return Expression.Lambda<Func<EffectParameterSet, IAbstractEffect>>(newExpression, parameters).Compile();
        }

        constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, []);
        if (constructor != null)
        {
            NewExpression newExpression = Expression.New(constructor);
            return Expression.Lambda<Func<EffectParameterSet, IAbstractEffect>>(newExpression, parameters).Compile();
        }

        throw new InvalidOperationException($"Abstract effect type {type} does not have supported constructor.");
    }
}