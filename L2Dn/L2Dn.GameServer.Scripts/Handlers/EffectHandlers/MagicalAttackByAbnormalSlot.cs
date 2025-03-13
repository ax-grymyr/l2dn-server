using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Magical Attack By Abnormal Slot effect implementation.
/// </summary>
public sealed class MagicalAttackByAbnormalSlot: AbstractEffect
{
    private readonly double _power;
    private readonly FrozenSet<AbnormalType> _abnormals;

    public MagicalAttackByAbnormalSlot(StatSet @params)
    {
        _power = @params.getDouble("power", 0);

        string abnormals = @params.getString("abnormalType", string.Empty);
        _abnormals = ParseUtil.ParseEnumSet<AbnormalType>(abnormals);
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !Formulas.calcSkillEvasion(effector, effected, skill);
    }

    public override EffectType getEffectType() => EffectType.MAGICAL_ATTACK;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isAlikeDead())
            return;

        bool hasAbnormalType = false;
        if (_abnormals.Count != 0)
        {
            foreach (AbnormalType abnormal in _abnormals)
            {
                if (effected.hasAbnormalType(abnormal))
                {
                    hasAbnormalType = true;
                    break;
                }
            }
        }

        if (!hasAbnormalType)
            return;

        Player? effectedPlayer = effected.getActingPlayer();
        if (effected.isPlayer() && effectedPlayer != null && effectedPlayer.isFakeDeath() &&
            Config.FAKE_DEATH_DAMAGE_STAND)
            effected.stopFakeDeath(true);

        bool sps = skill.useSpiritShot() && effector.isChargedShot(ShotType.SPIRITSHOTS);
        bool bss = skill.useSpiritShot() && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
        bool mcrit = Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);
        double damage = Formulas.calcMagicDam(effector, effected, skill, effector.getMAtk(), _power, effected.getMDef(),
            sps, bss, mcrit);

        effector.doAttack(damage, effected, skill, false, false, mcrit, false);
    }

    public override int GetHashCode() => HashCode.Combine(_power, _abnormals.GetSetHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._power, x._abnormals.GetSetComparable()));
}