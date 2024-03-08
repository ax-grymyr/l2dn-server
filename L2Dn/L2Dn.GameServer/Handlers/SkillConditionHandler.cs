using L2Dn.GameServer.Handlers.SkillConditionHandlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Scripting;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author NosBit
 */
public class SkillConditionHandler
{
	private readonly Map<String, Func<StatSet, ISkillCondition>> _skillConditionHandlerFactories = new();

	private SkillConditionHandler()
	{
		registerHandler("AssassinationPoints", stat => new AssassinationPointsSkillCondition(stat));
		registerHandler("BeastPoints", stat => new BeastPointsSkillCondition(stat));
		registerHandler("BuildAdvanceBase", stat => new BuildAdvanceBaseSkillCondition(stat));
		registerHandler("BuildCamp", stat => new BuildCampSkillCondition(stat));
		registerHandler("CanAddMaxEntranceInzone", stat => new CanAddMaxEntranceInzoneSkillCondition(stat));
		registerHandler("CanBookmarkAddSlot", stat => new CanBookmarkAddSlotSkillCondition(stat));
		registerHandler("CanChangeVitalItemCount", stat => new CanChangeVitalItemCountSkillCondition(stat));
		registerHandler("CanEnchantAttribute", stat => new CanEnchantAttributeSkillCondition(stat));
		registerHandler("CanMountForEvent", stat => new CanMountForEventSkillCondition(stat));
		registerHandler("CannotUseInTransform", stat => new CannotUseInTransformSkillCondition(stat));
		registerHandler("CanRefuelAirship", stat => new CanRefuelAirshipSkillCondition(stat));
		registerHandler("CanSummon", stat => new CanSummonSkillCondition(stat));
		registerHandler("CanSummonCubic", stat => new CanSummonCubicSkillCondition(stat));
		registerHandler("CanSummonMulti", stat => new CanSummonMultiSkillCondition(stat));
		registerHandler("CanSummonPet", stat => new CanSummonPetSkillCondition(stat));
		registerHandler("CanSummonSiegeGolem", stat => new CanSummonSiegeGolemSkillCondition(stat));
		registerHandler("CanTakeFort", stat => new CanTakeFortSkillCondition(stat));
		registerHandler("CanTransform", stat => new CanTransformSkillCondition(stat));
		registerHandler("CanTransformInDominion", stat => new CanTransformInDominionSkillCondition(stat));
		registerHandler("CanUntransform", stat => new CanUntransformSkillCondition(stat));
		registerHandler("CanUseInBattlefield", stat => new CanUseInBattlefieldSkillCondition(stat));
		registerHandler("CanUseInDragonLair", stat => new CanUseInDragonLairSkillCondition(stat));
		registerHandler("CanUseSwoopCannon", stat => new CanUseSwoopCannonSkillCondition(stat));
		registerHandler("HasVitalityPoints", stat => new HasVitalityPointsSkillCondition(stat));
		registerHandler("CanUseVitalityIncreaseItem", stat => new CanUseVitalityIncreaseItemSkillCondition(stat));
		registerHandler("CheckLevel", stat => new CheckLevelSkillCondition(stat));
		registerHandler("CheckSex", stat => new CheckSexSkillCondition(stat));
		registerHandler("ConsumeBody", stat => new ConsumeBodySkillCondition(stat));
		registerHandler("DeathPoints", stat => new DeathPointsSkillCondition(stat));
		registerHandler("EnergySaved", stat => new EnergySavedSkillCondition(stat));
		registerHandler("EquipArmor", stat => new EquipArmorSkillCondition(stat));
		registerHandler("EquippedCloakEnchant", stat => new EquippedCloakEnchantSkillCondition(stat));
		registerHandler("EquipShield", stat => new EquipShieldSkillCondition(stat));
		registerHandler("EquipSigil", stat => new EquipSigilSkillCondition(stat));
		registerHandler("EquipWeapon", stat => new EquipWeaponSkillCondition(stat));
		registerHandler("MaxMpSkillCondition", stat => new MaxMpSkillCondition(stat));
		registerHandler("NotFeared", stat => new NotFearedSkillCondition(stat));
		registerHandler("NotInUnderwater", stat => new NotInUnderwaterSkillCondition(stat));
		registerHandler("Op2hWeapon", stat => new Op2hWeaponSkillCondition(stat));
		registerHandler("OpAffectedBySkill", stat => new OpAffectedBySkillSkillCondition(stat));
		registerHandler("OpAgathionEnergy", stat => new OpAgathionEnergySkillCondition(stat));
		registerHandler("OpAlignment", stat => new OpAlignmentSkillCondition(stat));
		registerHandler("OpBaseStat", stat => new OpBaseStatSkillCondition(stat));
		registerHandler("OpBlink", stat => new OpBlinkSkillCondition(stat));
		registerHandler("OpCallPc", stat => new OpCallPcSkillCondition(stat));
		registerHandler("OpCanEscape", stat => new OpCanEscapeSkillCondition(stat));
		registerHandler("OpCanNotUseAirship", stat => new OpCanNotUseAirshipSkillCondition(stat));
		registerHandler("OpCannotUseTargetWithPrivateStore", stat => new OpCannotUseTargetWithPrivateStoreSkillCondition(stat));
		registerHandler("OpChangeWeapon", stat => new OpChangeWeaponSkillCondition(stat));
		registerHandler("OpCheckAbnormal", stat => new OpCheckAbnormalSkillCondition(stat));
		registerHandler("OpCheckAccountType", stat => new OpCheckAccountTypeSkillCondition(stat));
		registerHandler("OpCheckCastRange", stat => new OpCheckCastRangeSkillCondition(stat));
		registerHandler("OpCheckClass", stat => new OpCheckClassSkillCondition(stat));
		registerHandler("OpCheckClassList", stat => new OpCheckClassListSkillCondition(stat));
		registerHandler("OpCheckCrtEffect", stat => new OpCheckCrtEffectSkillCondition(stat));
		registerHandler("OpCheckFlag", stat => new OpCheckFlagSkillCondition(stat));
		registerHandler("OpCheckOnGoingEventCampaign", stat => new OpCheckOnGoingEventCampaignSkillCondition(stat));
		registerHandler("OpCheckPcbangPoint", stat => new OpCheckPcbangPointSkillCondition(stat));
		registerHandler("OpCheckResidence", stat => new OpCheckResidenceSkillCondition(stat));
		registerHandler("OpCheckSkill", stat => new OpCheckSkillSkillCondition(stat));
		registerHandler("OpCheckSkillList", stat => new OpCheckSkillListSkillCondition(stat));
		registerHandler("OpCompanion", stat => new OpCompanionSkillCondition(stat));
		registerHandler("OpEnchantRange", stat => new OpEnchantRangeSkillCondition(stat));
		registerHandler("OpEncumbered", stat => new OpEncumberedSkillCondition(stat));
		registerHandler("OpEnergyMax", stat => new OpEnergyMaxSkillCondition(stat));
		registerHandler("OpEquipItem", stat => new OpEquipItemSkillCondition(stat));
		registerHandler("OpExistNpc", stat => new OpExistNpcSkillCondition(stat));
		registerHandler("OpFishingCast", stat => new OpFishingCastSkillCondition(stat));
		registerHandler("OpFishingPumping", stat => new OpFishingPumpingSkillCondition(stat));
		registerHandler("OpFishingReeling", stat => new OpFishingReelingSkillCondition(stat));
		registerHandler("OpHaveSummon", stat => new OpHaveSummonSkillCondition(stat));
		registerHandler("OpHaveSummonedNpc", stat => new OpHaveSummonedNpcSkillCondition(stat));
		registerHandler("OpHome", stat => new OpHomeSkillCondition(stat));
		registerHandler("OpInSiege", stat => new OpInSiegeSkillCondition(stat));
		registerHandler("OpInstantzone", stat => new OpInstantzoneSkillCondition(stat));
		registerHandler("OpMainjob", stat => new OpMainjobSkillCondition(stat));
		registerHandler("OpNeedAgathion", stat => new OpNeedAgathionSkillCondition(stat));
		registerHandler("OpNeedSummonOrPet", stat => new OpNeedSummonOrPetSkillCondition(stat));
		registerHandler("OpNotAffectedBySkill", stat => new OpNotAffectedBySkillSkillCondition(stat));
		registerHandler("OpNotCursed", stat => new OpNotCursedSkillCondition(stat));
		registerHandler("OpNotInPeacezone", stat => new OpNotInPeacezoneSkillCondition(stat));
		registerHandler("OpNotInstantzone", stat => new OpNotInstantzoneSkillCondition(stat));
		registerHandler("OpNotOlympiad", stat => new OpNotOlympiadSkillCondition(stat));
		registerHandler("OpNotTerritory", stat => new OpNotTerritorySkillCondition(stat));
		registerHandler("OpOlympiad", stat => new OpOlympiadSkillCondition(stat));
		registerHandler("OpPeacezone", stat => new OpPeacezoneSkillCondition(stat));
		registerHandler("OpPkcount", stat => new OpPkcountSkillCondition(stat));
		registerHandler("OpPledge", stat => new OpPledgeSkillCondition(stat));
		registerHandler("OpRestartPoint", stat => new OpRestartPointSkillCondition(stat));
		registerHandler("OpResurrection", stat => new OpResurrectionSkillCondition(stat));
		registerHandler("OpSiegeHammer", stat => new OpSiegeHammerSkillCondition(stat));
		registerHandler("OpSkill", stat => new OpSkillSkillCondition(stat));
		registerHandler("OpSkillAcquire", stat => new OpSkillAcquireSkillCondition(stat));
		registerHandler("OpSocialClass", stat => new OpSocialClassSkillCondition(stat));
		registerHandler("OpSoulMax", stat => new OpSoulMaxSkillCondition(stat));
		registerHandler("OpStrider", stat => new OpStriderSkillCondition(stat));
		registerHandler("OpSubjob", stat => new OpSubjobSkillCondition(stat));
		registerHandler("OpSweeper", stat => new OpSweeperSkillCondition(stat));
		registerHandler("OpTargetAllItemType", stat => new OpTargetAllItemTypeSkillCondition(stat));
		registerHandler("OpTargetArmorType", stat => new OpTargetArmorTypeSkillCondition(stat));
		registerHandler("OpTargetDoor", stat => new OpTargetDoorSkillCondition(stat));
		registerHandler("OpTargetMyPledgeAcademy", stat => new OpTargetMyPledgeAcademySkillCondition(stat));
		registerHandler("OpTargetNpc", stat => new OpTargetNpcSkillCondition(stat));
		registerHandler("OpTargetPc", stat => new OpTargetPcSkillCondition(stat));
		registerHandler("OpTargetWeaponAttackType", stat => new OpTargetWeaponAttackTypeSkillCondition(stat));
		registerHandler("OpTerritory", stat => new OpTerritorySkillCondition(stat));
		registerHandler("OpUnlock", stat => new OpUnlockSkillCondition(stat));
		registerHandler("OpUseFirecracker", stat => new OpUseFirecrackerSkillCondition(stat));
		registerHandler("OpUsePraseed", stat => new OpUsePraseedSkillCondition(stat));
		registerHandler("OpWyvern", stat => new OpWyvernSkillCondition(stat));
		registerHandler("PossessHolything", stat => new PossessHolythingSkillCondition(stat));
		registerHandler("RemainCpPer", stat => new RemainCpPerSkillCondition(stat));
		registerHandler("RemainHpPer", stat => new RemainHpPerSkillCondition(stat));
		registerHandler("RemainMpPer", stat => new RemainMpPerSkillCondition(stat));
		registerHandler("SoulSaved", stat => new SoulSavedSkillCondition(stat));
		registerHandler("TargetAffectedBySkill", stat => new TargetAffectedBySkillSkillCondition(stat));
		registerHandler("TargetItemCrystalType", stat => new TargetItemCrystalTypeSkillCondition(stat));
		registerHandler("TargetMyMentee", stat => new TargetMyMenteeSkillCondition(stat));
		registerHandler("TargetMyParty", stat => new TargetMyPartySkillCondition(stat));
		registerHandler("TargetMyPledge", stat => new TargetMyPledgeSkillCondition(stat));
		registerHandler("TargetNotAffectedBySkill", stat => new TargetNotAffectedBySkillSkillCondition(stat));
		registerHandler("TargetRace", stat => new TargetRaceSkillCondition(stat));
	}
	
	public void registerHandler(String name, Func<StatSet, ISkillCondition> handlerFactory)
	{
		_skillConditionHandlerFactories.put(name, handlerFactory);
	}
	
	public Func<StatSet, ISkillCondition> getHandlerFactory(String name)
	{
		return _skillConditionHandlerFactories.get(name);
	}
	
	public int size()
	{
		return _skillConditionHandlerFactories.size();
	}
	
	public void executeScript()
	{
		try
		{
			ScriptEngineManager.getInstance().executeScript(ScriptEngineManager.SKILL_CONDITION_HANDLER_FILE);
		}
		catch (Exception e)
		{
			throw new Exception("Problems while running SkillMasterHandler", e);
		}
	}
	
	private static class SingletonHolder
	{
		public static readonly SkillConditionHandler INSTANCE = new SkillConditionHandler();
	}
	
	public static SkillConditionHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
}