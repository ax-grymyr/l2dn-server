using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Cp Heal Over Time effect implementation.
/// </summary>
public sealed class CpHealOverTime: AbstractEffect
{
    private readonly double _power;

    public CpHealOverTime(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks);
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return false;

        double cp = effected.getCurrentCp();
        double maxCp = effected.getMaxRecoverableCp();

        // Not needed to set the CP and send update packet if player is already at max CP
        if (_power > 0)
        {
            if (cp >= maxCp)
                return false;
        }
        else
        {
            if (cp - _power <= 0)
                return false;
        }

        double power = _power;
        if (item != null && (item.isPotion() || item.isElixir()))
            power += effected.getStat().getValue(Stat.ADDITIONAL_POTION_CP, 0) / Ticks;

        cp += power * TicksMultiplier;
        cp = _power > 0 ? Math.Min(cp, maxCp) : Math.Max(cp, 1);
        effected.setCurrentCp(cp, false);
        effected.broadcastStatusUpdate(effector);
        return true;
    }

    public override int GetHashCode() => HashCode.Combine(_power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x.Ticks));
}