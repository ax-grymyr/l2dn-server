using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Heal Percent effect implementation.
/// </summary>
public sealed class HealPercent: AbstractEffect
{
    private readonly int _power;

    public HealPercent(EffectParameterSet parameters)
    {
        _power = parameters.GetInt32(XmlSkillEffectParameterType.Power, 0);
    }

    public override EffectTypes EffectTypes => EffectTypes.HEAL;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead() || effected.isDoor() || effected.isHpBlocked())
            return;

        double power = _power;
        bool full = power == 100.0;

        double amount = full ? effected.getMaxHp() : effected.getMaxHp() * power / 100.0;
        if (item != null && (item.isPotion() || item.isElixir()))
        {
            amount += effected.getStat().getValue(Stat.ADDITIONAL_POTION_HP, 0);

            // Classic Potion Mastery
            // TODO: Create an effect if more mastery skills are added.
            amount *= 1 + effected.getAffectedSkillLevel((int)CommonSkill.POTION_MASTERY) / 100.0;
        }

        // Prevents overheal
        amount = Math.Min(amount, Math.Max(0, effected.getMaxRecoverableHp() - effected.getCurrentHp()));
        if (amount >= 0)
        {
            if (amount != 0)
            {
                double newHp = amount + effected.getCurrentHp();
                effected.setCurrentHp(newHp, false);
                effected.broadcastStatusUpdate(effector);
            }

            SystemMessagePacket sm;
            if (effector.ObjectId != effected.ObjectId)
            {
                sm = new SystemMessagePacket(SystemMessageId.S2_HP_HAS_BEEN_RECOVERED_BY_C1);
                sm.Params.addString(effector.getName());
            }
            else
            {
                sm = new SystemMessagePacket(SystemMessageId.YOU_VE_RECOVERED_S1_HP);
            }

            sm.Params.addInt((int)amount);
            effected.sendPacket(sm);
        }
        else
        {
            double damage = -amount;
            effected.reduceCurrentHp(damage, effector, skill, false, false, false, false);
            effector.sendDamageMessage(effected, skill, (int)damage, 0, false, false, false);
        }
    }

    public override int GetHashCode() => _power;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._power);
}