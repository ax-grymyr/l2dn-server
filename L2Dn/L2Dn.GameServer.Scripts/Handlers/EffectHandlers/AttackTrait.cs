using System.Collections.Frozen;
using System.Xml.Serialization;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Attack Trait effect implementation.
/// </summary>
public sealed class AttackTrait: AbstractEffect
{
    private static readonly FrozenDictionary<XmlSkillEffectParameterType, TraitType> _map = EnumUtil.
        GetValues<XmlSkillEffectParameterType>().Select(type =>
        {
            XmlEnumAttribute? attribute = type.GetCustomAttribute<XmlSkillEffectParameterType, XmlEnumAttribute>();
            string name = attribute?.Name ?? type.ToString();
            return Enum.TryParse(name, true, out TraitType traitType) ? (type, traitType) : (type, TraitType.NONE);
        }).Where(t => t.Item2 != TraitType.NONE).ToFrozenDictionary(t => t.Item1, t => t.Item2);

    private readonly FrozenDictionary<TraitType, float> _attackTraits;

    public AttackTrait(EffectParameterSet parameters)
    {
        _attackTraits = parameters.Keys.Select(key =>
        {
            if (!_map.TryGetValue(key, out TraitType traitType))
                return (TraitType.NONE, 0);

            float value = parameters.GetFloat(key) / 100f;
            return (traitType, value);
        }).Where(t => t.Item1 != TraitType.NONE).ToFrozenDictionary(t => t.Item1, t => t.Item2);

        if (_attackTraits.IsEmpty())
            throw new ArgumentException(nameof(AttackTrait) + " effect must have parameters!", nameof(parameters));
    }

    public static FrozenDictionary<XmlSkillEffectParameterType, TraitType> ParameterTraitTypeMap => _map;

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