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
/// Attack Trait effect implementation.
/// </summary>
public sealed class AttackTrait: AbstractEffect
{
    private readonly FrozenDictionary<TraitType, float> _attackTraits;

    public AttackTrait(StatSet @params)
    {
        if (@params.isEmpty())
            throw new ArgumentException(nameof(AttackTrait) + " effect must have parameters!", nameof(@params));

        _attackTraits = @params.getSet().Select(pair => new KeyValuePair<TraitType, float>(
                Enum.Parse<TraitType>(pair.Key, true),
                float.Parse(pair.Value.ToString() ?? string.Empty, CultureInfo.InvariantCulture) / 100f)).
            ToFrozenDictionary();
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        foreach ((TraitType key, float value) in _attackTraits)
            effected.getStat().mergeAttackTrait(key, value);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        foreach ((TraitType key, float value) in _attackTraits)
            effected.getStat().removeAttackTrait(key, value);
    }

    public override int GetHashCode() => _attackTraits.GetDictionaryHashCode();

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => x._attackTraits.GetDictionaryComparable());
}