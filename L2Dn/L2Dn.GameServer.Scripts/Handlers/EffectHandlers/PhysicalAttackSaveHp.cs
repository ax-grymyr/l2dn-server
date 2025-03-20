using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Physical Attack effect implementation.
/// Note: Initial formula taken from PhysicalAttack.
/// </summary>
public sealed class PhysicalAttackSaveHp: AbstractEffect
{
    private readonly double _power;
    private readonly double _criticalChance;
    private readonly bool _ignoreShieldDefence;
    private readonly bool _overHit;
    private readonly double _saveHp;

    public PhysicalAttackSaveHp(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        _criticalChance = parameters.GetDouble(XmlSkillEffectParameterType.CriticalChance, 0);
        _ignoreShieldDefence = parameters.GetBoolean(XmlSkillEffectParameterType.IgnoreShieldDefence, false);
        _overHit = parameters.GetBoolean(XmlSkillEffectParameterType.OverHit, false);
        _saveHp = parameters.GetDouble(XmlSkillEffectParameterType.SaveHp, 0);
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !Formulas.calcSkillEvasion(effector, effected, skill);
    }

    public override EffectTypes EffectTypes => EffectTypes.PHYSICAL_ATTACK;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isAlikeDead())
            return;

        Player? effectedPlayer = effected.getActingPlayer();
        if (effected.isPlayer() && effectedPlayer != null && effectedPlayer.isFakeDeath() &&
            Config.Character.FAKE_DEATH_DAMAGE_STAND)
        {
            effected.stopFakeDeath(true);
        }

        if (_overHit && effected.isAttackable())
            ((Attackable)effected).overhitEnabled(true);

        double attack = effector.getPAtk();
        double defence = effected.getPDef();

        double shieldDefenceIgnoreRemoval = effected.getStat().getValue(Stat.SHIELD_DEFENCE_IGNORE_REMOVAL, 1);
        double shieldDefenceIgnoreRemovalAdd = effected.getStat().getValue(Stat.SHIELD_DEFENCE_IGNORE_REMOVAL_ADD, 0);
        if (!_ignoreShieldDefence || shieldDefenceIgnoreRemoval > 1 || shieldDefenceIgnoreRemovalAdd > 0)
        {
            byte shield = Formulas.calcShldUse(effector, effected);
            switch (shield)
            {
                case Formulas.SHIELD_DEFENSE_SUCCEED:
                {
                    int shieldDef = effected.getShldDef();
                    if (_ignoreShieldDefence)
                    {
                        double shieldDefMod = Math.Max(0, shieldDefenceIgnoreRemoval - 1);
                        double ignoredShieldDef = shieldDef - shieldDef * shieldDefMod;
                        if (ignoredShieldDef > 0)
                        {
                            ignoredShieldDef = Math.Max(0, ignoredShieldDef - shieldDefenceIgnoreRemovalAdd);
                        }

                        defence += shieldDef - ignoredShieldDef;
                    }
                    else
                    {
                        defence += effected.getShldDef();
                    }

                    break;
                }
                case Formulas.SHIELD_DEFENSE_PERFECT_BLOCK:
                {
                    defence = -1;
                    break;
                }
            }
        }

        double damage = 1;
        bool critical = Formulas.calcCrit(_criticalChance, effector, effected, skill);

        if (defence != -1)
        {
            // Trait, elements
            double weaponTraitMod = Formulas.calcWeaponTraitBonus(effector, effected);
            double generalTraitMod = Formulas.calcGeneralTraitBonus(effector, effected, skill.TraitType, true);
            double weaknessMod = Formulas.calcWeaknessBonus(effector, effected, skill.TraitType);
            double attributeMod = Formulas.calcAttributeBonus(effector, effected, skill);
            double pvpPveMod = Formulas.calculatePvpPveBonus(effector, effected, skill, true);
            double randomMod = effector.getRandomDamageMultiplier();

            // Skill specific mods.
            double weaponMod = effector.getAttackType().isRanged() ? 70 : 77;
            double power = _power + effector.getStat().getValue(Stat.SKILL_POWER_ADD, 0);
            double rangedBonus = effector.getAttackType().isRanged() ? attack + power : 0;
            double critMod = critical ? Formulas.calcCritDamage(effector, effected, skill) : 1;
            double ssmod = 1;
            if (skill.UseSoulShot)
            {
                if (effector.isChargedShot(ShotType.SOULSHOTS))
                {
                    ssmod = 2 * effector.getStat().getValue(Stat.SHOTS_BONUS) *
                        effected.getStat().getValue(Stat.SOULSHOT_RESISTANCE, 1); // 2.04 for dual weapon?
                }
                else if (effector.isChargedShot(ShotType.BLESSED_SOULSHOTS))
                {
                    ssmod = 4 * effector.getStat().getValue(Stat.SHOTS_BONUS) *
                        effected.getStat().getValue(Stat.SOULSHOT_RESISTANCE, 1);
                }
            }

            // ...................____________Melee Damage_____________......................................___________________Ranged Damage____________________
            // ATTACK CALCULATION 77 * ((pAtk * lvlMod) + power) / pdef            RANGED ATTACK CALCULATION 70 * ((pAtk * lvlMod) + power + patk + power) / pdef
            // ```````````````````^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^``````````````````````````````````````^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            double baseMod = weaponMod * (attack * effector.getLevelMod() + power + rangedBonus) / defence;
            damage = baseMod * ssmod * critMod * weaponTraitMod * generalTraitMod * weaknessMod * attributeMod *
                pvpPveMod * randomMod;

            damage *= effector.getStat().getValue(Stat.PHYSICAL_SKILL_POWER, 1);
        }

        double minHp = effected.getMaxHp() * _saveHp / 100;

        if (effected.getCurrentHp() - damage < minHp)
            damage = effected.getCurrentHp() - minHp;

        effector.doAttack(damage, effected, skill, false, false, critical, false);
    }

    public override int GetHashCode() =>
        HashCode.Combine(_power, _criticalChance, _ignoreShieldDefence, _overHit, _saveHp);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._power, x._criticalChance, x._ignoreShieldDefence, x._overHit, x._saveHp));
}