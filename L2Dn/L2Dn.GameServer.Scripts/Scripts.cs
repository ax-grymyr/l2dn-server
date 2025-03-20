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
        AbstractEffectFactory abstractEffectFactory = AbstractEffectFactory.getInstance();
        abstractEffectFactory.registerHandler("AbnormalShield", stat => new AbnormalShield(stat));
        abstractEffectFactory.registerHandler("AbnormalTimeChange", stat => new AbnormalTimeChange(stat));
        abstractEffectFactory.registerHandler("AbnormalTimeChangeBySkillId", stat => new AbnormalTimeChangeBySkillId(stat));
        abstractEffectFactory.registerHandler("AbsorbDamage", stat => new AbsorbDamage(stat));
        abstractEffectFactory.registerHandler("Accuracy", stat => new Accuracy(stat));
        abstractEffectFactory.registerHandler("AddHate", stat => new AddHate(stat));
        abstractEffectFactory.registerHandler("AddHuntingTime", stat => new AddHuntingTime(stat));
        abstractEffectFactory.registerHandler("AdditionalPhysicalAttack", stat => new AdditionalPhysicalAttack(stat));
        abstractEffectFactory.registerHandler("AdditionalPotionCp", stat => new AdditionalPotionCp(stat));
        abstractEffectFactory.registerHandler("AdditionalPotionHp", stat => new AdditionalPotionHp(stat));
        abstractEffectFactory.registerHandler("AdditionalPotionMp", stat => new AdditionalPotionMp(stat));
        abstractEffectFactory.registerHandler("AddPcCafePoints", stat => new AddPcCafePoints(stat));
        abstractEffectFactory.registerHandler("AddMaxPhysicalCriticalRate", stat => new AddMaxPhysicalCriticalRate(stat));
        abstractEffectFactory.registerHandler("AddMaxMagicCriticalRate", stat => new AddMaxMagicCriticalRate(stat));
        abstractEffectFactory.registerHandler("AddSkillBySkill", stat => new AddSkillBySkill(stat));
        abstractEffectFactory.registerHandler("AddTeleportBookmarkSlot", stat => new AddTeleportBookmarkSlot(stat));
        abstractEffectFactory.registerHandler("AgathionSlot", stat => new AgathionSlot(stat));
        abstractEffectFactory.registerHandler("AreaDamage", stat => new AreaDamage(stat));
        abstractEffectFactory.registerHandler("AreaOfEffectDamageDefence", stat => new AreaOfEffectDamageDefence(stat));
        abstractEffectFactory.registerHandler("AreaOfEffectDamageModify", stat => new AreaOfEffectDamageModify(stat));
        abstractEffectFactory.registerHandler("ArtifactSlot", stat => new ArtifactSlot(stat));
        abstractEffectFactory.registerHandler("AttackAttribute", stat => new AttackAttribute(stat));
        abstractEffectFactory.registerHandler("AttackAttributeAdd", stat => new AttackAttributeAdd(stat));
        abstractEffectFactory.registerHandler("AttackBehind", stat => new AttackBehind());
        abstractEffectFactory.registerHandler("AttackTrait", stat => new AttackTrait(stat));
        abstractEffectFactory.registerHandler("AutoAttackDamageBonus", stat => new AutoAttackDamageBonus(stat));
        abstractEffectFactory.registerHandler("Backstab", stat => new Backstab(stat));
        abstractEffectFactory.registerHandler("Betray", stat => new Betray());
        abstractEffectFactory.registerHandler("Blink", stat => new Blink(stat));
        abstractEffectFactory.registerHandler("BlinkSwap", stat => new BlinkSwap());
        abstractEffectFactory.registerHandler("BlockAbnormalSlot", stat => new BlockAbnormalSlot(stat));
        abstractEffectFactory.registerHandler("BlockAction", stat => new BlockAction(stat));
        abstractEffectFactory.registerHandler("BlockActions", stat => new BlockActions(stat));
        abstractEffectFactory.registerHandler("BlockChat", stat => new BlockChat());
        abstractEffectFactory.registerHandler("BlockControl", stat => new BlockControl());
        abstractEffectFactory.registerHandler("BlockEscape", stat => new BlockEscape());
        abstractEffectFactory.registerHandler("BlockMove", stat => new BlockMove());
        abstractEffectFactory.registerHandler("BlockParty", stat => new BlockParty());
        abstractEffectFactory.registerHandler("BlockResurrection", stat => new BlockResurrection());
        abstractEffectFactory.registerHandler("BlockSkill", stat => new BlockSkill(stat));
        abstractEffectFactory.registerHandler("BlockTarget", stat => new BlockTarget());
        abstractEffectFactory.registerHandler("Bluff", stat => new Bluff(stat));
        abstractEffectFactory.registerHandler("BonusDropAdena", stat => new BonusDropAdena(stat));
        abstractEffectFactory.registerHandler("BonusDropAmount", stat => new BonusDropAmount(stat));
        abstractEffectFactory.registerHandler("BonusDropRate", stat => new BonusDropRate(stat));
        abstractEffectFactory.registerHandler("BonusDropRateLCoin", stat => new BonusDropRateLCoin(stat));
        abstractEffectFactory.registerHandler("BonusRaidPoints", stat => new BonusRaidPoints(stat));
        abstractEffectFactory.registerHandler("BonusSpoilRate", stat => new BonusSpoilRate(stat));
        abstractEffectFactory.registerHandler("Breath", stat => new Breath(stat));
        abstractEffectFactory.registerHandler("BuffBlock", stat => new BuffBlock());
        abstractEffectFactory.registerHandler("CallLearnedSkill", stat => new CallLearnedSkill(stat));
        abstractEffectFactory.registerHandler("CallParty", stat => new CallParty());
        abstractEffectFactory.registerHandler("CallPc", stat => new CallPc(stat));
        abstractEffectFactory.registerHandler("CallRandomSkill", stat => new CallRandomSkill(stat));
        abstractEffectFactory.registerHandler("CallSkill", stat => new CallSkill(stat));
        abstractEffectFactory.registerHandler("CallSkillOnActionTime", stat => new CallSkillOnActionTime(stat));
        abstractEffectFactory.registerHandler("CallTargetParty", stat => new CallTargetParty());
        abstractEffectFactory.registerHandler("CheapShot", stat => new CheapShot());
        abstractEffectFactory.registerHandler("ChameleonRest", stat => new ChameleonRest(stat));
        abstractEffectFactory.registerHandler("ChangeBody", stat => new ChangeBody(stat));
        abstractEffectFactory.registerHandler("ChangeFace", stat => new ChangeFace(stat));
        abstractEffectFactory.registerHandler("ChangeFishingMastery", stat => new ChangeFishingMastery());
        abstractEffectFactory.registerHandler("ChangeHairColor", stat => new ChangeHairColor(stat));
        abstractEffectFactory.registerHandler("ChangeHairStyle", stat => new ChangeHairStyle(stat));
        abstractEffectFactory.registerHandler("ClassChange", stat => new ClassChange(stat));
        abstractEffectFactory.registerHandler("Compelling", stat => new Compelling());
        abstractEffectFactory.registerHandler("Confuse", stat => new Confuse(stat));
        abstractEffectFactory.registerHandler("ConsumeBody", stat => new ConsumeBody());
        abstractEffectFactory.registerHandler("ConvertItem", stat => new ConvertItem());
        abstractEffectFactory.registerHandler("CounterPhysicalSkill", stat => new CounterPhysicalSkill(stat));
        abstractEffectFactory.registerHandler("Cp", stat => new Cp(stat));
        abstractEffectFactory.registerHandler("CpHeal", stat => new CpHeal(stat));
        abstractEffectFactory.registerHandler("CpHealOverTime", stat => new CpHealOverTime(stat));
        abstractEffectFactory.registerHandler("CpHealPercent", stat => new CpHealPercent(stat));
        abstractEffectFactory.registerHandler("CpRegen", stat => new CpRegen(stat));
        abstractEffectFactory.registerHandler("CraftingCritical", stat => new CraftingCritical(stat));
        abstractEffectFactory.registerHandler("CraftRate", stat => new CraftRate(stat));
        abstractEffectFactory.registerHandler("CriticalDamage", stat => new CriticalDamage(stat));
        abstractEffectFactory.registerHandler("CriticalDamagePosition", stat => new CriticalDamagePosition(stat));
        abstractEffectFactory.registerHandler("CriticalRate", stat => new CriticalRate(stat));
        abstractEffectFactory.registerHandler("CriticalRatePositionBonus", stat => new CriticalRatePositionBonus(stat));
        abstractEffectFactory.registerHandler("CubicMastery", stat => new CubicMastery(stat));
        abstractEffectFactory.registerHandler("DamageBlock", stat => new DamageBlock(stat));
        abstractEffectFactory.registerHandler("DamageByAttack", stat => new DamageByAttack(stat));
        abstractEffectFactory.registerHandler("DamageShield", stat => new DamageShield(stat));
        abstractEffectFactory.registerHandler("DamageShieldResist", stat => new DamageShieldResist(stat));
        abstractEffectFactory.registerHandler("DamOverTime", stat => new DamOverTime(stat));
        abstractEffectFactory.registerHandler("DamOverTimePercent", stat => new DamOverTimePercent(stat));
        abstractEffectFactory.registerHandler("DeathLink", stat => new DeathLink(stat));
        abstractEffectFactory.registerHandler("DebuffBlock", stat => new DebuffBlock());
        abstractEffectFactory.registerHandler("DefenceAttribute", stat => new DefenceAttribute(stat));
        abstractEffectFactory.registerHandler("DefenceCriticalDamage", stat => new DefenceCriticalDamage(stat));
        abstractEffectFactory.registerHandler("DefenceCriticalRate", stat => new DefenceCriticalRate(stat));
        abstractEffectFactory.registerHandler("DefenceIgnoreRemoval", stat => new DefenceIgnoreRemoval(stat));
        abstractEffectFactory.registerHandler("DefenceMagicCriticalDamage", stat => new DefenceMagicCriticalDamage(stat));
        abstractEffectFactory.registerHandler("DefenceMagicCriticalRate", stat => new DefenceMagicCriticalRate(stat));
        abstractEffectFactory.registerHandler("DefencePhysicalSkillCriticalDamage",
            stat => new DefencePhysicalSkillCriticalDamage(stat));

        abstractEffectFactory.registerHandler("DefencePhysicalSkillCriticalRate",
            stat => new DefencePhysicalSkillCriticalRate(stat));

        abstractEffectFactory.registerHandler("DefenceTrait", stat => new DefenceTrait(stat));
        abstractEffectFactory.registerHandler("DeleteHate", stat => new DeleteHate(stat));
        abstractEffectFactory.registerHandler("DeleteHateOfMe", stat => new DeleteHateOfMe(stat));
        abstractEffectFactory.registerHandler("DeleteTopAgro", stat => new DeleteTopAgro(stat));
        abstractEffectFactory.registerHandler("DetectHiddenObjects", stat => new DetectHiddenObjects());
        abstractEffectFactory.registerHandler("Detection", stat => new Detection());
        abstractEffectFactory.registerHandler("DisableSkill", stat => new DisableSkill(stat));
        abstractEffectFactory.registerHandler("DisableTargeting", stat => new DisableTargeting());
        abstractEffectFactory.registerHandler("Disarm", stat => new Disarm());
        abstractEffectFactory.registerHandler("Disarmor", stat => new Disarmor(stat));
        abstractEffectFactory.registerHandler("DispelAll", stat => new DispelAll());
        abstractEffectFactory.registerHandler("DispelByCategory", stat => new DispelByCategory(stat));
        abstractEffectFactory.registerHandler("DispelBySlot", stat => new DispelBySlot(stat));
        abstractEffectFactory.registerHandler("DispelBySlotMyself", stat => new DispelBySlotMyself(stat));
        abstractEffectFactory.registerHandler("DispelBySlotProbability", stat => new DispelBySlotProbability(stat));
        abstractEffectFactory.registerHandler("DoubleCast", stat => new DoubleCast());
        abstractEffectFactory.registerHandler("DuelistFury", stat => new DuelistFury());
        abstractEffectFactory.registerHandler("ElementalSpiritAttack", stat => new ElementalSpiritAttack(stat));
        abstractEffectFactory.registerHandler("ElementalSpiritDefense", stat => new ElementalSpiritDefense(stat));
        abstractEffectFactory.registerHandler("ElixirUsageLimit", stat => new ElixirUsageLimit(stat));
        abstractEffectFactory.registerHandler("EnableCloak", stat => new EnableCloak());
        abstractEffectFactory.registerHandler("EnchantRate", stat => new EnchantRate(stat));
        abstractEffectFactory.registerHandler("EnergyAttack", stat => new EnergyAttack(stat));
        abstractEffectFactory.registerHandler("EnlargeAbnormalSlot", stat => new EnlargeAbnormalSlot(stat));
        abstractEffectFactory.registerHandler("EnlargeSlot", stat => new EnlargeSlot(stat));
        abstractEffectFactory.registerHandler("Escape", stat => new Escape(stat));
        abstractEffectFactory.registerHandler("ExpModify", stat => new ExpModify(stat));
        abstractEffectFactory.registerHandler("ExpModifyPet", stat => new ExpModifyPet(stat));
        abstractEffectFactory.registerHandler("Faceoff", stat => new Faceoff());
        abstractEffectFactory.registerHandler("FakeDeath", stat => new FakeDeath(stat));
        abstractEffectFactory.registerHandler("FatalBlow", stat => new FatalBlow(stat));
        abstractEffectFactory.registerHandler("FatalBlowRate", stat => new FatalBlowRate(stat));
        abstractEffectFactory.registerHandler("FatalBlowRateDefence", stat => new FatalBlowRateDefence(stat));
        abstractEffectFactory.registerHandler("Fear", stat => new Fear());
        abstractEffectFactory.registerHandler("Feed", stat => new Feed(stat));
        abstractEffectFactory.registerHandler("FeedModify", stat => new FeedModify(stat));
        abstractEffectFactory.registerHandler("FishingExpSpBonus", stat => new FishingExpSpBonus(stat));
        abstractEffectFactory.registerHandler("Flag", stat => new Flag());
        abstractEffectFactory.registerHandler("FocusEnergy", stat => new FocusEnergy(stat));
        abstractEffectFactory.registerHandler("FocusMomentum", stat => new FocusMomentum(stat));
        abstractEffectFactory.registerHandler("FocusMaxMomentum", stat => new FocusMaxMomentum());
        abstractEffectFactory.registerHandler("FocusSouls", stat => new FocusSouls(stat));
        abstractEffectFactory.registerHandler("GetAgro", stat => new GetAgro());
        abstractEffectFactory.registerHandler("GetDamageLimit", stat => new GetDamageLimit(stat));
        abstractEffectFactory.registerHandler("GetMomentum", stat => new GetMomentum(stat));
        abstractEffectFactory.registerHandler("GiveClanReputation", stat => new GiveClanReputation(stat));
        abstractEffectFactory.registerHandler("GiveExpAndSp", stat => new GiveExpAndSp(stat));
        abstractEffectFactory.registerHandler("GiveFame", stat => new GiveFame(stat));
        abstractEffectFactory.registerHandler("GiveHonorCoins", stat => new GiveHonorCoins(stat));
        abstractEffectFactory.registerHandler("GiveItemByExp", stat => new GiveItemByExp(stat));
        abstractEffectFactory.registerHandler("GivePetXp", stat => new GivePetXp(stat));
        abstractEffectFactory.registerHandler("GiveRecommendation", stat => new GiveRecommendation(stat));
        abstractEffectFactory.registerHandler("GiveSp", stat => new GiveSp(stat));
        abstractEffectFactory.registerHandler("GiveXp", stat => new GiveXp(stat));
        abstractEffectFactory.registerHandler("Grow", stat => new Grow());
        abstractEffectFactory.registerHandler("HairAccessorySet", stat => new HairAccessorySet());
        abstractEffectFactory.registerHandler("Harvesting", stat => new Harvesting());
        abstractEffectFactory.registerHandler("HateAttack", stat => new HateAttack(stat));
        abstractEffectFactory.registerHandler("HeadquarterCreate", stat => new HeadquarterCreate(stat));
        abstractEffectFactory.registerHandler("Heal", stat => new Heal(stat));
        abstractEffectFactory.registerHandler("HealEffect", stat => new HealEffect(stat));
        abstractEffectFactory.registerHandler("HealOverTime", stat => new HealOverTime(stat));
        abstractEffectFactory.registerHandler("HealPercent", stat => new HealPercent(stat));
        abstractEffectFactory.registerHandler("Hide", stat => new Hide());
        abstractEffectFactory.registerHandler("HitNumber", stat => new HitNumber(stat));
        abstractEffectFactory.registerHandler("Hp", stat => new Hp(stat));
        abstractEffectFactory.registerHandler("HpByLevel", stat => new HpByLevel(stat));
        abstractEffectFactory.registerHandler("HpCpHeal", stat => new HpCpHeal(stat));
        abstractEffectFactory.registerHandler("HpCpHealCritical", stat => new HpCpHealCritical());
        abstractEffectFactory.registerHandler("HpDrain", stat => new HpDrain(stat));
        abstractEffectFactory.registerHandler("HpLimit", stat => new HpLimit(stat));
        abstractEffectFactory.registerHandler("HpRegen", stat => new HpRegen(stat));
        abstractEffectFactory.registerHandler("HpToOwner", stat => new HpToOwner(stat));
        abstractEffectFactory.registerHandler("IgnoreDeath", stat => new IgnoreDeath());
        abstractEffectFactory.registerHandler("IgnoreReduceDamage", stat => new IgnoreReduceDamage(stat));
        abstractEffectFactory.registerHandler("ImmobileDamageBonus", stat => new ImmobileDamageBonus(stat));
        abstractEffectFactory.registerHandler("ImmobileDamageResist", stat => new ImmobileDamageResist(stat));
        abstractEffectFactory.registerHandler("ImmobilePetBuff", stat => new ImmobilePetBuff());
        abstractEffectFactory.registerHandler("InstantKillResist", stat => new InstantKillResist(stat));
        abstractEffectFactory.registerHandler("JewelSlot", stat => new JewelSlot(stat));
        abstractEffectFactory.registerHandler("KarmaCount", stat => new KarmaCount(stat));
        abstractEffectFactory.registerHandler("KnockBack", stat => new KnockBack(stat));
        abstractEffectFactory.registerHandler("Lethal", stat => new Lethal(stat));
        abstractEffectFactory.registerHandler("LimitCp", stat => new LimitCp(stat));
        abstractEffectFactory.registerHandler("LimitHp", stat => new LimitHp(stat));
        abstractEffectFactory.registerHandler("LimitMp", stat => new LimitMp(stat));
        abstractEffectFactory.registerHandler("Lucky", stat => new Lucky());
        abstractEffectFactory.registerHandler("MagicAccuracy", stat => new MagicAccuracy(stat));
        abstractEffectFactory.registerHandler("MagicalAbnormalDispelAttack", stat => new MagicalAbnormalDispelAttack(stat));
        abstractEffectFactory.registerHandler("MagicalAbnormalResist", stat => new MagicalAbnormalResist(stat));
        abstractEffectFactory.registerHandler("MagicalAttack", stat => new MagicalAttack(stat));
        abstractEffectFactory.registerHandler("MagicalAttackByAbnormal", stat => new MagicalAttackByAbnormal(stat));
        abstractEffectFactory.registerHandler("MagicalAttackByAbnormalSlot", stat => new MagicalAttackByAbnormalSlot(stat));
        abstractEffectFactory.registerHandler("MagicalAttackMp", stat => new MagicalAttackMp(stat));
        abstractEffectFactory.registerHandler("MagicalAttackRange", stat => new MagicalAttackRange(stat));
        abstractEffectFactory.registerHandler("MagicalAttackSpeed", stat => new MagicalAttackSpeed(stat));
        abstractEffectFactory.registerHandler("MagicalDamOverTime", stat => new MagicalDamOverTime(stat));
        abstractEffectFactory.registerHandler("MagicalDefence", stat => new MagicalDefence(stat));
        abstractEffectFactory.registerHandler("MagicalEvasion", stat => new MagicalEvasion(stat));
        abstractEffectFactory.registerHandler("MagicalSkillPower", stat => new MagicalSkillPower(stat));
        abstractEffectFactory.registerHandler("MagicalSoulAttack", stat => new MagicalSoulAttack(stat));
        abstractEffectFactory.registerHandler("MagicCriticalDamage", stat => new MagicCriticalDamage(stat));
        abstractEffectFactory.registerHandler("MagicCriticalRate", stat => new MagicCriticalRate(stat));
        abstractEffectFactory.registerHandler("MagicCriticalRateByCriticalRate",
            stat => new MagicCriticalRateByCriticalRate(stat));

        abstractEffectFactory.registerHandler("MagicLampExpRate", stat => new MagicLampExpRate(stat));
        abstractEffectFactory.registerHandler("MagicMpCost", stat => new MagicMpCost(stat));
        abstractEffectFactory.registerHandler("ManaCharge", stat => new ManaCharge(stat));
        abstractEffectFactory.registerHandler("ManaDamOverTime", stat => new ManaDamOverTime(stat));
        abstractEffectFactory.registerHandler("ManaHeal", stat => new ManaHeal(stat));
        abstractEffectFactory.registerHandler("ManaHealByLevel", stat => new ManaHealByLevel(stat));
        abstractEffectFactory.registerHandler("ManaHealOverTime", stat => new ManaHealOverTime(stat));
        abstractEffectFactory.registerHandler("ManaHealPercent", stat => new ManaHealPercent(stat));
        abstractEffectFactory.registerHandler("MAtk", stat => new MAtk(stat));
        abstractEffectFactory.registerHandler("MAtkByPAtk", stat => new MAtkByPAtk(stat));
        abstractEffectFactory.registerHandler("MaxCp", stat => new MaxCp(stat));
        abstractEffectFactory.registerHandler("MaxHp", stat => new MaxHp(stat));
        abstractEffectFactory.registerHandler("MaxMp", stat => new MaxMp(stat));
        abstractEffectFactory.registerHandler("ModifyAssassinationPoints", stat => new ModifyAssassinationPoints(stat));
        abstractEffectFactory.registerHandler("ModifyBeastPoints", stat => new ModifyBeastPoints(stat));
        abstractEffectFactory.registerHandler("ModifyCraftPoints", stat => new ModifyCraftPoints(stat));
        abstractEffectFactory.registerHandler("ModifyDeathPoints", stat => new ModifyDeathPoints(stat));
        abstractEffectFactory.registerHandler("ModifyMagicLampPoints", stat => new ModifyMagicLampPoints(stat));
        abstractEffectFactory.registerHandler("ModifyVital", stat => new ModifyVital(stat));
        abstractEffectFactory.registerHandler("Mp", stat => new Mp(stat));
        abstractEffectFactory.registerHandler("MpConsumePerLevel", stat => new MpConsumePerLevel(stat));
        abstractEffectFactory.registerHandler("MpRegen", stat => new MpRegen(stat));
        abstractEffectFactory.registerHandler("MpShield", stat => new MpShield(stat));
        abstractEffectFactory.registerHandler("MpVampiricAttack", stat => new MpVampiricAttack(stat));
        abstractEffectFactory.registerHandler("Mute", stat => new Mute());
        abstractEffectFactory.registerHandler("NewHennaSlot", stat => new NewHennaSlot(stat));
        abstractEffectFactory.registerHandler("NightStatModify", stat => new NightStatModify(stat));
        abstractEffectFactory.registerHandler("NoblesseBless", stat => new NoblesseBless());
        abstractEffectFactory.registerHandler("OpenChest", stat => new OpenChest());
        abstractEffectFactory.registerHandler("OpenCommonRecipeBook", stat => new OpenCommonRecipeBook());
        abstractEffectFactory.registerHandler("OpenDoor", stat => new OpenDoor(stat));
        abstractEffectFactory.registerHandler("OpenDwarfRecipeBook", stat => new OpenDwarfRecipeBook());
        abstractEffectFactory.registerHandler("Passive", stat => new Passive());
        abstractEffectFactory.registerHandler("PAtk", stat => new PAtk(stat));
        abstractEffectFactory.registerHandler("PhysicalAbnormalResist", stat => new PhysicalAbnormalResist(stat));
        abstractEffectFactory.registerHandler("PhysicalAttack", stat => new PhysicalAttack(stat));
        abstractEffectFactory.registerHandler("PhysicalAttackHpLink", stat => new PhysicalAttackHpLink(stat));
        abstractEffectFactory.registerHandler("PhysicalAttackMute", stat => new PhysicalAttackMute());
        abstractEffectFactory.registerHandler("PhysicalAttackRange", stat => new PhysicalAttackRange(stat));
        abstractEffectFactory.registerHandler("PhysicalAttackSaveHp", stat => new PhysicalAttackSaveHp(stat));
        abstractEffectFactory.registerHandler("PhysicalAttackSpeed", stat => new PhysicalAttackSpeed(stat));
        abstractEffectFactory.registerHandler("PhysicalAttackWeaponBonus", stat => new PhysicalAttackWeaponBonus(stat));
        abstractEffectFactory.registerHandler("PhysicalDefence", stat => new PhysicalDefence(stat));
        abstractEffectFactory.registerHandler("PhysicalEvasion", stat => new PhysicalEvasion(stat));
        abstractEffectFactory.registerHandler("PhysicalMute", stat => new PhysicalMute());
        abstractEffectFactory.registerHandler("PhysicalShieldAngleAll", stat => new PhysicalShieldAngleAll());
        abstractEffectFactory.registerHandler("PhysicalSkillCriticalDamage", stat => new PhysicalSkillCriticalDamage(stat));
        abstractEffectFactory.registerHandler("PhysicalSkillCriticalRate", stat => new PhysicalSkillCriticalRate(stat));
        abstractEffectFactory.registerHandler("PhysicalSkillPower", stat => new PhysicalSkillPower(stat));
        abstractEffectFactory.registerHandler("PhysicalSoulAttack", stat => new PhysicalSoulAttack(stat));
        abstractEffectFactory.registerHandler("PkCount", stat => new PkCount(stat));
        abstractEffectFactory.registerHandler("Plunder", stat => new Plunder());
        abstractEffectFactory.registerHandler("PolearmSingleTarget", stat => new PolearmSingleTarget());
        abstractEffectFactory.registerHandler("ProtectionBlessing", stat => new ProtectionBlessing());
        abstractEffectFactory.registerHandler("ProtectDeathPenalty", stat => new ProtectDeathPenalty());
        abstractEffectFactory.registerHandler("PullBack", stat => new PullBack(stat));
        abstractEffectFactory.registerHandler("PveMagicalSkillDamageBonus", stat => new PveMagicalSkillDamageBonus(stat));
        abstractEffectFactory.registerHandler("PveMagicalSkillDefenceBonus", stat => new PveMagicalSkillDefenceBonus(stat));
        abstractEffectFactory.registerHandler("PvePhysicalAttackDamageBonus", stat => new PvePhysicalAttackDamageBonus(stat));
        abstractEffectFactory.registerHandler("PvePhysicalAttackDefenceBonus", stat => new PvePhysicalAttackDefenceBonus(stat));
        abstractEffectFactory.registerHandler("PvePhysicalSkillDamageBonus", stat => new PvePhysicalSkillDamageBonus(stat));
        abstractEffectFactory.registerHandler("PvePhysicalSkillDefenceBonus", stat => new PvePhysicalSkillDefenceBonus(stat));
        abstractEffectFactory.registerHandler("PveRaidMagicalSkillDamageBonus",
            stat => new PveRaidMagicalSkillDamageBonus(stat));

        abstractEffectFactory.registerHandler("PveRaidMagicalSkillDefenceBonus",
            stat => new PveRaidMagicalSkillDefenceBonus(stat));

        abstractEffectFactory.registerHandler("PveRaidPhysicalAttackDamageBonus",
            stat => new PveRaidPhysicalAttackDamageBonus(stat));

        abstractEffectFactory.registerHandler("PveRaidPhysicalAttackDefenceBonus",
            stat => new PveRaidPhysicalAttackDefenceBonus(stat));

        abstractEffectFactory.registerHandler("PveRaidPhysicalSkillDamageBonus",
            stat => new PveRaidPhysicalSkillDamageBonus(stat));

        abstractEffectFactory.registerHandler("PveRaidPhysicalSkillDefenceBonus",
            stat => new PveRaidPhysicalSkillDefenceBonus(stat));

        abstractEffectFactory.registerHandler("PvpMagicalSkillDamageBonus", stat => new PvpMagicalSkillDamageBonus(stat));
        abstractEffectFactory.registerHandler("PvpMagicalSkillDefenceBonus", stat => new PvpMagicalSkillDefenceBonus(stat));
        abstractEffectFactory.registerHandler("PvpPhysicalAttackDamageBonus", stat => new PvpPhysicalAttackDamageBonus(stat));
        abstractEffectFactory.registerHandler("PvpPhysicalAttackDefenceBonus", stat => new PvpPhysicalAttackDefenceBonus(stat));
        abstractEffectFactory.registerHandler("PvpPhysicalSkillDamageBonus", stat => new PvpPhysicalSkillDamageBonus(stat));
        abstractEffectFactory.registerHandler("PvpPhysicalSkillDefenceBonus", stat => new PvpPhysicalSkillDefenceBonus(stat));
        abstractEffectFactory.registerHandler("RandomizeHate", stat => new RandomizeHate(stat));
        abstractEffectFactory.registerHandler("RealDamage", stat => new RealDamage(stat));
        abstractEffectFactory.registerHandler("RealDamageResist", stat => new RealDamageResist(stat));
        abstractEffectFactory.registerHandler("RearDamage", stat => new RearDamage(stat));
        abstractEffectFactory.registerHandler("RebalanceHP", stat => new RebalanceHP());
        abstractEffectFactory.registerHandler("RebalanceHPSummon", stat => new RebalanceHPSummon());
        abstractEffectFactory.registerHandler("RecoverVitalityInPeaceZone", stat => new RecoverVitalityInPeaceZone(stat));
        abstractEffectFactory.registerHandler("ReduceDamage", stat => new ReduceDamage(stat));
        abstractEffectFactory.registerHandler("ReduceCancel", stat => new ReduceCancel(stat));
        abstractEffectFactory.registerHandler("ReduceDropPenalty", stat => new ReduceDropPenalty(stat));
        abstractEffectFactory.registerHandler("ReflectMagic", stat => new ReflectMagic(stat));
        abstractEffectFactory.registerHandler("ReflectSkill", stat => new ReflectSkill(stat));
        abstractEffectFactory.registerHandler("RefuelAirship", stat => new RefuelAirship(stat));
        abstractEffectFactory.registerHandler("Relax", stat => new Relax(stat));
        abstractEffectFactory.registerHandler("ReplaceSkillBySkill", stat => new ReplaceSkillBySkill(stat));
        abstractEffectFactory.registerHandler("ResetInstanceEntry", stat => new ResetInstanceEntry(stat));
        abstractEffectFactory.registerHandler("ResistAbnormalByCategory", stat => new ResistAbnormalByCategory(stat));
        abstractEffectFactory.registerHandler("ResistDDMagic", stat => new ResistDDMagic(stat));
        abstractEffectFactory.registerHandler("ResistDispelByCategory", stat => new ResistDispelByCategory(stat));
        abstractEffectFactory.registerHandler("ResistSkill", stat => new ResistSkill(stat));
        abstractEffectFactory.registerHandler("Restoration", stat => new Restoration(stat));
        abstractEffectFactory.registerHandler("RestorationRandom", stat => new RestorationRandom(stat));
        abstractEffectFactory.registerHandler("Resurrection", stat => new Resurrection(stat));
        abstractEffectFactory.registerHandler("ResurrectionFeeModifier", stat => new ResurrectionFeeModifier(stat));
        abstractEffectFactory.registerHandler("ResurrectionSpecial", stat => new ResurrectionSpecial(stat));
        abstractEffectFactory.registerHandler("Reuse", stat => new Reuse(stat));
        abstractEffectFactory.registerHandler("ReuseSkillById", stat => new ReuseSkillById(stat));
        abstractEffectFactory.registerHandler("ReuseSkillIdByDamage", stat => new ReuseSkillIdByDamage(stat));
        abstractEffectFactory.registerHandler("Root", stat => new Root());
        abstractEffectFactory.registerHandler("SacrificeSummon", stat => new SacrificeSummon());
        abstractEffectFactory.registerHandler("SafeFallHeight", stat => new SafeFallHeight(stat));
        abstractEffectFactory.registerHandler("SayhaGraceSupport", stat => new SayhaGraceSupport());
        abstractEffectFactory.registerHandler("SendSystemMessageToClan", stat => new SendSystemMessageToClan(stat));
        abstractEffectFactory.registerHandler("ServitorShare", stat => new ServitorShare(stat));
        abstractEffectFactory.registerHandler("ServitorShareSkills", stat => new ServitorShareSkills());
        abstractEffectFactory.registerHandler("SetHp", stat => new SetHp(stat));
        abstractEffectFactory.registerHandler("SetCp", stat => new SetCp(stat));
        abstractEffectFactory.registerHandler("SetSkill", stat => new SetSkill(stat));
        abstractEffectFactory.registerHandler("ShieldDefence", stat => new ShieldDefence(stat));
        abstractEffectFactory.registerHandler("ShieldDefenceIgnoreRemoval", stat => new ShieldDefenceIgnoreRemoval(stat));
        abstractEffectFactory.registerHandler("ShieldDefenceRate", stat => new ShieldDefenceRate(stat));
        abstractEffectFactory.registerHandler("ShotsBonus", stat => new ShotsBonus(stat));
        abstractEffectFactory.registerHandler("SilentMove", stat => new SilentMove());
        abstractEffectFactory.registerHandler("SkillBonusRange", stat => new SkillBonusRange(stat));
        abstractEffectFactory.registerHandler("SkillEvasion", stat => new SkillEvasion(stat));
        abstractEffectFactory.registerHandler("SkillMastery", stat => new SkillMastery(stat));
        abstractEffectFactory.registerHandler("SkillMasteryRate", stat => new SkillMasteryRate(stat));
        abstractEffectFactory.registerHandler("SkillPowerAdd", stat => new SkillPowerAdd(stat));
        abstractEffectFactory.registerHandler("SkillTurning", stat => new SkillTurning(stat));
        abstractEffectFactory.registerHandler("SkillTurningOverTime", stat => new SkillTurningOverTime(stat));
        abstractEffectFactory.registerHandler("SoulBlow", stat => new SoulBlow(stat));
        abstractEffectFactory.registerHandler("SoulEating", stat => new SoulEating(stat));
        abstractEffectFactory.registerHandler("SoulshotResistance", stat => new SoulshotResistance(stat));
        abstractEffectFactory.registerHandler("Sow", stat => new Sow());
        abstractEffectFactory.registerHandler("Speed", stat => new Speed(stat));
        abstractEffectFactory.registerHandler("SphericBarrier", stat => new SphericBarrier(stat));
        abstractEffectFactory.registerHandler("SpeedLimit", stat => new SpeedLimit(stat));
        abstractEffectFactory.registerHandler("SpiritExpModify", stat => new SpiritExpModify(stat));
        abstractEffectFactory.registerHandler("SpiritshotResistance", stat => new SpiritshotResistance(stat));
        abstractEffectFactory.registerHandler("SpModify", stat => new SpModify(stat));
        abstractEffectFactory.registerHandler("Spoil", stat => new Spoil());
        abstractEffectFactory.registerHandler("StatAddForLevel", stat => new StatAddForLevel(stat));
        abstractEffectFactory.registerHandler("StatAddForMp", stat => new StatAddForMp(stat));
        abstractEffectFactory.registerHandler("StatAddForStat", stat => new StatAddForStat(stat));
        abstractEffectFactory.registerHandler("StatBonusSkillCritical", stat => new StatBonusSkillCritical(stat));
        abstractEffectFactory.registerHandler("StatBonusSpeed", stat => new StatBonusSpeed(stat));
        abstractEffectFactory.registerHandler("StatByMoveType", stat => new StatByMoveType(stat));
        abstractEffectFactory.registerHandler("StatMulForBaseStat", stat => new StatMulForBaseStat(stat));
        abstractEffectFactory.registerHandler("StatMulForLevel", stat => new StatMulForLevel(stat));
        abstractEffectFactory.registerHandler("StatUp", stat => new StatUp(stat));
        abstractEffectFactory.registerHandler("StealAbnormal", stat => new StealAbnormal(stat));
        abstractEffectFactory.registerHandler("Summon", stat => new Summon(stat));
        abstractEffectFactory.registerHandler("SummonAgathion", stat => new SummonAgathion(stat));
        abstractEffectFactory.registerHandler("SummonCubic", stat => new SummonCubic(stat));
        abstractEffectFactory.registerHandler("SummonHallucination", stat => new SummonHallucination(stat));
        abstractEffectFactory.registerHandler("SummonMulti", stat => new SummonMulti(stat));
        abstractEffectFactory.registerHandler("SummonNpc", stat => new SummonNpc(stat));
        abstractEffectFactory.registerHandler("SummonPet", stat => new SummonPet());
        abstractEffectFactory.registerHandler("SummonPoints", stat => new SummonPoints(stat));
        abstractEffectFactory.registerHandler("SummonTrap", stat => new SummonTrap(stat));
        abstractEffectFactory.registerHandler("Sweeper", stat => new Sweeper(stat));
        abstractEffectFactory.registerHandler("Synergy", stat => new Synergy(stat));
        abstractEffectFactory.registerHandler("TakeCastle", stat => new TakeCastle(stat));
        abstractEffectFactory.registerHandler("TakeCastleStart", stat => new TakeCastleStart());
        abstractEffectFactory.registerHandler("TakeFort", stat => new TakeFort());
        abstractEffectFactory.registerHandler("TakeFortStart", stat => new TakeFortStart());
        abstractEffectFactory.registerHandler("TalismanSlot", stat => new TalismanSlot(stat));
        abstractEffectFactory.registerHandler("TargetCancel", stat => new TargetCancel(stat));
        abstractEffectFactory.registerHandler("TargetMe", stat => new TargetMe());
        abstractEffectFactory.registerHandler("TargetMeProbability", stat => new TargetMeProbability(stat));
        abstractEffectFactory.registerHandler("Teleport", stat => new Teleport(stat));
        abstractEffectFactory.registerHandler("TeleportToNpc", stat => new TeleportToNpc(stat));
        abstractEffectFactory.registerHandler("TeleportToPlayer", stat => new TeleportToPlayer());
        abstractEffectFactory.registerHandler("TeleportToSummon", stat => new TeleportToSummon(stat));
        abstractEffectFactory.registerHandler("TeleportToTarget", stat => new TeleportToTarget());
        abstractEffectFactory.registerHandler("TeleportToTeleportLocation", stat => new TeleportToTeleportLocation());
        abstractEffectFactory.registerHandler("FlyAway", stat => new FlyAway(stat));
        abstractEffectFactory.registerHandler("TransferDamageToPlayer", stat => new TransferDamageToPlayer(stat));
        abstractEffectFactory.registerHandler("TransferDamageToSummon", stat => new TransferDamageToSummon(stat));
        abstractEffectFactory.registerHandler("TransferHate", stat => new TransferHate(stat));
        abstractEffectFactory.registerHandler("Transformation", stat => new Transformation(stat));
        abstractEffectFactory.registerHandler("TrapDetect", stat => new TrapDetect(stat));
        abstractEffectFactory.registerHandler("TrapRemove", stat => new TrapRemove(stat));
        abstractEffectFactory.registerHandler("TriggerHealPercentBySkill", stat => new TriggerHealPercentBySkill(stat));
        abstractEffectFactory.registerHandler("TriggerSkill", stat => new TriggerSkill(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByAttack", stat => new TriggerSkillByAttack(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByAvoid", stat => new TriggerSkillByAvoid(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByBaseStat", stat => new TriggerSkillByBaseStat(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByDamage", stat => new TriggerSkillByDamage(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByDeathBlow", stat => new TriggerSkillByDeathBlow(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByDualRange", stat => new TriggerSkillByDualRange(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByHpPercent", stat => new TriggerSkillByHpPercent(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByKill", stat => new TriggerSkillByKill(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByMagicType", stat => new TriggerSkillByMagicType(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByMaxHp", stat => new TriggerSkillByMaxHp(stat));
        abstractEffectFactory.registerHandler("TriggerSkillBySkill", stat => new TriggerSkillBySkill(stat));
        abstractEffectFactory.registerHandler("TriggerSkillBySkillAttack", stat => new TriggerSkillBySkillAttack(stat));
        abstractEffectFactory.registerHandler("TriggerSkillByStat", stat => new TriggerSkillByStat(stat));
        abstractEffectFactory.registerHandler("TwoHandedBluntBonus", stat => new TwoHandedBluntBonus(stat));
        abstractEffectFactory.registerHandler("TwoHandedStance", stat => new TwoHandedStance(stat));
        abstractEffectFactory.registerHandler("TwoHandedSwordBonus", stat => new TwoHandedSwordBonus(stat));
        abstractEffectFactory.registerHandler("Unsummon", stat => new Unsummon(stat));
        abstractEffectFactory.registerHandler("UnsummonAgathion", stat => new UnsummonAgathion());
        abstractEffectFactory.registerHandler("UnsummonServitors", stat => new UnsummonServitors());
        abstractEffectFactory.registerHandler("Untargetable", stat => new Untargetable());
        abstractEffectFactory.registerHandler("VampiricAttack", stat => new VampiricAttack(stat));
        abstractEffectFactory.registerHandler("VampiricDefence", stat => new VampiricDefence(stat));
        abstractEffectFactory.registerHandler("VipUp", stat => new VipUp(stat));
        abstractEffectFactory.registerHandler("VitalityExpRate", stat => new VitalityExpRate(stat));
        abstractEffectFactory.registerHandler("VitalityPointsRate", stat => new VitalityPointsRate(stat));
        abstractEffectFactory.registerHandler("VitalityPointUp", stat => new VitalityPointUp(stat));
        abstractEffectFactory.registerHandler("WeaponAttackAngleBonus", stat => new WeaponAttackAngleBonus(stat));
        abstractEffectFactory.registerHandler("WeaponBonusMAtk", stat => new WeaponBonusMAtk(stat));
        abstractEffectFactory.registerHandler("WeaponBonusMAtkMultiplier", stat => new WeaponBonusMAtkMultiplier(stat));
        abstractEffectFactory.registerHandler("WeaponBonusPAtk", stat => new WeaponBonusPAtk(stat));
        abstractEffectFactory.registerHandler("WeaponBonusPAtkMultiplier", stat => new WeaponBonusPAtkMultiplier(stat));
        abstractEffectFactory.registerHandler("WeightLimit", stat => new WeightLimit(stat));
        abstractEffectFactory.registerHandler("WeightPenalty", stat => new WeightPenalty(stat));
        abstractEffectFactory.registerHandler("WorldChatPoints", stat => new WorldChatPoints(stat));

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
        SkillConditionFactory skillConditionFactory = SkillConditionFactory.getInstance();
        skillConditionFactory.registerHandler("AssassinationPoints",
            stat => new AssassinationPointsSkillCondition(stat));

        skillConditionFactory.registerHandler("BeastPoints", stat => new BeastPointsSkillCondition(stat));
        skillConditionFactory.registerHandler("BuildAdvanceBase", stat => new BuildAdvanceBaseSkillCondition());
        skillConditionFactory.registerHandler("BuildCamp", stat => new BuildCampSkillCondition());
        skillConditionFactory.registerHandler("CanAddMaxEntranceInzone",
            stat => new CanAddMaxEntranceInzoneSkillCondition());

        skillConditionFactory.registerHandler("CanBookmarkAddSlot", stat => new CanBookmarkAddSlotSkillCondition(stat));
        skillConditionFactory.registerHandler("CanChangeVitalItemCount",
            stat => new CanChangeVitalItemCountSkillCondition());

        skillConditionFactory.registerHandler("CanEnchantAttribute",
            stat => new CanEnchantAttributeSkillCondition());

        skillConditionFactory.registerHandler("CanMountForEvent", stat => new CanMountForEventSkillCondition());
        skillConditionFactory.registerHandler("CannotUseInTransform",
            stat => new CannotUseInTransformSkillCondition(stat));

        skillConditionFactory.registerHandler("CanRefuelAirship", stat => new CanRefuelAirshipSkillCondition(stat));
        skillConditionFactory.registerHandler("CanSummon", stat => new CanSummonSkillCondition());
        skillConditionFactory.registerHandler("CanSummonCubic", stat => new CanSummonCubicSkillCondition());
        skillConditionFactory.registerHandler("CanSummonMulti", stat => new CanSummonMultiSkillCondition(stat));
        skillConditionFactory.registerHandler("CanSummonPet", stat => new CanSummonPetSkillCondition());
        skillConditionFactory.registerHandler("CanSummonSiegeGolem",
            stat => new CanSummonSiegeGolemSkillCondition());

        skillConditionFactory.registerHandler("CanTakeFort", stat => new CanTakeFortSkillCondition());
        skillConditionFactory.registerHandler("CanTransform", stat => new CanTransformSkillCondition(stat));
        skillConditionFactory.registerHandler("CanTransformInDominion",
            stat => new CanTransformInDominionSkillCondition());

        skillConditionFactory.registerHandler("CanUntransform", stat => new CanUntransformSkillCondition());
        skillConditionFactory.registerHandler("CanUseInBattlefield",
            stat => new CanUseInBattlefieldSkillCondition());

        skillConditionFactory.registerHandler("CanUseInDragonLair", stat => new CanUseInDragonLairSkillCondition());
        skillConditionFactory.registerHandler("CanUseSwoopCannon", stat => new CanUseSwoopCannonSkillCondition());
        skillConditionFactory.registerHandler("HasVitalityPoints", stat => new HasVitalityPointsSkillCondition(stat));
        skillConditionFactory.registerHandler("CanUseVitalityIncreaseItem",
            stat => new CanUseVitalityIncreaseItemSkillCondition(stat));

        skillConditionFactory.registerHandler("CheckLevel", stat => new CheckLevelSkillCondition(stat));
        skillConditionFactory.registerHandler("CheckSex", stat => new CheckSexSkillCondition(stat));
        skillConditionFactory.registerHandler("ConsumeBody", stat => new ConsumeBodySkillCondition());
        skillConditionFactory.registerHandler("DeathPoints", stat => new DeathPointsSkillCondition(stat));
        skillConditionFactory.registerHandler("EnergySaved", stat => new EnergySavedSkillCondition(stat));
        skillConditionFactory.registerHandler("EquipArmor", stat => new EquipArmorSkillCondition(stat));
        skillConditionFactory.registerHandler("EquippedCloakEnchant",
            stat => new EquippedCloakEnchantSkillCondition(stat));

        skillConditionFactory.registerHandler("EquipShield", stat => new EquipShieldSkillCondition());
        skillConditionFactory.registerHandler("EquipSigil", stat => new EquipSigilSkillCondition());
        skillConditionFactory.registerHandler("EquipWeapon", stat => new EquipWeaponSkillCondition(stat));
        skillConditionFactory.registerHandler("MaxMpSkillCondition", stat => new MaxMpSkillCondition(stat));
        skillConditionFactory.registerHandler("NotFeared", stat => new NotFearedSkillCondition());
        skillConditionFactory.registerHandler("NotInUnderwater", stat => new NotInUnderwaterSkillCondition());
        skillConditionFactory.registerHandler("Op2hWeapon", stat => new Op2hWeaponSkillCondition(stat));
        skillConditionFactory.registerHandler("OpAffectedBySkill", stat => new OpAffectedBySkillSkillCondition(stat));
        skillConditionFactory.registerHandler("OpAgathionEnergy", stat => new OpAgathionEnergySkillCondition());
        skillConditionFactory.registerHandler("OpAlignment", stat => new OpAlignmentSkillCondition(stat));
        skillConditionFactory.registerHandler("OpBaseStat", stat => new OpBaseStatSkillCondition(stat));
        skillConditionFactory.registerHandler("OpBlink", stat => new OpBlinkSkillCondition(stat));
        skillConditionFactory.registerHandler("OpCallPc", stat => new OpCallPcSkillCondition());
        skillConditionFactory.registerHandler("OpCanEscape", stat => new OpCanEscapeSkillCondition());
        skillConditionFactory.registerHandler("OpCanNotUseAirship", stat => new OpCanNotUseAirshipSkillCondition());
        skillConditionFactory.registerHandler("OpCannotUseTargetWithPrivateStore",
            stat => new OpCannotUseTargetWithPrivateStoreSkillCondition());

        skillConditionFactory.registerHandler("OpChangeWeapon", stat => new OpChangeWeaponSkillCondition());
        skillConditionFactory.registerHandler("OpCheckAbnormal", stat => new OpCheckAbnormalSkillCondition(stat));
        skillConditionFactory.registerHandler("OpCheckAccountType", stat => new OpCheckAccountTypeSkillCondition());
        skillConditionFactory.registerHandler("OpCheckCastRange", stat => new OpCheckCastRangeSkillCondition(stat));
        skillConditionFactory.registerHandler("OpCheckClass", stat => new OpCheckClassSkillCondition(stat));
        skillConditionFactory.registerHandler("OpCheckClassList", stat => new OpCheckClassListSkillCondition(stat));
        skillConditionFactory.registerHandler("OpCheckCrtEffect", stat => new OpCheckCrtEffectSkillCondition());
        skillConditionFactory.registerHandler("OpCheckFlag", stat => new OpCheckFlagSkillCondition());
        skillConditionFactory.registerHandler("OpCheckOnGoingEventCampaign",
            stat => new OpCheckOnGoingEventCampaignSkillCondition());

        skillConditionFactory.registerHandler("OpCheckPcbangPoint", stat => new OpCheckPcbangPointSkillCondition());
        skillConditionFactory.registerHandler("OpCheckResidence", stat => new OpCheckResidenceSkillCondition(stat));
        skillConditionFactory.registerHandler("OpCheckSkill", stat => new OpCheckSkillSkillCondition(stat));
        skillConditionFactory.registerHandler("OpCheckSkillList", stat => new OpCheckSkillListSkillCondition(stat));
        skillConditionFactory.registerHandler("OpCompanion", stat => new OpCompanionSkillCondition(stat));
        skillConditionFactory.registerHandler("OpEnchantRange", stat => new OpEnchantRangeSkillCondition());
        skillConditionFactory.registerHandler("OpEncumbered", stat => new OpEncumberedSkillCondition(stat));
        skillConditionFactory.registerHandler("OpEnergyMax", stat => new OpEnergyMaxSkillCondition(stat));
        skillConditionFactory.registerHandler("OpEquipItem", stat => new OpEquipItemSkillCondition(stat));
        skillConditionFactory.registerHandler("OpExistNpc", stat => new OpExistNpcSkillCondition(stat));
        skillConditionFactory.registerHandler("OpFishingCast", stat => new OpFishingCastSkillCondition());
        skillConditionFactory.registerHandler("OpFishingPumping", stat => new OpFishingPumpingSkillCondition());
        skillConditionFactory.registerHandler("OpFishingReeling", stat => new OpFishingReelingSkillCondition());
        skillConditionFactory.registerHandler("OpHaveSummon", stat => new OpHaveSummonSkillCondition());
        skillConditionFactory.registerHandler("OpHaveSummonedNpc", stat => new OpHaveSummonedNpcSkillCondition(stat));
        skillConditionFactory.registerHandler("OpHome", stat => new OpHomeSkillCondition(stat));
        skillConditionFactory.registerHandler("OpInSiege", stat => new OpInSiegeSkillCondition(stat));
        skillConditionFactory.registerHandler("OpInstantzone", stat => new OpInstantzoneSkillCondition(stat));
        skillConditionFactory.registerHandler("OpMainjob", stat => new OpMainjobSkillCondition());
        skillConditionFactory.registerHandler("OpNeedAgathion", stat => new OpNeedAgathionSkillCondition());
        skillConditionFactory.registerHandler("OpNeedSummonOrPet", stat => new OpNeedSummonOrPetSkillCondition(stat));
        skillConditionFactory.registerHandler("OpNotAffectedBySkill",
            stat => new OpNotAffectedBySkillSkillCondition(stat));

        skillConditionFactory.registerHandler("OpNotCursed", stat => new OpNotCursedSkillCondition());
        skillConditionFactory.registerHandler("OpNotInPeacezone", stat => new OpNotInPeacezoneSkillCondition());
        skillConditionFactory.registerHandler("OpNotInstantzone", stat => new OpNotInstantzoneSkillCondition());
        skillConditionFactory.registerHandler("OpNotOlympiad", stat => new OpNotOlympiadSkillCondition());
        skillConditionFactory.registerHandler("OpNotTerritory", stat => new OpNotTerritorySkillCondition());
        skillConditionFactory.registerHandler("OpOlympiad", stat => new OpOlympiadSkillCondition());
        skillConditionFactory.registerHandler("OpPeacezone", stat => new OpPeacezoneSkillCondition());
        skillConditionFactory.registerHandler("OpPkcount", stat => new OpPkcountSkillCondition(stat));
        skillConditionFactory.registerHandler("OpPledge", stat => new OpPledgeSkillCondition(stat));
        skillConditionFactory.registerHandler("OpRestartPoint", stat => new OpRestartPointSkillCondition());
        skillConditionFactory.registerHandler("OpResurrection", stat => new OpResurrectionSkillCondition());
        skillConditionFactory.registerHandler("OpSiegeHammer", stat => new OpSiegeHammerSkillCondition());
        skillConditionFactory.registerHandler("OpSkill", stat => new OpSkillSkillCondition(stat));
        skillConditionFactory.registerHandler("OpSkillAcquire", stat => new OpSkillAcquireSkillCondition(stat));
        skillConditionFactory.registerHandler("OpSocialClass", stat => new OpSocialClassSkillCondition(stat));
        skillConditionFactory.registerHandler("OpSoulMax", stat => new OpSoulMaxSkillCondition(stat));
        skillConditionFactory.registerHandler("OpStrider", stat => new OpStriderSkillCondition());
        skillConditionFactory.registerHandler("OpSubjob", stat => new OpSubjobSkillCondition());
        skillConditionFactory.registerHandler("OpSweeper", stat => new OpSweeperSkillCondition());
        skillConditionFactory.registerHandler("OpTargetAllItemType",
            stat => new OpTargetAllItemTypeSkillCondition());

        skillConditionFactory.registerHandler("OpTargetArmorType", stat => new OpTargetArmorTypeSkillCondition(stat));
        skillConditionFactory.registerHandler("OpTargetDoor", stat => new OpTargetDoorSkillCondition(stat));
        skillConditionFactory.registerHandler("OpTargetMyPledgeAcademy",
            stat => new OpTargetMyPledgeAcademySkillCondition());

        skillConditionFactory.registerHandler("OpTargetNpc", stat => new OpTargetNpcSkillCondition(stat));
        skillConditionFactory.registerHandler("OpTargetPc", stat => new OpTargetPcSkillCondition());
        skillConditionFactory.registerHandler("OpTargetWeaponAttackType",
            stat => new OpTargetWeaponAttackTypeSkillCondition(stat));

        skillConditionFactory.registerHandler("OpTerritory", stat => new OpTerritorySkillCondition());
        skillConditionFactory.registerHandler("OpUnlock", stat => new OpUnlockSkillCondition());
        skillConditionFactory.registerHandler("OpUseFirecracker", stat => new OpUseFirecrackerSkillCondition());
        skillConditionFactory.registerHandler("OpUsePraseed", stat => new OpUsePraseedSkillCondition());
        skillConditionFactory.registerHandler("OpWyvern", stat => new OpWyvernSkillCondition());
        skillConditionFactory.registerHandler("PossessHolything", stat => new PossessHolythingSkillCondition());
        skillConditionFactory.registerHandler("RemainCpPer", stat => new RemainCpPerSkillCondition(stat));
        skillConditionFactory.registerHandler("RemainHpPer", stat => new RemainHpPerSkillCondition(stat));
        skillConditionFactory.registerHandler("RemainMpPer", stat => new RemainMpPerSkillCondition(stat));
        skillConditionFactory.registerHandler("SoulSaved", stat => new SoulSavedSkillCondition(stat));
        skillConditionFactory.registerHandler("TargetAffectedBySkill",
            stat => new TargetAffectedBySkillSkillCondition(stat));

        skillConditionFactory.registerHandler("TargetItemCrystalType",
            stat => new TargetItemCrystalTypeSkillCondition());

        skillConditionFactory.registerHandler("TargetMyMentee", stat => new TargetMyMenteeSkillCondition());
        skillConditionFactory.registerHandler("TargetMyParty", stat => new TargetMyPartySkillCondition(stat));
        skillConditionFactory.registerHandler("TargetMyPledge", stat => new TargetMyPledgeSkillCondition());
        skillConditionFactory.registerHandler("TargetNotAffectedBySkill",
            stat => new TargetNotAffectedBySkillSkillCondition(stat));

        skillConditionFactory.registerHandler("TargetRace", stat => new TargetRaceSkillCondition(stat));

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