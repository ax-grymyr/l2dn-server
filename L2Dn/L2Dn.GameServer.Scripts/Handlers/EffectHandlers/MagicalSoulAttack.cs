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
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Soul Attack effect implementation.
/// </summary>
public sealed class MagicalSoulAttack: AbstractEffect
{
    private readonly double _power;

    public MagicalSoulAttack(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !Formulas.calcSkillEvasion(effector, effected, skill);
    }

    public override EffectTypes EffectTypes => EffectTypes.MAGICAL_ATTACK;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isAlikeDead())
            return;

        Player? player = effector.getActingPlayer();
        if (player == null)
            return;

        Player? effectedPlayer = effected.getActingPlayer();
        if (effected.isPlayer() && effectedPlayer != null && effectedPlayer.isFakeDeath() &&
            Config.Character.FAKE_DEATH_DAMAGE_STAND)
        {
            effected.stopFakeDeath(true);
        }

        int chargedLightSouls = Math.Min(skill.MaxLightSoulConsumeCount, player.getCharges());
        if (chargedLightSouls > 0 && !player.decreaseCharges(chargedLightSouls))
        {
            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);

            sm.Params.addSkillName(skill);
            effector.sendPacket(sm);
            return;
        }

        int chargedShadowSouls = Math.Min(skill.MaxShadowSoulConsumeCount, player.getCharges());
        if (chargedShadowSouls > 0 && !player.decreaseCharges(chargedShadowSouls))
        {
            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);

            sm.Params.addSkillName(skill);
            effector.sendPacket(sm);
            return;
        }

        int chargedSouls = chargedLightSouls + chargedShadowSouls;
        if (chargedSouls < 1)
        {
            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);

            sm.Params.addSkillName(skill);
            effector.sendPacket(sm);
            return;
        }

        bool sps = skill.UseSpiritShot && effector.isChargedShot(ShotType.SPIRITSHOTS);
        bool bss = skill.UseSpiritShot && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
        bool mcrit = Formulas.calcCrit(skill.MagicCriticalRate, effector, effected, skill);
        double mAtk = effector.getMAtk() * (chargedSouls > 0 ? 1.3 + chargedSouls * 0.05 : 1);
        double damage =
            Formulas.calcMagicDam(effector, effected, skill, mAtk, _power, effected.getMDef(), sps, bss, mcrit);

        effector.doAttack(damage, effected, skill, false, false, mcrit, false);
    }

    public override int GetHashCode() => HashCode.Combine(_power);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._power);
}