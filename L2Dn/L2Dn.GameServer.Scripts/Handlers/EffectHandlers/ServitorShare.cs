using System.Collections.Frozen;
using System.Globalization;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Servitor Share effect implementation.
/// </summary>
public sealed class ServitorShare: AbstractEffect
{
    private readonly FrozenDictionary<Stat, float> _sharedStats;

    public ServitorShare(StatSet @params)
    {
        _sharedStats = @params.getSet().Select(pair => new KeyValuePair<Stat, float>(Enum.Parse<Stat>(pair.Key, true),
                float.Parse(pair.Value.ToString() ?? string.Empty, CultureInfo.InvariantCulture) / 100)).
            ToFrozenDictionary();
    }

    public override bool canPump(Creature? effector, Creature effected, Skill? skill)
    {
        return effected.isSummon();
    }

    public override void pump(Creature effected, Skill skill)
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