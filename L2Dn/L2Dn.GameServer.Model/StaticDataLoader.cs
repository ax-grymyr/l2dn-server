using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.InstanceManagers.Events;
using L2Dn.GameServer.InstanceManagers.Games;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Vips;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.TaskManagers;

namespace L2Dn.GameServer;

public static class StaticDataLoader
{
    public static void Load()
    {
        // XML config files
        AccessLevelData.Instance.Load();
        AdminCommandData.Instance.Load();
        SecondaryAuthData.Instance.Load();
        ClanRewardData.Instance.Load();

        // XML data files

        // Map
        World.getInstance();
        MapRegionData.Instance.Load();
        ZoneManager.Instance.Load(); // for now, zones cannot be separated from the Creature class

        // Unclassified
        GameTimeTaskManager.getInstance();
        IdManager.getInstance();

        DoorData.getInstance();
        FenceData.getInstance();
        AnnouncementsTable.getInstance();
        GlobalVariablesManager.getInstance();

        ActionData.Instance.Load();
        CategoryData.Instance.Load();
        SayuneData.getInstance();
        MissionLevel.getInstance();
        DailyMissionHandler.getInstance(); //.executeScript();
        DailyMissionData.getInstance();
        ElementalSpiritData.Instance.Load();
        RankingPowerManager.getInstance();
        SubjugationData.getInstance();
        SubjugationGacha.getInstance();
        PurgeRankingManager.getInstance();
        NewQuestData.getInstance();

        SkillData.Instance.Load();
        SkillTreeData.getInstance();
        PetSkillData.getInstance();
        PetAcquireList.getInstance();
        SkillEnchantData.getInstance();
        OptionData.Instance.Load(); // depends on SkillData

        ItemData.getInstance();
        EnchantItemGroupsData.getInstance();
        EnchantItemData.getInstance();
        EnchantItemOptionsData.getInstance(); // depends on ItemData, OptionData
        EnchantChallengePointData.getInstance();
        ElementalAttributeData.getInstance();
        ItemCrystallizationData.getInstance();
        VariationData.getInstance();
        EnsoulData.getInstance();
        EnchantItemHPBonusData.getInstance();
        BuyListData.getInstance();
        MultisellData.getInstance();
        CombinationItemsData.getInstance();
        EquipmentUpgradeData.getInstance();
        EquipmentUpgradeNormalData.getInstance();
        AgathionData.getInstance();
        RaidTeleportListData.getInstance();
        RecipeData.getInstance();
        ArmorSetData.getInstance();
        FishingData.getInstance();
        HennaData.getInstance();
        HennaCombinationData.getInstance();
        HennaPatternPotentialData.getInstance();
        PrimeShopData.getInstance();
        LimitShopData.getInstance();
        LimitShopCraftData.getInstance();
        LimitShopClanData.getInstance();
        CollectionData.getInstance();
        RaidDropAnnounceData.getInstance();
        PcCafePointsManager.getInstance();
        AppearanceItemData.getInstance();
        ItemCommissionManager.getInstance();
        WorldExchangeManager.getInstance();
        PrivateStoreHistoryManager.getInstance().restore();
        LuckyGameData.getInstance();
        AttendanceRewardData.getInstance();
        MagicLampData.getInstance();
        RandomCraftData.getInstance();
        RevengeHistoryManager.getInstance();
        VipData.getInstance();

        CharacterClassData.Instance.Load();
        InitialEquipmentData.getInstance();
        InitialShortcutData.getInstance();
        ExperienceData.Instance.Load();
        PlayerXpPercentLostData.getInstance();
        KarmaData.Instance.Load();
        HitConditionBonusData.getInstance();
        PlayerTemplateData.getInstance();
        CharInfoTable.getInstance();
        PetDataTable.getInstance();
        PetTypeData.getInstance();
        PetExtractData.getInstance();
        CubicData.getInstance();
        CharSummonTable.getInstance().init();
        BeautyShopData.Instance.Load();
        MentorManager.getInstance();
        VipManager.getInstance();

        if (Config.PremiumSystem.PREMIUM_SYSTEM_ENABLED)
        {
            PremiumManager.getInstance();
        }

        ClanLevelData.Instance.Load();
        ClanTable.getInstance();
        ResidenceFunctionsData.getInstance();
        ClanHallData.getInstance();
        ClanHallAuctionManager.getInstance();
        ClanEntryManager.getInstance();
        CastleData.Instance.Load();

        GeoEngine.getInstance();

        NpcData.getInstance();
        FakePlayerData.getInstance();
        FakePlayerChatManager.getInstance();
        SpawnData.getInstance();
        WalkingManager.getInstance();
        StaticObjectData.getInstance();
        ItemAuctionManager.getInstance();
        CastleManager.getInstance().loadInstances();
        SchemeBufferTable.getInstance();
        GrandBossManager.getInstance();
        EventDropManager.getInstance();

        InstanceManager.getInstance();

        Olympiad.getInstance();
        Hero.getInstance();

        HtmCache.getInstance();
        CrestTable.getInstance();
        TeleportListData.getInstance();
        SharedTeleportManager.getInstance();
        TeleporterData.getInstance();
        TimedHuntingZoneData.getInstance();
        MatchingRoomManager.getInstance();
        PetitionManager.getInstance();
        CursedWeaponsManager.getInstance();
        TransformData.getInstance();
        BotReportTable.getInstance();
        RankManager.getInstance();
        if (Config.SellBuffs.SELLBUFF_ENABLED)
        {
            SellBuffsManager.getInstance();
        }

        if (Config.MultilingualSupport.MULTILANG_ENABLE)
        {
            //SystemMessageId.loadLocalisations();
            //NpcStringId.loadLocalisations();
            //SendMessageLocalisationData.getInstance();
            //NpcNameLocalisationData.getInstance();
        }

        QuestManager.getInstance();
        BoatManager.getInstance();
        AirShipManager.getInstance();
        ShuttleData.getInstance();
        GraciaSeedsManager.getInstance();

        SpawnData.getInstance().init();
        DbSpawnManager.getInstance();

        SiegeManager.getInstance().getSieges();
        CastleManager.getInstance().activateInstances();
        FortManager.getInstance().loadInstances();
        FortManager.getInstance().activateInstances();
        FortSiegeManager.getInstance();
        SiegeScheduleData.getInstance();

        CastleManorManager.getInstance();
        SiegeGuardManager.getInstance();
        QuestManager.getInstance().report();

        if (Config.General.SAVE_DROPPED_ITEM)
        {
            ItemsOnGroundManager.getInstance();
        }

        if (Config.General.AUTODESTROY_ITEM_AFTER > 0 || Config.General.HERB_AUTO_DESTROY_TIME > 0)
        {
            ItemsAutoDestroyTaskManager.getInstance();
        }

        MonsterRace.getInstance();

        TaskManager.getInstance();

        DailyTaskManager.getInstance();

        AntiFeedManager.getInstance().registerEvent(AntiFeedManager.GAME_ID);

        if (Config.General.ALLOW_MAIL)
        {
            MailManager.getInstance();
        }

        if (Config.CustomMailManager.CUSTOM_MAIL_MANAGER_ENABLED)
        {
            CustomMailManager.getInstance();
        }

        if (GlobalEvents.Global.HasSubscribers<OnServerStart>())
        {
            GlobalEvents.Global.NotifyAsync(new OnServerStart());
        }

        PunishmentManager.getInstance();

        AdminCommandHandler.getInstance();

        //Runtime.getRuntime().addShutdownHook(Shutdown.getInstance());

        //_logger.Info("IdManager: Free ObjectID's remaining: " + IdManager.getInstance());

        if ((Config.OfflineTrade.OFFLINE_TRADE_ENABLE || Config.OfflineTrade.OFFLINE_CRAFT_ENABLE) &&
            Config.OfflineTrade.RESTORE_OFFLINERS)
        {
            OfflineTraderTable.getInstance().restoreOfflineTraders();
        }

        if (Config.Server.SERVER_RESTART_SCHEDULE_ENABLED)
        {
            ServerRestartManager.getInstance();
        }

        if (Config.Server.PRECAUTIONARY_RESTART_ENABLED)
        {
            PrecautionaryRestartManager.getInstance();
        }

        // if (Config.DEADLOCK_DETECTOR)
        // {
        // 	_deadDetectThread = new DeadLockDetector(Duration.ofSeconds(Config.DEADLOCK_CHECK_INTERVAL), () ->
        // 	{
        // 		if (Config.RESTART_ON_DEADLOCK)
        // 		{
        // 			Broadcast.toAllOnlinePlayers("Server has stability issues - restarting now.");
        // 			Shutdown.getInstance().startShutdown(null, 60, true);
        // 		}
        // 	});
        // 	_deadDetectThread.setDaemon(true);
        // 	_deadDetectThread.start();
        // }
        // else
        // {
        // 	_deadDetectThread = null;
        // }
    }
}