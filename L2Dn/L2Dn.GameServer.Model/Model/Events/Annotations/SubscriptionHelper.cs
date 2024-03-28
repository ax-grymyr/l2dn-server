using System.Collections.Immutable;
using System.Reflection;
using L2Dn.Events;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Olympiads;
using NLog;

namespace L2Dn.GameServer.Model.Events.Annotations;

internal static class SubscriptionHelper
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(SubscriptionHelper));
    
    private static readonly MethodInfo _subscribeMethod =
        typeof(SubscriptionHelper).GetMethod(nameof(SubscribeMethod), BindingFlags.Static | BindingFlags.NonPublic)!;

    public static IReadOnlyCollection<EventContainer> Subscribe(SubscriptionType subscriptionType, object obj,
	    MethodInfo method)
    {
	    string methodName = $"class {method.DeclaringType?.FullName}, method {method.Name}";

	    if (!Enum.IsDefined(subscriptionType))
	    {
		    _logger.Warn($"Subscription type {subscriptionType} is not supported in {methodName}");
		    return Array.Empty<EventContainer>();
	    }
	    
	    if (method.ReturnType != typeof(void))
	    {
		    _logger.Warn($"Invalid return type: {methodName}");
		    return Array.Empty<EventContainer>();
	    }

	    ParameterInfo[] parameters = method.GetParameters();
	    if (parameters.Length != 1)
	    {
		    _logger.Warn($"Invalid parameter count {parameters.Length} in {methodName}");
		    return Array.Empty<EventContainer>();
	    }

	    ParameterInfo parameter = parameters[0];
	    Type eventType = parameter.ParameterType;
	    if (!eventType.IsSubclassOf(typeof(EventBase)))
	    {
		    _logger.Warn($"Invalid parameter type {eventType} in {methodName}");
		    return Array.Empty<EventContainer>();
	    }

	    ImmutableArray<int> ids = ImmutableArray<int>.Empty;
	    ImmutableArray<Range<int>> idRanges = ImmutableArray<Range<int>>.Empty;
	    ImmutableArray<int> levels = ImmutableArray<int>.Empty;
	    ImmutableArray<Range<int>> levelRanges = ImmutableArray<Range<int>>.Empty;
	    switch (subscriptionType)
	    {
		    case SubscriptionType.Global:
		    case SubscriptionType.GlobalNpcs:
		    case SubscriptionType.GlobalMonsters:
		    case SubscriptionType.GlobalPlayers:
		    case SubscriptionType.Olympiad:
		    {
			    if (HasIds(method))
				    _logger.Warn($"Ids must not be defined for subscription type {subscriptionType} in {methodName}");

			    if (HasLevels(method))
				    _logger.Warn($"Npc levels must not be defined for subscription type {subscriptionType} in {methodName}");

			    break;
		    }

		    case SubscriptionType.InstanceTemplate:
		    case SubscriptionType.ZoneType:
		    case SubscriptionType.Fortress:
		    case SubscriptionType.Castle:
		    case SubscriptionType.ItemTemplate:
		    {
			    ids = GetIds(method);
			    idRanges = GetIdRanges(method);
			    if (ids.IsDefaultOrEmpty && idRanges.IsDefaultOrEmpty)
				    _logger.Warn($"Ids must be defined for subscription type {subscriptionType} in {methodName}");

			    if (HasLevels(method))
				    _logger.Warn($"Npc levels must not be defined for subscription type {subscriptionType} in {methodName}");

			    break;
		    }

		    case SubscriptionType.NpcTemplate:
		    {
			    ids = GetIds(method);
			    idRanges = GetIdRanges(method);
			    levels = GetLevels(method);
			    levelRanges = GetLevelRanges(method);
			    if (ids.IsDefaultOrEmpty && idRanges.IsDefaultOrEmpty && levels.IsDefaultOrEmpty &&
			        levelRanges.IsDefaultOrEmpty)
				    _logger.Warn($"Npc ids or levels must be defined for subscription type {subscriptionType} in {methodName}");

			    break;
		    }
	    }
	    
	    ImmutableList<EventContainer> eventContainers = GetEventContainers(subscriptionType, methodName, ids, idRanges, levels, levelRanges);
	    MethodInfo genericSubscribeMethod = _subscribeMethod.MakeGenericMethod(eventType);
	    foreach (EventContainer container in eventContainers)
		    genericSubscribeMethod.Invoke(null, [container, obj, method]);

	    return eventContainers;
    }

    private static void Subscribe(this EventContainer container, object obj, MethodInfo method)
    {
        Type eventType = method.GetParameters()[0].ParameterType;
        _subscribeMethod.MakeGenericMethod(eventType).Invoke(null, [container, obj, method]);
    }

    private static ImmutableArray<int> GetIds(MethodInfo method)
    {
	    return method.GetCustomAttributes<IdListAttribute>().Where(x => !x.Ids.IsDefaultOrEmpty)
		    .SelectMany(x => x.Ids).ToImmutableArray();
    }

    private static ImmutableArray<Range<int>> GetIdRanges(MethodInfo method)
    {
	    return method.GetCustomAttributes<IdRangeAttribute>().Select(x => new Range<int>(x.FromId, x.ToId))
		    .ToImmutableArray();
    }

    private static ImmutableArray<int> GetLevels(MethodInfo method)
    {
	    return method.GetCustomAttributes<LevelListAttribute>().Where(x => !x.Levels.IsDefaultOrEmpty)
		    .SelectMany(x => x.Levels)
		    .ToImmutableArray();
    }

    private static ImmutableArray<Range<int>> GetLevelRanges(MethodInfo method)
    {
	    return method.GetCustomAttributes<LevelRangeAttribute>().Select(x => new Range<int>(x.FromLevel, x.ToLevel))
		    .ToImmutableArray();
    }

    public static ImmutableList<EventContainer> GetEventContainers(SubscriptionType subscriptionType, string source,
	    IReadOnlyCollection<int>? ids = null, IReadOnlyCollection<Range<int>>? idRanges = null,
	    IReadOnlyCollection<int>? levels = null, IReadOnlyCollection<Range<int>>? levelRanges = null) =>
	    subscriptionType switch
	    {
		    SubscriptionType.Global => [GlobalEvents.Global],
		    SubscriptionType.GlobalNpcs => [GlobalEvents.Npcs],
		    SubscriptionType.GlobalMonsters => [GlobalEvents.Monsters],
		    SubscriptionType.GlobalPlayers => [GlobalEvents.Players],
		    SubscriptionType.Olympiad => [Olympiad.getInstance().Events],
		    SubscriptionType.Castle => GetEventContainers(ids, idRanges, source,
			    id => CastleManager.getInstance().getCastleById(id)),
		    SubscriptionType.Fortress => GetEventContainers(ids, idRanges, source,
			    id => FortManager.getInstance().getFortById(id)),
		    SubscriptionType.ZoneType => GetEventContainers(ids, idRanges, source,
			    id => ZoneManager.getInstance().getZoneById(id)),
		    SubscriptionType.InstanceTemplate => GetEventContainers(ids, idRanges, source,
			    id => InstanceManager.getInstance().getInstanceTemplate(id)),
		    SubscriptionType.ItemTemplate => GetEventContainers(ids, idRanges, source,
			    id => ItemData.getInstance().getTemplate(id)),
		    SubscriptionType.NpcTemplate => GetNpcEventContainers(ids, idRanges, levels, levelRanges, source),
		    _ => throw new ArgumentException($"Invalid subscription type {subscriptionType} in {source}")
	    };

    private static bool HasIds(MethodInfo method)
    {
	    return method.GetCustomAttributes<IdListAttribute>().Any(x => !x.Ids.IsDefaultOrEmpty) ||
	           method.GetCustomAttributes<IdRangeAttribute>().Any();
    }

    private static bool HasLevels(MethodInfo method)
    {
	    return method.GetCustomAttributes<LevelListAttribute>().Any(x => !x.Levels.IsDefaultOrEmpty) ||
	           method.GetCustomAttributes<LevelRangeAttribute>().Any();
    }

    private static (ImmutableList<EventContainer>.Builder, SortedSet<int>) CheckIdsAndGetEventContainers<T>(
	    IReadOnlyCollection<int>? ids, IReadOnlyCollection<Range<int>>? idRanges, string source, Func<int, T?> func)
	    where T: class, IEventContainerProvider
    {
	    SortedSet<int> idSet = new SortedSet<int>();
	    ImmutableList<EventContainer>.Builder builder = ImmutableList<EventContainer>.Empty.ToBuilder();

	    if (ids != null && ids.Count != 0)
	    {
		    foreach (int id in ids)
		    {
			    if (id <= 0)
				    _logger.Warn($"Invalid {typeof(T).Name} Id={id} in {source}");
			    else if (idSet.Add(id))
			    {
				    T? provider = func(id);
				    if (provider is null)
					    _logger.Warn($"Non-existing {typeof(T).Name} with Id={id} in {source}");
				    else
					    builder.Add(provider.Events);
			    }
			    else
				    _logger.Warn($"Duplicated {typeof(T).Name} Id={id} in {source}");
		    }
	    }

	    if (idRanges != null && idRanges.Count != 0)
	    {
		    foreach (Range<int> range in idRanges)
		    {
			    if (range.Left < range.Right)
			    {
				    _logger.Warn($"Invalid {typeof(T).Name} Id range [{range.Left}..{range.Right}] in {source}");
				    continue;
			    }

                if (range.Left <= 0)
	                _logger.Warn($"Invalid {typeof(T).Name} Id range [{range.Left}..{range.Right}] in {source}");
                
			    int min = Math.Max(1, range.Left);
			    int max = Math.Max(min, range.Right);
			    for (int id = min; id <= max; id++)
			    {
				    if (idSet.Add(id))
				    {
					    T? provider = func(id);
					    if (provider is null)
						    _logger.Warn($"Non-existing {typeof(T).Name} with Id={id} in {source}");
					    else
						    builder.Add(provider.Events);
				    }
				    else
					    _logger.Warn($"Duplicated {typeof(T).Name} Id={id} in {source}");
			    }
		    }
	    }

	    return (builder, idSet);
    }  
    
    private static ImmutableList<EventContainer> GetEventContainers<T>(IReadOnlyCollection<int>? ids,
	    IReadOnlyCollection<Range<int>>? idRanges, string source, Func<int, T?> func)
	    where T: class, IEventContainerProvider
    {
	    if ((ids == null || ids.Count == 0) && (idRanges == null || idRanges.Count == 0))
	    {
		    _logger.Warn($"No ids defined for {typeof(T).Name} in {source}");
		    return ImmutableList<EventContainer>.Empty;
	    }

	    var (builder, idSet) = CheckIdsAndGetEventContainers(ids, idRanges, source, func);
	    if (idSet.Count == 0)
	    {
		    _logger.Warn($"No {typeof(T).Name} found with provided ids in {source}");
		    return ImmutableList<EventContainer>.Empty;
	    }
	    
	    return builder.ToImmutable();
    }

    private static ImmutableList<EventContainer> GetNpcEventContainers(IReadOnlyCollection<int>? ids,
	    IReadOnlyCollection<Range<int>>? idRanges, IReadOnlyCollection<int>? levels,
	    IReadOnlyCollection<Range<int>>? levelRanges, string source)
    {
	    if ((ids == null || ids.Count == 0) && (idRanges == null || idRanges.Count == 0) &&
	        (levels == null || levels.Count == 0) && (levelRanges == null || levelRanges.Count == 0))
	    {
		    _logger.Warn($"No ids or levels defined for NpcTemplate subscription in {source}");
		    return ImmutableList<EventContainer>.Empty;
	    }

	    Func<int, NpcTemplate> func = static x => NpcData.getInstance().getTemplate(x);
	    var (builder, idSet) = CheckIdsAndGetEventContainers(ids, idRanges, source, func);

	    if (levels != null && levels.Count != 0)
	    {
		    foreach (int level in levels)
		    {
			    if (level < 1)
			    {
				    _logger.Warn($"Invalid Npc Level={level} in {source}");
				    continue;
			    }

			    List<NpcTemplate> npcByLevel = NpcData.getInstance().getAllOfLevel(level);
			    if (npcByLevel.Count == 0)
				    _logger.Warn($"No Npc with level={level} found in {source}");

			    foreach (NpcTemplate npcTemplate in npcByLevel)
			    {
				    if (idSet.Add(npcTemplate.getId()))
					    builder.Add(npcTemplate.Events);
				    else
					    _logger.Warn($"Duplicated Npc Id {npcTemplate.getId()} (level {level}) in {source}");
			    }
		    }
	    }

	    if (levelRanges != null && levelRanges.Count != 0)
	    {
		    foreach (Range<int> range in levelRanges)
		    {
			    if (range.Left < range.Right)
			    {
				    _logger.Warn($"Invalid Npc level range [{range.Left}..{range.Right}] in {source}");
				    continue;
			    }

			    if (range.Left <= 0)
				    _logger.Warn($"Invalid Npc level range [{range.Left}..{range.Right}] in {source}");

			    int min = Math.Max(1, range.Left);
			    int max = Math.Max(min, range.Right);
			    for (int level = min; level <= max; level++)
			    {
				    List<NpcTemplate> npcByLevel = NpcData.getInstance().getAllOfLevel(level);
				    foreach (NpcTemplate npcTemplate in npcByLevel)
				    {
					    if (idSet.Add(npcTemplate.getId()))
						    builder.Add(npcTemplate.Events);
					    else
						    _logger.Warn($"Duplicated Npc Id {npcTemplate.getId()} (level {level}) in {source}");
				    }
			    }
		    }
	    }

	    if (idSet.Count == 0)
	    {
		    _logger.Warn($"No Npc found with provided ids and levels in {source}");
		    return ImmutableList<EventContainer>.Empty;
	    }

	    return builder.ToImmutable();
    }

    private static void SubscribeMethod<T>(EventContainer container, object obj, MethodInfo method)
        where T: EventBase
    {
        Action<T> action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), obj, method);
        container.Subscribe(obj, action);
    } 
}