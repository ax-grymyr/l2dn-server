using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Configuration;
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
using L2Dn.GameServer.TaskManagers;

namespace L2Dn.GameServer.Data;

public static class StaticData
{
    public static void Load()
    {
        GameTimeTaskManager.getInstance();
        IdManager.getInstance();

        World.getInstance();
        MapRegionManager.getInstance();
        ZoneManager.getInstance();
        DoorData.getInstance();
        FenceData.getInstance();
        AnnouncementsTable.getInstance();
        GlobalVariablesManager.getInstance();

        ActionData.getInstance();
        CategoryData.getInstance();
        SecondaryAuthData.getInstance();
        SayuneData.getInstance();
        ClanRewardData.getInstance();
        MissionLevel.getInstance();
        DailyMissionHandler.getInstance(); //.executeScript();
        DailyMissionData.getInstance();
        ElementalSpiritData.getInstance();
        RankingPowerManager.getInstance();
        SubjugationData.getInstance();
        SubjugationGacha.getInstance();
        PurgeRankingManager.getInstance();
        NewQuestData.getInstance();

        SkillConditionHandler.getInstance(); //.executeScript();
        EffectHandler.getInstance(); //.executeScript();
        SkillData.getInstance();
        SkillTreeData.getInstance();
        PetSkillData.getInstance();
        PetAcquireList.getInstance();
        SkillEnchantData.getInstance();

        ConditionHandler.getInstance(); //.executeScript();
        ItemData.getInstance();
        EnchantItemGroupsData.getInstance();
        EnchantItemData.getInstance();
        EnchantItemOptionsData.getInstance();
        EnchantChallengePointData.getInstance();
        ElementalAttributeData.getInstance();
        ItemCrystallizationData.getInstance();
        OptionData.getInstance();
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

        ClassListData.getInstance();
        InitialEquipmentData.getInstance();
        InitialShortcutData.getInstance();
        ExperienceData.getInstance();
        PlayerXpPercentLostData.getInstance();
        KarmaData.getInstance();
        HitConditionBonusData.getInstance();
        PlayerTemplateData.getInstance();
        CharInfoTable.getInstance();
        AdminData.getInstance();
        PetDataTable.getInstance();
        PetTypeData.getInstance();
        PetExtractData.getInstance();
        CubicData.getInstance();
        CharSummonTable.getInstance().init();
        BeautyShopData.getInstance();
        MentorManager.getInstance();
        VipManager.getInstance();

        if (Config.PREMIUM_SYSTEM_ENABLED)
        {
            PremiumManager.getInstance();
        }

        ClanLevelData.getInstance();
        ClanTable.getInstance();
        ResidenceFunctionsData.getInstance();
        ClanHallData.getInstance();
        ClanHallAuctionManager.getInstance();
        ClanEntryManager.getInstance();

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
        if (Config.SELLBUFF_ENABLED)
        {
            SellBuffsManager.getInstance();
        }

        if (Config.MULTILANG_ENABLE)
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

		if (Config.SAVE_DROPPED_ITEM)
		{
			ItemsOnGroundManager.getInstance();
		}

		if (Config.AUTODESTROY_ITEM_AFTER > 0 || Config.HERB_AUTO_DESTROY_TIME > 0)
		{
			ItemsAutoDestroyTaskManager.getInstance();
		}

		MonsterRace.getInstance();

		TaskManager.getInstance();

		DailyTaskManager.getInstance();

		AntiFeedManager.getInstance().registerEvent(AntiFeedManager.GAME_ID);

		if (Config.ALLOW_MAIL)
		{
			MailManager.getInstance();
		}

		if (Config.CUSTOM_MAIL_MANAGER_ENABLED)
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

		if ((Config.OFFLINE_TRADE_ENABLE || Config.OFFLINE_CRAFT_ENABLE) && Config.RESTORE_OFFLINERS)
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