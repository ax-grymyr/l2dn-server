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
        AbstractEffectFactory.Instance.Register(typeof(Scripts).Assembly);

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