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
/// Dispel By Slot effect implementation.
/// </summary>
[HandlerStringKey("DispelBySlotMyself")]
public sealed class DispelBySlotMyself: AbstractEffect
{
    private readonly FrozenSet<AbnormalType> _dispelAbnormals;

    public DispelBySlotMyself(EffectParameterSet parameters)
    {
        string dispel = parameters.GetString(XmlSkillEffectParameterType.Dispel);
        _dispelAbnormals = ParseUtil.ParseEnumSet<AbnormalType>(dispel);
    }

    public override EffectTypes EffectTypes => EffectTypes.DISPEL_BY_SLOT;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_dispelAbnormals.Count == 0)
            return;

        // The effectlist should already check if it has buff with this abnormal type or not.
        effected.getEffectList().stopEffects(info => !info.getSkill().IsIrreplacableBuff &&
            _dispelAbnormals.Contains(info.getSkill().AbnormalType), true, true);
    }

    public override int GetHashCode() => _dispelAbnormals.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._dispelAbnormals.GetSetComparable());
}