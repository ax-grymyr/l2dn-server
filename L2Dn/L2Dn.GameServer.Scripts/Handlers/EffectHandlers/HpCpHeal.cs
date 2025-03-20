using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// HpCpHeal effect implementation.
/// </summary>
public sealed class HpCpHeal: AbstractEffect
{
    private readonly double _power;

    public HpCpHeal(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
    }

    public override EffectTypes EffectTypes => EffectTypes.HEAL;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead() || effected.isDoor() || effected.isHpBlocked())
            return;

        if (effected != effector && effected.isAffected(EffectFlags.FACEOFF))
            return;

        double amount = _power;
        double staticShotBonus = 0;
        double mAtkMul = 1;
        bool sps = skill.IsMagic && effector.isChargedShot(ShotType.SPIRITSHOTS);
        bool bss = skill.IsMagic && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
        double shotsBonus = effector.getStat().getValue(Stat.SHOTS_BONUS);

        Player? player = effector.getActingPlayer();
        if (((sps || bss) && effector.isPlayer() && player != null && player.isMageClass()) || effector.isSummon())
        {
            staticShotBonus = skill.MpConsume; // static bonus for spiritshots
            mAtkMul = bss ? 4 * shotsBonus : 2 * shotsBonus;
            staticShotBonus *= bss ? 2.4 : 1.0;
        }
        else if ((sps || bss) && effector.isNpc())
        {
            staticShotBonus = 2.4 * skill.MpConsume; // always blessed spiritshots
            mAtkMul = 4 * shotsBonus;
        }
        else
        {
            // no static bonus
            // grade dynamic bonus
            Item? weaponInst = effector.getActiveWeaponInstance();
            if (weaponInst != null)
            {
                mAtkMul = weaponInst.getTemplate().getCrystalType() == CrystalType.S84 ? 4 :
                    weaponInst.getTemplate().getCrystalType() == CrystalType.S80 ? 2 : 1;
            }

            // shot dynamic bonus
            mAtkMul = bss ? mAtkMul * 4 : mAtkMul + 1;
        }

        if (!skill.IsStatic)
        {
            amount += staticShotBonus + Math.Sqrt(mAtkMul * effector.getMAtk());
            amount *= effected.getStat().getValue(Stat.HEAL_EFFECT, 1);
            amount += effected.getStat().getValue(Stat.HEAL_EFFECT_ADD, 0);
            amount *= item == null && effector.isPlayable() && player != null
                ? Config.ClassBalance.PLAYER_HEALING_SKILL_MULTIPLIERS[player.getClassId()]
                : 1f;

            // Heal critic, since CT2.3 Gracia Final
            if (skill.IsMagic && (Formulas.calcCrit(skill.MagicCriticalRate, effector, effected, skill) ||
                    effector.isAffected(EffectFlags.HPCPHEAL_CRITICAL)))
            {
                amount *= 3;
                effector.sendPacket(SystemMessageId.M_CRITICAL);
                effector.sendPacket(new ExMagicAttackInfoPacket(effector.ObjectId, effected.ObjectId,
                    ExMagicAttackInfoPacket.CRITICAL_HEAL));

                if (effected.isPlayer() && effected != effector)
                {
                    effected.sendPacket(new ExMagicAttackInfoPacket(effector.ObjectId, effected.ObjectId,
                        ExMagicAttackInfoPacket.CRITICAL_HEAL));
                }
            }
        }

        // Additional potion HP.
        double additionalHp = 0;

        // Additional potion CP.
        double additionalCp = 0;

        if (item != null && (item.isPotion() || item.isElixir()))
        {
            additionalHp = effected.getStat().getValue(Stat.ADDITIONAL_POTION_HP, 0);
            additionalCp = effected.getStat().getValue(Stat.ADDITIONAL_POTION_CP, 0);

            // Classic Potion Mastery
            // TODO: Create an effect if more mastery skills are added.
            amount *= 1 + effected.getAffectedSkillLevel((int)CommonSkill.POTION_MASTERY) / 100.0;
        }

        // Prevents overheal and negative amount
        double healAmount = Math.Max(Math.Min(amount, effected.getMaxRecoverableHp() - effected.getCurrentHp()), 0);
        if (healAmount != 0)
        {
            double newHp = healAmount + effected.getCurrentHp();
            if (newHp + additionalHp > effected.getMaxRecoverableHp())
            {
                additionalHp = Math.Max(effected.getMaxRecoverableHp() - newHp, 0);
            }

            effected.setCurrentHp(newHp + additionalHp, false);

            if (effector.isPlayer() && effector != effected)
            {
                SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S2_HP_HAS_BEEN_RECOVERED_BY_C1);
                sm.Params.addString(effector.getName());
                sm.Params.addInt((int)(healAmount + additionalHp));
                effected.sendPacket(sm);
            }
            else
            {
                SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_RECOVERED_S1_HP);
                sm.Params.addInt((int)(healAmount + additionalHp));
                effected.sendPacket(sm);
            }
        }

        // CP recovery.
        if (effected.isPlayer())
        {
            amount = Math.Max(Math.Min(amount - healAmount, effected.getMaxRecoverableCp() - effected.getCurrentCp()),
                0);

            if (amount != 0)
            {
                double newCp = amount + effected.getCurrentCp();
                if (newCp + additionalCp > effected.getMaxRecoverableCp())
                {
                    additionalCp = Math.Max(effected.getMaxRecoverableCp() - newCp, 0);
                }

                effected.setCurrentCp(newCp + additionalCp, false);

                if (effector != effected)
                {
                    SystemMessagePacket sm =
                        new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOVERED_S2_CP_WITH_C1_S_HELP);

                    sm.Params.addString(effector.getName());
                    sm.Params.addInt((int)(amount + additionalCp));
                    effected.sendPacket(sm);
                }
                else
                {
                    SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOVERED_S1_CP);
                    sm.Params.addInt((int)(amount + additionalCp));
                    effected.sendPacket(sm);
                }
            }
        }

        effected.broadcastStatusUpdate(effector);
    }

    public override int GetHashCode() => HashCode.Combine(_power);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._power);
}