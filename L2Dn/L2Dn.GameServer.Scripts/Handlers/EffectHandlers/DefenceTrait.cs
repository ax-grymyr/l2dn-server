using System.Collections.Frozen;
using System.Globalization;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Defence Trait effect implementation.
/// </summary>
public sealed class DefenceTrait: AbstractEffect
{
    private readonly FrozenDictionary<TraitType, float> _defenceTraits;

    public DefenceTrait(StatSet @params)
    {
        if (@params.isEmpty())
            throw new ArgumentException(nameof(DefenceTrait) + ": must have parameters.");

        _defenceTraits = @params.getSet().Select(p => new KeyValuePair<TraitType, float>(
            Enum.Parse<TraitType>(p.Key, true),
            float.Parse(p.Value.ToString() ?? string.Empty, CultureInfo.InvariantCulture) / 100)).ToFrozenDictionary();
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        foreach ((TraitType key, float value) in _defenceTraits)
        {
            if (value < 1.0f)
                effected.getStat().mergeDefenceTrait(key, value);
            else
                effected.getStat().mergeInvulnerableTrait(key);
        }
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        foreach ((TraitType key, float value) in _defenceTraits)
        {
            if (value < 1.0f)
                effected.getStat().removeDefenceTrait(key, value);
            else
                effected.getStat().removeInvulnerableTrait(key);
        }
    }

    public override int GetHashCode() => _defenceTraits.GetDictionaryHashCode();

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => x._defenceTraits.GetDictionaryComparable());
}