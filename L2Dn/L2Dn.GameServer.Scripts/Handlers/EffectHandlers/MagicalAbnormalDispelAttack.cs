using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Magical Abnormal-depending dispel Attack effect implementation.
/// </summary>
[HandlerName("MagicalAbnormalDispelAttack")]
public sealed class MagicalAbnormalDispelAttack: AbstractEffect
{
    private readonly double _power;
    private readonly AbnormalType _abnormalType;

    public MagicalAbnormalDispelAttack(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        _abnormalType = parameters.GetEnum(XmlSkillEffectParameterType.AbnormalType, (AbnormalType)0);
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !Formulas.calcSkillEvasion(effector, effected, skill);
    }

    public override EffectTypes EffectTypes => EffectTypes.MAGICAL_ATTACK;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        // First dispells the effect, then does damage. Sometimes the damage is evaded, but debuff is still dispelled.
        if (effector.isAlikeDead() || _abnormalType == AbnormalType.NONE ||
            !effected.getEffectList().stopEffects(_abnormalType))
            return;

        Player? effectedPlayer = effected.getActingPlayer();
        if (effected.isPlayer() && effectedPlayer != null && effectedPlayer.isFakeDeath() &&
            Config.Character.FAKE_DEATH_DAMAGE_STAND)
        {
            effected.stopFakeDeath(true);
        }

        bool sps = skill.UseSpiritShot && effector.isChargedShot(ShotType.SPIRITSHOTS);
        bool bss = skill.UseSpiritShot && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
        bool mcrit = Formulas.calcCrit(skill.MagicCriticalRate, effector, effected, skill);
        double damage = Formulas.calcMagicDam(effector, effected, skill, effector.getMAtk(), _power, effected.getMDef(),
            sps, bss, mcrit);

        effector.doAttack(damage, effected, skill, false, false, mcrit, false);
    }

    public override int GetHashCode() => HashCode.Combine(_power, _abnormalType);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x._abnormalType));
}