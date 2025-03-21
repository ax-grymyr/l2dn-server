using System.Reflection;
using FluentAssertions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Scripts.Handlers.EffectHandlers;
using L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

namespace L2Dn.GameServer.Scripts.Tests;

public sealed class SkillConditionFactoryTests
{
[Fact]
    public void TestAttributes()
    {
        Dictionary<string, Type> oldList = GetOldList();
        Dictionary<string, Type> newList = GetNewList();
        List<string> missingKeys = oldList.Keys.Where(k => !newList.ContainsKey(k)).ToList();
        if (missingKeys.Count != 0)
            Assert.Fail($"Missing keys: {string.Join(", ", missingKeys)}");

        oldList.Count.Should().BeLessOrEqualTo(newList.Count);
        foreach (KeyValuePair<string, Type> pair in oldList)
        {
            newList.ContainsKey(pair.Key).Should().BeTrue();
            newList[pair.Key].Should().Be(pair.Value);
        }
    }

    private static Dictionary<string, Type> GetNewList()
    {
        Dictionary<string, Type> dict = typeof(Scripts).Assembly.GetTypes().
            Where(t => t.GetInterfaces().Contains(typeof(ISkillConditionBase))).
            Select(t => (t, t.GetCustomAttribute<HandlerKeyAttribute<string>>()?.Key ?? string.Empty)).
            Where(t => !string.IsNullOrEmpty(t.Item2)).ToDictionary(t => t.Item2, t => t.Item1);

        return dict;
    }

    private static Dictionary<string, Type> GetOldList()
    {
        Dictionary<string, Type> old = new(StringComparer.OrdinalIgnoreCase);
        old.Add("AssassinationPoints", typeof(AssassinationPointsSkillCondition));
        old.Add("BeastPoints", typeof(BeastPointsSkillCondition));
        old.Add("BuildAdvanceBase", typeof(BuildAdvanceBaseSkillCondition));
        old.Add("BuildCamp", typeof(BuildCampSkillCondition));
        old.Add("CanAddMaxEntranceInzone", typeof(CanAddMaxEntranceInzoneSkillCondition));
        old.Add("CanBookmarkAddSlot", typeof(CanBookmarkAddSlotSkillCondition));
        old.Add("CanChangeVitalItemCount", typeof(CanChangeVitalItemCountSkillCondition));
        old.Add("CanEnchantAttribute", typeof(CanEnchantAttributeSkillCondition));
        old.Add("CanMountForEvent", typeof(CanMountForEventSkillCondition));
        old.Add("CannotUseInTransform", typeof(CannotUseInTransformSkillCondition));
        old.Add("CanRefuelAirship", typeof(CanRefuelAirshipSkillCondition));
        old.Add("CanSummon", typeof(CanSummonSkillCondition));
        old.Add("CanSummonCubic", typeof(CanSummonCubicSkillCondition));
        old.Add("CanSummonMulti", typeof(CanSummonMultiSkillCondition));
        old.Add("CanSummonPet", typeof(CanSummonPetSkillCondition));
        old.Add("CanSummonSiegeGolem", typeof(CanSummonSiegeGolemSkillCondition));
        old.Add("CanTakeFort", typeof(CanTakeFortSkillCondition));
        old.Add("CanTransform", typeof(CanTransformSkillCondition));
        old.Add("CanTransformInDominion", typeof(CanTransformInDominionSkillCondition));
        old.Add("CanUntransform", typeof(CanUntransformSkillCondition));
        old.Add("CanUseInBattlefield", typeof(CanUseInBattlefieldSkillCondition));
        old.Add("CanUseInDragonLair", typeof(CanUseInDragonLairSkillCondition));
        old.Add("CanUseSwoopCannon", typeof(CanUseSwoopCannonSkillCondition));
        old.Add("HasVitalityPoints", typeof(HasVitalityPointsSkillCondition));
        old.Add("CanUseVitalityIncreaseItem", typeof(CanUseVitalityIncreaseItemSkillCondition));
        old.Add("CheckLevel", typeof(CheckLevelSkillCondition));
        old.Add("CheckSex", typeof(CheckSexSkillCondition));
        old.Add("ConsumeBody", typeof(ConsumeBodySkillCondition));
        old.Add("DeathPoints", typeof(DeathPointsSkillCondition));
        old.Add("EnergySaved", typeof(EnergySavedSkillCondition));
        old.Add("EquipArmor", typeof(EquipArmorSkillCondition));
        old.Add("EquippedCloakEnchant", typeof(EquippedCloakEnchantSkillCondition));
        old.Add("EquipShield", typeof(EquipShieldSkillCondition));
        old.Add("EquipSigil", typeof(EquipSigilSkillCondition));
        old.Add("EquipWeapon", typeof(EquipWeaponSkillCondition));
        old.Add("MaxMp", typeof(MaxMpSkillCondition));
        old.Add("NotFeared", typeof(NotFearedSkillCondition));
        old.Add("NotInUnderwater", typeof(NotInUnderwaterSkillCondition));
        old.Add("Op2hWeapon", typeof(Op2hWeaponSkillCondition));
        old.Add("OpAffectedBySkill", typeof(OpAffectedBySkillSkillCondition));
        old.Add("OpAgathionEnergy", typeof(OpAgathionEnergySkillCondition));
        old.Add("OpAlignment", typeof(OpAlignmentSkillCondition));
        old.Add("OpBaseStat", typeof(OpBaseStatSkillCondition));
        old.Add("OpBlink", typeof(OpBlinkSkillCondition));
        old.Add("OpCallPc", typeof(OpCallPcSkillCondition));
        old.Add("OpCanEscape", typeof(OpCanEscapeSkillCondition));
        old.Add("OpCanNotUseAirship", typeof(OpCanNotUseAirshipSkillCondition));
        old.Add("OpCannotUseTargetWithPrivateStore", typeof(OpCannotUseTargetWithPrivateStoreSkillCondition));
        old.Add("OpChangeWeapon", typeof(OpChangeWeaponSkillCondition));
        old.Add("OpCheckAbnormal", typeof(OpCheckAbnormalSkillCondition));
        old.Add("OpCheckAccountType", typeof(OpCheckAccountTypeSkillCondition));
        old.Add("OpCheckCastRange", typeof(OpCheckCastRangeSkillCondition));
        old.Add("OpCheckClass", typeof(OpCheckClassSkillCondition));
        old.Add("OpCheckClassList", typeof(OpCheckClassListSkillCondition));
        old.Add("OpCheckCrtEffect", typeof(OpCheckCrtEffectSkillCondition));
        old.Add("OpCheckFlag", typeof(OpCheckFlagSkillCondition));
        old.Add("OpCheckOnGoingEventCampaign", typeof(OpCheckOnGoingEventCampaignSkillCondition));
        old.Add("OpCheckPcbangPoint", typeof(OpCheckPcbangPointSkillCondition));
        old.Add("OpCheckResidence", typeof(OpCheckResidenceSkillCondition));
        old.Add("OpCheckSkill", typeof(OpCheckSkillSkillCondition));
        old.Add("OpCheckSkillList", typeof(OpCheckSkillListSkillCondition));
        old.Add("OpCompanion", typeof(OpCompanionSkillCondition));
        old.Add("OpEnchantRange", typeof(OpEnchantRangeSkillCondition));
        old.Add("OpEncumbered", typeof(OpEncumberedSkillCondition));
        old.Add("OpEnergyMax", typeof(OpEnergyMaxSkillCondition));
        old.Add("OpEquipItem", typeof(OpEquipItemSkillCondition));
        old.Add("OpExistNpc", typeof(OpExistNpcSkillCondition));
        old.Add("OpFishingCast", typeof(OpFishingCastSkillCondition));
        old.Add("OpFishingPumping", typeof(OpFishingPumpingSkillCondition));
        old.Add("OpFishingReeling", typeof(OpFishingReelingSkillCondition));
        old.Add("OpHaveSummon", typeof(OpHaveSummonSkillCondition));
        old.Add("OpHaveSummonedNpc", typeof(OpHaveSummonedNpcSkillCondition));
        old.Add("OpHome", typeof(OpHomeSkillCondition));
        old.Add("OpInSiege", typeof(OpInSiegeSkillCondition));
        old.Add("OpInstantzone", typeof(OpInstantzoneSkillCondition));
        old.Add("OpMainjob", typeof(OpMainjobSkillCondition));
        old.Add("OpNeedAgathion", typeof(OpNeedAgathionSkillCondition));
        old.Add("OpNeedSummonOrPet", typeof(OpNeedSummonOrPetSkillCondition));
        old.Add("OpNotAffectedBySkill", typeof(OpNotAffectedBySkillSkillCondition));
        old.Add("OpNotCursed", typeof(OpNotCursedSkillCondition));
        old.Add("OpNotInPeacezone", typeof(OpNotInPeacezoneSkillCondition));
        old.Add("OpNotInstantzone", typeof(OpNotInstantzoneSkillCondition));
        old.Add("OpNotOlympiad", typeof(OpNotOlympiadSkillCondition));
        old.Add("OpNotTerritory", typeof(OpNotTerritorySkillCondition));
        old.Add("OpOlympiad", typeof(OpOlympiadSkillCondition));
        old.Add("OpPeacezone", typeof(OpPeacezoneSkillCondition));
        old.Add("OpPkcount", typeof(OpPkcountSkillCondition));
        old.Add("OpPledge", typeof(OpPledgeSkillCondition));
        old.Add("OpRestartPoint", typeof(OpRestartPointSkillCondition));
        old.Add("OpResurrection", typeof(OpResurrectionSkillCondition));
        old.Add("OpSiegeHammer", typeof(OpSiegeHammerSkillCondition));
        old.Add("OpSkill", typeof(OpSkillSkillCondition));
        old.Add("OpSkillAcquire", typeof(OpSkillAcquireSkillCondition));
        old.Add("OpSocialClass", typeof(OpSocialClassSkillCondition));
        old.Add("OpSoulMax", typeof(OpSoulMaxSkillCondition));
        old.Add("OpStrider", typeof(OpStriderSkillCondition));
        old.Add("OpSubjob", typeof(OpSubjobSkillCondition));
        old.Add("OpSweeper", typeof(OpSweeperSkillCondition));
        old.Add("OpTargetAllItemType", typeof(OpTargetAllItemTypeSkillCondition));
        old.Add("OpTargetArmorType", typeof(OpTargetArmorTypeSkillCondition));
        old.Add("OpTargetDoor", typeof(OpTargetDoorSkillCondition));
        old.Add("OpTargetMyPledgeAcademy", typeof(OpTargetMyPledgeAcademySkillCondition));
        old.Add("OpTargetNpc", typeof(OpTargetNpcSkillCondition));
        old.Add("OpTargetPc", typeof(OpTargetPcSkillCondition));
        old.Add("OpTargetWeaponAttackType", typeof(OpTargetWeaponAttackTypeSkillCondition));
        old.Add("OpTerritory", typeof(OpTerritorySkillCondition));
        old.Add("OpUnlock", typeof(OpUnlockSkillCondition));
        old.Add("OpUseFirecracker", typeof(OpUseFirecrackerSkillCondition));
        old.Add("OpUsePraseed", typeof(OpUsePraseedSkillCondition));
        old.Add("OpWyvern", typeof(OpWyvernSkillCondition));
        old.Add("PossessHolything", typeof(PossessHolythingSkillCondition));
        old.Add("RemainCpPer", typeof(RemainCpPerSkillCondition));
        old.Add("RemainHpPer", typeof(RemainHpPerSkillCondition));
        old.Add("RemainMpPer", typeof(RemainMpPerSkillCondition));
        old.Add("SoulSaved", typeof(SoulSavedSkillCondition));
        old.Add("TargetAffectedBySkill", typeof(TargetAffectedBySkillSkillCondition));
        old.Add("TargetItemCrystalType", typeof(TargetItemCrystalTypeSkillCondition));
        old.Add("TargetMyMentee", typeof(TargetMyMenteeSkillCondition));
        old.Add("TargetMyParty", typeof(TargetMyPartySkillCondition));
        old.Add("TargetMyPledge", typeof(TargetMyPledgeSkillCondition));
        old.Add("TargetNotAffectedBySkill", typeof(TargetNotAffectedBySkillSkillCondition));
        old.Add("TargetRace", typeof(TargetRaceSkillCondition));
        return old;
    }
}