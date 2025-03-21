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
/// HP change effect. It is mostly used for potions and static damage.
/// </summary>
[HandlerStringKey("Hp")]
public sealed class Hp: AbstractEffect
{
    private readonly int _amount;
    private readonly StatModifierType _mode;

    public Hp(EffectParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillEffectParameterType.Amount, 0);
        _mode = parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead() || effected.isDoor() || effected.isHpBlocked() || effected.isRaid())
            return;

        double basicAmount = _amount;
        if (item != null && (item.isPotion() || item.isElixir()))
        {
            basicAmount += effected.getStat().getValue(Stat.ADDITIONAL_POTION_HP, 0);

            // Classic Potion Mastery
            // TODO: Create an effect if more mastery skills are added.
            basicAmount *= 1 + effected.getAffectedSkillLevel((int)CommonSkill.POTION_MASTERY) / 100.0;
        }

        double amount = 0;
        switch (_mode)
        {
            case StatModifierType.DIFF:
            {
                amount = Math.Min(basicAmount, Math.Max(0, effected.getMaxRecoverableHp() - effected.getCurrentHp()));
                break;
            }
            case StatModifierType.PER:
            {
                amount = Math.Min(effected.getMaxHp() * basicAmount / 100,
                    Math.Max(0, effected.getMaxRecoverableHp() - effected.getCurrentHp()));

                break;
            }
        }

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

    public override int GetHashCode() => HashCode.Combine(_amount, _mode);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._mode));
}