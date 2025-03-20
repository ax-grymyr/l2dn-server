using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Mana Heal Over Time effect implementation.
/// </summary>
public sealed class ManaHealOverTime: AbstractEffect
{
    private readonly double _power;

    public ManaHealOverTime(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks);
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return false;

        double mp = effected.getCurrentMp();
        double maxmp = effected.getMaxRecoverableMp();

        // Not needed to set the MP and send update packet if player is already at max MP
        if (_power > 0)
        {
            if (mp >= maxmp)
                return true;
        }
        else
        {
            if (mp - _power <= 0)
                return true;
        }

        double power = _power;
        if (item != null && (item.isPotion() || item.isElixir()))
            power += effected.getStat().getValue(Stat.ADDITIONAL_POTION_MP, 0) / Ticks;

        mp += power * TicksMultiplier;
        mp = _power > 0 ? Math.Min(mp, maxmp) : Math.Max(mp, 1);
        effected.setCurrentMp(mp, false);
        effected.broadcastStatusUpdate(effector);
        return skill.IsToggle;
    }

    public override int GetHashCode() => HashCode.Combine(_power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x.Ticks));
}