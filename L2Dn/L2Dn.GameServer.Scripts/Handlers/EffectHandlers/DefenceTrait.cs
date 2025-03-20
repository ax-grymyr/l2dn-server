using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Defence Trait effect implementation.
/// </summary>
[AbstractEffectName("DefenceTrait")]
public sealed class DefenceTrait: AbstractEffect
{
    private readonly FrozenDictionary<TraitType, float> _defenceTraits;

    public DefenceTrait(EffectParameterSet parameters)
    {
        FrozenDictionary<XmlSkillEffectParameterType, TraitType> map = AttackTrait.ParameterTraitTypeMap;
        _defenceTraits = parameters.Keys.Select(key =>
        {
            if (!map.TryGetValue(key, out TraitType traitType))
                return (TraitType.NONE, 0);

            float value = parameters.GetFloat(key) / 100f;
            return (traitType, value);
        }).Where(t => t.Item1 != TraitType.NONE).ToFrozenDictionary(t => t.Item1, t => t.Item2);

        if (_defenceTraits.IsEmpty())
            throw new ArgumentException(nameof(DefenceTrait) + ": must have parameters.");
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