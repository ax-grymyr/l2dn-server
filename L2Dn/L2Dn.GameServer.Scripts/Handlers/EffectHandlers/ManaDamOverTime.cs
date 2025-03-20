using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Mana Damage Over Time effect implementation.
/// </summary>
public sealed class ManaDamOverTime: AbstractEffect
{
    private readonly double _power;

    public ManaDamOverTime(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks);
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return false;

        double manaDam = _power * TicksMultiplier;
        if (manaDam > effected.getCurrentMp() && skill.IsToggle)
        {
            effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
            return false;
        }

        effected.reduceCurrentMp(manaDam);
        return skill.IsToggle;
    }

    public override int GetHashCode() => HashCode.Combine(_power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x.Ticks));
}