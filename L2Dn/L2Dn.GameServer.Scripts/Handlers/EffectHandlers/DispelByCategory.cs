using L2Dn.GameServer.Enums;
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
/// Dispel By Category effect implementation.
/// </summary>
[HandlerStringKey("DispelByCategory")]
public sealed class DispelByCategory: AbstractEffect
{
    private readonly DispelSlotType _slot;
    private readonly int _rate;
    private readonly int _max;

    public DispelByCategory(EffectParameterSet parameters)
    {
        _slot = parameters.GetEnum(XmlSkillEffectParameterType.Slot, DispelSlotType.BUFF);
        _rate = parameters.GetInt32(XmlSkillEffectParameterType.Rate, 0);
        _max = parameters.GetInt32(XmlSkillEffectParameterType.Max, 0);
    }

    public override EffectTypes EffectTypes => EffectTypes.DISPEL;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return;

        List<BuffInfo> canceled = Formulas.calcCancelStealEffects(effector, effected, skill, _slot, _rate, _max);
        foreach (BuffInfo can in canceled)
            effected.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, can.getSkill());
    }

    public override int GetHashCode() => HashCode.Combine(_slot, _rate, _max);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._slot, x._rate, x._max));
}