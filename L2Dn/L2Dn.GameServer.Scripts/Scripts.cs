using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Scripts.AI.Players;
using L2Dn.GameServer.Scripts.AI.Teleporters;
using L2Dn.GameServer.Scripts.Handlers.ActionHandlers;
using L2Dn.GameServer.Scripts.Handlers.ActionShiftHandlers;
using L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;
using L2Dn.GameServer.Scripts.Handlers.BypassHandlers;
using L2Dn.GameServer.Scripts.Handlers.ChatHandlers;
using L2Dn.GameServer.Scripts.Handlers.CommunityBoard;
using L2Dn.GameServer.Scripts.Handlers.ConditionHandlers;
using L2Dn.GameServer.Scripts.Handlers.DailyMissionHandlers;
using L2Dn.GameServer.Scripts.Handlers.EffectHandlers;
using L2Dn.GameServer.Scripts.Handlers.ItemHandlers;
using L2Dn.GameServer.Scripts.Handlers.PlayerActions;
using L2Dn.GameServer.Scripts.Handlers.PunishmentHandlers;
using L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;
using L2Dn.GameServer.Scripts.Handlers.TargetHandlers;
using L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectObjects;
using L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;
using L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;
using L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;
using L2Dn.GameServer.Scripts.Quests;
using PlayerAction = L2Dn.GameServer.Scripts.Handlers.ActionHandlers.PlayerAction;
using Single = L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes.Single;
using Summon = L2Dn.GameServer.Scripts.Handlers.EffectHandlers.Summon;
using TrapAction = L2Dn.GameServer.Scripts.Handlers.ActionHandlers.TrapAction;

namespace L2Dn.GameServer.Scripts;

public static class Scripts
{
    public static void RegisterHandlers()
    {
        // Action handlers
        ActionHandler actionHandler = ActionHandler.getInstance();
        actionHandler.registerHandler(new ArtefactAction());
        actionHandler.registerHandler(new DecoyAction());
        actionHandler.registerHandler(new DoorAction());
        actionHandler.registerHandler(new ItemAction());
        actionHandler.registerHandler(new NpcAction());
        actionHandler.registerHandler(new PlayerAction());
        actionHandler.registerHandler(new PetAction());
        actionHandler.registerHandler(new StaticObjectAction());
        actionHandler.registerHandler(new SummonAction());
        actionHandler.registerHandler(new TrapAction());

        // Action shift handlers
        ActionShiftHandler actionShiftHandler = ActionShiftHandler.getInstance();
        actionShiftHandler.registerHandler(new DoorActionShift());
        actionShiftHandler.registerHandler(new ItemActionShift());
        actionShiftHandler.registerHandler(new NpcActionShift());
        actionShiftHandler.registerHandler(new PlayerActionShift());
        actionShiftHandler.registerHandler(new StaticObjectActionShift());
        actionShiftHandler.registerHandler(new SummonActionShift());

        // Affect object handler
        AffectObjectHandler affectObjectHandler = AffectObjectHandler.getInstance();
        affectObjectHandler.registerHandler(new All());
        affectObjectHandler.registerHandler(new Clan());
        affectObjectHandler.registerHandler(new Friend());
        affectObjectHandler.registerHandler(new FriendPc());
        affectObjectHandler.registerHandler(new HiddenPlace());
        affectObjectHandler.registerHandler(new Invisible());
        affectObjectHandler.registerHandler(new NotFriend());
        affectObjectHandler.registerHandler(new NotFriendPc());
        affectObjectHandler.registerHandler(new ObjectDeadNpcBody());
        affectObjectHandler.registerHandler(new UndeadRealEnemy());
        affectObjectHandler.registerHandler(new WyvernObject());

        // Affect scope handlers
        AffectScopeHandler affectScopeHandler = AffectScopeHandler.getInstance();
        affectScopeHandler.registerHandler(new BalakasScope());
        affectScopeHandler.registerHandler(new DeadParty());
        affectScopeHandler.registerHandler(new DeadPartyPledge());
        affectScopeHandler.registerHandler(new DeadPledge());
        affectScopeHandler.registerHandler(new DeadUnion());
        affectScopeHandler.registerHandler(new Fan());
        affectScopeHandler.registerHandler(new FanPB());
        affectScopeHandler.registerHandler(new Party());
        affectScopeHandler.registerHandler(new PartyPledge());
        affectScopeHandler.registerHandler(new Pledge());
        affectScopeHandler.registerHandler(new PointBlank());
        affectScopeHandler.registerHandler(new Handlers.TargetHandlers.AffectScopes.Range());
        affectScopeHandler.registerHandler(new RangeSortByHp());
        affectScopeHandler.registerHandler(new RingRange());
        affectScopeHandler.registerHandler(new Single());
        affectScopeHandler.registerHandler(new Square());
        affectScopeHandler.registerHandler(new SquarePB());
        affectScopeHandler.registerHandler(new StaticObjectScope());
        affectScopeHandler.registerHandler(new SummonExceptMaster());

        // Bypass handlers
        BypassHandler bypassHandler = BypassHandler.getInstance();
        bypassHandler.registerHandler(new Augment());
        bypassHandler.registerHandler(new Buy());
        bypassHandler.registerHandler(new ChatLink());
        bypassHandler.registerHandler(new ClanWarehouse());
        bypassHandler.registerHandler(new EnsoulWindow());
        bypassHandler.registerHandler(new FindPvP());
        bypassHandler.registerHandler(new Freight());
        bypassHandler.registerHandler(new ItemAuctionLink());
        bypassHandler.registerHandler(new Link());
        bypassHandler.registerHandler(new Multisell());
        bypassHandler.registerHandler(new NpcViewMod());
        bypassHandler.registerHandler(new Observation());
        bypassHandler.registerHandler(new PetExtractWindow());
        bypassHandler.registerHandler(new QuestLink());
        bypassHandler.registerHandler(new PlayerHelp());
        bypassHandler.registerHandler(new PrivateWarehouse());
        bypassHandler.registerHandler(new ReleaseAttribute());
        bypassHandler.registerHandler(new SkillList());
        bypassHandler.registerHandler(new SupportBlessing());
        bypassHandler.registerHandler(new SupportMagic());
        bypassHandler.registerHandler(new TerritoryStatus());
        bypassHandler.registerHandler(new TutorialClose());
        bypassHandler.registerHandler(new UpgradeEquipment());
        bypassHandler.registerHandler(new VoiceCommand());
        bypassHandler.registerHandler(new Wear());

        // Chat handlers
        ChatHandler chatHandler = ChatHandler.getInstance();
        chatHandler.registerHandler(new ChatGeneral());
        chatHandler.registerHandler(new ChatAlliance());
        chatHandler.registerHandler(new ChatClan());
        chatHandler.registerHandler(new ChatHeroVoice());
        chatHandler.registerHandler(new ChatParty());
        chatHandler.registerHandler(new ChatPartyMatchRoom());
        chatHandler.registerHandler(new ChatPartyRoomAll());
        chatHandler.registerHandler(new ChatPartyRoomCommander());
        chatHandler.registerHandler(new ChatPetition());
        chatHandler.registerHandler(new ChatShout());
        chatHandler.registerHandler(new ChatWhisper());
        chatHandler.registerHandler(new ChatTrade());
        chatHandler.registerHandler(new ChatWorld());

        // Condition handlers
        ConditionHandler conditionHandler = ConditionHandler.getInstance();
        conditionHandler.registerHandler("CategoryType", stat => new CategoryTypeCondition(stat));
        conditionHandler.registerHandler("NpcLevel", stat => new NpcLevelCondition(stat));
        conditionHandler.registerHandler("PlayerLevel", stat => new PlayerLevelCondition(stat));

        // Effect handlers
        EffectHandler effectHandler = EffectHandler.getInstance();
        effectHandler.registerHandler("AbnormalShield", stat => new AbnormalShield(stat));
        effectHandler.registerHandler("AbnormalTimeChange", stat => new AbnormalTimeChange(stat));
        effectHandler.registerHandler("AbnormalTimeChangeBySkillId", stat => new AbnormalTimeChangeBySkillId(stat));
        effectHandler.registerHandler("AbsorbDamage", stat => new AbsorbDamage(stat));
        effectHandler.registerHandler("Accuracy", stat => new Accuracy(stat));
        effectHandler.registerHandler("AddHate", stat => new AddHate(stat));
        effectHandler.registerHandler("AddHuntingTime", stat => new AddHuntingTime(stat));
        effectHandler.registerHandler("AdditionalPhysicalAttack", stat => new AdditionalPhysicalAttack(stat));
        effectHandler.registerHandler("AdditionalPotionCp", stat => new AdditionalPotionCp(stat));
        effectHandler.registerHandler("AdditionalPotionHp", stat => new AdditionalPotionHp(stat));
        effectHandler.registerHandler("AdditionalPotionMp", stat => new AdditionalPotionMp(stat));
        effectHandler.registerHandler("AddPcCafePoints", stat => new AddPcCafePoints(stat));
        effectHandler.registerHandler("AddMaxPhysicalCriticalRate", stat => new AddMaxPhysicalCriticalRate(stat));
        effectHandler.registerHandler("AddMaxMagicCriticalRate", stat => new AddMaxMagicCriticalRate(stat));
        effectHandler.registerHandler("AddSkillBySkill", stat => new AddSkillBySkill(stat));
        effectHandler.registerHandler("AddTeleportBookmarkSlot", stat => new AddTeleportBookmarkSlot(stat));
        effectHandler.registerHandler("AgathionSlot", stat => new AgathionSlot(stat));
        effectHandler.registerHandler("AreaDamage", stat => new AreaDamage(stat));
        effectHandler.registerHandler("AreaOfEffectDamageDefence", stat => new AreaOfEffectDamageDefence(stat));
        effectHandler.registerHandler("AreaOfEffectDamageModify", stat => new AreaOfEffectDamageModify(stat));
        effectHandler.registerHandler("ArtifactSlot", stat => new ArtifactSlot(stat));
        effectHandler.registerHandler("AttackAttribute", stat => new AttackAttribute(stat));
        effectHandler.registerHandler("AttackAttributeAdd", stat => new AttackAttributeAdd(stat));
        effectHandler.registerHandler("AttackBehind", stat => new AttackBehind());
        effectHandler.registerHandler("AttackTrait", stat => new AttackTrait(stat));
        effectHandler.registerHandler("AutoAttackDamageBonus", stat => new AutoAttackDamageBonus(stat));
        effectHandler.registerHandler("Backstab", stat => new Backstab(stat));
        effectHandler.registerHandler("Betray", stat => new Betray());
        effectHandler.registerHandler("Blink", stat => new Blink(stat));
        effectHandler.registerHandler("BlinkSwap", stat => new BlinkSwap());
        effectHandler.registerHandler("BlockAbnormalSlot", stat => new BlockAbnormalSlot(stat));
        effectHandler.registerHandler("BlockAction", stat => new BlockAction(stat));
        effectHandler.registerHandler("BlockActions", stat => new BlockActions(stat));
        effectHandler.registerHandler("BlockChat", stat => new BlockChat());
        effectHandler.registerHandler("BlockControl", stat => new BlockControl());
        effectHandler.registerHandler("BlockEscape", stat => new BlockEscape());
        effectHandler.registerHandler("BlockMove", stat => new BlockMove());
        effectHandler.registerHandler("BlockParty", stat => new BlockParty());
        effectHandler.registerHandler("BlockResurrection", stat => new BlockResurrection());
        effectHandler.registerHandler("BlockSkill", stat => new BlockSkill(stat));
        effectHandler.registerHandler("BlockTarget", stat => new BlockTarget());
        effectHandler.registerHandler("Bluff", stat => new Bluff(stat));
        effectHandler.registerHandler("BonusDropAdena", stat => new BonusDropAdena(stat));
        effectHandler.registerHandler("BonusDropAmount", stat => new BonusDropAmount(stat));
        effectHandler.registerHandler("BonusDropRate", stat => new BonusDropRate(stat));
        effectHandler.registerHandler("BonusDropRateLCoin", stat => new BonusDropRateLCoin(stat));
        effectHandler.registerHandler("BonusRaidPoints", stat => new BonusRaidPoints(stat));
        effectHandler.registerHandler("BonusSpoilRate", stat => new BonusSpoilRate(stat));
        effectHandler.registerHandler("Breath", stat => new Breath(stat));
        effectHandler.registerHandler("BuffBlock", stat => new BuffBlock());
        effectHandler.registerHandler("CallLearnedSkill", stat => new CallLearnedSkill(stat));
        effectHandler.registerHandler("CallParty", stat => new CallParty());
        effectHandler.registerHandler("CallPc", stat => new CallPc(stat));
        effectHandler.registerHandler("CallRandomSkill", stat => new CallRandomSkill(stat));
        effectHandler.registerHandler("CallSkill", stat => new CallSkill(stat));
        effectHandler.registerHandler("CallSkillOnActionTime", stat => new CallSkillOnActionTime(stat));
        effectHandler.registerHandler("CallTargetParty", stat => new CallTargetParty());
        effectHandler.registerHandler("CheapShot", stat => new CheapShot());
        effectHandler.registerHandler("ChameleonRest", stat => new ChameleonRest(stat));
        effectHandler.registerHandler("ChangeBody", stat => new ChangeBody(stat));
        effectHandler.registerHandler("ChangeFace", stat => new ChangeFace(stat));
        effectHandler.registerHandler("ChangeFishingMastery", stat => new ChangeFishingMastery());
        effectHandler.registerHandler("ChangeHairColor", stat => new ChangeHairColor(stat));
        effectHandler.registerHandler("ChangeHairStyle", stat => new ChangeHairStyle(stat));
        effectHandler.registerHandler("ClassChange", stat => new ClassChange(stat));
        effectHandler.registerHandler("Compelling", stat => new Compelling());
        effectHandler.registerHandler("Confuse", stat => new Confuse(stat));
        effectHandler.registerHandler("ConsumeBody", stat => new ConsumeBody());
        effectHandler.registerHandler("ConvertItem", stat => new ConvertItem());
        effectHandler.registerHandler("CounterPhysicalSkill", stat => new CounterPhysicalSkill(stat));
        effectHandler.registerHandler("Cp", stat => new Cp(stat));
        effectHandler.registerHandler("CpHeal", stat => new CpHeal(stat));
        effectHandler.registerHandler("CpHealOverTime", stat => new CpHealOverTime(stat));
        effectHandler.registerHandler("CpHealPercent", stat => new CpHealPercent(stat));
        effectHandler.registerHandler("CpRegen", stat => new CpRegen(stat));
        effectHandler.registerHandler("CraftingCritical", stat => new CraftingCritical(stat));
        effectHandler.registerHandler("CraftRate", stat => new CraftRate(stat));
        effectHandler.registerHandler("CriticalDamage", stat => new CriticalDamage(stat));
        effectHandler.registerHandler("CriticalDamagePosition", stat => new CriticalDamagePosition(stat));
        effectHandler.registerHandler("CriticalRate", stat => new CriticalRate(stat));
        effectHandler.registerHandler("CriticalRatePositionBonus", stat => new CriticalRatePositionBonus(stat));
        effectHandler.registerHandler("CubicMastery", stat => new CubicMastery(stat));
        effectHandler.registerHandler("DamageBlock", stat => new DamageBlock(stat));
        effectHandler.registerHandler("DamageByAttack", stat => new DamageByAttack(stat));
        effectHandler.registerHandler("DamageShield", stat => new DamageShield(stat));
        effectHandler.registerHandler("DamageShieldResist", stat => new DamageShieldResist(stat));
        effectHandler.registerHandler("DamOverTime", stat => new DamOverTime(stat));
        effectHandler.registerHandler("DamOverTimePercent", stat => new DamOverTimePercent(stat));
        effectHandler.registerHandler("DeathLink", stat => new DeathLink(stat));
        effectHandler.registerHandler("DebuffBlock", stat => new DebuffBlock());
        effectHandler.registerHandler("DefenceAttribute", stat => new DefenceAttribute(stat));
        effectHandler.registerHandler("DefenceCriticalDamage", stat => new DefenceCriticalDamage(stat));
        effectHandler.registerHandler("DefenceCriticalRate", stat => new DefenceCriticalRate(stat));
        effectHandler.registerHandler("DefenceIgnoreRemoval", stat => new DefenceIgnoreRemoval(stat));
        effectHandler.registerHandler("DefenceMagicCriticalDamage", stat => new DefenceMagicCriticalDamage(stat));
        effectHandler.registerHandler("DefenceMagicCriticalRate", stat => new DefenceMagicCriticalRate(stat));
        effectHandler.registerHandler("DefencePhysicalSkillCriticalDamage",
            stat => new DefencePhysicalSkillCriticalDamage(stat));

        effectHandler.registerHandler("DefencePhysicalSkillCriticalRate",
            stat => new DefencePhysicalSkillCriticalRate(stat));

        effectHandler.registerHandler("DefenceTrait", stat => new DefenceTrait(stat));
        effectHandler.registerHandler("DeleteHate", stat => new DeleteHate(stat));
        effectHandler.registerHandler("DeleteHateOfMe", stat => new DeleteHateOfMe(stat));
        effectHandler.registerHandler("DeleteTopAgro", stat => new DeleteTopAgro(stat));
        effectHandler.registerHandler("DetectHiddenObjects", stat => new DetectHiddenObjects());
        effectHandler.registerHandler("Detection", stat => new Detection());
        effectHandler.registerHandler("DisableSkill", stat => new DisableSkill(stat));
        effectHandler.registerHandler("DisableTargeting", stat => new DisableTargeting());
        effectHandler.registerHandler("Disarm", stat => new Disarm());
        effectHandler.registerHandler("Disarmor", stat => new Disarmor(stat));
        effectHandler.registerHandler("DispelAll", stat => new DispelAll());
        effectHandler.registerHandler("DispelByCategory", stat => new DispelByCategory(stat));
        effectHandler.registerHandler("DispelBySlot", stat => new DispelBySlot(stat));
        effectHandler.registerHandler("DispelBySlotMyself", stat => new DispelBySlotMyself(stat));
        effectHandler.registerHandler("DispelBySlotProbability", stat => new DispelBySlotProbability(stat));
        effectHandler.registerHandler("DoubleCast", stat => new DoubleCast());
        effectHandler.registerHandler("DuelistFury", stat => new DuelistFury());
        effectHandler.registerHandler("ElementalSpiritAttack", stat => new ElementalSpiritAttack(stat));
        effectHandler.registerHandler("ElementalSpiritDefense", stat => new ElementalSpiritDefense(stat));
        effectHandler.registerHandler("ElixirUsageLimit", stat => new ElixirUsageLimit(stat));
        effectHandler.registerHandler("EnableCloak", stat => new EnableCloak());
        effectHandler.registerHandler("EnchantRate", stat => new EnchantRate(stat));
        effectHandler.registerHandler("EnergyAttack", stat => new EnergyAttack(stat));
        effectHandler.registerHandler("EnlargeAbnormalSlot", stat => new EnlargeAbnormalSlot(stat));
        effectHandler.registerHandler("EnlargeSlot", stat => new EnlargeSlot(stat));
        effectHandler.registerHandler("Escape", stat => new Escape(stat));
        effectHandler.registerHandler("ExpModify", stat => new ExpModify(stat));
        effectHandler.registerHandler("ExpModifyPet", stat => new ExpModifyPet(stat));
        effectHandler.registerHandler("Faceoff", stat => new Faceoff());
        effectHandler.registerHandler("FakeDeath", stat => new FakeDeath(stat));
        effectHandler.registerHandler("FatalBlow", stat => new FatalBlow(stat));
        effectHandler.registerHandler("FatalBlowRate", stat => new FatalBlowRate(stat));
        effectHandler.registerHandler("FatalBlowRateDefence", stat => new FatalBlowRateDefence(stat));
        effectHandler.registerHandler("Fear", stat => new Fear());
        effectHandler.registerHandler("Feed", stat => new Feed(stat));
        effectHandler.registerHandler("FeedModify", stat => new FeedModify(stat));
        effectHandler.registerHandler("FishingExpSpBonus", stat => new FishingExpSpBonus(stat));
        effectHandler.registerHandler("Flag", stat => new Flag());
        effectHandler.registerHandler("FocusEnergy", stat => new FocusEnergy(stat));
        effectHandler.registerHandler("FocusMomentum", stat => new FocusMomentum(stat));
        effectHandler.registerHandler("FocusMaxMomentum", stat => new FocusMaxMomentum());
        effectHandler.registerHandler("FocusSouls", stat => new FocusSouls(stat));
        effectHandler.registerHandler("GetAgro", stat => new GetAgro());
        effectHandler.registerHandler("GetDamageLimit", stat => new GetDamageLimit(stat));
        effectHandler.registerHandler("GetMomentum", stat => new GetMomentum(stat));
        effectHandler.registerHandler("GiveClanReputation", stat => new GiveClanReputation(stat));
        effectHandler.registerHandler("GiveExpAndSp", stat => new GiveExpAndSp(stat));
        effectHandler.registerHandler("GiveFame", stat => new GiveFame(stat));
        effectHandler.registerHandler("GiveHonorCoins", stat => new GiveHonorCoins(stat));
        effectHandler.registerHandler("GiveItemByExp", stat => new GiveItemByExp(stat));
        effectHandler.registerHandler("GivePetXp", stat => new GivePetXp(stat));
        effectHandler.registerHandler("GiveRecommendation", stat => new GiveRecommendation(stat));
        effectHandler.registerHandler("GiveSp", stat => new GiveSp(stat));
        effectHandler.registerHandler("GiveXp", stat => new GiveXp(stat));
        effectHandler.registerHandler("Grow", stat => new Grow());
        effectHandler.registerHandler("HairAccessorySet", stat => new HairAccessorySet());
        effectHandler.registerHandler("Harvesting", stat => new Harvesting());
        effectHandler.registerHandler("HateAttack", stat => new HateAttack(stat));
        effectHandler.registerHandler("HeadquarterCreate", stat => new HeadquarterCreate(stat));
        effectHandler.registerHandler("Heal", stat => new Heal(stat));
        effectHandler.registerHandler("HealEffect", stat => new HealEffect(stat));
        effectHandler.registerHandler("HealOverTime", stat => new HealOverTime(stat));
        effectHandler.registerHandler("HealPercent", stat => new HealPercent(stat));
        effectHandler.registerHandler("Hide", stat => new Hide());
        effectHandler.registerHandler("HitNumber", stat => new HitNumber(stat));
        effectHandler.registerHandler("Hp", stat => new Hp(stat));
        effectHandler.registerHandler("HpByLevel", stat => new HpByLevel(stat));
        effectHandler.registerHandler("HpCpHeal", stat => new HpCpHeal(stat));
        effectHandler.registerHandler("HpCpHealCritical", stat => new HpCpHealCritical());
        effectHandler.registerHandler("HpDrain", stat => new HpDrain(stat));
        effectHandler.registerHandler("HpLimit", stat => new HpLimit(stat));
        effectHandler.registerHandler("HpRegen", stat => new HpRegen(stat));
        effectHandler.registerHandler("HpToOwner", stat => new HpToOwner(stat));
        effectHandler.registerHandler("IgnoreDeath", stat => new IgnoreDeath());
        effectHandler.registerHandler("IgnoreReduceDamage", stat => new IgnoreReduceDamage(stat));
        effectHandler.registerHandler("ImmobileDamageBonus", stat => new ImmobileDamageBonus(stat));
        effectHandler.registerHandler("ImmobileDamageResist", stat => new ImmobileDamageResist(stat));
        effectHandler.registerHandler("ImmobilePetBuff", stat => new ImmobilePetBuff());
        effectHandler.registerHandler("InstantKillResist", stat => new InstantKillResist(stat));
        effectHandler.registerHandler("JewelSlot", stat => new JewelSlot(stat));
        effectHandler.registerHandler("KarmaCount", stat => new KarmaCount(stat));
        effectHandler.registerHandler("KnockBack", stat => new KnockBack(stat));
        effectHandler.registerHandler("Lethal", stat => new Lethal(stat));
        effectHandler.registerHandler("LimitCp", stat => new LimitCp(stat));
        effectHandler.registerHandler("LimitHp", stat => new LimitHp(stat));
        effectHandler.registerHandler("LimitMp", stat => new LimitMp(stat));
        effectHandler.registerHandler("Lucky", stat => new Lucky());
        effectHandler.registerHandler("MagicAccuracy", stat => new MagicAccuracy(stat));
        effectHandler.registerHandler("MagicalAbnormalDispelAttack", stat => new MagicalAbnormalDispelAttack(stat));
        effectHandler.registerHandler("MagicalAbnormalResist", stat => new MagicalAbnormalResist(stat));
        effectHandler.registerHandler("MagicalAttack", stat => new MagicalAttack(stat));
        effectHandler.registerHandler("MagicalAttackByAbnormal", stat => new MagicalAttackByAbnormal(stat));
        effectHandler.registerHandler("MagicalAttackByAbnormalSlot", stat => new MagicalAttackByAbnormalSlot(stat));
        effectHandler.registerHandler("MagicalAttackMp", stat => new MagicalAttackMp(stat));
        effectHandler.registerHandler("MagicalAttackRange", stat => new MagicalAttackRange(stat));
        effectHandler.registerHandler("MagicalAttackSpeed", stat => new MagicalAttackSpeed(stat));
        effectHandler.registerHandler("MagicalDamOverTime", stat => new MagicalDamOverTime(stat));
        effectHandler.registerHandler("MagicalDefence", stat => new MagicalDefence(stat));
        effectHandler.registerHandler("MagicalEvasion", stat => new MagicalEvasion(stat));
        effectHandler.registerHandler("MagicalSkillPower", stat => new MagicalSkillPower(stat));
        effectHandler.registerHandler("MagicalSoulAttack", stat => new MagicalSoulAttack(stat));
        effectHandler.registerHandler("MagicCriticalDamage", stat => new MagicCriticalDamage(stat));
        effectHandler.registerHandler("MagicCriticalRate", stat => new MagicCriticalRate(stat));
        effectHandler.registerHandler("MagicCriticalRateByCriticalRate",
            stat => new MagicCriticalRateByCriticalRate(stat));

        effectHandler.registerHandler("MagicLampExpRate", stat => new MagicLampExpRate(stat));
        effectHandler.registerHandler("MagicMpCost", stat => new MagicMpCost(stat));
        effectHandler.registerHandler("ManaCharge", stat => new ManaCharge(stat));
        effectHandler.registerHandler("ManaDamOverTime", stat => new ManaDamOverTime(stat));
        effectHandler.registerHandler("ManaHeal", stat => new ManaHeal(stat));
        effectHandler.registerHandler("ManaHealByLevel", stat => new ManaHealByLevel(stat));
        effectHandler.registerHandler("ManaHealOverTime", stat => new ManaHealOverTime(stat));
        effectHandler.registerHandler("ManaHealPercent", stat => new ManaHealPercent(stat));
        effectHandler.registerHandler("MAtk", stat => new MAtk(stat));
        effectHandler.registerHandler("MAtkByPAtk", stat => new MAtkByPAtk(stat));
        effectHandler.registerHandler("MaxCp", stat => new MaxCp(stat));
        effectHandler.registerHandler("MaxHp", stat => new MaxHp(stat));
        effectHandler.registerHandler("MaxMp", stat => new MaxMp(stat));
        effectHandler.registerHandler("ModifyAssassinationPoints", stat => new ModifyAssassinationPoints(stat));
        effectHandler.registerHandler("ModifyBeastPoints", stat => new ModifyBeastPoints(stat));
        effectHandler.registerHandler("ModifyCraftPoints", stat => new ModifyCraftPoints(stat));
        effectHandler.registerHandler("ModifyDeathPoints", stat => new ModifyDeathPoints(stat));
        effectHandler.registerHandler("ModifyMagicLampPoints", stat => new ModifyMagicLampPoints(stat));
        effectHandler.registerHandler("ModifyVital", stat => new ModifyVital(stat));
        effectHandler.registerHandler("Mp", stat => new Mp(stat));
        effectHandler.registerHandler("MpConsumePerLevel", stat => new MpConsumePerLevel(stat));
        effectHandler.registerHandler("MpRegen", stat => new MpRegen(stat));
        effectHandler.registerHandler("MpShield", stat => new MpShield(stat));
        effectHandler.registerHandler("MpVampiricAttack", stat => new MpVampiricAttack(stat));
        effectHandler.registerHandler("Mute", stat => new Mute());
        effectHandler.registerHandler("NewHennaSlot", stat => new NewHennaSlot(stat));
        effectHandler.registerHandler("NightStatModify", stat => new NightStatModify(stat));
        effectHandler.registerHandler("NoblesseBless", stat => new NoblesseBless());
        effectHandler.registerHandler("OpenChest", stat => new OpenChest());
        effectHandler.registerHandler("OpenCommonRecipeBook", stat => new OpenCommonRecipeBook());
        effectHandler.registerHandler("OpenDoor", stat => new OpenDoor(stat));
        effectHandler.registerHandler("OpenDwarfRecipeBook", stat => new OpenDwarfRecipeBook());
        effectHandler.registerHandler("Passive", stat => new Passive());
        effectHandler.registerHandler("PAtk", stat => new PAtk(stat));
        effectHandler.registerHandler("PhysicalAbnormalResist", stat => new PhysicalAbnormalResist(stat));
        effectHandler.registerHandler("PhysicalAttack", stat => new PhysicalAttack(stat));
        effectHandler.registerHandler("PhysicalAttackHpLink", stat => new PhysicalAttackHpLink(stat));
        effectHandler.registerHandler("PhysicalAttackMute", stat => new PhysicalAttackMute());
        effectHandler.registerHandler("PhysicalAttackRange", stat => new PhysicalAttackRange(stat));
        effectHandler.registerHandler("PhysicalAttackSaveHp", stat => new PhysicalAttackSaveHp(stat));
        effectHandler.registerHandler("PhysicalAttackSpeed", stat => new PhysicalAttackSpeed(stat));
        effectHandler.registerHandler("PhysicalAttackWeaponBonus", stat => new PhysicalAttackWeaponBonus(stat));
        effectHandler.registerHandler("PhysicalDefence", stat => new PhysicalDefence(stat));
        effectHandler.registerHandler("PhysicalEvasion", stat => new PhysicalEvasion(stat));
        effectHandler.registerHandler("PhysicalMute", stat => new PhysicalMute());
        effectHandler.registerHandler("PhysicalShieldAngleAll", stat => new PhysicalShieldAngleAll());
        effectHandler.registerHandler("PhysicalSkillCriticalDamage", stat => new PhysicalSkillCriticalDamage(stat));
        effectHandler.registerHandler("PhysicalSkillCriticalRate", stat => new PhysicalSkillCriticalRate(stat));
        effectHandler.registerHandler("PhysicalSkillPower", stat => new PhysicalSkillPower(stat));
        effectHandler.registerHandler("PhysicalSoulAttack", stat => new PhysicalSoulAttack(stat));
        effectHandler.registerHandler("PkCount", stat => new PkCount(stat));
        effectHandler.registerHandler("Plunder", stat => new Plunder());
        effectHandler.registerHandler("PolearmSingleTarget", stat => new PolearmSingleTarget());
        effectHandler.registerHandler("ProtectionBlessing", stat => new ProtectionBlessing());
        effectHandler.registerHandler("ProtectDeathPenalty", stat => new ProtectDeathPenalty());
        effectHandler.registerHandler("PullBack", stat => new PullBack(stat));
        effectHandler.registerHandler("PveMagicalSkillDamageBonus", stat => new PveMagicalSkillDamageBonus(stat));
        effectHandler.registerHandler("PveMagicalSkillDefenceBonus", stat => new PveMagicalSkillDefenceBonus(stat));
        effectHandler.registerHandler("PvePhysicalAttackDamageBonus", stat => new PvePhysicalAttackDamageBonus(stat));
        effectHandler.registerHandler("PvePhysicalAttackDefenceBonus", stat => new PvePhysicalAttackDefenceBonus(stat));
        effectHandler.registerHandler("PvePhysicalSkillDamageBonus", stat => new PvePhysicalSkillDamageBonus(stat));
        effectHandler.registerHandler("PvePhysicalSkillDefenceBonus", stat => new PvePhysicalSkillDefenceBonus(stat));
        effectHandler.registerHandler("PveRaidMagicalSkillDamageBonus",
            stat => new PveRaidMagicalSkillDamageBonus(stat));

        effectHandler.registerHandler("PveRaidMagicalSkillDefenceBonus",
            stat => new PveRaidMagicalSkillDefenceBonus(stat));

        effectHandler.registerHandler("PveRaidPhysicalAttackDamageBonus",
            stat => new PveRaidPhysicalAttackDamageBonus(stat));

        effectHandler.registerHandler("PveRaidPhysicalAttackDefenceBonus",
            stat => new PveRaidPhysicalAttackDefenceBonus(stat));

        effectHandler.registerHandler("PveRaidPhysicalSkillDamageBonus",
            stat => new PveRaidPhysicalSkillDamageBonus(stat));

        effectHandler.registerHandler("PveRaidPhysicalSkillDefenceBonus",
            stat => new PveRaidPhysicalSkillDefenceBonus(stat));

        effectHandler.registerHandler("PvpMagicalSkillDamageBonus", stat => new PvpMagicalSkillDamageBonus(stat));
        effectHandler.registerHandler("PvpMagicalSkillDefenceBonus", stat => new PvpMagicalSkillDefenceBonus(stat));
        effectHandler.registerHandler("PvpPhysicalAttackDamageBonus", stat => new PvpPhysicalAttackDamageBonus(stat));
        effectHandler.registerHandler("PvpPhysicalAttackDefenceBonus", stat => new PvpPhysicalAttackDefenceBonus(stat));
        effectHandler.registerHandler("PvpPhysicalSkillDamageBonus", stat => new PvpPhysicalSkillDamageBonus(stat));
        effectHandler.registerHandler("PvpPhysicalSkillDefenceBonus", stat => new PvpPhysicalSkillDefenceBonus(stat));
        effectHandler.registerHandler("RandomizeHate", stat => new RandomizeHate(stat));
        effectHandler.registerHandler("RealDamage", stat => new RealDamage(stat));
        effectHandler.registerHandler("RealDamageResist", stat => new RealDamageResist(stat));
        effectHandler.registerHandler("RearDamage", stat => new RearDamage(stat));
        effectHandler.registerHandler("RebalanceHP", stat => new RebalanceHP());
        effectHandler.registerHandler("RebalanceHPSummon", stat => new RebalanceHPSummon());
        effectHandler.registerHandler("RecoverVitalityInPeaceZone", stat => new RecoverVitalityInPeaceZone(stat));
        effectHandler.registerHandler("ReduceDamage", stat => new ReduceDamage(stat));
        effectHandler.registerHandler("ReduceCancel", stat => new ReduceCancel(stat));
        effectHandler.registerHandler("ReduceDropPenalty", stat => new ReduceDropPenalty(stat));
        effectHandler.registerHandler("ReflectMagic", stat => new ReflectMagic(stat));
        effectHandler.registerHandler("ReflectSkill", stat => new ReflectSkill(stat));
        effectHandler.registerHandler("RefuelAirship", stat => new RefuelAirship(stat));
        effectHandler.registerHandler("Relax", stat => new Relax(stat));
        effectHandler.registerHandler("ReplaceSkillBySkill", stat => new ReplaceSkillBySkill(stat));
        effectHandler.registerHandler("ResetInstanceEntry", stat => new ResetInstanceEntry(stat));
        effectHandler.registerHandler("ResistAbnormalByCategory", stat => new ResistAbnormalByCategory(stat));
        effectHandler.registerHandler("ResistDDMagic", stat => new ResistDDMagic(stat));
        effectHandler.registerHandler("ResistDispelByCategory", stat => new ResistDispelByCategory(stat));
        effectHandler.registerHandler("ResistSkill", stat => new ResistSkill(stat));
        effectHandler.registerHandler("Restoration", stat => new Restoration(stat));
        effectHandler.registerHandler("RestorationRandom", stat => new RestorationRandom(stat));
        effectHandler.registerHandler("Resurrection", stat => new Resurrection(stat));
        effectHandler.registerHandler("ResurrectionFeeModifier", stat => new ResurrectionFeeModifier(stat));
        effectHandler.registerHandler("ResurrectionSpecial", stat => new ResurrectionSpecial(stat));
        effectHandler.registerHandler("Reuse", stat => new Reuse(stat));
        effectHandler.registerHandler("ReuseSkillById", stat => new ReuseSkillById(stat));
        effectHandler.registerHandler("ReuseSkillIdByDamage", stat => new ReuseSkillIdByDamage(stat));
        effectHandler.registerHandler("Root", stat => new Root());
        effectHandler.registerHandler("SacrificeSummon", stat => new SacrificeSummon());
        effectHandler.registerHandler("SafeFallHeight", stat => new SafeFallHeight(stat));
        effectHandler.registerHandler("SayhaGraceSupport", stat => new SayhaGraceSupport());
        effectHandler.registerHandler("SendSystemMessageToClan", stat => new SendSystemMessageToClan(stat));
        effectHandler.registerHandler("ServitorShare", stat => new ServitorShare(stat));
        effectHandler.registerHandler("ServitorShareSkills", stat => new ServitorShareSkills());
        effectHandler.registerHandler("SetHp", stat => new SetHp(stat));
        effectHandler.registerHandler("SetCp", stat => new SetCp(stat));
        effectHandler.registerHandler("SetSkill", stat => new SetSkill(stat));
        effectHandler.registerHandler("ShieldDefence", stat => new ShieldDefence(stat));
        effectHandler.registerHandler("ShieldDefenceIgnoreRemoval", stat => new ShieldDefenceIgnoreRemoval(stat));
        effectHandler.registerHandler("ShieldDefenceRate", stat => new ShieldDefenceRate(stat));
        effectHandler.registerHandler("ShotsBonus", stat => new ShotsBonus(stat));
        effectHandler.registerHandler("SilentMove", stat => new SilentMove());
        effectHandler.registerHandler("SkillBonusRange", stat => new SkillBonusRange(stat));
        effectHandler.registerHandler("SkillEvasion", stat => new SkillEvasion(stat));
        effectHandler.registerHandler("SkillMastery", stat => new SkillMastery(stat));
        effectHandler.registerHandler("SkillMasteryRate", stat => new SkillMasteryRate(stat));
        effectHandler.registerHandler("SkillPowerAdd", stat => new SkillPowerAdd(stat));
        effectHandler.registerHandler("SkillTurning", stat => new SkillTurning(stat));
        effectHandler.registerHandler("SkillTurningOverTime", stat => new SkillTurningOverTime(stat));
        effectHandler.registerHandler("SoulBlow", stat => new SoulBlow(stat));
        effectHandler.registerHandler("SoulEating", stat => new SoulEating(stat));
        effectHandler.registerHandler("SoulshotResistance", stat => new SoulshotResistance(stat));
        effectHandler.registerHandler("Sow", stat => new Sow());
        effectHandler.registerHandler("Speed", stat => new Speed(stat));
        effectHandler.registerHandler("SphericBarrier", stat => new SphericBarrier(stat));
        effectHandler.registerHandler("SpeedLimit", stat => new SpeedLimit(stat));
        effectHandler.registerHandler("SpiritExpModify", stat => new SpiritExpModify(stat));
        effectHandler.registerHandler("SpiritshotResistance", stat => new SpiritshotResistance(stat));
        effectHandler.registerHandler("SpModify", stat => new SpModify(stat));
        effectHandler.registerHandler("Spoil", stat => new Spoil());
        effectHandler.registerHandler("StatAddForLevel", stat => new StatAddForLevel(stat));
        effectHandler.registerHandler("StatAddForMp", stat => new StatAddForMp(stat));
        effectHandler.registerHandler("StatAddForStat", stat => new StatAddForStat(stat));
        effectHandler.registerHandler("StatBonusSkillCritical", stat => new StatBonusSkillCritical(stat));
        effectHandler.registerHandler("StatBonusSpeed", stat => new StatBonusSpeed(stat));
        effectHandler.registerHandler("StatByMoveType", stat => new StatByMoveType(stat));
        effectHandler.registerHandler("StatMulForBaseStat", stat => new StatMulForBaseStat(stat));
        effectHandler.registerHandler("StatMulForLevel", stat => new StatMulForLevel(stat));
        effectHandler.registerHandler("StatUp", stat => new StatUp(stat));
        effectHandler.registerHandler("StealAbnormal", stat => new StealAbnormal(stat));
        effectHandler.registerHandler("Summon", stat => new Summon(stat));
        effectHandler.registerHandler("SummonAgathion", stat => new SummonAgathion(stat));
        effectHandler.registerHandler("SummonCubic", stat => new SummonCubic(stat));
        effectHandler.registerHandler("SummonHallucination", stat => new SummonHallucination(stat));
        effectHandler.registerHandler("SummonMulti", stat => new SummonMulti(stat));
        effectHandler.registerHandler("SummonNpc", stat => new SummonNpc(stat));
        effectHandler.registerHandler("SummonPet", stat => new SummonPet());
        effectHandler.registerHandler("SummonPoints", stat => new SummonPoints(stat));
        effectHandler.registerHandler("SummonTrap", stat => new SummonTrap(stat));
        effectHandler.registerHandler("Sweeper", stat => new Sweeper(stat));
        effectHandler.registerHandler("Synergy", stat => new Synergy(stat));
        effectHandler.registerHandler("TakeCastle", stat => new TakeCastle(stat));
        effectHandler.registerHandler("TakeCastleStart", stat => new TakeCastleStart());
        effectHandler.registerHandler("TakeFort", stat => new TakeFort());
        effectHandler.registerHandler("TakeFortStart", stat => new TakeFortStart());
        effectHandler.registerHandler("TalismanSlot", stat => new TalismanSlot(stat));
        effectHandler.registerHandler("TargetCancel", stat => new TargetCancel(stat));
        effectHandler.registerHandler("TargetMe", stat => new TargetMe());
        effectHandler.registerHandler("TargetMeProbability", stat => new TargetMeProbability(stat));
        effectHandler.registerHandler("Teleport", stat => new Teleport(stat));
        effectHandler.registerHandler("TeleportToNpc", stat => new TeleportToNpc(stat));
        effectHandler.registerHandler("TeleportToPlayer", stat => new TeleportToPlayer());
        effectHandler.registerHandler("TeleportToSummon", stat => new TeleportToSummon(stat));
        effectHandler.registerHandler("TeleportToTarget", stat => new TeleportToTarget());
        effectHandler.registerHandler("TeleportToTeleportLocation", stat => new TeleportToTeleportLocation());
        effectHandler.registerHandler("FlyAway", stat => new FlyAway(stat));
        effectHandler.registerHandler("TransferDamageToPlayer", stat => new TransferDamageToPlayer(stat));
        effectHandler.registerHandler("TransferDamageToSummon", stat => new TransferDamageToSummon(stat));
        effectHandler.registerHandler("TransferHate", stat => new TransferHate(stat));
        effectHandler.registerHandler("Transformation", stat => new Transformation(stat));
        effectHandler.registerHandler("TrapDetect", stat => new TrapDetect(stat));
        effectHandler.registerHandler("TrapRemove", stat => new TrapRemove(stat));
        effectHandler.registerHandler("TriggerHealPercentBySkill", stat => new TriggerHealPercentBySkill(stat));
        effectHandler.registerHandler("TriggerSkill", stat => new TriggerSkill(stat));
        effectHandler.registerHandler("TriggerSkillByAttack", stat => new TriggerSkillByAttack(stat));
        effectHandler.registerHandler("TriggerSkillByAvoid", stat => new TriggerSkillByAvoid(stat));
        effectHandler.registerHandler("TriggerSkillByBaseStat", stat => new TriggerSkillByBaseStat(stat));
        effectHandler.registerHandler("TriggerSkillByDamage", stat => new TriggerSkillByDamage(stat));
        effectHandler.registerHandler("TriggerSkillByDeathBlow", stat => new TriggerSkillByDeathBlow(stat));
        effectHandler.registerHandler("TriggerSkillByDualRange", stat => new TriggerSkillByDualRange(stat));
        effectHandler.registerHandler("TriggerSkillByHpPercent", stat => new TriggerSkillByHpPercent(stat));
        effectHandler.registerHandler("TriggerSkillByKill", stat => new TriggerSkillByKill(stat));
        effectHandler.registerHandler("TriggerSkillByMagicType", stat => new TriggerSkillByMagicType(stat));
        effectHandler.registerHandler("TriggerSkillByMaxHp", stat => new TriggerSkillByMaxHp(stat));
        effectHandler.registerHandler("TriggerSkillBySkill", stat => new TriggerSkillBySkill(stat));
        effectHandler.registerHandler("TriggerSkillBySkillAttack", stat => new TriggerSkillBySkillAttack(stat));
        effectHandler.registerHandler("TriggerSkillByStat", stat => new TriggerSkillByStat(stat));
        effectHandler.registerHandler("TwoHandedBluntBonus", stat => new TwoHandedBluntBonus(stat));
        effectHandler.registerHandler("TwoHandedStance", stat => new TwoHandedStance(stat));
        effectHandler.registerHandler("TwoHandedSwordBonus", stat => new TwoHandedSwordBonus(stat));
        effectHandler.registerHandler("Unsummon", stat => new Unsummon(stat));
        effectHandler.registerHandler("UnsummonAgathion", stat => new UnsummonAgathion());
        effectHandler.registerHandler("UnsummonServitors", stat => new UnsummonServitors());
        effectHandler.registerHandler("Untargetable", stat => new Untargetable());
        effectHandler.registerHandler("VampiricAttack", stat => new VampiricAttack(stat));
        effectHandler.registerHandler("VampiricDefence", stat => new VampiricDefence(stat));
        effectHandler.registerHandler("VipUp", stat => new VipUp(stat));
        effectHandler.registerHandler("VitalityExpRate", stat => new VitalityExpRate(stat));
        effectHandler.registerHandler("VitalityPointsRate", stat => new VitalityPointsRate(stat));
        effectHandler.registerHandler("VitalityPointUp", stat => new VitalityPointUp(stat));
        effectHandler.registerHandler("WeaponAttackAngleBonus", stat => new WeaponAttackAngleBonus(stat));
        effectHandler.registerHandler("WeaponBonusMAtk", stat => new WeaponBonusMAtk(stat));
        effectHandler.registerHandler("WeaponBonusMAtkMultiplier", stat => new WeaponBonusMAtkMultiplier(stat));
        effectHandler.registerHandler("WeaponBonusPAtk", stat => new WeaponBonusPAtk(stat));
        effectHandler.registerHandler("WeaponBonusPAtkMultiplier", stat => new WeaponBonusPAtkMultiplier(stat));
        effectHandler.registerHandler("WeightLimit", stat => new WeightLimit(stat));
        effectHandler.registerHandler("WeightPenalty", stat => new WeightPenalty(stat));
        effectHandler.registerHandler("WorldChatPoints", stat => new WorldChatPoints(stat));

        // Item handlers
        ItemHandler itemHandler = ItemHandler.getInstance();
        itemHandler.registerHandler(new AddSpiritExp());
        itemHandler.registerHandler(new Appearance());
        itemHandler.registerHandler(new BeastSoulShot());
        itemHandler.registerHandler(new BeastSpiritShot());
        itemHandler.registerHandler(new BlessedSoulShots());
        itemHandler.registerHandler(new BlessedSpiritShot());
        itemHandler.registerHandler(new BlessingScrolls());
        itemHandler.registerHandler(new Book());
        itemHandler.registerHandler(new Bypass());
        itemHandler.registerHandler(new Calculator());
        itemHandler.registerHandler(new ChallengePointsCoupon());
        itemHandler.registerHandler(new ChangeAttributeCrystal());
        itemHandler.registerHandler(new CharmOfCourage());
        itemHandler.registerHandler(new Elixir());
        itemHandler.registerHandler(new EnchantAttribute());
        itemHandler.registerHandler(new EnchantScrolls());
        itemHandler.registerHandler(new ExtractableItems());
        itemHandler.registerHandler(new FatedSupportBox());
        itemHandler.registerHandler(new FishShots());
        itemHandler.registerHandler(new Harvester());
        itemHandler.registerHandler(new ItemSkills());
        itemHandler.registerHandler(new ItemSkillsTemplate());
        itemHandler.registerHandler(new LimitedSayha());
        itemHandler.registerHandler(new Maps());
        itemHandler.registerHandler(new MercTicket());
        itemHandler.registerHandler(new NicknameColor());
        itemHandler.registerHandler(new PetFood());
        itemHandler.registerHandler(new Recipes());
        itemHandler.registerHandler(new RollingDice());
        itemHandler.registerHandler(new Seed());
        itemHandler.registerHandler(new SoulShots());
        itemHandler.registerHandler(new SpecialXMas());
        itemHandler.registerHandler(new SpiritShot());
        itemHandler.registerHandler(new SummonItems());

        // Player action handlers
        PlayerActionHandler playerActionHandler = PlayerActionHandler.getInstance();
        playerActionHandler.registerHandler(new AirshipAction());
        playerActionHandler.registerHandler(new BotReport());
        playerActionHandler.registerHandler(new InstanceZoneInfo());
        playerActionHandler.registerHandler(new PetAttack());
        playerActionHandler.registerHandler(new PetHold());
        playerActionHandler.registerHandler(new PetMove());
        playerActionHandler.registerHandler(new PetSkillUse());
        playerActionHandler.registerHandler(new PetStop());
        playerActionHandler.registerHandler(new PrivateStore());
        playerActionHandler.registerHandler(new Ride());
        playerActionHandler.registerHandler(new RunWalk());
        playerActionHandler.registerHandler(new ServitorAttack());
        playerActionHandler.registerHandler(new ServitorHold());
        playerActionHandler.registerHandler(new ServitorMode());
        playerActionHandler.registerHandler(new ServitorMove());
        playerActionHandler.registerHandler(new ServitorSkillUse());
        playerActionHandler.registerHandler(new ServitorStop());
        playerActionHandler.registerHandler(new SitStand());
        playerActionHandler.registerHandler(new SocialAction());
        playerActionHandler.registerHandler(new TacticalSignTarget());
        playerActionHandler.registerHandler(new TacticalSignUse());
        playerActionHandler.registerHandler(new TeleportBookmark());
        playerActionHandler.registerHandler(new UnsummonPet());
        playerActionHandler.registerHandler(new UnsummonServitor());

        // SkillConditionHandler
        SkillConditionHandler skillConditionHandler = SkillConditionHandler.getInstance();
        skillConditionHandler.registerHandler("AssassinationPoints",
            stat => new AssassinationPointsSkillCondition(stat));

        skillConditionHandler.registerHandler("BeastPoints", stat => new BeastPointsSkillCondition(stat));
        skillConditionHandler.registerHandler("BuildAdvanceBase", stat => new BuildAdvanceBaseSkillCondition());
        skillConditionHandler.registerHandler("BuildCamp", stat => new BuildCampSkillCondition());
        skillConditionHandler.registerHandler("CanAddMaxEntranceInzone",
            stat => new CanAddMaxEntranceInzoneSkillCondition());

        skillConditionHandler.registerHandler("CanBookmarkAddSlot", stat => new CanBookmarkAddSlotSkillCondition(stat));
        skillConditionHandler.registerHandler("CanChangeVitalItemCount",
            stat => new CanChangeVitalItemCountSkillCondition());

        skillConditionHandler.registerHandler("CanEnchantAttribute",
            stat => new CanEnchantAttributeSkillCondition());

        skillConditionHandler.registerHandler("CanMountForEvent", stat => new CanMountForEventSkillCondition());
        skillConditionHandler.registerHandler("CannotUseInTransform",
            stat => new CannotUseInTransformSkillCondition(stat));

        skillConditionHandler.registerHandler("CanRefuelAirship", stat => new CanRefuelAirshipSkillCondition(stat));
        skillConditionHandler.registerHandler("CanSummon", stat => new CanSummonSkillCondition());
        skillConditionHandler.registerHandler("CanSummonCubic", stat => new CanSummonCubicSkillCondition());
        skillConditionHandler.registerHandler("CanSummonMulti", stat => new CanSummonMultiSkillCondition(stat));
        skillConditionHandler.registerHandler("CanSummonPet", stat => new CanSummonPetSkillCondition());
        skillConditionHandler.registerHandler("CanSummonSiegeGolem",
            stat => new CanSummonSiegeGolemSkillCondition());

        skillConditionHandler.registerHandler("CanTakeFort", stat => new CanTakeFortSkillCondition());
        skillConditionHandler.registerHandler("CanTransform", stat => new CanTransformSkillCondition(stat));
        skillConditionHandler.registerHandler("CanTransformInDominion",
            stat => new CanTransformInDominionSkillCondition());

        skillConditionHandler.registerHandler("CanUntransform", stat => new CanUntransformSkillCondition());
        skillConditionHandler.registerHandler("CanUseInBattlefield",
            stat => new CanUseInBattlefieldSkillCondition());

        skillConditionHandler.registerHandler("CanUseInDragonLair", stat => new CanUseInDragonLairSkillCondition());
        skillConditionHandler.registerHandler("CanUseSwoopCannon", stat => new CanUseSwoopCannonSkillCondition());
        skillConditionHandler.registerHandler("HasVitalityPoints", stat => new HasVitalityPointsSkillCondition(stat));
        skillConditionHandler.registerHandler("CanUseVitalityIncreaseItem",
            stat => new CanUseVitalityIncreaseItemSkillCondition(stat));

        skillConditionHandler.registerHandler("CheckLevel", stat => new CheckLevelSkillCondition(stat));
        skillConditionHandler.registerHandler("CheckSex", stat => new CheckSexSkillCondition(stat));
        skillConditionHandler.registerHandler("ConsumeBody", stat => new ConsumeBodySkillCondition());
        skillConditionHandler.registerHandler("DeathPoints", stat => new DeathPointsSkillCondition(stat));
        skillConditionHandler.registerHandler("EnergySaved", stat => new EnergySavedSkillCondition(stat));
        skillConditionHandler.registerHandler("EquipArmor", stat => new EquipArmorSkillCondition(stat));
        skillConditionHandler.registerHandler("EquippedCloakEnchant",
            stat => new EquippedCloakEnchantSkillCondition(stat));

        skillConditionHandler.registerHandler("EquipShield", stat => new EquipShieldSkillCondition());
        skillConditionHandler.registerHandler("EquipSigil", stat => new EquipSigilSkillCondition());
        skillConditionHandler.registerHandler("EquipWeapon", stat => new EquipWeaponSkillCondition(stat));
        skillConditionHandler.registerHandler("MaxMpSkillCondition", stat => new MaxMpSkillCondition(stat));
        skillConditionHandler.registerHandler("NotFeared", stat => new NotFearedSkillCondition());
        skillConditionHandler.registerHandler("NotInUnderwater", stat => new NotInUnderwaterSkillCondition());
        skillConditionHandler.registerHandler("Op2hWeapon", stat => new Op2hWeaponSkillCondition(stat));
        skillConditionHandler.registerHandler("OpAffectedBySkill", stat => new OpAffectedBySkillSkillCondition(stat));
        skillConditionHandler.registerHandler("OpAgathionEnergy", stat => new OpAgathionEnergySkillCondition());
        skillConditionHandler.registerHandler("OpAlignment", stat => new OpAlignmentSkillCondition(stat));
        skillConditionHandler.registerHandler("OpBaseStat", stat => new OpBaseStatSkillCondition(stat));
        skillConditionHandler.registerHandler("OpBlink", stat => new OpBlinkSkillCondition(stat));
        skillConditionHandler.registerHandler("OpCallPc", stat => new OpCallPcSkillCondition());
        skillConditionHandler.registerHandler("OpCanEscape", stat => new OpCanEscapeSkillCondition());
        skillConditionHandler.registerHandler("OpCanNotUseAirship", stat => new OpCanNotUseAirshipSkillCondition());
        skillConditionHandler.registerHandler("OpCannotUseTargetWithPrivateStore",
            stat => new OpCannotUseTargetWithPrivateStoreSkillCondition());

        skillConditionHandler.registerHandler("OpChangeWeapon", stat => new OpChangeWeaponSkillCondition());
        skillConditionHandler.registerHandler("OpCheckAbnormal", stat => new OpCheckAbnormalSkillCondition(stat));
        skillConditionHandler.registerHandler("OpCheckAccountType", stat => new OpCheckAccountTypeSkillCondition());
        skillConditionHandler.registerHandler("OpCheckCastRange", stat => new OpCheckCastRangeSkillCondition(stat));
        skillConditionHandler.registerHandler("OpCheckClass", stat => new OpCheckClassSkillCondition(stat));
        skillConditionHandler.registerHandler("OpCheckClassList", stat => new OpCheckClassListSkillCondition(stat));
        skillConditionHandler.registerHandler("OpCheckCrtEffect", stat => new OpCheckCrtEffectSkillCondition());
        skillConditionHandler.registerHandler("OpCheckFlag", stat => new OpCheckFlagSkillCondition());
        skillConditionHandler.registerHandler("OpCheckOnGoingEventCampaign",
            stat => new OpCheckOnGoingEventCampaignSkillCondition());

        skillConditionHandler.registerHandler("OpCheckPcbangPoint", stat => new OpCheckPcbangPointSkillCondition());
        skillConditionHandler.registerHandler("OpCheckResidence", stat => new OpCheckResidenceSkillCondition(stat));
        skillConditionHandler.registerHandler("OpCheckSkill", stat => new OpCheckSkillSkillCondition(stat));
        skillConditionHandler.registerHandler("OpCheckSkillList", stat => new OpCheckSkillListSkillCondition(stat));
        skillConditionHandler.registerHandler("OpCompanion", stat => new OpCompanionSkillCondition(stat));
        skillConditionHandler.registerHandler("OpEnchantRange", stat => new OpEnchantRangeSkillCondition());
        skillConditionHandler.registerHandler("OpEncumbered", stat => new OpEncumberedSkillCondition(stat));
        skillConditionHandler.registerHandler("OpEnergyMax", stat => new OpEnergyMaxSkillCondition(stat));
        skillConditionHandler.registerHandler("OpEquipItem", stat => new OpEquipItemSkillCondition(stat));
        skillConditionHandler.registerHandler("OpExistNpc", stat => new OpExistNpcSkillCondition(stat));
        skillConditionHandler.registerHandler("OpFishingCast", stat => new OpFishingCastSkillCondition());
        skillConditionHandler.registerHandler("OpFishingPumping", stat => new OpFishingPumpingSkillCondition());
        skillConditionHandler.registerHandler("OpFishingReeling", stat => new OpFishingReelingSkillCondition());
        skillConditionHandler.registerHandler("OpHaveSummon", stat => new OpHaveSummonSkillCondition());
        skillConditionHandler.registerHandler("OpHaveSummonedNpc", stat => new OpHaveSummonedNpcSkillCondition(stat));
        skillConditionHandler.registerHandler("OpHome", stat => new OpHomeSkillCondition(stat));
        skillConditionHandler.registerHandler("OpInSiege", stat => new OpInSiegeSkillCondition(stat));
        skillConditionHandler.registerHandler("OpInstantzone", stat => new OpInstantzoneSkillCondition(stat));
        skillConditionHandler.registerHandler("OpMainjob", stat => new OpMainjobSkillCondition());
        skillConditionHandler.registerHandler("OpNeedAgathion", stat => new OpNeedAgathionSkillCondition());
        skillConditionHandler.registerHandler("OpNeedSummonOrPet", stat => new OpNeedSummonOrPetSkillCondition(stat));
        skillConditionHandler.registerHandler("OpNotAffectedBySkill",
            stat => new OpNotAffectedBySkillSkillCondition(stat));

        skillConditionHandler.registerHandler("OpNotCursed", stat => new OpNotCursedSkillCondition());
        skillConditionHandler.registerHandler("OpNotInPeacezone", stat => new OpNotInPeacezoneSkillCondition());
        skillConditionHandler.registerHandler("OpNotInstantzone", stat => new OpNotInstantzoneSkillCondition());
        skillConditionHandler.registerHandler("OpNotOlympiad", stat => new OpNotOlympiadSkillCondition());
        skillConditionHandler.registerHandler("OpNotTerritory", stat => new OpNotTerritorySkillCondition());
        skillConditionHandler.registerHandler("OpOlympiad", stat => new OpOlympiadSkillCondition());
        skillConditionHandler.registerHandler("OpPeacezone", stat => new OpPeacezoneSkillCondition());
        skillConditionHandler.registerHandler("OpPkcount", stat => new OpPkcountSkillCondition(stat));
        skillConditionHandler.registerHandler("OpPledge", stat => new OpPledgeSkillCondition(stat));
        skillConditionHandler.registerHandler("OpRestartPoint", stat => new OpRestartPointSkillCondition());
        skillConditionHandler.registerHandler("OpResurrection", stat => new OpResurrectionSkillCondition());
        skillConditionHandler.registerHandler("OpSiegeHammer", stat => new OpSiegeHammerSkillCondition());
        skillConditionHandler.registerHandler("OpSkill", stat => new OpSkillSkillCondition(stat));
        skillConditionHandler.registerHandler("OpSkillAcquire", stat => new OpSkillAcquireSkillCondition(stat));
        skillConditionHandler.registerHandler("OpSocialClass", stat => new OpSocialClassSkillCondition(stat));
        skillConditionHandler.registerHandler("OpSoulMax", stat => new OpSoulMaxSkillCondition(stat));
        skillConditionHandler.registerHandler("OpStrider", stat => new OpStriderSkillCondition());
        skillConditionHandler.registerHandler("OpSubjob", stat => new OpSubjobSkillCondition());
        skillConditionHandler.registerHandler("OpSweeper", stat => new OpSweeperSkillCondition());
        skillConditionHandler.registerHandler("OpTargetAllItemType",
            stat => new OpTargetAllItemTypeSkillCondition());

        skillConditionHandler.registerHandler("OpTargetArmorType", stat => new OpTargetArmorTypeSkillCondition(stat));
        skillConditionHandler.registerHandler("OpTargetDoor", stat => new OpTargetDoorSkillCondition(stat));
        skillConditionHandler.registerHandler("OpTargetMyPledgeAcademy",
            stat => new OpTargetMyPledgeAcademySkillCondition());

        skillConditionHandler.registerHandler("OpTargetNpc", stat => new OpTargetNpcSkillCondition(stat));
        skillConditionHandler.registerHandler("OpTargetPc", stat => new OpTargetPcSkillCondition());
        skillConditionHandler.registerHandler("OpTargetWeaponAttackType",
            stat => new OpTargetWeaponAttackTypeSkillCondition(stat));

        skillConditionHandler.registerHandler("OpTerritory", stat => new OpTerritorySkillCondition());
        skillConditionHandler.registerHandler("OpUnlock", stat => new OpUnlockSkillCondition());
        skillConditionHandler.registerHandler("OpUseFirecracker", stat => new OpUseFirecrackerSkillCondition());
        skillConditionHandler.registerHandler("OpUsePraseed", stat => new OpUsePraseedSkillCondition());
        skillConditionHandler.registerHandler("OpWyvern", stat => new OpWyvernSkillCondition());
        skillConditionHandler.registerHandler("PossessHolything", stat => new PossessHolythingSkillCondition());
        skillConditionHandler.registerHandler("RemainCpPer", stat => new RemainCpPerSkillCondition(stat));
        skillConditionHandler.registerHandler("RemainHpPer", stat => new RemainHpPerSkillCondition(stat));
        skillConditionHandler.registerHandler("RemainMpPer", stat => new RemainMpPerSkillCondition(stat));
        skillConditionHandler.registerHandler("SoulSaved", stat => new SoulSavedSkillCondition(stat));
        skillConditionHandler.registerHandler("TargetAffectedBySkill",
            stat => new TargetAffectedBySkillSkillCondition(stat));

        skillConditionHandler.registerHandler("TargetItemCrystalType",
            stat => new TargetItemCrystalTypeSkillCondition());

        skillConditionHandler.registerHandler("TargetMyMentee", stat => new TargetMyMenteeSkillCondition());
        skillConditionHandler.registerHandler("TargetMyParty", stat => new TargetMyPartySkillCondition(stat));
        skillConditionHandler.registerHandler("TargetMyPledge", stat => new TargetMyPledgeSkillCondition());
        skillConditionHandler.registerHandler("TargetNotAffectedBySkill",
            stat => new TargetNotAffectedBySkillSkillCondition(stat));

        skillConditionHandler.registerHandler("TargetRace", stat => new TargetRaceSkillCondition(stat));

        // Target handlers
        TargetHandler targetHandler = TargetHandler.getInstance();
        targetHandler.registerHandler(new AdvanceBase());
        targetHandler.registerHandler(new Artillery());
        targetHandler.registerHandler(new DoorTreasure());
        targetHandler.registerHandler(new Enemy());
        targetHandler.registerHandler(new EnemyNot());
        targetHandler.registerHandler(new EnemyOnly());
        targetHandler.registerHandler(new FortressFlagpole());
        targetHandler.registerHandler(new Ground());
        targetHandler.registerHandler(new HolyThing());
        targetHandler.registerHandler(new Item());
        targetHandler.registerHandler(new MyMentor());
        targetHandler.registerHandler(new MyParty());
        targetHandler.registerHandler(new None());
        targetHandler.registerHandler(new NpcBody());
        targetHandler.registerHandler(new Others());
        targetHandler.registerHandler(new OwnerPet());
        targetHandler.registerHandler(new PcBody());
        targetHandler.registerHandler(new Pet());
        targetHandler.registerHandler(new Self());
        targetHandler.registerHandler(new Handlers.TargetHandlers.Summon());
        targetHandler.registerHandler(new Target());
        targetHandler.registerHandler(new WyvernTarget());

        // User command handlers
        UserCommandHandler userCommandHandler = UserCommandHandler.getInstance();
        userCommandHandler.registerHandler(new ClanPenalty());
        userCommandHandler.registerHandler(new ClanWarsList());
        userCommandHandler.registerHandler(new Dismount());
        userCommandHandler.registerHandler(new Unstuck());
        userCommandHandler.registerHandler(new InstanceZone());
        userCommandHandler.registerHandler(new Loc());
        userCommandHandler.registerHandler(new Mount());
        userCommandHandler.registerHandler(new PartyInfo());
        userCommandHandler.registerHandler(new Time());
        userCommandHandler.registerHandler(new OlympiadStat());
        userCommandHandler.registerHandler(new ChannelLeave());
        userCommandHandler.registerHandler(new ChannelDelete());
        userCommandHandler.registerHandler(new ChannelInfo());
        userCommandHandler.registerHandler(new MyBirthday());
        userCommandHandler.registerHandler(new SiegeStatus());

        // Voiced command handlers
        VoicedCommandHandler voicedCommandHandler = VoicedCommandHandler.getInstance();
        voicedCommandHandler.registerHandler(new ExperienceGain());

        if (Config.Banking.BANKING_SYSTEM_ENABLED)
            voicedCommandHandler.registerHandler(new Banking());

        if (Config.ChatModeration.CHAT_ADMIN)
            voicedCommandHandler.registerHandler(new ChatAdmin());

        if (Config.MultilingualSupport.MULTILANG_ENABLE && Config.MultilingualSupport.MULTILANG_VOICED_ALLOW)
            voicedCommandHandler.registerHandler(new Lang());

        if (Config.PasswordChange.ALLOW_CHANGE_PASSWORD)
            voicedCommandHandler.registerHandler(new ChangePassword());

        if (Config.OfflinePlay.ENABLE_OFFLINE_PLAY_COMMAND)
            voicedCommandHandler.registerHandler(new OfflinePlay());

        if (Config.OfflineTrade.ENABLE_OFFLINE_COMMAND &&
            (Config.OfflineTrade.OFFLINE_TRADE_ENABLE || Config.OfflineTrade.OFFLINE_CRAFT_ENABLE))
            voicedCommandHandler.registerHandler(new Offline());

        if (Config.OnlineInfo.ENABLE_ONLINE_COMMAND)
            voicedCommandHandler.registerHandler(new Online());

        if (Config.PremiumSystem.PREMIUM_SYSTEM_ENABLED)
            voicedCommandHandler.registerHandler(new Premium());

        if (Config.AutoPotions.AUTO_POTIONS_ENABLED)
            voicedCommandHandler.registerHandler(new AutoPotion());

        // TODO: Add configuration options for this voiced commands:
        voicedCommandHandler.registerHandler(new CastleVCmd());
        voicedCommandHandler.registerHandler(new SetVCmd());

        // Punishment handlers
        PunishmentHandler punishmentHandler = PunishmentHandler.getInstance();
        punishmentHandler.registerHandler(new BanHandler());
        punishmentHandler.registerHandler(new ChatBanHandler());
        punishmentHandler.registerHandler(new JailHandler());

        // Daily mission handlers
        DailyMissionHandler dailyMissionHandler = DailyMissionHandler.getInstance();
        dailyMissionHandler.registerHandler("level", h => new LevelDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("loginweekend", h => new LoginWeekendDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("loginmonth", h => new LoginMonthDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("quest", h => new QuestDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("olympiad", h => new OlympiadDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("siege", h => new SiegeDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("boss", h => new BossDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("monster", h => new MonsterDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("fishing", h => new FishingDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("spirit", h => new SpiritDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("joinclan", h => new JoinClanDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("purge", h => new PurgeRewardDailyMissionHandler(h));
        dailyMissionHandler.registerHandler("useitem", h => new UseItemDailyMissionHandler(h));

        // Admin command handler
        AdminCommandHandler adminCommandHandler = AdminCommandHandler.getInstance();
        adminCommandHandler.registerHandler(new AdminAdmin());
        adminCommandHandler.registerHandler(new AdminAnnouncements());
        adminCommandHandler.registerHandler(new AdminBBS());
        adminCommandHandler.registerHandler(new AdminBuffs());
        adminCommandHandler.registerHandler(new AdminCamera());
        adminCommandHandler.registerHandler(new AdminChangeAccessLevel());
        adminCommandHandler.registerHandler(new AdminClan());
        adminCommandHandler.registerHandler(new AdminClanHall());
        adminCommandHandler.registerHandler(new AdminCastle());
        adminCommandHandler.registerHandler(new AdminPcCondOverride());
        adminCommandHandler.registerHandler(new AdminCreateItem());
        adminCommandHandler.registerHandler(new AdminCursedWeapons());
        adminCommandHandler.registerHandler(new AdminDelete());
        adminCommandHandler.registerHandler(new AdminDestroyItems());
        adminCommandHandler.registerHandler(new AdminDisconnect());
        adminCommandHandler.registerHandler(new AdminDoorControl());
        adminCommandHandler.registerHandler(new AdminEditChar());
        adminCommandHandler.registerHandler(new AdminEffects());
        adminCommandHandler.registerHandler(new AdminElement());
        adminCommandHandler.registerHandler(new AdminEnchant());
        adminCommandHandler.registerHandler(new AdminEvents());
        adminCommandHandler.registerHandler(new AdminExpSp());
        adminCommandHandler.registerHandler(new AdminFakePlayers());
        adminCommandHandler.registerHandler(new AdminFence());
        adminCommandHandler.registerHandler(new AdminFightCalculator());
        adminCommandHandler.registerHandler(new AdminFortSiege());
        adminCommandHandler.registerHandler(new AdminGeodata());
        adminCommandHandler.registerHandler(new AdminGm());
        adminCommandHandler.registerHandler(new AdminGmChat());
        adminCommandHandler.registerHandler(new AdminGmSpeed());
        adminCommandHandler.registerHandler(new AdminGraciaSeeds());
        adminCommandHandler.registerHandler(new AdminGrandBoss());
        adminCommandHandler.registerHandler(new AdminHeal());
        adminCommandHandler.registerHandler(new AdminHide());
        adminCommandHandler.registerHandler(new AdminHtml());
        adminCommandHandler.registerHandler(new AdminInstance());
        adminCommandHandler.registerHandler(new AdminInstanceZone());
        adminCommandHandler.registerHandler(new AdminInvul());
        adminCommandHandler.registerHandler(new AdminKick());
        adminCommandHandler.registerHandler(new AdminKill());
        adminCommandHandler.registerHandler(new AdminLevel());
        //adminCommandHandler.registerHandler(new AdminLogin());
        adminCommandHandler.registerHandler(new AdminManor());
        adminCommandHandler.registerHandler(new AdminMenu());
        adminCommandHandler.registerHandler(new AdminMessages());
        adminCommandHandler.registerHandler(new AdminMissingHtmls());
        adminCommandHandler.registerHandler(new AdminMobGroup());
        adminCommandHandler.registerHandler(new AdminOlympiad());
        adminCommandHandler.registerHandler(new AdminOnline());
        adminCommandHandler.registerHandler(new AdminPathNode());
        adminCommandHandler.registerHandler(new AdminPcCafePoints());
        adminCommandHandler.registerHandler(new AdminPetition());
        //adminCommandHandler.registerHandler(new AdminPForge());
        adminCommandHandler.registerHandler(new AdminPledge());
        adminCommandHandler.registerHandler(new AdminZones());
        //adminCommandHandler.registerHandler(new AdminPremium());
        adminCommandHandler.registerHandler(new AdminPrimePoints());
        adminCommandHandler.registerHandler(new AdminPunishment());
        //adminCommandHandler.registerHandler(new AdminQuest());
        adminCommandHandler.registerHandler(new AdminReload());
        adminCommandHandler.registerHandler(new AdminRepairChar());
        adminCommandHandler.registerHandler(new AdminRes());
        adminCommandHandler.registerHandler(new AdminRide());
        adminCommandHandler.registerHandler(new AdminScan());
        adminCommandHandler.registerHandler(new AdminServerInfo());
        adminCommandHandler.registerHandler(new AdminShop());
        adminCommandHandler.registerHandler(new AdminShowQuests());
        adminCommandHandler.registerHandler(new AdminShutdown());
        adminCommandHandler.registerHandler(new AdminSkill());
        adminCommandHandler.registerHandler(new AdminSpawn());
        adminCommandHandler.registerHandler(new AdminSummon());
        adminCommandHandler.registerHandler(new AdminSuperHaste());
        adminCommandHandler.registerHandler(new AdminTarget());
        adminCommandHandler.registerHandler(new AdminTargetSay());
        adminCommandHandler.registerHandler(new AdminTeleport());
        adminCommandHandler.registerHandler(new AdminTest());
        adminCommandHandler.registerHandler(new AdminTransform());
        adminCommandHandler.registerHandler(new AdminVitality());
        adminCommandHandler.registerHandler(new AdminZone());

        // Community board
        CommunityBoardHandler communityBoardHandler = CommunityBoardHandler.getInstance();
        communityBoardHandler.registerHandler(new ClanBoard());
        communityBoardHandler.registerHandler(new FavoriteBoard());
        communityBoardHandler.registerHandler(new FriendsBoard());
        communityBoardHandler.registerHandler(new HomeBoard());
        communityBoardHandler.registerHandler(new HomepageBoard());
        communityBoardHandler.registerHandler(new MailBoard());
        communityBoardHandler.registerHandler(new MemoBoard());
        communityBoardHandler.registerHandler(new RegionBoard());
        communityBoardHandler.registerHandler(new DropSearchBoard());
    }

    public static void RegisterQuests()
    {
        // Quests
        QuestManager questManager = QuestManager.getInstance();
        questManager.addQuest(new Q00206Tutorial());
    }

    public static void RegisterScripts()
    {
        ScriptManager.AddScript(new PlayerClassChange());
        ScriptManager.AddScript(new TeleportToRaceTrack());
    }
}