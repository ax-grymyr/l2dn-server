using System.Collections.Immutable;
using System.Globalization;
using L2Dn.Configuration;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class ClassBalance
    {
        // TODO: change all multiplier's types from dictionary to array for fast lookup
        public static ImmutableDictionary<CharacterClass, double> PVE_MAGICAL_SKILL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_MAGICAL_SKILL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_MAGICAL_SKILL_DEFENCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_MAGICAL_SKILL_DEFENCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_MAGICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_MAGICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_MAGICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_MAGICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_SKILL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_SKILL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_SKILL_DEFENCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_SKILL_DEFENCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_ATTACK_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_ATTACK_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_ATTACK_DEFENCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_ATTACK_DEFENCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_ATTACK_CRITICAL_CHANCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_ATTACK_CRITICAL_CHANCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_ATTACK_CRITICAL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_ATTACK_CRITICAL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_BLOW_SKILL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_BLOW_SKILL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_BLOW_SKILL_DEFENCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_BLOW_SKILL_DEFENCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_ENERGY_SKILL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_ENERGY_SKILL_DAMAGE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVE_ENERGY_SKILL_DEFENCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PVP_ENERGY_SKILL_DEFENCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> PLAYER_HEALING_SKILL_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> SKILL_MASTERY_CHANCE_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> EXP_AMOUNT_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        public static ImmutableDictionary<CharacterClass, double> SP_AMOUNT_MULTIPLIERS =
            ImmutableDictionary<CharacterClass, double>.Empty;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.ClassBalance);

            PVE_MAGICAL_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PveMagicalSkillDamageMultipliers");
            PVP_MAGICAL_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvpMagicalSkillDamageMultipliers");
            PVE_MAGICAL_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PveMagicalSkillDefenceMultipliers");
            PVP_MAGICAL_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvpMagicalSkillDefenceMultipliers");
            PVE_MAGICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
                GetMultipliers(parser, "PveMagicalSkillCriticalChanceMultipliers");

            PVP_MAGICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
                GetMultipliers(parser, "PvpMagicalSkillCriticalChanceMultipliers");

            PVE_MAGICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
                GetMultipliers(parser, "PveMagicalSkillCriticalDamageMultipliers");

            PVP_MAGICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
                GetMultipliers(parser, "PvpMagicalSkillCriticalDamageMultipliers");

            PVE_PHYSICAL_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvePhysicalSkillDamageMultipliers");
            PVP_PHYSICAL_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvpPhysicalSkillDamageMultipliers");
            PVE_PHYSICAL_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvePhysicalSkillDefenceMultipliers");
            PVP_PHYSICAL_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvpPhysicalSkillDefenceMultipliers");
            PVE_PHYSICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
                GetMultipliers(parser, "PvePhysicalSkillCriticalChanceMultipliers");

            PVP_PHYSICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
                GetMultipliers(parser, "PvpPhysicalSkillCriticalChanceMultipliers");

            PVE_PHYSICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
                GetMultipliers(parser, "PvePhysicalSkillCriticalDamageMultipliers");

            PVP_PHYSICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
                GetMultipliers(parser, "PvpPhysicalSkillCriticalDamageMultipliers");

            PVE_PHYSICAL_ATTACK_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvePhysicalAttackDamageMultipliers");
            PVP_PHYSICAL_ATTACK_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvpPhysicalAttackDamageMultipliers");
            PVE_PHYSICAL_ATTACK_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvePhysicalAttackDefenceMultipliers");
            PVP_PHYSICAL_ATTACK_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvpPhysicalAttackDefenceMultipliers");
            PVE_PHYSICAL_ATTACK_CRITICAL_CHANCE_MULTIPLIERS =
                GetMultipliers(parser, "PvePhysicalAttackCriticalChanceMultipliers");

            PVP_PHYSICAL_ATTACK_CRITICAL_CHANCE_MULTIPLIERS =
                GetMultipliers(parser, "PvpPhysicalAttackCriticalChanceMultipliers");

            PVE_PHYSICAL_ATTACK_CRITICAL_DAMAGE_MULTIPLIERS =
                GetMultipliers(parser, "PvePhysicalAttackCriticalDamageMultipliers");

            PVP_PHYSICAL_ATTACK_CRITICAL_DAMAGE_MULTIPLIERS =
                GetMultipliers(parser, "PvpPhysicalAttackCriticalDamageMultipliers");

            PVE_BLOW_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PveBlowSkillDamageMultipliers");
            PVP_BLOW_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvpBlowSkillDamageMultipliers");
            PVE_BLOW_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PveBlowSkillDefenceMultipliers");
            PVP_BLOW_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvpBlowSkillDefenceMultipliers");
            PVE_ENERGY_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PveEnergySkillDamageMultipliers");
            PVP_ENERGY_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvpEnergySkillDamageMultipliers");
            PVE_ENERGY_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PveEnergySkillDefenceMultipliers");
            PVP_ENERGY_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvpEnergySkillDefenceMultipliers");
            PLAYER_HEALING_SKILL_MULTIPLIERS = GetMultipliers(parser, "PlayerHealingSkillMultipliers");
            SKILL_MASTERY_CHANCE_MULTIPLIERS = GetMultipliers(parser, "SkillMasteryChanceMultipliers");
            EXP_AMOUNT_MULTIPLIERS = GetMultipliers(parser, "ExpAmountMultipliers");
            SP_AMOUNT_MULTIPLIERS = GetMultipliers(parser, "SpAmountMultipliers");
        }

        private static ImmutableDictionary<CharacterClass, double> GetMultipliers(ConfigurationParser parser,
            string key)
        {
            // Format:
            // ELVEN_FIGHTER*2;PALUS_KNIGHT*2.5;...

            var builder = ImmutableDictionary<CharacterClass, double>.Empty.ToBuilder();
            parser.GetList(key, ';', s =>
            {
                string[] item = s.Split('*');
                CharacterClass classId;
                bool ok = double.TryParse(item[1], CultureInfo.InvariantCulture, out double rate);
                if (int.TryParse(item[0], CultureInfo.InvariantCulture, out int classNum))
                    classId = (CharacterClass)classNum;
                else if (!Enum.TryParse(item[0], false, out classId))
                    ok = false;

                return ((classId, rate), ok);
            }, true).ForEach(tuple =>
            {
                try
                {
                    builder.Add(tuple.classId, tuple.rate);
                }
                catch (ArgumentException)
                {
                    _logger.Error(
                        $"Duplicated class '{tuple.classId}' in entry '{key}' in configuration file '{parser.FilePath}'");
                }

            });

            return builder.ToImmutable();
        }
    }
}