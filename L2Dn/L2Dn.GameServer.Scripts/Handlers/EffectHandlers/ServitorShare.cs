using System.Collections.Frozen;
using System.Xml.Serialization;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Servitor Share effect implementation.
/// </summary>
[HandlerName("ServitorShare")]
public sealed class ServitorShare: AbstractEffect
{
    private static readonly FrozenDictionary<XmlSkillEffectParameterType, Stat> _map = EnumUtil.
        GetValues<XmlSkillEffectParameterType>().Select(type =>
        {
            XmlEnumAttribute? attribute = type.GetCustomAttribute<XmlSkillEffectParameterType, XmlEnumAttribute>();
            string name = attribute?.Name ?? type.ToString();
            return Enum.TryParse(name, true, out Stat stat) ? (type, (Stat?)stat) : (type, null);
        }).Where(t => t.Item2 != null).ToFrozenDictionary(t => t.Item1, t => t.Item2!.Value);

    private readonly FrozenDictionary<Stat, float> _sharedStats;

    public ServitorShare(EffectParameterSet parameters)
    {
        _sharedStats = parameters.Keys.Select(key =>
        {
            if (!_map.TryGetValue(key, out Stat stat))
                return ((Stat?)null, 0f);

            float value = parameters.GetFloat(key) / 100f;
            return (stat, value);
        }).Where(t => t.Item1 != null).ToFrozenDictionary(t => t.Item1!.Value, t => t.Item2);
    }

    public override bool CanPump(Creature? effector, Creature effected, Skill? skill)
    {
        return effected.isSummon();
    }

    public override void Pump(Creature effected, Skill skill)
    {
        Player? owner = effected.getActingPlayer();
        if (owner != null)
        {
            foreach ((Stat key, float value) in _sharedStats)
                effected.getStat().mergeAdd(key, owner.getStat().getValue(key) * value);
        }
    }

    public override int GetHashCode() => _sharedStats.GetDictionaryHashCode();

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => x._sharedStats.GetDictionaryComparable());
}