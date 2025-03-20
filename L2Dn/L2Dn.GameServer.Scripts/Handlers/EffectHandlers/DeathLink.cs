using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Death Link effect implementation.
/// </summary>
public sealed class DeathLink: AbstractEffect
{
    private readonly double _power;

    public DeathLink(StatSet @params)
    {
        _power = @params.getDouble("power", 0);
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !Formulas.calcSkillEvasion(effector, effected, skill);
    }

    public override EffectTypes EffectType => EffectTypes.DEATH_LINK;

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isAlikeDead())
            return;

        bool sps = skill.UseSpiritShot && effector.isChargedShot(ShotType.SPIRITSHOTS);
        bool bss = skill.UseSpiritShot && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);

        Player? player = effected.getActingPlayer();
        if (effected.isPlayer() && player != null && player.isFakeDeath() && Config.Character.FAKE_DEATH_DAMAGE_STAND)
            effected.stopFakeDeath(true);

        bool mcrit = Formulas.calcCrit(skill.MagicCriticalRate, effector, effected, skill);
        double damage = Formulas.calcMagicDam(effector, effected, skill, effector.getMAtk(),
            _power * (-(effector.getCurrentHp() * 2 / effector.getMaxHp()) + 2), effected.getMDef(), sps, bss, mcrit);

        effector.doAttack(damage, effected, skill, false, false, mcrit, false);
    }

    public override int GetHashCode() => HashCode.Combine(_power);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._power);
}