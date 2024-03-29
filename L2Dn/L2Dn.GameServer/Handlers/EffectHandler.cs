using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Scripting;
using L2Dn.GameServer.Scripts.Handlers.EffectHandlers;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Handlers;

/**
 * @author BiggBoss, UnAfraid
 */
public class EffectHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EffectHandler));
	private readonly Map<String, Func<StatSet, AbstractEffect>> _effectHandlerFactories = new();

	private EffectHandler()
	{
		registerHandler("AbnormalShield", stat => new AbnormalShield(stat));
		registerHandler("AbnormalTimeChange", stat => new AbnormalTimeChange(stat));
		registerHandler("AbnormalTimeChangeBySkillId", stat => new AbnormalTimeChangeBySkillId(stat));
		registerHandler("AbsorbDamage", stat => new AbsorbDamage(stat));
		registerHandler("Accuracy", stat => new Accuracy(stat));
		registerHandler("AddHate", stat => new AddHate(stat));
		registerHandler("AddHuntingTime", stat => new AddHuntingTime(stat));
		registerHandler("AdditionalPhysicalAttack", stat => new AdditionalPhysicalAttack(stat));
		registerHandler("AdditionalPotionCp", stat => new AdditionalPotionCp(stat));
		registerHandler("AdditionalPotionHp", stat => new AdditionalPotionHp(stat));
		registerHandler("AdditionalPotionMp", stat => new AdditionalPotionMp(stat));
		registerHandler("AddPcCafePoints", stat => new AddPcCafePoints(stat));
		registerHandler("AddMaxPhysicalCriticalRate", stat => new AddMaxPhysicalCriticalRate(stat));
		registerHandler("AddMaxMagicCriticalRate", stat => new AddMaxMagicCriticalRate(stat));
		registerHandler("AddSkillBySkill", stat => new AddSkillBySkill(stat));
		registerHandler("AddTeleportBookmarkSlot", stat => new AddTeleportBookmarkSlot(stat));
		registerHandler("AgathionSlot", stat => new AgathionSlot(stat));
		registerHandler("AreaDamage", stat => new AreaDamage(stat));
		registerHandler("AreaOfEffectDamageDefence", stat => new AreaOfEffectDamageDefence(stat));
		registerHandler("AreaOfEffectDamageModify", stat => new AreaOfEffectDamageModify(stat));
		registerHandler("ArtifactSlot", stat => new ArtifactSlot(stat));
		registerHandler("AttackAttribute", stat => new AttackAttribute(stat));
		registerHandler("AttackAttributeAdd", stat => new AttackAttributeAdd(stat));
		registerHandler("AttackBehind", stat => new AttackBehind(stat));
		registerHandler("AttackTrait", stat => new AttackTrait(stat));
		registerHandler("AutoAttackDamageBonus", stat => new AutoAttackDamageBonus(stat));
		registerHandler("Backstab", stat => new Backstab(stat));
		registerHandler("Betray", stat => new Betray(stat));
		registerHandler("Blink", stat => new Blink(stat));
		registerHandler("BlinkSwap", stat => new BlinkSwap(stat));
		registerHandler("BlockAbnormalSlot", stat => new BlockAbnormalSlot(stat));
		registerHandler("BlockAction", stat => new BlockAction(stat));
		registerHandler("BlockActions", stat => new BlockActions(stat));
		registerHandler("BlockChat", stat => new BlockChat(stat));
		registerHandler("BlockControl", stat => new BlockControl(stat));
		registerHandler("BlockEscape", stat => new BlockEscape(stat));
		registerHandler("BlockMove", stat => new BlockMove(stat));
		registerHandler("BlockParty", stat => new BlockParty(stat));
		registerHandler("BlockResurrection", stat => new BlockResurrection(stat));
		registerHandler("BlockSkill", stat => new BlockSkill(stat));
		registerHandler("BlockTarget", stat => new BlockTarget(stat));
		registerHandler("Bluff", stat => new Bluff(stat));
		registerHandler("BonusDropAdena", stat => new BonusDropAdena(stat));
		registerHandler("BonusDropAmount", stat => new BonusDropAmount(stat));
		registerHandler("BonusDropRate", stat => new BonusDropRate(stat));
		registerHandler("BonusDropRateLCoin", stat => new BonusDropRateLCoin(stat));
		registerHandler("BonusRaidPoints", stat => new BonusRaidPoints(stat));
		registerHandler("BonusSpoilRate", stat => new BonusSpoilRate(stat));
		registerHandler("Breath", stat => new Breath(stat));
		registerHandler("BuffBlock", stat => new BuffBlock(stat));
		registerHandler("CallLearnedSkill", stat => new CallLearnedSkill(stat));
		registerHandler("CallParty", stat => new CallParty(stat));
		registerHandler("CallPc", stat => new CallPc(stat));
		registerHandler("CallRandomSkill", stat => new CallRandomSkill(stat));
		registerHandler("CallSkill", stat => new CallSkill(stat));
		registerHandler("CallSkillOnActionTime", stat => new CallSkillOnActionTime(stat));
		registerHandler("CallTargetParty", stat => new CallTargetParty(stat));
		registerHandler("CheapShot", stat => new CheapShot(stat));
		registerHandler("ChameleonRest", stat => new ChameleonRest(stat));
		registerHandler("ChangeBody", stat => new ChangeBody(stat));
		registerHandler("ChangeFace", stat => new ChangeFace(stat));
		registerHandler("ChangeFishingMastery", stat => new ChangeFishingMastery(stat));
		registerHandler("ChangeHairColor", stat => new ChangeHairColor(stat));
		registerHandler("ChangeHairStyle", stat => new ChangeHairStyle(stat));
		registerHandler("ClassChange", stat => new ClassChange(stat));
		registerHandler("Compelling", stat => new Compelling(stat));
		registerHandler("Confuse", stat => new Confuse(stat));
		registerHandler("ConsumeBody", stat => new ConsumeBody(stat));
		registerHandler("ConvertItem", stat => new ConvertItem(stat));
		registerHandler("CounterPhysicalSkill", stat => new CounterPhysicalSkill(stat));
		registerHandler("Cp", stat => new Cp(stat));
		registerHandler("CpHeal", stat => new CpHeal(stat));
		registerHandler("CpHealOverTime", stat => new CpHealOverTime(stat));
		registerHandler("CpHealPercent", stat => new CpHealPercent(stat));
		registerHandler("CpRegen", stat => new CpRegen(stat));
		registerHandler("CraftingCritical", stat => new CraftingCritical(stat));
		registerHandler("CraftRate", stat => new CraftRate(stat));
		registerHandler("CriticalDamage", stat => new CriticalDamage(stat));
		registerHandler("CriticalDamagePosition", stat => new CriticalDamagePosition(stat));
		registerHandler("CriticalRate", stat => new CriticalRate(stat));
		registerHandler("CriticalRatePositionBonus", stat => new CriticalRatePositionBonus(stat));
		registerHandler("CubicMastery", stat => new CubicMastery(stat));
		registerHandler("DamageBlock", stat => new DamageBlock(stat));
		registerHandler("DamageByAttack", stat => new DamageByAttack(stat));
		registerHandler("DamageShield", stat => new DamageShield(stat));
		registerHandler("DamageShieldResist", stat => new DamageShieldResist(stat));
		registerHandler("DamOverTime", stat => new DamOverTime(stat));
		registerHandler("DamOverTimePercent", stat => new DamOverTimePercent(stat));
		registerHandler("DeathLink", stat => new DeathLink(stat));
		registerHandler("DebuffBlock", stat => new DebuffBlock(stat));
		registerHandler("DefenceAttribute", stat => new DefenceAttribute(stat));
		registerHandler("DefenceCriticalDamage", stat => new DefenceCriticalDamage(stat));
		registerHandler("DefenceCriticalRate", stat => new DefenceCriticalRate(stat));
		registerHandler("DefenceIgnoreRemoval", stat => new DefenceIgnoreRemoval(stat));
		registerHandler("DefenceMagicCriticalDamage", stat => new DefenceMagicCriticalDamage(stat));
		registerHandler("DefenceMagicCriticalRate", stat => new DefenceMagicCriticalRate(stat));
		registerHandler("DefencePhysicalSkillCriticalDamage", stat => new DefencePhysicalSkillCriticalDamage(stat));
		registerHandler("DefencePhysicalSkillCriticalRate", stat => new DefencePhysicalSkillCriticalRate(stat));
		registerHandler("DefenceTrait", stat => new DefenceTrait(stat));
		registerHandler("DeleteHate", stat => new DeleteHate(stat));
		registerHandler("DeleteHateOfMe", stat => new DeleteHateOfMe(stat));
		registerHandler("DeleteTopAgro", stat => new DeleteTopAgro(stat));
		registerHandler("DetectHiddenObjects", stat => new DetectHiddenObjects(stat));
		registerHandler("Detection", stat => new Detection(stat));
		registerHandler("DisableSkill", stat => new DisableSkill(stat));
		registerHandler("DisableTargeting", stat => new DisableTargeting(stat));
		registerHandler("Disarm", stat => new Disarm(stat));
		registerHandler("Disarmor", stat => new Disarmor(stat));
		registerHandler("DispelAll", stat => new DispelAll(stat));
		registerHandler("DispelByCategory", stat => new DispelByCategory(stat));
		registerHandler("DispelBySlot", stat => new DispelBySlot(stat));
		registerHandler("DispelBySlotMyself", stat => new DispelBySlotMyself(stat));
		registerHandler("DispelBySlotProbability", stat => new DispelBySlotProbability(stat));
		registerHandler("DoubleCast", stat => new DoubleCast(stat));
		registerHandler("DuelistFury", stat => new DuelistFury(stat));
		registerHandler("ElementalSpiritAttack", stat => new ElementalSpiritAttack(stat));
		registerHandler("ElementalSpiritDefense", stat => new ElementalSpiritDefense(stat));
		registerHandler("ElixirUsageLimit", stat => new ElixirUsageLimit(stat));
		registerHandler("EnableCloak", stat => new EnableCloak(stat));
		registerHandler("EnchantRate", stat => new EnchantRate(stat));
		registerHandler("EnergyAttack", stat => new EnergyAttack(stat));
		registerHandler("EnlargeAbnormalSlot", stat => new EnlargeAbnormalSlot(stat));
		registerHandler("EnlargeSlot", stat => new EnlargeSlot(stat));
		registerHandler("Escape", stat => new Escape(stat));
		registerHandler("ExpModify", stat => new ExpModify(stat));
		registerHandler("ExpModifyPet", stat => new ExpModifyPet(stat));
		registerHandler("Faceoff", stat => new Faceoff(stat));
		registerHandler("FakeDeath", stat => new FakeDeath(stat));
		registerHandler("FatalBlow", stat => new FatalBlow(stat));
		registerHandler("FatalBlowRate", stat => new FatalBlowRate(stat));
		registerHandler("FatalBlowRateDefence", stat => new FatalBlowRateDefence(stat));
		registerHandler("Fear", stat => new Fear(stat));
		registerHandler("Feed", stat => new Feed(stat));
		registerHandler("FeedModify", stat => new FeedModify(stat));
		registerHandler("FishingExpSpBonus", stat => new FishingExpSpBonus(stat));
		registerHandler("Flag", stat => new Flag(stat));
		registerHandler("FocusEnergy", stat => new FocusEnergy(stat));
		registerHandler("FocusMomentum", stat => new FocusMomentum(stat));
		registerHandler("FocusMaxMomentum", stat => new FocusMaxMomentum(stat));
		registerHandler("FocusSouls", stat => new FocusSouls(stat));
		registerHandler("GetAgro", stat => new GetAgro(stat));
		registerHandler("GetDamageLimit", stat => new GetDamageLimit(stat));
		registerHandler("GetMomentum", stat => new GetMomentum(stat));
		registerHandler("GiveClanReputation", stat => new GiveClanReputation(stat));
		registerHandler("GiveExpAndSp", stat => new GiveExpAndSp(stat));
		registerHandler("GiveFame", stat => new GiveFame(stat));
		registerHandler("GiveHonorCoins", stat => new GiveHonorCoins(stat));
		registerHandler("GiveItemByExp", stat => new GiveItemByExp(stat));
		registerHandler("GivePetXp", stat => new GivePetXp(stat));
		registerHandler("GiveRecommendation", stat => new GiveRecommendation(stat));
		registerHandler("GiveSp", stat => new GiveSp(stat));
		registerHandler("GiveXp", stat => new GiveXp(stat));
		registerHandler("Grow", stat => new Grow(stat));
		registerHandler("HairAccessorySet", stat => new HairAccessorySet(stat));
		registerHandler("Harvesting", stat => new Harvesting(stat));
		registerHandler("HateAttack", stat => new HateAttack(stat));
		registerHandler("HeadquarterCreate", stat => new HeadquarterCreate(stat));
		registerHandler("Heal", stat => new Heal(stat));
		registerHandler("HealEffect", stat => new HealEffect(stat));
		registerHandler("HealOverTime", stat => new HealOverTime(stat));
		registerHandler("HealPercent", stat => new HealPercent(stat));
		registerHandler("Hide", stat => new Hide(stat));
		registerHandler("HitNumber", stat => new HitNumber(stat));
		registerHandler("Hp", stat => new Hp(stat));
		registerHandler("HpByLevel", stat => new HpByLevel(stat));
		registerHandler("HpCpHeal", stat => new HpCpHeal(stat));
		registerHandler("HpCpHealCritical", stat => new HpCpHealCritical(stat));
		registerHandler("HpDrain", stat => new HpDrain(stat));
		registerHandler("HpLimit", stat => new HpLimit(stat));
		registerHandler("HpRegen", stat => new HpRegen(stat));
		registerHandler("HpToOwner", stat => new HpToOwner(stat));
		registerHandler("IgnoreDeath", stat => new IgnoreDeath(stat));
		registerHandler("IgnoreReduceDamage", stat => new IgnoreReduceDamage(stat));
		registerHandler("ImmobileDamageBonus", stat => new ImmobileDamageBonus(stat));
		registerHandler("ImmobileDamageResist", stat => new ImmobileDamageResist(stat));
		registerHandler("ImmobilePetBuff", stat => new ImmobilePetBuff(stat));
		registerHandler("InstantKillResist", stat => new InstantKillResist(stat));
		registerHandler("JewelSlot", stat => new JewelSlot(stat));
		registerHandler("KarmaCount", stat => new KarmaCount(stat));
		registerHandler("KnockBack", stat => new KnockBack(stat));
		registerHandler("Lethal", stat => new Lethal(stat));
		registerHandler("LimitCp", stat => new LimitCp(stat));
		registerHandler("LimitHp", stat => new LimitHp(stat));
		registerHandler("LimitMp", stat => new LimitMp(stat));
		registerHandler("Lucky", stat => new Lucky(stat));
		registerHandler("MagicAccuracy", stat => new MagicAccuracy(stat));
		registerHandler("MagicalAbnormalDispelAttack", stat => new MagicalAbnormalDispelAttack(stat));
		registerHandler("MagicalAbnormalResist", stat => new MagicalAbnormalResist(stat));
		registerHandler("MagicalAttack", stat => new MagicalAttack(stat));
		registerHandler("MagicalAttackByAbnormal", stat => new MagicalAttackByAbnormal(stat));
		registerHandler("MagicalAttackByAbnormalSlot", stat => new MagicalAttackByAbnormalSlot(stat));
		registerHandler("MagicalAttackMp", stat => new MagicalAttackMp(stat));
		registerHandler("MagicalAttackRange", stat => new MagicalAttackRange(stat));
		registerHandler("MagicalAttackSpeed", stat => new MagicalAttackSpeed(stat));
		registerHandler("MagicalDamOverTime", stat => new MagicalDamOverTime(stat));
		registerHandler("MagicalDefence", stat => new MagicalDefence(stat));
		registerHandler("MagicalEvasion", stat => new MagicalEvasion(stat));
		registerHandler("MagicalSkillPower", stat => new MagicalSkillPower(stat));
		registerHandler("MagicalSoulAttack", stat => new MagicalSoulAttack(stat));
		registerHandler("MagicCriticalDamage", stat => new MagicCriticalDamage(stat));
		registerHandler("MagicCriticalRate", stat => new MagicCriticalRate(stat));
		registerHandler("MagicCriticalRateByCriticalRate", stat => new MagicCriticalRateByCriticalRate(stat));
		registerHandler("MagicLampExpRate", stat => new MagicLampExpRate(stat));
		registerHandler("MagicMpCost", stat => new MagicMpCost(stat));
		registerHandler("ManaCharge", stat => new ManaCharge(stat));
		registerHandler("ManaDamOverTime", stat => new ManaDamOverTime(stat));
		registerHandler("ManaHeal", stat => new ManaHeal(stat));
		registerHandler("ManaHealByLevel", stat => new ManaHealByLevel(stat));
		registerHandler("ManaHealOverTime", stat => new ManaHealOverTime(stat));
		registerHandler("ManaHealPercent", stat => new ManaHealPercent(stat));
		registerHandler("MAtk", stat => new MAtk(stat));
		registerHandler("MAtkByPAtk", stat => new MAtkByPAtk(stat));
		registerHandler("MaxCp", stat => new MaxCp(stat));
		registerHandler("MaxHp", stat => new MaxHp(stat));
		registerHandler("MaxMp", stat => new MaxMp(stat));
		registerHandler("ModifyAssassinationPoints", stat => new ModifyAssassinationPoints(stat));
		registerHandler("ModifyBeastPoints", stat => new ModifyBeastPoints(stat));
		registerHandler("ModifyCraftPoints", stat => new ModifyCraftPoints(stat));
		registerHandler("ModifyDeathPoints", stat => new ModifyDeathPoints(stat));
		registerHandler("ModifyMagicLampPoints", stat => new ModifyMagicLampPoints(stat));
		registerHandler("ModifyVital", stat => new ModifyVital(stat));
		registerHandler("Mp", stat => new Mp(stat));
		registerHandler("MpConsumePerLevel", stat => new MpConsumePerLevel(stat));
		registerHandler("MpRegen", stat => new MpRegen(stat));
		registerHandler("MpShield", stat => new MpShield(stat));
		registerHandler("MpVampiricAttack", stat => new MpVampiricAttack(stat));
		registerHandler("Mute", stat => new Mute(stat));
		registerHandler("NewHennaSlot", stat => new NewHennaSlot(stat));
		registerHandler("NightStatModify", stat => new NightStatModify(stat));
		registerHandler("NoblesseBless", stat => new NoblesseBless(stat));
		registerHandler("OpenChest", stat => new OpenChest(stat));
		registerHandler("OpenCommonRecipeBook", stat => new OpenCommonRecipeBook(stat));
		registerHandler("OpenDoor", stat => new OpenDoor(stat));
		registerHandler("OpenDwarfRecipeBook", stat => new OpenDwarfRecipeBook(stat));
		registerHandler("Passive", stat => new Passive(stat));
		registerHandler("PAtk", stat => new PAtk(stat));
		registerHandler("PhysicalAbnormalResist", stat => new PhysicalAbnormalResist(stat));
		registerHandler("PhysicalAttack", stat => new PhysicalAttack(stat));
		registerHandler("PhysicalAttackHpLink", stat => new PhysicalAttackHpLink(stat));
		registerHandler("PhysicalAttackMute", stat => new PhysicalAttackMute(stat));
		registerHandler("PhysicalAttackRange", stat => new PhysicalAttackRange(stat));
		registerHandler("PhysicalAttackSaveHp", stat => new PhysicalAttackSaveHp(stat));
		registerHandler("PhysicalAttackSpeed", stat => new PhysicalAttackSpeed(stat));
		registerHandler("PhysicalAttackWeaponBonus", stat => new PhysicalAttackWeaponBonus(stat));
		registerHandler("PhysicalDefence", stat => new PhysicalDefence(stat));
		registerHandler("PhysicalEvasion", stat => new PhysicalEvasion(stat));
		registerHandler("PhysicalMute", stat => new PhysicalMute(stat));
		registerHandler("PhysicalShieldAngleAll", stat => new PhysicalShieldAngleAll(stat));
		registerHandler("PhysicalSkillCriticalDamage", stat => new PhysicalSkillCriticalDamage(stat));
		registerHandler("PhysicalSkillCriticalRate", stat => new PhysicalSkillCriticalRate(stat));
		registerHandler("PhysicalSkillPower", stat => new PhysicalSkillPower(stat));
		registerHandler("PhysicalSoulAttack", stat => new PhysicalSoulAttack(stat));
		registerHandler("PkCount", stat => new PkCount(stat));
		registerHandler("Plunder", stat => new Plunder(stat));
		registerHandler("PolearmSingleTarget", stat => new PolearmSingleTarget(stat));
		registerHandler("ProtectionBlessing", stat => new ProtectionBlessing(stat));
		registerHandler("ProtectDeathPenalty", stat => new ProtectDeathPenalty(stat));
		registerHandler("PullBack", stat => new PullBack(stat));
		registerHandler("PveMagicalSkillDamageBonus", stat => new PveMagicalSkillDamageBonus(stat));
		registerHandler("PveMagicalSkillDefenceBonus", stat => new PveMagicalSkillDefenceBonus(stat));
		registerHandler("PvePhysicalAttackDamageBonus", stat => new PvePhysicalAttackDamageBonus(stat));
		registerHandler("PvePhysicalAttackDefenceBonus", stat => new PvePhysicalAttackDefenceBonus(stat));
		registerHandler("PvePhysicalSkillDamageBonus", stat => new PvePhysicalSkillDamageBonus(stat));
		registerHandler("PvePhysicalSkillDefenceBonus", stat => new PvePhysicalSkillDefenceBonus(stat));
		registerHandler("PveRaidMagicalSkillDamageBonus", stat => new PveRaidMagicalSkillDamageBonus(stat));
		registerHandler("PveRaidMagicalSkillDefenceBonus", stat => new PveRaidMagicalSkillDefenceBonus(stat));
		registerHandler("PveRaidPhysicalAttackDamageBonus", stat => new PveRaidPhysicalAttackDamageBonus(stat));
		registerHandler("PveRaidPhysicalAttackDefenceBonus", stat => new PveRaidPhysicalAttackDefenceBonus(stat));
		registerHandler("PveRaidPhysicalSkillDamageBonus", stat => new PveRaidPhysicalSkillDamageBonus(stat));
		registerHandler("PveRaidPhysicalSkillDefenceBonus", stat => new PveRaidPhysicalSkillDefenceBonus(stat));
		registerHandler("PvpMagicalSkillDamageBonus", stat => new PvpMagicalSkillDamageBonus(stat));
		registerHandler("PvpMagicalSkillDefenceBonus", stat => new PvpMagicalSkillDefenceBonus(stat));
		registerHandler("PvpPhysicalAttackDamageBonus", stat => new PvpPhysicalAttackDamageBonus(stat));
		registerHandler("PvpPhysicalAttackDefenceBonus", stat => new PvpPhysicalAttackDefenceBonus(stat));
		registerHandler("PvpPhysicalSkillDamageBonus", stat => new PvpPhysicalSkillDamageBonus(stat));
		registerHandler("PvpPhysicalSkillDefenceBonus", stat => new PvpPhysicalSkillDefenceBonus(stat));
		registerHandler("RandomizeHate", stat => new RandomizeHate(stat));
		registerHandler("RealDamage", stat => new RealDamage(stat));
		registerHandler("RealDamageResist", stat => new RealDamageResist(stat));
		registerHandler("RearDamage", stat => new RearDamage(stat));
		registerHandler("RebalanceHP", stat => new RebalanceHP(stat));
		registerHandler("RebalanceHPSummon", stat => new RebalanceHPSummon(stat));
		registerHandler("RecoverVitalityInPeaceZone", stat => new RecoverVitalityInPeaceZone(stat));
		registerHandler("ReduceDamage", stat => new ReduceDamage(stat));
		registerHandler("ReduceCancel", stat => new ReduceCancel(stat));
		registerHandler("ReduceDropPenalty", stat => new ReduceDropPenalty(stat));
		registerHandler("ReflectMagic", stat => new ReflectMagic(stat));
		registerHandler("ReflectSkill", stat => new ReflectSkill(stat));
		registerHandler("RefuelAirship", stat => new RefuelAirship(stat));
		registerHandler("Relax", stat => new Relax(stat));
		registerHandler("ReplaceSkillBySkill", stat => new ReplaceSkillBySkill(stat));
		registerHandler("ResetInstanceEntry", stat => new ResetInstanceEntry(stat));
		registerHandler("ResistAbnormalByCategory", stat => new ResistAbnormalByCategory(stat));
		registerHandler("ResistDDMagic", stat => new ResistDDMagic(stat));
		registerHandler("ResistDispelByCategory", stat => new ResistDispelByCategory(stat));
		registerHandler("ResistSkill", stat => new ResistSkill(stat));
		registerHandler("Restoration", stat => new Restoration(stat));
		registerHandler("RestorationRandom", stat => new RestorationRandom(stat));
		registerHandler("Resurrection", stat => new Resurrection(stat));
		registerHandler("ResurrectionFeeModifier", stat => new ResurrectionFeeModifier(stat));
		registerHandler("ResurrectionSpecial", stat => new ResurrectionSpecial(stat));
		registerHandler("Reuse", stat => new Reuse(stat));
		registerHandler("ReuseSkillById", stat => new ReuseSkillById(stat));
		registerHandler("ReuseSkillIdByDamage", stat => new ReuseSkillIdByDamage(stat));
		registerHandler("Root", stat => new Root(stat));
		registerHandler("SacrificeSummon", stat => new SacrificeSummon(stat));
		registerHandler("SafeFallHeight", stat => new SafeFallHeight(stat));
		registerHandler("SayhaGraceSupport", stat => new SayhaGraceSupport(stat));
		registerHandler("SendSystemMessageToClan", stat => new SendSystemMessageToClan(stat));
		registerHandler("ServitorShare", stat => new ServitorShare(stat));
		registerHandler("ServitorShareSkills", stat => new ServitorShareSkills(stat));
		registerHandler("SetHp", stat => new SetHp(stat));
		registerHandler("SetCp", stat => new SetCp(stat));
		registerHandler("SetSkill", stat => new SetSkill(stat));
		registerHandler("ShieldDefence", stat => new ShieldDefence(stat));
		registerHandler("ShieldDefenceIgnoreRemoval", stat => new ShieldDefenceIgnoreRemoval(stat));
		registerHandler("ShieldDefenceRate", stat => new ShieldDefenceRate(stat));
		registerHandler("ShotsBonus", stat => new ShotsBonus(stat));
		registerHandler("SilentMove", stat => new SilentMove(stat));
		registerHandler("SkillBonusRange", stat => new SkillBonusRange(stat));
		registerHandler("SkillEvasion", stat => new SkillEvasion(stat));
		registerHandler("SkillMastery", stat => new SkillMastery(stat));
		registerHandler("SkillMasteryRate", stat => new SkillMasteryRate(stat));
		registerHandler("SkillPowerAdd", stat => new SkillPowerAdd(stat));
		registerHandler("SkillTurning", stat => new SkillTurning(stat));
		registerHandler("SkillTurningOverTime", stat => new SkillTurningOverTime(stat));
		registerHandler("SoulBlow", stat => new SoulBlow(stat));
		registerHandler("SoulEating", stat => new SoulEating(stat));
		registerHandler("SoulshotResistance", stat => new SoulshotResistance(stat));
		registerHandler("Sow", stat => new Sow(stat));
		registerHandler("Speed", stat => new Speed(stat));
		registerHandler("SphericBarrier", stat => new SphericBarrier(stat));
		registerHandler("SpeedLimit", stat => new SpeedLimit(stat));
		registerHandler("SpiritExpModify", stat => new SpiritExpModify(stat));
		registerHandler("SpiritshotResistance", stat => new SpiritshotResistance(stat));
		registerHandler("SpModify", stat => new SpModify(stat));
		registerHandler("Spoil", stat => new Spoil(stat));
		registerHandler("StatAddForLevel", stat => new StatAddForLevel(stat));
		registerHandler("StatAddForMp", stat => new StatAddForMp(stat));
		registerHandler("StatAddForStat", stat => new StatAddForStat(stat));
		registerHandler("StatBonusSkillCritical", stat => new StatBonusSkillCritical(stat));
		registerHandler("StatBonusSpeed", stat => new StatBonusSpeed(stat));
		registerHandler("StatByMoveType", stat => new StatByMoveType(stat));
		registerHandler("StatMulForBaseStat", stat => new StatMulForBaseStat(stat));
		registerHandler("StatMulForLevel", stat => new StatMulForLevel(stat));
		registerHandler("StatUp", stat => new StatUp(stat));
		registerHandler("StealAbnormal", stat => new StealAbnormal(stat));
		registerHandler("Summon", stat => new Summon(stat));
		registerHandler("SummonAgathion", stat => new SummonAgathion(stat));
		registerHandler("SummonCubic", stat => new SummonCubic(stat));
		registerHandler("SummonHallucination", stat => new SummonHallucination(stat));
		registerHandler("SummonMulti", stat => new SummonMulti(stat));
		registerHandler("SummonNpc", stat => new SummonNpc(stat));
		registerHandler("SummonPet", stat => new SummonPet(stat));
		registerHandler("SummonPoints", stat => new SummonPoints(stat));
		registerHandler("SummonTrap", stat => new SummonTrap(stat));
		registerHandler("Sweeper", stat => new Sweeper(stat));
		registerHandler("Synergy", stat => new Synergy(stat));
		registerHandler("TakeCastle", stat => new TakeCastle(stat));
		registerHandler("TakeCastleStart", stat => new TakeCastleStart(stat));
		registerHandler("TakeFort", stat => new TakeFort(stat));
		registerHandler("TakeFortStart", stat => new TakeFortStart(stat));
		registerHandler("TalismanSlot", stat => new TalismanSlot(stat));
		registerHandler("TargetCancel", stat => new TargetCancel(stat));
		registerHandler("TargetMe", stat => new TargetMe(stat));
		registerHandler("TargetMeProbability", stat => new TargetMeProbability(stat));
		registerHandler("Teleport", stat => new Teleport(stat));
		registerHandler("TeleportToNpc", stat => new TeleportToNpc(stat));
		registerHandler("TeleportToPlayer", stat => new TeleportToPlayer(stat));
		registerHandler("TeleportToSummon", stat => new TeleportToSummon(stat));
		registerHandler("TeleportToTarget", stat => new TeleportToTarget(stat));
		registerHandler("TeleportToTeleportLocation", stat => new TeleportToTeleportLocation(stat));
		registerHandler("FlyAway", stat => new FlyAway(stat));
		registerHandler("TransferDamageToPlayer", stat => new TransferDamageToPlayer(stat));
		registerHandler("TransferDamageToSummon", stat => new TransferDamageToSummon(stat));
		registerHandler("TransferHate", stat => new TransferHate(stat));
		registerHandler("Transformation", stat => new Transformation(stat));
		registerHandler("TrapDetect", stat => new TrapDetect(stat));
		registerHandler("TrapRemove", stat => new TrapRemove(stat));
		registerHandler("TriggerHealPercentBySkill", stat => new TriggerHealPercentBySkill(stat));
		registerHandler("TriggerSkill", stat => new TriggerSkill(stat));
		registerHandler("TriggerSkillByAttack", stat => new TriggerSkillByAttack(stat));
		registerHandler("TriggerSkillByAvoid", stat => new TriggerSkillByAvoid(stat));
		registerHandler("TriggerSkillByBaseStat", stat => new TriggerSkillByBaseStat(stat));
		registerHandler("TriggerSkillByDamage", stat => new TriggerSkillByDamage(stat));
		registerHandler("TriggerSkillByDeathBlow", stat => new TriggerSkillByDeathBlow(stat));
		registerHandler("TriggerSkillByDualRange", stat => new TriggerSkillByDualRange(stat));
		registerHandler("TriggerSkillByHpPercent", stat => new TriggerSkillByHpPercent(stat));
		registerHandler("TriggerSkillByKill", stat => new TriggerSkillByKill(stat));
		registerHandler("TriggerSkillByMagicType", stat => new TriggerSkillByMagicType(stat));
		registerHandler("TriggerSkillByMaxHp", stat => new TriggerSkillByMaxHp(stat));
		registerHandler("TriggerSkillBySkill", stat => new TriggerSkillBySkill(stat));
		registerHandler("TriggerSkillBySkillAttack", stat => new TriggerSkillBySkillAttack(stat));
		registerHandler("TriggerSkillByStat", stat => new TriggerSkillByStat(stat));
		registerHandler("TwoHandedBluntBonus", stat => new TwoHandedBluntBonus(stat));
		registerHandler("TwoHandedStance", stat => new TwoHandedStance(stat));
		registerHandler("TwoHandedSwordBonus", stat => new TwoHandedSwordBonus(stat));
		registerHandler("Unsummon", stat => new Unsummon(stat));
		registerHandler("UnsummonAgathion", stat => new UnsummonAgathion(stat));
		registerHandler("UnsummonServitors", stat => new UnsummonServitors(stat));
		registerHandler("Untargetable", stat => new Untargetable(stat));
		registerHandler("VampiricAttack", stat => new VampiricAttack(stat));
		registerHandler("VampiricDefence", stat => new VampiricDefence(stat));
		registerHandler("VipUp", stat => new VipUp(stat));
		registerHandler("VitalityExpRate", stat => new VitalityExpRate(stat));
		registerHandler("VitalityPointsRate", stat => new VitalityPointsRate(stat));
		registerHandler("VitalityPointUp", stat => new VitalityPointUp(stat));
		registerHandler("WeaponAttackAngleBonus", stat => new WeaponAttackAngleBonus(stat));
		registerHandler("WeaponBonusMAtk", stat => new WeaponBonusMAtk(stat));
		registerHandler("WeaponBonusMAtkMultiplier", stat => new WeaponBonusMAtkMultiplier(stat));
		registerHandler("WeaponBonusPAtk", stat => new WeaponBonusPAtk(stat));
		registerHandler("WeaponBonusPAtkMultiplier", stat => new WeaponBonusPAtkMultiplier(stat));
		registerHandler("WeightLimit", stat => new WeightLimit(stat));
		registerHandler("WeightPenalty", stat => new WeightPenalty(stat));
		registerHandler("WorldChatPoints", stat => new WorldChatPoints(stat));
		_logger.Info(GetType().Name + ": Loaded " + size() + " effect handlers.");
	}
	
	public void registerHandler(String name, Func<StatSet, AbstractEffect> handlerFactory)
	{
		_effectHandlerFactories.put(name, handlerFactory);
	}
	
	public Func<StatSet, AbstractEffect> getHandlerFactory(String name)
	{
		return _effectHandlerFactories.get(name);
	}
	
	public int size()
	{
		return _effectHandlerFactories.size();
	}
	
	public void executeScript()
	{
		try
		{
			ScriptEngineManager.getInstance().executeScript(ScriptEngineManager.EFFECT_MASTER_HANDLER_FILE);
		}
		catch (Exception e)
		{
			throw new Exception("Problems while running EffectMasterHandler", e);
		}
	}
	
	private static class SingletonHolder
	{
		public static readonly EffectHandler INSTANCE = new();
	}
	
	public static EffectHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
}