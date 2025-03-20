using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Physical Attack effect implementation.
/// Current formulas were tested to be the best matching retail, damage appears to be identical:
/// For melee skills: 70 * graciaSkillBonus1.10113 * (patk * lvlmod + power) * crit * ss * skillpowerbonus / pdef
/// For ranged skills: 70 * (patk * lvlmod + power + patk + power) * crit * ss * skillpower / pdef
/// </summary>
[AbstractEffectName("PhysicalAttack")]
public class PhysicalAttack: AbstractEffect
{
    private readonly double _power;
    private readonly double _pAtkMod;
    private readonly double _pDefMod;
    private readonly double _criticalChance;
    private readonly bool _ignoreShieldDefence;
    private readonly bool _overHit;
    private readonly FrozenSet<AbnormalType> _abnormals;
    private readonly double _abnormalDamageMod;
    private readonly double _abnormalPowerMod;
    private readonly double _raceModifier;
    private readonly int _chanceToRepeat;
    private readonly int _repeatCount;
    private readonly FrozenSet<Race> _races;

    public PhysicalAttack(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        _pAtkMod = parameters.GetDouble(XmlSkillEffectParameterType.PAtkMod, 1.0);
        _pDefMod = parameters.GetDouble(XmlSkillEffectParameterType.PDefMod, 1.0);
        _criticalChance = parameters.GetDouble(XmlSkillEffectParameterType.CriticalChance, 10);
        _ignoreShieldDefence = parameters.GetBoolean(XmlSkillEffectParameterType.IgnoreShieldDefence, false);
        _overHit = parameters.GetBoolean(XmlSkillEffectParameterType.OverHit, false);

        string abnormals = parameters.GetString(XmlSkillEffectParameterType.AbnormalType, string.Empty);
        _abnormals = ParseUtil.ParseEnumSet<AbnormalType>(abnormals);

        _abnormalDamageMod = parameters.GetDouble(XmlSkillEffectParameterType.DamageModifier, 1);
        _abnormalPowerMod = parameters.GetDouble(XmlSkillEffectParameterType.PowerModifier, 1);
        _raceModifier = parameters.GetDouble(XmlSkillEffectParameterType.RaceModifier, 1);

        string races = parameters.GetString(XmlSkillEffectParameterType.Races, string.Empty);
        _races = ParseUtil.ParseEnumSet<Race>(races);

        _repeatCount = parameters.GetInt32(XmlSkillEffectParameterType.RepeatCount, 1);
        _chanceToRepeat = parameters.GetInt32(XmlSkillEffectParameterType.ChanceToRepeat, 0);
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
            effected.stopFakeDeath(true);

        if (_overHit && effected.isAttackable())
            ((Attackable)effected).overhitEnabled(true);

        double attack = effector.getPAtk() * _pAtkMod;

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

        for (int i = 0; i < _repeatCount; i++)
        {
            if (i > 0 && _chanceToRepeat > 0 && Rnd.get(100) >= _chanceToRepeat)
                break;

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

                // Skill specific modifiers.
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

                double power = _power * (hasAbnormalType ? _abnormalPowerMod : 1) +
                    effector.getStat().getValue(Stat.SKILL_POWER_ADD, 0);

                double weaponMod = effector.getAttackType().isRanged() ? 70 : 77;
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
                // Nasseka rev. 10200: generalTraitMod == 0 ? 1 : generalTraitMod (no invulnerable traits).
                damage = baseMod * (hasAbnormalType ? _abnormalDamageMod : 1) * ssmod * critMod * weaponTraitMod *
                    (generalTraitMod == 0 ? 1 : generalTraitMod) * weaknessMod * attributeMod * pvpPveMod * randomMod;

                damage *= effector.getStat().getValue(Stat.PHYSICAL_SKILL_POWER, 1);

                // Apply race modifier.
                if (_races.Contains(effected.getRace()))
                    damage *= _raceModifier;
            }

            effector.doAttack(damage, effected, skill, false, false, critical, false);
        }
    }

    public override int GetHashCode() =>
        HashCode.Combine(
            HashCode.Combine(_power, _pAtkMod, _pDefMod, _criticalChance, _ignoreShieldDefence, _overHit,
                _abnormals.GetSetHashCode(), _abnormalDamageMod), _abnormalPowerMod, _raceModifier, _chanceToRepeat,
            _repeatCount, _races.GetSetHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._power, x._pAtkMod, x._pDefMod, x._criticalChance, x._ignoreShieldDefence,
            x._overHit, x._abnormals.GetSetComparable(), x._abnormalDamageMod, x._abnormalPowerMod, x._raceModifier,
            x._chanceToRepeat, x._repeatCount, x._races.GetSetComparable()));
}