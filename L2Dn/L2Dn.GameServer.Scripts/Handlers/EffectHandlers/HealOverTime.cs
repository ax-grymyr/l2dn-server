using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Heal Over Time effect implementation.
/// </summary>
public sealed class HealOverTime: AbstractEffect
{
    private readonly double _power;

    public HealOverTime(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks);
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead() || effected.isDoor())
            return false;

        double hp = effected.getCurrentHp();
        double maxhp = effected.getMaxRecoverableHp();

        // Not needed to set the HP and send update packet if player is already at max HP
        if (_power > 0)
        {
            if (hp >= maxhp)
                return false;
        }
        else
        {
            if (hp - _power <= 0)
                return false;
        }

        double power = _power;
        if (item != null && (item.isPotion() || item.isElixir()))
        {
            power += effected.getStat().getValue(Stat.ADDITIONAL_POTION_HP, 0) / Ticks;

            // Classic Potion Mastery
            // TODO: Create an effect if more mastery skills are added.
            power *= 1.0 + effected.getAffectedSkillLevel((int)CommonSkill.POTION_MASTERY) / 100.0;
        }

        hp += power * TicksMultiplier;
        hp = _power > 0 ? Math.Min(hp, maxhp) : Math.Max(hp, 1);
        effected.setCurrentHp(hp, false);
        effected.broadcastStatusUpdate(effector);
        return skill.IsToggle;
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isPlayer() && Ticks > 0 && skill.AbnormalType == AbnormalType.HP_RECOVER)
        {
            double power = _power;
            if (item != null && (item.isPotion() || item.isElixir()))
            {
                double bonus = effected.getStat().getValue(Stat.ADDITIONAL_POTION_HP, 0);
                if (bonus > 0)
                {
                    power += bonus / Ticks;
                }
            }

            effected.sendPacket(new ExRegenMaxPacket((int)(skill.AbnormalTime ?? TimeSpan.Zero).TotalSeconds,
                Ticks, power));
        }
    }

    public override int GetHashCode() => HashCode.Combine(_power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x.Ticks));
}