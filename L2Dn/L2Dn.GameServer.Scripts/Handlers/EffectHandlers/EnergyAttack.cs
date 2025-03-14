using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Energy Attack effect implementation.
/// </summary>
public sealed class EnergyAttack: AbstractEffect
{
    private readonly double _power;
    private readonly int _chargeConsume;
    private readonly int _criticalChance;
    private readonly bool _ignoreShieldDefence;
    private readonly bool _overHit;
    private readonly double _pDefMod;

    public EnergyAttack(StatSet @params)
    {
        _power = @params.getDouble("power", 0);
        _criticalChance = @params.getInt("criticalChance", 10);
        _ignoreShieldDefence = @params.getBoolean("ignoreShieldDefence", false);
        _overHit = @params.getBoolean("overHit", false);
        _chargeConsume = @params.getInt("chargeConsume", 0);
        _pDefMod = @params.getDouble("pDefMod", 1.0);
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        // TODO: Verify this on retail
        return !Formulas.calcSkillEvasion(effector, effected, skill);
    }

    public override EffectType getEffectType() => EffectType.PHYSICAL_ATTACK;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? attacker = effector.getActingPlayer();
        if (!effector.isPlayer() || attacker == null)
            return;

        int charge = Math.Min(_chargeConsume, attacker.getCharges());
        if (!attacker.decreaseCharges(charge))
        {
            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);

            sm.Params.addSkillName(skill);
            attacker.sendPacket(sm);
            return;
        }

        if (_overHit && effected.isAttackable())
            ((Attackable)effected).overhitEnabled(true);

        double defenceIgnoreRemoval = effected.getStat().getValue(Stat.DEFENCE_IGNORE_REMOVAL, 1);
        double defenceIgnoreRemovalAdd = effected.getStat().getValue(Stat.DEFENCE_IGNORE_REMOVAL_ADD, 0);
        double pDefMod = Math.Min(1, defenceIgnoreRemoval - 1 + _pDefMod);
        int pDef = effected.getPDef();
        double ignoredPDef = pDef - pDef * pDefMod;
        if (ignoredPDef > 0)
            ignoredPDef = Math.Max(0, ignoredPDef - defenceIgnoreRemovalAdd);

        double defence = effected.getPDef() - ignoredPDef;

        double shieldDefenceIgnoreRemoval = effected.getStat().getValue(Stat.SHIELD_DEFENCE_IGNORE_REMOVAL, 1);
        double shieldDefenceIgnoreRemovalAdd = effected.getStat().getValue(Stat.SHIELD_DEFENCE_IGNORE_REMOVAL_ADD, 0);
        if (!_ignoreShieldDefence || shieldDefenceIgnoreRemoval > 1 || shieldDefenceIgnoreRemovalAdd > 0)
        {
            byte shield = Formulas.calcShldUse(attacker, effected);
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
        bool critical = Formulas.calcCrit(_criticalChance, attacker, effected, skill);

        if (defence != -1)
        {
            // Trait, elements
            double weaponTraitMod = Formulas.calcWeaponTraitBonus(attacker, effected);
            double generalTraitMod = Formulas.calcGeneralTraitBonus(attacker, effected, skill.getTraitType(), true);
            double weaknessMod = Formulas.calcWeaknessBonus(attacker, effected, skill.getTraitType());
            double attributeMod = Formulas.calcAttributeBonus(attacker, effected, skill);
            double pvpPveMod = Formulas.calculatePvpPveBonus(attacker, effected, skill, true);

            // Skill specific mods.
            double energyChargesBoost = 1 + charge * 0.1; // 10% bonus damage for each charge used.
            double critMod = critical ? Formulas.calcCritDamage(attacker, effected, skill) : 1;
            double ssmod = 1;
            if (skill.useSoulShot())
            {
                if (attacker.isChargedShot(ShotType.SOULSHOTS))
                {
                    ssmod = 2 * attacker.getStat().getValue(Stat.SHOTS_BONUS) *
                        effected.getStat().getValue(Stat.SOULSHOT_RESISTANCE, 1); // 2.04 for dual weapon?
                }
                else if (attacker.isChargedShot(ShotType.BLESSED_SOULSHOTS))
                {
                    ssmod = 4 * attacker.getStat().getValue(Stat.SHOTS_BONUS) *
                        effected.getStat().getValue(Stat.SOULSHOT_RESISTANCE, 1);
                }
            }

            // ...................________Initial Damage_________...__Charges Additional Damage__...____________________________________
            // ATTACK CALCULATION ((77 * ((pAtk * lvlMod) + power) * (1 + (0.1 * chargesConsumed)) / pdef) * skillPower) + skillPowerAdd
            // ```````````````````^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^```^^^^^^^^^^^^^^^^^^^^^^^^^^^^^```^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            double baseMod = 77 * (attacker.getPAtk() * attacker.getLevelMod() + _power +
                effector.getStat().getValue(Stat.SKILL_POWER_ADD, 0)) / defence;

            damage = baseMod * ssmod * critMod * weaponTraitMod * generalTraitMod * weaknessMod * attributeMod *
                energyChargesBoost * pvpPveMod;
        }

        double balanceMod = 1;
        if (attacker.isPlayable())
        {
            balanceMod = effected.isPlayable()
                ? Config.ClassBalance.PVP_ENERGY_SKILL_DAMAGE_MULTIPLIERS[attacker.getActingPlayer().getClassId()]
                : Config.ClassBalance.PVE_ENERGY_SKILL_DAMAGE_MULTIPLIERS[attacker.getActingPlayer().getClassId()];
        }

        Player? effectedPlayer = effected.getActingPlayer();
        if (effected.isPlayable() && effectedPlayer != null)
        {
            // TODO: why defence not used
            defence *= attacker.isPlayable()
                ? Config.ClassBalance.PVP_ENERGY_SKILL_DEFENCE_MULTIPLIERS[effectedPlayer.getClassId()]
                : Config.ClassBalance.PVE_ENERGY_SKILL_DEFENCE_MULTIPLIERS[effectedPlayer.getClassId()];
        }

        damage = Math.Max(0, damage * effector.getStat().getValue(Stat.PHYSICAL_SKILL_POWER, 1)) * balanceMod;
        effector.doAttack(damage, effected, skill, false, false, critical, false);
    }

    public override int GetHashCode() =>
        HashCode.Combine(_power, _chargeConsume, _criticalChance, _ignoreShieldDefence, _overHit, _pDefMod);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._power, x._chargeConsume, x._criticalChance, x._ignoreShieldDefence, x._overHit,
                x._pDefMod));
}