using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using System.Xml.Linq;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Items;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Conditions;

[HandlerStringKey("Conditions")]
public sealed class ConditionFactory: IConditionFactory<XmlItemConditionList>
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ConditionFactory));

    public IConditionBase Create(XmlItemConditionList source)
    {
        List<Condition> conditions = [];
        if (source.Conditions is not null)
        {
            foreach (object xmlCondition in source.Conditions)
                conditions.Add(ParseCondition(xmlCondition));
        }

        Condition condition;
        if (conditions.Count == 0)
        {
            _logger.Error($"{nameof(ConditionFactory)}: No conditions defined");
            condition = new ConditionTrue();
        }
        else if (conditions.Count == 1)
            condition = conditions[0];
        else
        {
            ConditionLogicAnd conditionLogicAnd = new ConditionLogicAnd();
            foreach (Condition condition1 in conditions)
                conditionLogicAnd.add(condition1);

            condition = conditionLogicAnd;
        }

        if (source.MessageSpecified)
            condition.setMessage(source.Message);

        if (source.MessageIdSpecified)
            condition.setMessageId((SystemMessageId)source.MessageId);

        if (source.AddNameSpecified)
            condition.addName();

        return condition;
    }

    private static Condition ParseCondition(object xmlCondition) =>
        xmlCondition switch
        {
            XmlItemConditionGame xmlItemConditionGame => ParseGameCondition(xmlItemConditionGame),
            XmlItemConditionUsing xmlItemConditionUsing => ParseUsingCondition(xmlItemConditionUsing),
            XmlItemConditionPlayer xmlItemConditionPlayer => ParsePlayerCondition(xmlItemConditionPlayer),
            XmlItemConditionTarget xmlItemConditionTarget => ParseTargetCondition(xmlItemConditionTarget),
            XmlItemConditionAnd xmlItemConditionAnd => ParseAndCondition(xmlItemConditionAnd),
            XmlItemConditionOr xmlItemConditionOr => ParseOrCondition(xmlItemConditionOr),
            XmlItemConditionNot xmlItemConditionNot => ParseNotCondition(xmlItemConditionNot),
            _ => throw new InvalidOperationException(
                $"{nameof(ConditionFactory)}: Unknown condition type: {xmlCondition.GetType()}"),
        };

    private static Condition ParseAndCondition(XmlItemConditionAnd xmlItemConditionAnd)
    {
        ConditionLogicAnd condition = new ConditionLogicAnd();
        if (xmlItemConditionAnd.Conditions is not null)
        {
            foreach (object xmlCondition in xmlItemConditionAnd.Conditions)
                condition.add(ParseCondition(xmlCondition));
        }

        if (condition.conditions.Count == 0)
            throw new InvalidOperationException($"{nameof(ConditionFactory)}: Empty <and> condition");

        return condition;
    }

    private static Condition ParseOrCondition(XmlItemConditionOr xmlItemConditionOr)
    {
        ConditionLogicOr condition = new ConditionLogicOr();
        if (xmlItemConditionOr.Conditions is not null)
        {
            foreach (object xmlCondition in xmlItemConditionOr.Conditions)
                condition.add(ParseCondition(xmlCondition));
        }

        if (condition.conditions.Count == 0)
            throw new InvalidOperationException($"{nameof(ConditionFactory)}: Empty <or> condition");

        return condition;
    }

    private static Condition ParseNotCondition(XmlItemConditionNot xmlItemConditionNot)
    {
        if (xmlItemConditionNot.Condition is null)
            throw new InvalidOperationException($"{nameof(ConditionFactory)}: Empty <not> condition");

        return new ConditionLogicNot(ParseCondition(xmlItemConditionNot.Condition));
    }

    private static Condition ParsePlayerCondition(XmlItemConditionPlayer xmlItemConditionPlayer)
    {
        List<Condition> conditions = [];
        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.Races))
        {
            FrozenSet<Race> races = ParseUtil.ParseEnumSet<Race>(xmlItemConditionPlayer.Races, ',');
            conditions.Add(new ConditionPlayerRace(races));
        }

        if (xmlItemConditionPlayer.LevelSpecified)
            conditions.Add(new ConditionPlayerLevel(xmlItemConditionPlayer.Level));

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.LevelRange))
        {
            ImmutableArray<int> range = ParseUtil.ParseList<int>(xmlItemConditionPlayer.LevelRange);
            if (range.Length == 2)
            {
                int minLevel = range[0];
                int maxLevel = range[1];
                conditions.Add(new ConditionPlayerLevelRange(minLevel, maxLevel));
            }
            else
                throw new InvalidOperationException($"{nameof(ConditionFactory)}: Invalid <player> condition");
        }

        if (xmlItemConditionPlayer.RestingSpecified)
            conditions.Add(new ConditionPlayerState(PlayerState.RESTING, xmlItemConditionPlayer.Resting));

        if (xmlItemConditionPlayer.FlyingSpecified)
            conditions.Add(new ConditionPlayerState(PlayerState.FLYING, xmlItemConditionPlayer.Flying));

        if (xmlItemConditionPlayer.MovingSpecified)
            conditions.Add(new ConditionPlayerState(PlayerState.MOVING, xmlItemConditionPlayer.Moving));

        if (xmlItemConditionPlayer.RunningSpecified)
            conditions.Add(new ConditionPlayerState(PlayerState.RUNNING, xmlItemConditionPlayer.Running));

        if (xmlItemConditionPlayer.StandingSpecified)
            conditions.Add(new ConditionPlayerState(PlayerState.STANDING, xmlItemConditionPlayer.Standing));

        if (xmlItemConditionPlayer.BehindSpecified)
            conditions.Add(new ConditionPlayerState(PlayerState.BEHIND, xmlItemConditionPlayer.Behind));

        if (xmlItemConditionPlayer.FrontSpecified)
            conditions.Add(new ConditionPlayerState(PlayerState.FRONT, xmlItemConditionPlayer.Front));

        if (xmlItemConditionPlayer.ChaoticSpecified)
            conditions.Add(new ConditionPlayerState(PlayerState.CHAOTIC, xmlItemConditionPlayer.Chaotic));

        if (xmlItemConditionPlayer.OlympiadSpecified)
            conditions.Add(new ConditionPlayerState(PlayerState.OLYMPIAD, xmlItemConditionPlayer.Olympiad));

        if (xmlItemConditionPlayer.IsHeroSpecified)
            conditions.Add(new ConditionPlayerIsHero(xmlItemConditionPlayer.IsHero));

        if (xmlItemConditionPlayer.IsPvpFlaggedSpecified)
            conditions.Add(new ConditionPlayerIsPvpFlagged(xmlItemConditionPlayer.IsPvpFlagged));

        if (xmlItemConditionPlayer.TransformationIdSpecified)
            conditions.Add(new ConditionPlayerTransformationId(xmlItemConditionPlayer.TransformationId));

        if (xmlItemConditionPlayer.HpSpecified)
            conditions.Add(new ConditionPlayerHp(xmlItemConditionPlayer.Hp));

        if (xmlItemConditionPlayer.MpSpecified)
            conditions.Add(new ConditionPlayerMp(xmlItemConditionPlayer.Mp));

        if (xmlItemConditionPlayer.CpSpecified)
            conditions.Add(new ConditionPlayerCp(xmlItemConditionPlayer.Cp));

        if (xmlItemConditionPlayer.PkCountSpecified)
            conditions.Add(new ConditionPlayerPkCount(xmlItemConditionPlayer.PkCount));

        if (xmlItemConditionPlayer.SiegeZoneSpecified)
            conditions.Add(new ConditionSiegeZone(xmlItemConditionPlayer.SiegeZone, true));

        if (xmlItemConditionPlayer.SiegeSideSpecified)
            conditions.Add(new ConditionPlayerSiegeSide(xmlItemConditionPlayer.SiegeSide));

        if (xmlItemConditionPlayer.ChargesSpecified)
            conditions.Add(new ConditionPlayerCharges(xmlItemConditionPlayer.Charges));

        if (xmlItemConditionPlayer.SoulsSpecified)
        {
            // TODO: SoulType?
            conditions.Add(new ConditionPlayerSouls(xmlItemConditionPlayer.Souls, SoulType.LIGHT));
        }

        if (xmlItemConditionPlayer.WeightSpecified)
            conditions.Add(new ConditionPlayerWeight(xmlItemConditionPlayer.Weight));

        if (xmlItemConditionPlayer.InventorySizeSpecified)
            conditions.Add(new ConditionPlayerInvSize(xmlItemConditionPlayer.InventorySize));

        if (xmlItemConditionPlayer.IsClanLeaderSpecified)
            conditions.Add(new ConditionPlayerIsClanLeader(xmlItemConditionPlayer.IsClanLeader));

        if (xmlItemConditionPlayer.PledgeClassSpecified)
            conditions.Add(new ConditionPlayerPledgeClass((SocialClass)xmlItemConditionPlayer.PledgeClass));

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.ClanHall))
        {
            FrozenSet<int> clanHalls = ParseUtil.ParseSet<int>(xmlItemConditionPlayer.ClanHall, ',');
            conditions.Add(new ConditionPlayerHasClanHall(clanHalls));
        }

        if (xmlItemConditionPlayer.FortSpecified)
            conditions.Add(new ConditionPlayerHasFort(xmlItemConditionPlayer.Fort));

        if (xmlItemConditionPlayer.CastleSpecified)
            conditions.Add(new ConditionPlayerHasCastle(xmlItemConditionPlayer.Castle));

        if (xmlItemConditionPlayer.SexSpecified)
            conditions.Add(new ConditionPlayerSex(xmlItemConditionPlayer.Sex == 1 ? Sex.Female : Sex.Male));

        if (xmlItemConditionPlayer.FlyMountedSpecified)
            conditions.Add(new ConditionPlayerFlyMounted(xmlItemConditionPlayer.FlyMounted));

        if (xmlItemConditionPlayer.VehicleMountedSpecified)
            conditions.Add(new ConditionPlayerVehicleMounted(xmlItemConditionPlayer.VehicleMounted));

        if (xmlItemConditionPlayer.LandingZoneSpecified)
            conditions.Add(new ConditionPlayerLandingZone(xmlItemConditionPlayer.LandingZone));

        if (xmlItemConditionPlayer.ActiveEffectIdSpecified)
            conditions.Add(new ConditionPlayerActiveEffectId(xmlItemConditionPlayer.ActiveEffectId));

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.ActiveEffectIdLevel))
        {
            ImmutableArray<int> list = ParseUtil.ParseList<int>(xmlItemConditionPlayer.ActiveEffectIdLevel, ',');
            int effectId = list[0];
            int effectLvl = list[1];
            conditions.Add(new ConditionPlayerActiveEffectId(effectId, effectLvl));
        }

        if (xmlItemConditionPlayer.ActiveSkillIdSpecified)
            conditions.Add(new ConditionPlayerActiveSkillId(xmlItemConditionPlayer.ActiveSkillId));

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.ActiveSkillIdLevel))
        {
            ImmutableArray<int> list = ParseUtil.ParseList<int>(xmlItemConditionPlayer.ActiveSkillIdLevel, ',');
            int skillId = list[0];
            int skillLvl = list[1];
            conditions.Add(new ConditionPlayerActiveSkillId(skillId, skillLvl));
        }

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.ClassIdRestriction))
        {
            ImmutableArray<int> list = ParseUtil.ParseList<int>(xmlItemConditionPlayer.ClassIdRestriction, ',');
            conditions.Add(new ConditionPlayerClassIdRestriction(list.Select(x => (CharacterClass)x).ToFrozenSet()));
        }

        if (xmlItemConditionPlayer.SubclassSpecified)
            conditions.Add(new ConditionPlayerSubclass(xmlItemConditionPlayer.Subclass));

        if (xmlItemConditionPlayer.DualClassSpecified)
            conditions.Add(new ConditionPlayerDualclass(xmlItemConditionPlayer.DualClass));

        if (xmlItemConditionPlayer.CanSwitchSubclassSpecified)
            conditions.Add(new ConditionPlayerCanSwitchSubclass(xmlItemConditionPlayer.CanSwitchSubclass));

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.InstanceId))
        {
            FrozenSet<int> list = ParseUtil.ParseSet<int>(xmlItemConditionPlayer.InstanceId, ',');
            conditions.Add(new ConditionPlayerInstanceId(list));
        }

        if (xmlItemConditionPlayer.AgathionIdSpecified)
            conditions.Add(new ConditionPlayerAgathionId(xmlItemConditionPlayer.AgathionId));

        if (xmlItemConditionPlayer.CloakStatusSpecified)
            conditions.Add(new ConditionPlayerCloakStatus(xmlItemConditionPlayer.CloakStatus));

        if (xmlItemConditionPlayer.HasSummonSpecified)
            conditions.Add(new ConditionPlayerHasSummon(xmlItemConditionPlayer.HasSummon));

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.HasPet))
        {
            FrozenSet<int> list = ParseUtil.ParseSet<int>(xmlItemConditionPlayer.HasPet, ',');
            conditions.Add(new ConditionPlayerHasPet(list));
        }

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.ServitorNpcId))
        {
            FrozenSet<int> list = ParseUtil.ParseSet<int>(xmlItemConditionPlayer.ServitorNpcId, ',');
            conditions.Add(new ConditionPlayerServitorNpcId(list));
        }

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.NpcIdRadius))
        {
            string[] values = xmlItemConditionPlayer.NpcIdRadius.Split(',');
            if (values.Length == 3)
            {
                FrozenSet<int> npcIds = ParseUtil.ParseSet<int>(values[0]);
                int radius = int.Parse(values[1], CultureInfo.InvariantCulture);
                bool value = bool.Parse(values[2]);
                conditions.Add(new ConditionPlayerRangeFromNpc(npcIds, radius, value));
            }
        }

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.SummonedNpcIdRadius))
        {
            string[] values = xmlItemConditionPlayer.SummonedNpcIdRadius.Split(',');
            if (values.Length == 3)
            {
                FrozenSet<int> npcIds = ParseUtil.ParseSet<int>(values[0]);
                int radius = int.Parse(values[1], CultureInfo.InvariantCulture);
                bool value = bool.Parse(values[2]);
                conditions.Add(new ConditionPlayerRangeFromSummonedNpc(npcIds, radius, value));
            }
        }

        if (xmlItemConditionPlayer.CallPcSpecified)
            conditions.Add(new ConditionPlayerCallPc(xmlItemConditionPlayer.CallPc));

        if (xmlItemConditionPlayer.CanCreateBaseSpecified)
            conditions.Add(new ConditionPlayerCanCreateBase(xmlItemConditionPlayer.CanCreateBase));

        if (xmlItemConditionPlayer.CanEscapeSpecified)
            conditions.Add(new ConditionPlayerCanEscape(xmlItemConditionPlayer.CanEscape));

        if (xmlItemConditionPlayer.CanRefuelAirShipSpecified)
            conditions.Add(new ConditionPlayerCanRefuelAirship(xmlItemConditionPlayer.CanRefuelAirShip));

        if (xmlItemConditionPlayer.CanResurrectSpecified)
            conditions.Add(new ConditionPlayerCanResurrect(xmlItemConditionPlayer.CanResurrect));

        if (xmlItemConditionPlayer.CanSummonPetSpecified)
            conditions.Add(new ConditionPlayerCanSummonPet(xmlItemConditionPlayer.CanSummonPet));

        if (xmlItemConditionPlayer.CanSummonServitorSpecified)
            conditions.Add(new ConditionPlayerCanSummonServitor(xmlItemConditionPlayer.CanSummonServitor));

        if (xmlItemConditionPlayer.HasFreeSummonPointsSpecified)
            conditions.Add(new ConditionPlayerHasFreeSummonPoints(xmlItemConditionPlayer.HasFreeSummonPoints));

        if (xmlItemConditionPlayer.HasFreeTeleportBookmarkSlotsSpecified)
            conditions.Add(new ConditionPlayerHasFreeTeleportBookmarkSlots(xmlItemConditionPlayer.HasFreeTeleportBookmarkSlots));

        if (xmlItemConditionPlayer.CanSummonSiegeGolemSpecified)
            conditions.Add(new ConditionPlayerCanSummonSiegeGolem(xmlItemConditionPlayer.CanSummonSiegeGolem));

        if (xmlItemConditionPlayer.CanSweepSpecified)
            conditions.Add(new ConditionPlayerCanSweep(xmlItemConditionPlayer.CanSweep));

        if (xmlItemConditionPlayer.CanTakeCastleSpecified)
            conditions.Add(new ConditionPlayerCanTakeCastle(xmlItemConditionPlayer.CanTakeCastle));

        if (xmlItemConditionPlayer.CanTakeFortSpecified)
            conditions.Add(new ConditionPlayerCanTakeFort(xmlItemConditionPlayer.CanTakeFort));

        if (xmlItemConditionPlayer.CanTransformSpecified)
            conditions.Add(new ConditionPlayerCanTransform(xmlItemConditionPlayer.CanTransform));

        if (xmlItemConditionPlayer.CanUntransformSpecified)
            conditions.Add(new ConditionPlayerCanUntransform(xmlItemConditionPlayer.CanUntransform));

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.InsideZoneId))
        {
            FrozenSet<int> list = ParseUtil.ParseSet<int>(xmlItemConditionPlayer.InsideZoneId, ',');
            conditions.Add(new ConditionPlayerInsideZoneId(list));
        }

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.CheckAbnormal))
        {
            if (xmlItemConditionPlayer.CheckAbnormal.Contains(','))
            {
                string[] values = xmlItemConditionPlayer.CheckAbnormal.Split(',');
                AbnormalType type = Enum.Parse<AbnormalType>(values[0], true);
                int value = int.Parse(values[1], CultureInfo.InvariantCulture);
                conditions.Add(new ConditionPlayerCheckAbnormal(type, value));
            }
            else
            {
                AbnormalType type = Enum.Parse<AbnormalType>(xmlItemConditionPlayer.CheckAbnormal, true);
                conditions.Add(new ConditionPlayerCheckAbnormal(type));
            }
        }

        if (!string.IsNullOrEmpty(xmlItemConditionPlayer.CategoryType))
        {
            FrozenSet<CategoryType> list =
                ParseUtil.ParseEnumSet<CategoryType>(xmlItemConditionPlayer.CategoryType, ',');

            conditions.Add(new ConditionCategoryType(list));
        }

        if (xmlItemConditionPlayer.ImmobileSpecified)
            conditions.Add(new ConditionPlayerImmobile(xmlItemConditionPlayer.Immobile));

        if (xmlItemConditionPlayer.InCombatSpecified)
            conditions.Add(new ConditionPlayerIsInCombat(xmlItemConditionPlayer.InCombat));

        if (xmlItemConditionPlayer.CastleSideSpecified)
            conditions.Add(new ConditionPlayerIsOnSide(xmlItemConditionPlayer.CastleSide));

        if (xmlItemConditionPlayer.InInstanceSpecified)
            conditions.Add(new ConditionPlayerInInstance(xmlItemConditionPlayer.InInstance));

        if (xmlItemConditionPlayer.MinimumVitalityPointsSpecified)
            conditions.Add(new ConditionMinimumVitalityPoints(xmlItemConditionPlayer.MinimumVitalityPoints));

        return GetCondition(conditions, "player");
    }

    private static Condition ParseTargetCondition(XmlItemConditionTarget xmlItemConditionTarget)
    {
        List<Condition> conditions = [];
        if (!string.IsNullOrEmpty(xmlItemConditionTarget.CategoryType))
        {
            FrozenSet<CategoryType> list =
                ParseUtil.ParseEnumSet<CategoryType>(xmlItemConditionTarget.CategoryType, ',');

            conditions.Add(new ConditionTargetCategoryType(list));
        }

        if (!string.IsNullOrEmpty(xmlItemConditionTarget.LevelRange))
        {
            ImmutableArray<int> range = ParseUtil.ParseList<int>(xmlItemConditionTarget.LevelRange);
            if (range.Length == 2)
            {
                int minLevel = range[0];
                int maxLevel = range[1];
                conditions.Add(new ConditionTargetLevelRange(minLevel, maxLevel));
            }
            else
                throw new InvalidOperationException($"{nameof(ConditionFactory)}: Invalid <target> condition");
        }

        if (xmlItemConditionTarget.AggroSpecified)
            conditions.Add(new ConditionTargetAggro(xmlItemConditionTarget.Aggro));

        if (xmlItemConditionTarget.LevelSpecified)
            conditions.Add(new ConditionTargetLevel(xmlItemConditionTarget.Level));

        if (xmlItemConditionTarget.SiegeZoneSpecified)
            conditions.Add(new ConditionSiegeZone(xmlItemConditionTarget.SiegeZone, false));

        if (xmlItemConditionTarget.MyPartyExceptMeSpecified)
            conditions.Add(new ConditionTargetMyPartyExceptMe(xmlItemConditionTarget.MyPartyExceptMe));

        if (xmlItemConditionTarget.PlayableSpecified && xmlItemConditionTarget.Playable)
            conditions.Add(new ConditionTargetPlayable());

        if (xmlItemConditionTarget.PlayerSpecified && xmlItemConditionTarget.Player)
            conditions.Add(new ConditionTargetPlayer());

        if (!string.IsNullOrEmpty(xmlItemConditionTarget.ClassIdRestriction))
        {
            ImmutableArray<int> list = ParseUtil.ParseList<int>(xmlItemConditionTarget.ClassIdRestriction, ',');
            conditions.Add(new ConditionTargetClassIdRestriction(list.Select(x => (CharacterClass)x).ToFrozenSet()));
        }

        if (xmlItemConditionTarget.ActiveEffectIdSpecified)
            conditions.Add(new ConditionTargetActiveEffectId(xmlItemConditionTarget.ActiveEffectId));

        if (!string.IsNullOrEmpty(xmlItemConditionTarget.ActiveEffectIdLevel))
        {
            ImmutableArray<int> list = ParseUtil.ParseList<int>(xmlItemConditionTarget.ActiveEffectIdLevel, ',');
            int effectId = list[0];
            int effectLvl = list[1];
            conditions.Add(new ConditionTargetActiveEffectId(effectId, effectLvl));
        }

        if (xmlItemConditionTarget.ActiveSkillIdSpecified)
            conditions.Add(new ConditionTargetActiveSkillId(xmlItemConditionTarget.ActiveSkillId));

        if (!string.IsNullOrEmpty(xmlItemConditionTarget.ActiveSkillIdLevel))
        {
            ImmutableArray<int> list = ParseUtil.ParseList<int>(xmlItemConditionTarget.ActiveSkillIdLevel, ',');
            int skillId = list[0];
            int skillLvl = list[1];
            conditions.Add(new ConditionTargetActiveSkillId(skillId, skillLvl));
        }

        if (xmlItemConditionTarget.AbnormalTypeSpecified)
            conditions.Add(new ConditionTargetAbnormalType(xmlItemConditionTarget.AbnormalType));

        if (xmlItemConditionTarget.MinDistanceSpecified)
            conditions.Add(new ConditionMinDistance(xmlItemConditionTarget.MinDistance));

        if (xmlItemConditionTarget.RaceSpecified)
            conditions.Add(new ConditionTargetRace(xmlItemConditionTarget.Race));

        if (!string.IsNullOrEmpty(xmlItemConditionTarget.Using))
        {
            ItemTypeMask mask = ItemTypeMask.Zero;
            string[] values = xmlItemConditionTarget.Using.Split(',');
            foreach (string value in values)
            {
                if (Enum.TryParse(value, true, out WeaponType weaponType))
                    mask |= weaponType;
                else if (Enum.TryParse(value, true, out ArmorType armorType))
                    mask |= armorType;
                else
                    _logger.Error($"{nameof(ConditionFactory)}: Unknown item type name in <target> condition: " + value);
            }

            conditions.Add(new ConditionTargetUsesWeaponKind(mask));
        }

        if (!string.IsNullOrEmpty(xmlItemConditionTarget.NpcId))
        {
            FrozenSet<int> list = ParseUtil.ParseSet<int>(xmlItemConditionTarget.NpcId, ',');
            conditions.Add(new ConditionTargetNpcId(list));
        }

        if (!string.IsNullOrEmpty(xmlItemConditionTarget.NpcType))
        {
            FrozenSet<InstanceType> list = ParseUtil.ParseEnumSet<InstanceType>(xmlItemConditionTarget.NpcType, ',');
            conditions.Add(new ConditionTargetNpcType(list));
        }

        if (xmlItemConditionTarget.WeightSpecified)
            conditions.Add(new ConditionTargetWeight(xmlItemConditionTarget.Weight));

        if (xmlItemConditionTarget.InventorySizeSpecified)
            conditions.Add(new ConditionTargetInvSize(xmlItemConditionTarget.InventorySize));

        if (xmlItemConditionTarget.CheckCrtEffectSpecified)
            conditions.Add(new ConditionTargetCheckCrtEffect(xmlItemConditionTarget.CheckCrtEffect));

        return GetCondition(conditions, "target");
    }

    private static Condition ParseUsingCondition(XmlItemConditionUsing xmlItemConditionUsing)
    {
        List<Condition> conditions = [];
        if (!string.IsNullOrEmpty(xmlItemConditionUsing.Kind))
        {
            ItemTypeMask mask = ItemTypeMask.Zero;
            string[] values = xmlItemConditionUsing.Kind.Split(',');
            foreach (string value in values)
            {
                if (Enum.TryParse(value, true, out WeaponType weaponType))
                    mask |= weaponType;
                else if (Enum.TryParse(value, true, out ArmorType armorType))
                    mask |= armorType;
                else
                    _logger.Error($"{nameof(ConditionFactory)}: Unknown item type name in <using> condition: " + value);
            }

            conditions.Add(new ConditionUsingItemType(mask));
        }

        if (!string.IsNullOrEmpty(xmlItemConditionUsing.Slot))
        {
            long mask = 0;
            string[] values = xmlItemConditionUsing.Slot.Split(',');
            foreach (string value in values)
            {
                if (ItemData.SlotNameMap.TryGetValue(value, out long slot))
                    mask |= slot;
                else
                    _logger.Error($"{nameof(ConditionFactory)}: Unknown item slot name in <using> condition: " + value);
            }

            conditions.Add(new ConditionUsingSlotType(mask));
        }

        if (xmlItemConditionUsing.SkillSpecified)
            conditions.Add(new ConditionUsingSkill(xmlItemConditionUsing.Skill));

        if (xmlItemConditionUsing.WeaponChangeSpecified)
            conditions.Add(new ConditionChangeWeapon(xmlItemConditionUsing.WeaponChange));

        if (!string.IsNullOrEmpty(xmlItemConditionUsing.SlotItem))
        {
            ImmutableArray<int> list = ParseUtil.ParseList<int>(xmlItemConditionUsing.SlotItem);
            int id = list[0];
            int slot = list[1];
            int enchant = list.Length > 2 ? list[2] : 0;
            conditions.Add(new ConditionSlotItemId(slot, id, enchant));
        }

        return GetCondition(conditions, "using");
    }

    private static Condition ParseGameCondition(XmlItemConditionGame xmlItemConditionGame)
    {
        List<Condition> conditions = [];

        if (xmlItemConditionGame.SkillSpecified)
            conditions.Add(new ConditionWithSkill(xmlItemConditionGame.Skill));

        if (xmlItemConditionGame.NightSpecified)
            conditions.Add(new ConditionGameTime(xmlItemConditionGame.Night));

        if (xmlItemConditionGame.ChanceSpecified)
            conditions.Add(new ConditionGameChance(xmlItemConditionGame.Chance));

        return GetCondition(conditions, "game");
    }

    private static Condition GetCondition(List<Condition> conditions, string name)
    {
        if (conditions.Count == 0)
            throw new InvalidOperationException($"{nameof(ConditionFactory)}: Unrecognized <{name}> condition");

        if (conditions.Count == 1)
            return conditions[0];

        ConditionLogicAnd conditionLogicAnd = new ConditionLogicAnd();
        foreach (Condition condition in conditions)
            conditionLogicAnd.add(condition);

        return conditionLogicAnd;
    }
}