using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Dispel By Slot Probability effect implementation.
/// </summary>
[AbstractEffectName("DispelBySlotProbability")]
public sealed class DispelBySlotProbability: AbstractEffect
{
    private readonly FrozenSet<AbnormalType> _dispelAbnormals;
    private readonly int _rate;

    public DispelBySlotProbability(EffectParameterSet parameters)
    {
        string dispelEffects = parameters.GetString(XmlSkillEffectParameterType.Dispel);
        _dispelAbnormals = ParseUtil.ParseEnumSet<AbnormalType>(dispelEffects);
        _rate = parameters.GetInt32(XmlSkillEffectParameterType.Rate, 100);
    }

    public override EffectTypes EffectTypes => EffectTypes.DISPEL;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == null)
            return;

        // The effectlist should already check if it has buff with this abnormal type or not.
        effected.getEffectList().stopEffects(info => !info.getSkill().IsIrreplacableBuff && Rnd.get(100) < _rate &&
            _dispelAbnormals.Contains(info.getSkill().AbnormalType), true, true);
    }

    public override int GetHashCode() => HashCode.Combine(_dispelAbnormals.GetSetHashCode(), _rate);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._dispelAbnormals.GetSetComparable(), x._rate));
}