using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Quests.NewQuestData;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Attendance;
using L2Dn.GameServer.Network.OutgoingPackets.Collections;
using L2Dn.GameServer.Network.OutgoingPackets.DailyMissions;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.GameServer.Network.OutgoingPackets.Friends;
using L2Dn.GameServer.Network.OutgoingPackets.HuntPasses;
using L2Dn.GameServer.Network.OutgoingPackets.LimitShop;
using L2Dn.GameServer.Network.OutgoingPackets.MagicLamp;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeDonation;
using L2Dn.GameServer.Network.OutgoingPackets.Quests;
using L2Dn.GameServer.Network.OutgoingPackets.RandomCraft;
using L2Dn.GameServer.Network.OutgoingPackets.Settings;
using L2Dn.GameServer.Network.OutgoingPackets.SteadyBoxes;
using L2Dn.GameServer.Network.OutgoingPackets.Subjugation;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct EnterWorldPacket: IIncomingPacket<GameSession>
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EnterWorldPacket)); 
	
    public void ReadContent(PacketBitReader reader)
    {
        // 447 protocol
        // 20 bytes - tracert
        // 4 bytes - unknown int
        // 4 bytes - unknown int
        // 4 bytes - unknown int
        // 4 bytes - unknown int
        // 64 bytes - unknown data
        // 4 bytes - unknown int
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
		{
			_logger.Error("EnterWorld failed! player returned 'null'.");
			Disconnection.of(session).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		session.State = GameSessionState.InGame;
		
		// string[] adress = new String[5];
		// for (int i = 0; i < 5; i++)
		// {
		// 	adress[i] = _tracert[i][0] + "." + _tracert[i][1] + "." + _tracert[i][2] + "." + _tracert[i][3];
		// }
		
		//LoginServerThread.getInstance().sendClientTracert(player.getAccountName(), adress);
		//client.setClientTracert(_tracert);
		
		connection.Send(new UserInfoPacket(player));
		
		// Restore to instanced area if enabled
		if (Config.RESTORE_PLAYER_INSTANCE)
		{
			PlayerVariables vars = player.getVariables();
			Instance instance = InstanceManager.getInstance().getPlayerInstance(player, false);
			if ((instance != null) && (instance.getId() == vars.getInt("INSTANCE_RESTORE", 0)))
			{
				player.setInstance(instance);
			}
			
			vars.remove("INSTANCE_RESTORE");
		}
		
		if (!player.isGM())
		{
			player.updatePvpTitleAndColor(false);
		}

		// Apply special GM properties to the GM when entering
		else
		{
			if (Config.GM_STARTUP_BUILDER_HIDE &&
			    AdminData.getInstance().hasAccess("admin_hide", player.getAccessLevel()))
			{
				BuilderUtil.setHiding(player, true);
				BuilderUtil.sendSysMessage(player, "hide is default for builder.");
				BuilderUtil.sendSysMessage(player, "FriendAddOff is default for builder.");
				BuilderUtil.sendSysMessage(player, "whisperoff is default for builder.");

				// It isn't recommend to use the below custom L2J GMStartup functions together with retail-like
				// GMStartupBuilderHide, so breaking the process at that stage.
			}
			else
			{
				if (Config.GM_STARTUP_INVULNERABLE &&
				    AdminData.getInstance().hasAccess("admin_invul", player.getAccessLevel()))
				{
					player.setInvul(true);
				}

				if (Config.GM_STARTUP_INVISIBLE &&
				    AdminData.getInstance().hasAccess("admin_invisible", player.getAccessLevel()))
				{
					player.setInvisible(true);
					player.getEffectList().startAbnormalVisualEffect(AbnormalVisualEffect.STEALTH);
				}

				if (Config.GM_STARTUP_SILENCE &&
				    AdminData.getInstance().hasAccess("admin_silence", player.getAccessLevel()))
				{
					player.setSilenceMode(true);
				}

				if (Config.GM_STARTUP_DIET_MODE &&
				    AdminData.getInstance().hasAccess("admin_diet", player.getAccessLevel()))
				{
					player.setDietMode(true);
					player.refreshOverloaded(true);
				}
			}

			if (Config.GM_STARTUP_AUTO_LIST &&
			    AdminData.getInstance().hasAccess("admin_gmliston", player.getAccessLevel()))
			{
				AdminData.getInstance().addGm(player, false);
			}
			else
			{
				AdminData.getInstance().addGm(player, true);
			}

			if (Config.GM_GIVE_SPECIAL_SKILLS)
			{
				SkillTreeData.getInstance().addSkills(player, false);
			}

			if (Config.GM_GIVE_SPECIAL_AURA_SKILLS)
			{
				SkillTreeData.getInstance().addSkills(player, true);
			}
		}

		// Set dead status if applies
		if (player.getCurrentHp() < 0.5)
		{
			player.setDead(true);
		}
		
		bool showClanNotice = false;
		
		// Clan related checks are here
		Clan clan = player.getClan();
		if (clan != null)
		{
			notifyClanMembers(connection, player);
			notifySponsorOrApprentice(player);
			
			foreach (Siege siege in SiegeManager.getInstance().getSieges())
			{
				if (!siege.isInProgress())
				{
					continue;
				}
				
				if (siege.checkIsAttacker(clan))
				{
					player.setSiegeState(1);
					player.setSiegeSide(siege.getCastle().getResidenceId());
				}
				
				else if (siege.checkIsDefender(clan))
				{
					player.setSiegeState(2);
					player.setSiegeSide(siege.getCastle().getResidenceId());
				}
			}
			
			foreach (FortSiege siege in FortSiegeManager.getInstance().getSieges())
			{
				if (!siege.isInProgress())
				{
					continue;
				}
				
				if (siege.checkIsAttacker(clan))
				{
					player.setSiegeState(1);
					player.setSiegeSide(siege.getFort().getResidenceId());
				}
				
				else if (siege.checkIsDefender(clan))
				{
					player.setSiegeState(2);
					player.setSiegeSide(siege.getFort().getResidenceId());
				}
			}
			
			// Residential skills support
			if (clan.getCastleId() > 0)
			{
				Castle castle = CastleManager.getInstance().getCastleByOwner(clan);
				if (castle != null)
				{
					castle.giveResidentialSkills(player);
				}
			}
			
			if (clan.getFortId() > 0)
			{
				Fort fort = FortManager.getInstance().getFortByOwner(clan);
				if (fort != null)
				{
					fort.giveResidentialSkills(player);
				}
			}
			
			showClanNotice = clan.isNoticeEnabled();
		}
		
		// Send time.
		connection.Send(new ExEnterWorldPacket());
		
		// Send Macro List
		player.getMacros().sendAllMacros();
		
		// Send Teleport Bookmark List
		connection.Send(new ExGetBookMarkInfoPacket(player));
		
		// Send Item List
		connection.Send(new ItemListPacket(1, player));
		connection.Send(new ItemListPacket(2, player));
		
		// Send Quest Item List
		connection.Send(new ExQuestItemListPacket(1, player));
		connection.Send(new ExQuestItemListPacket(2, player));
		
		// Send Shortcuts
		connection.Send(new ShortCutInitPacket(player));
		
		// Send Action list
		connection.Send(ExBasicActionListPacket.STATIC_PACKET);
		
		// Send blank skill list
		connection.Send(new SkillListPacket(0));
		
		// Send GG check
		// player.queryGameGuard();
		
		// Send Dye Information
		connection.Send(new HennaInfoPacket(player));
		
		// Send Skill list
		player.sendSkillList();
		
		// Send EtcStatusUpdate
		connection.Send(new EtcStatusUpdatePacket(player));
		
		// Calculate stat increase skills.
		player.calculateStatIncreaseSkills();

		SystemMessagePacket sm;
		
		// Clan packets
		if (clan != null)
		{
			clan.broadcastToOnlineMembers(new PledgeShowMemberListUpdatePacket(player));
			PledgeShowMemberListAllPacket.sendAllTo(player);
			clan.broadcastToOnlineMembers(new ExPledgeCountPacket(clan));
			connection.Send(new PledgeSkillListPacket(clan));
			ClanHall ch = ClanHallData.getInstance().getClanHallByClan(clan);
			if ((ch != null) && (ch.getCostFailDay() > 0) && (ch.getResidenceId() < 186))
			{
				sm = new SystemMessagePacket(SystemMessageId.THE_PAYMENT_FOR_YOUR_CLAN_HALL_HAS_NOT_BEEN_MADE_PLEASE_DEPOSIT_THE_NECESSARY_AMOUNT_OF_ADENA_TO_YOUR_CLAN_WAREHOUSE_BY_S1_TOMORROW);
				sm.Params.addInt(ch.getLease());
				connection.Send(sm);
			}
		}
		else
		{
			connection.Send(ExPledgeWaitingListAlarmPacket.STATIC_PACKET);
		}
		
		// Send SubClass Info
		connection.Send(new ExSubJobInfoPacket(player, SubclassInfoType.NO_CHANGES));
		
		// Send Inventory Info
		connection.Send(new ExUserInfoInventoryWeightPacket(player));
		
		// Send Adena / Inventory Count Info
		connection.Send(new ExAdenaInvenCountPacket(player));
		
		// Send LCoin count.
		connection.Send(new ExBloodyCoinCountPacket(player));
		
		// Send honor coin count.
		connection.Send(new ExPledgeCoinInfoPacket(player));
		
		// Send VIP/Premium Info
		connection.Send(new ExBrPremiumStatePacket(player));
		
		// Send Challenge Point info.
		connection.Send(new ExEnchantChallengePointInfoPacket(player));
		
		// Send Unread Mail Count
		if (MailManager.getInstance().hasUnreadPost(player))
		{
			int mailCount = MailManager.getInstance().getUnreadCount(player);
			connection.Send(new ExUnReadMailCountPacket(mailCount));
		}
		
		// Faction System
		if (Config.FACTION_SYSTEM_ENABLED)
		{
			if (player.isGood())
			{
				player.getAppearance().setNameColor(Config.FACTION_GOOD_NAME_COLOR);
				player.getAppearance().setTitleColor(Config.FACTION_GOOD_NAME_COLOR);
				player.sendMessage("Welcome " + player.getName() + ", you are fighting for the " + Config.FACTION_GOOD_TEAM_NAME + " faction.");
				connection.Send(new ExShowScreenMessagePacket("Welcome " + player.getName() + ", you are fighting for the " + Config.FACTION_GOOD_TEAM_NAME + " faction.", 10000));
			}
			else if (player.isEvil())
			{
				player.getAppearance().setNameColor(Config.FACTION_EVIL_NAME_COLOR);
				player.getAppearance().setTitleColor(Config.FACTION_EVIL_NAME_COLOR);
				player.sendMessage("Welcome " + player.getName() + ", you are fighting for the " + Config.FACTION_EVIL_TEAM_NAME + " faction.");
				connection.Send(new ExShowScreenMessagePacket("Welcome " + player.getName() + ", you are fighting for the " + Config.FACTION_EVIL_TEAM_NAME + " faction.", 10000));
			}
		}
		
		Quest.playerEnter(player);
		
		// Send quest list.
		if (!Config.DISABLE_TUTORIAL)
		{
			connection.Send(new ExQuestNotificationAllPacket(player));
			foreach (NewQuest newQuest in NewQuestData.getInstance().getQuests())
			{
				Quest quest = QuestManager.getInstance().getQuest(newQuest.getId());
				if (quest != null)
				{
					QuestState questState = player.getQuestState(quest.getScriptName());
					if ((questState == null) && quest.canStartQuest(player) && !newQuest.getConditions().getSpecificStart())
					{
						connection.Send(new ExQuestDialogPacket(quest.getId(), QuestDialogType.ACCEPT));
						break; // Only send first dialog.
					}
				}
			}
		}
		
		if (Config.PLAYER_SPAWN_PROTECTION > 0)
		{
			player.setSpawnProtection(true);
		}
		
		player.spawnMe(player.getX(), player.getY(), player.getZ());
		connection.Send(new ExRotationPacket(player.getObjectId(), player.getHeading()));
		
		if (player.isCursedWeaponEquipped())
		{
			CursedWeaponsManager.getInstance().getCursedWeapon(player.getCursedWeaponEquippedId()).cursedOnLogin();
		}
		
		if (Config.PC_CAFE_ENABLED)
		{
			if (player.getPcCafePoints() > 0)
			{
				connection.Send(new ExPcCafePointInfoPacket(player.getPcCafePoints(), 0, 1));
			}
			else
			{
				connection.Send(new ExPcCafePointInfoPacket());
			}
		}
		
		// Expand Skill
		player.sendStorageMaxCount();
		
		// Send Equipped Items
		connection.Send(new ExUserInfoEquipSlotPacket(player));
		
		// Friend list
		connection.Send(new L2FriendListPacket(player));
		
		sm = new SystemMessagePacket(SystemMessageId.YOUR_FRIEND_S1_JUST_LOGGED_IN);
		sm.Params.addString(player.getName());
		foreach (int id in player.getFriendList())
		{
			WorldObject obj = World.getInstance().findObject(id);
			if (obj != null)
			{
				obj.sendPacket(sm);
			}
		}
		
		connection.Send(new SystemMessagePacket(SystemMessageId.WELCOME_TO_THE_WORLD_OF_LINEAGE_II));
		
		AnnouncementsTable.getInstance().showAnnouncements(player);
		
		if ((Config.SERVER_RESTART_SCHEDULE_ENABLED) && (Config.SERVER_RESTART_SCHEDULE_MESSAGE))
		{
			connection.Send(new CreatureSayPacket(null, ChatType.BATTLEFIELD, "[SERVER]",
				"Next restart is scheduled at " + ServerRestartManager.getInstance().getNextRestartTime() + "."));
		}
		
		if (showClanNotice)
		{
			HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/clanNotice.htm");
			helper.Replace("%clan_name%", player.getClan().getName());
			helper.Replace("%notice_text%", player.getClan().getNotice().replaceAll("(\r\n|\n)", "<br>"));
			connection.Send(new NpcHtmlMessagePacket(helper));
		}
		else if (Config.SERVER_NEWS)
		{
			HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/servnews.htm");
			connection.Send(new NpcHtmlMessagePacket(helper));
		}

		if (Config.PETITIONING_ALLOWED)
		{
			PetitionManager.getInstance().checkPetitionMessages(player);
		}
		
		player.onPlayerEnter();
		
		connection.Send(new SkillCoolTimePacket(player));
		connection.Send(new ExVoteSystemInfoPacket(player));
		
		if (player.isAlikeDead()) // dead or fake dead
		{
			// no broadcast needed since the player will already spawn dead to others
			connection.Send(new DiePacket(player));
		}
		
		foreach (Item item in player.getInventory().getItems())
		{
			if (item.isTimeLimitedItem())
			{
				item.scheduleLifeTimeTask();
			}
			if (item.isShadowItem() && item.isEquipped())
			{
				item.decreaseMana(false);
			}
		}
		
		foreach (Item whItem in player.getWarehouse().getItems())
		{
			if (whItem.isTimeLimitedItem())
			{
				whItem.scheduleLifeTimeTask();
			}
		}
		
		if (player.getClanJoinExpiryTime() > DateTime.UtcNow)
		{
			connection.Send(new SystemMessagePacket(SystemMessageId.YOU_ARE_DISMISSED_FROM_A_CLAN_YOU_CANNOT_JOIN_ANOTHER_FOR_24_H));
		}
		
		// remove combat flag before teleporting
		if (player.getInventory().getItemByItemId(FortManager.ORC_FORTRESS_FLAG) != null)
		{
			Fort fort = FortManager.getInstance().getFort(player);
			if (fort != null)
			{
				FortSiegeManager.getInstance().dropCombatFlag(player, fort.getResidenceId());
			}
			else
			{
				long slot = player.getInventory().getSlotFromItem(player.getInventory().getItemByItemId(FortManager.ORC_FORTRESS_FLAG));
				player.getInventory().unEquipItemInBodySlot(slot);
				player.destroyItem("CombatFlag", player.getInventory().getItemByItemId(FortManager.ORC_FORTRESS_FLAG), null, true);
			}
		}
		
		// Attacker or spectator logging in to a siege zone.
		// Actually should be checked for inside castle only?
		if (!player.canOverrideCond(PlayerCondOverride.ZONE_CONDITIONS) && player.isInsideZone(ZoneId.SIEGE) &&
		    (!player.isInSiege() || (player.getSiegeState() < 2)))
		{
			player.teleToLocation(TeleportWhereType.TOWN);
		}

		// Over-enchant protection.
		if (Config.OVER_ENCHANT_PROTECTION && !player.isGM())
		{
			bool punish = false;
			foreach (Item item in player.getInventory().getItems())
			{
				if (item.isEquipable() //
				    && ((item.isWeapon() && (item.getEnchantLevel() >
				                             EnchantItemGroupsData.getInstance().getMaxWeaponEnchant())) //
				        || ((item.getTemplate().getType2() == ItemTemplate.TYPE2_ACCESSORY) && (item.getEnchantLevel() >
					        EnchantItemGroupsData.getInstance().getMaxAccessoryEnchant())) //
				        || (item.isArmor() && (item.getTemplate().getType2() != ItemTemplate.TYPE2_ACCESSORY) &&
				            (item.getEnchantLevel() > EnchantItemGroupsData.getInstance().getMaxArmorEnchant()))))
				{
					_logger.Info("Over-enchanted (+" + item.getEnchantLevel() + ") " + item +
					             " has been removed from " + player);
					player.getInventory().destroyItem("Over-enchant protection", item, player, null);
					punish = true;
				}
			}

			if (punish && (Config.OVER_ENCHANT_PUNISHMENT != IllegalActionPunishmentType.NONE))
			{
				player.sendMessage("[Server]: You have over-enchanted items!");
				player.sendMessage("[Server]: Respect our server rules.");
				connection.Send(new ExShowScreenMessagePacket("You have over-enchanted items!", 6000));
				Util.handleIllegalPlayerAction(player, player.getName() + " has over-enchanted items.", Config.OVER_ENCHANT_PUNISHMENT);
			}
		}
		
		// Remove demonic weapon if character is not cursed weapon equipped.
		if ((player.getInventory().getItemByItemId(8190) != null) && !player.isCursedWeaponEquipped())
		{
			player.destroyItem("Zariche", player.getInventory().getItemByItemId(8190), null, true);
		}
		
		if ((player.getInventory().getItemByItemId(8689) != null) && !player.isCursedWeaponEquipped())
		{
			player.destroyItem("Akamanah", player.getInventory().getItemByItemId(8689), null, true);
		}
		
		if (Config.ALLOW_MAIL)
		{
			if (MailManager.getInstance().hasUnreadPost(player))
			{
				connection.Send(new ExNoticePostArrivedPacket(false));
			}
		}
		
		if (Config.WELCOME_MESSAGE_ENABLED)
		{
			connection.Send(new ExShowScreenMessagePacket(Config.WELCOME_MESSAGE_TEXT, Config.WELCOME_MESSAGE_TIME));
		}
		
		if (!player.getPremiumItemList().isEmpty())
		{
			connection.Send(ExNotifyPremiumItemPacket.STATIC_PACKET);
		}
		
		if ((Config.OFFLINE_TRADE_ENABLE || Config.OFFLINE_CRAFT_ENABLE) && Config.STORE_OFFLINE_TRADE_IN_REALTIME)
		{
			OfflineTraderTable.getInstance().onTransaction(player, true, false);
		}
		
		// Check if expoff is enabled.
		if (player.getVariables().getBoolean("EXPOFF", false))
		{
			player.disableExpGain();
			player.sendMessage("Experience gain is disabled.");
		}
		
		player.broadcastUserInfo();
		
		if (BeautyShopData.getInstance().hasBeautyData(player.getRace(), player.getAppearance().getSex()))
		{
			connection.Send(new ExBeautyItemListPacket(player));
		}
		
		if (Config.ENABLE_WORLD_CHAT)
		{
			connection.Send(new ExWorldCharCntPacket(player));
		}
		
		// Initial mission level progress for correct show RewardList.
		player.getMissionLevelProgress();
		connection.Send(new ExConnectedTimeAndGettableRewardPacket(player));
		connection.Send(new ExOneDayReceiveRewardListPacket(player, true));
		
		// Handle soulshots, disable all on EnterWorld
		connection.Send(new ExAutoSoulShotPacket(0, true, 0));
		connection.Send(new ExAutoSoulShotPacket(0, true, 1));
		connection.Send(new ExAutoSoulShotPacket(0, true, 2));
		connection.Send(new ExAutoSoulShotPacket(0, true, 3));
		
		// Auto use restore.
		player.restoreAutoShortcuts();
		player.restoreAutoSettings();
		
		// Client settings restore.
		player.getClientSettings();
		connection.Send(new ExItemAnnounceSettingPacket(player.getClientSettings().isAnnounceEnabled()));
		
		// Fix for equipped item skills
		if (!player.getEffectList().getCurrentAbnormalVisualEffects().isEmpty())
		{
			player.updateAbnormalVisualEffects();
		}
		
		// Death Knight death points init.
		if (player.isDeathKnight())
		{
			// Send twice.
			player.setDeathPoints(500);
			player.setDeathPoints(player.getVariables().getInt(PlayerVariables.DEATH_POINT_COUNT, 0));
		}
		// Vanguard beast points init.
		else if (player.isVanguard())
		{
			player.setBeastPoints(1000);
			player.setBeastPoints(player.getVariables().getInt(PlayerVariables.BEAST_POINT_COUNT, 1000));
		}
		// Assassin points init.
		else if (player.isAssassin() && player.isInCategory(CategoryType.FOURTH_CLASS_GROUP))
		{
			player.setAssassinationPoints(player.getVariables().getInt(PlayerVariables.ASSASSINATION_POINT_COUNT, 0));
		}
		
		// Sayha's Grace.
		if (Config.ENABLE_VITALITY)
		{
			connection.Send(new ExVitalityEffectInfoPacket(player));
		}
		
		if (Config.ENABLE_MAGIC_LAMP)
		{
			connection.Send(new ExMagicLampInfoPacket(player));
		}
		
		if (Config.ENABLE_RANDOM_CRAFT)
		{
			connection.Send(new ExCraftInfoPacket(player));
		}
		
		if (Config.ENABLE_HUNT_PASS)
		{
			connection.Send(new HuntPassSimpleInfoPacket(player));
		}
		
		if (Config.ENABLE_ACHIEVEMENT_BOX)
		{
			connection.Send(new ExSteadyBoxUiInitPacket(player));
		}
		
		if ((player.getLevel() >= 40) && (player.getClassId().GetLevel() > 1))
		{
			player.initElementalSpirits();
		}
		
		for (int category = 1; category <= 7; category++)
		{
			connection.Send(new ExCollectionInfoPacket(player, category));
		}
		
		connection.Send(new ExCollectionActiveEventPacket());
		
		connection.Send(new ExSubjugationSidebarPacket(player, player.getPurgePoints().get(player.getPurgeLastCategory())));
		
		connection.Send(new ItemDeletionInfoPacket());
		
		player.applyKarmaPenalty();
		
		SiegeManager.getInstance().sendSiegeInfo(player);
		
		// Activate first agathion when available.
		Item agathion = player.getInventory().unEquipItemInBodySlot(ItemTemplate.SLOT_AGATHION);
		if (agathion != null)
		{
			player.getInventory().equipItemAndRecord(agathion);
		}
		
		// Old ammunition check.
		Item leftHandItem = player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_LHAND);
		if ((leftHandItem != null) && ((leftHandItem.getItemType() == EtcItemType.ARROW) || (leftHandItem.getItemType() == EtcItemType.BOLT) || (leftHandItem.getItemType() == EtcItemType.ELEMENTAL_ORB)))
		{
			player.getInventory().unEquipItemInBodySlot(Inventory.PAPERDOLL_LHAND);
		}
		
		// World Trade.
		WorldExchangeManager.getInstance().checkPlayerSellAlarm(player);
		
		// Dual inventory.
		player.restoreDualInventory();
		
		if (Config.ENABLE_ATTENDANCE_REWARDS)
		{
			AttendanceInfoHolder attendanceInfo = player.getAttendanceInfo();
			if (attendanceInfo.isRewardAvailable())
			{
				player.setAttendanceDelay(Config.ATTENDANCE_REWARD_DELAY);
			}
			
			ThreadPool.schedule(() =>
			{
				// Check if player can receive reward today.
				if (attendanceInfo.isRewardAvailable())
				{
					int lastRewardIndex = attendanceInfo.getRewardIndex() + 1;
					connection.Send(new ExShowScreenMessagePacket("Your attendance day " + lastRewardIndex + " reward is ready.", ExShowScreenMessagePacket.TOP_CENTER, 7000, 0, true, true));
					player.sendMessage("Your attendance day " + lastRewardIndex + " reward is ready.");
					player.sendMessage("Click on General Menu -> Attendance Check.");
					if (Config.ATTENDANCE_POPUP_WINDOW)
					{
						connection.Send(new ExVipAttendanceListPacket(player));
						connection.Send(new ExVipAttendanceNotifyPacket());
					}
				}
			}, Config.ATTENDANCE_REWARD_DELAY);
			
			if (Config.ATTENDANCE_POPUP_START)
			{
				connection.Send(new ExVipAttendanceListPacket(player));
				connection.Send(new ExVipAttendanceNotifyPacket());
			}
		}
		
		// Delayed HWID checks.
		if (Config.HARDWARE_INFO_ENABLED)
		{
			// ThreadPool.schedule(() =>
			// {
			// 	// Generate trace string.
			// 	StringBuilder sb = new StringBuilder();
			// 	foreach (int[] i in _tracert)
			// 	{
			// 		foreach (int j in i)
			// 		{
			// 			sb.Append(j);
			// 			sb.Append(".");
			// 		}
			// 	}
			// 	String trace = sb.ToString();
			// 	
			// 	// Get hardware info from client.
			// 	ClientHardwareInfoHolder hwInfo = client.getHardwareInfo();
			// 	if (hwInfo != null)
			// 	{
			// 		hwInfo.store(player);
			// 		TRACE_HWINFO.put(trace, hwInfo);
			// 	}
			// 	else
			// 	{
			// 		// Get hardware info from stored tracert map.
			// 		hwInfo = TRACE_HWINFO.get(trace);
			// 		if (hwInfo != null)
			// 		{
			// 			hwInfo.store(player);
			// 			client.setHardwareInfo(hwInfo);
			// 		}
			// 		// Get hardware info from account variables.
			// 		else
			// 		{
			// 			String storedInfo = player.getAccountVariables().getString(AccountVariables.HWID, "");
			// 			if (!storedInfo.isEmpty())
			// 			{
			// 				hwInfo = new ClientHardwareInfoHolder(storedInfo);
			// 				TRACE_HWINFO.put(trace, hwInfo);
			// 				client.setHardwareInfo(hwInfo);
			// 			}
			// 		}
			// 	}
			// 	
			// 	// Banned?
			// 	if ((hwInfo != null) && PunishmentManager.getInstance().hasPunishment(hwInfo.getMacAddress(), PunishmentAffect.HWID, PunishmentType.BAN))
			// 	{
			// 		Disconnection.of(client).defaultSequence(LeaveWorld.STATIC_PACKET);
			// 		return;
			// 	}
			// 	
			// 	// Check max players.
			// 	if (Config.KICK_MISSING_HWID && (hwInfo == null))
			// 	{
			// 		Disconnection.of(client).defaultSequence(LeaveWorld.STATIC_PACKET);
			// 	}
			// 	else if (Config.MAX_PLAYERS_PER_HWID > 0)
			// 	{
			// 		int count = 0;
			// 		foreach (Player plr in World.getInstance().getPlayers())
			// 		{
			// 			if (plr.getOnlineStatus() == CharacterOnlineStatus.Online)
			// 			{
			// 				ClientHardwareInfoHolder hwi = plr.getClient().getHardwareInfo();
			// 				if ((hwi != null) && hwi.equals(hwInfo))
			// 				{
			// 					count++;
			// 				}
			// 			}
			// 		}
			// 		if (count > Config.MAX_PLAYERS_PER_HWID)
			// 		{
			// 			Disconnection.of(session).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
			// 		}
			// 	}
			// }, 5000);
		}
		
		// Chat banned icon.
		ThreadPool.schedule(() =>
		{
			if (player.isChatBanned())
			{
				player.getEffectList().startAbnormalVisualEffect(AbnormalVisualEffect.NO_CHAT);
			}
		}, 5500);
		
		// EnterWorld has finished.
		player.setEnteredWorld();
		
		if ((player.hasPremiumStatus() || !Config.PC_CAFE_ONLY_PREMIUM) && Config.PC_CAFE_RETAIL_LIKE)
		{
			PcCafePointsManager.getInstance().run(player);
		}

		return ValueTask.CompletedTask;
    }
	
	/**
	 * @param player
	 */
	private static void notifyClanMembers(Connection connection, Player player)
	{
		Clan clan = player.getClan();
		if (clan != null)
		{
			clan.getClanMember(player.getObjectId()).setPlayer(player);
			
			SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.CLAN_MEMBER_S1_HAS_LOGGED_IN);
			msg.Params.addString(player.getName());
			clan.broadcastToOtherOnlineMembers(msg, player);
			clan.broadcastToOtherOnlineMembers(new PledgeShowMemberListUpdatePacket(player), player);
			
			connection.Send(new ExPledgeContributionListPacket(clan.getMembers()));
		}
	}
	
	/**
	 * @param player
	 */
	private void notifySponsorOrApprentice(Player player)
	{
		if (player.getSponsor() != null)
		{
			Player sponsor = World.getInstance().getPlayer(player.getSponsor().Value);
			if (sponsor != null)
			{
				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOUR_MENTEE_S1_HAS_LOGGED_IN);
				msg.Params.addString(player.getName());
				sponsor.sendPacket(msg);
			}
		}
		else if (player.getApprentice() != 0)
		{
			Player apprentice = World.getInstance().getPlayer(player.getApprentice());
			if (apprentice != null)
			{
				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOUR_SPONSOR_C1_HAS_LOGGED_IN);
				msg.Params.addString(player.getName());
				apprentice.sendPacket(msg);
			}
		}
	}
}
