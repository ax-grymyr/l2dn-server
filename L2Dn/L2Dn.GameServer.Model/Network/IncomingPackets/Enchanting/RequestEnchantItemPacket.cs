using System.Text;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting;

public struct RequestEnchantItemPacket: IIncomingPacket<GameSession>
{
	private static readonly Logger _enchantLogger = LogManager.GetLogger("enchant.items");
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        
        // + Unknown bool
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;
		
		EnchantItemRequest request = player.getRequest<EnchantItemRequest>();
		if ((request == null) || request.isProcessing())
			return ValueTask.CompletedTask;
		
		request.setEnchantingItem(_objectId);
		request.setProcessing(true);
		
		if (!player.isOnline() || session.IsDetached)
		{
			player.removeRequest<EnchantItemRequest>();
			return ValueTask.CompletedTask;
		}
		
		if (player.isProcessingTransaction() || player.isInStoreMode())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_ENCHANT_WHILE_OPERATING_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
			player.removeRequest<EnchantItemRequest>();
			return ValueTask.CompletedTask;
		}
		
		Item item = request.getEnchantingItem();
		Item scroll = request.getEnchantingScroll();
		Item support = request.getSupportItem();
		if ((item == null) || (scroll == null))
		{
			player.removeRequest<EnchantItemRequest>();
			return ValueTask.CompletedTask;
		}
		
		// Template for scroll.
		EnchantScroll scrollTemplate = EnchantItemData.getInstance().getEnchantScroll(scroll);
		if (scrollTemplate == null)
			return ValueTask.CompletedTask;
		
		// Template for support item, if exist.
		EnchantSupportItem supportTemplate = null;
		if (support != null)
		{
			supportTemplate = EnchantItemData.getInstance().getSupportItem(support);
			if (supportTemplate == null)
			{
				player.removeRequest<EnchantItemRequest>();
				return ValueTask.CompletedTask;
			}
		}
		
		// First validation check, also over enchant check.
		if (!scrollTemplate.isValid(item, supportTemplate) || (Config.DISABLE_OVER_ENCHANTING && ((item.getEnchantLevel() == scrollTemplate.getMaxEnchantLevel()) || (!(item.getTemplate().getEnchantLimit() == 0) && (item.getEnchantLevel() == item.getTemplate().getEnchantLimit())))))
		{
			player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
			player.removeRequest<EnchantItemRequest>();
			player.sendPacket(new EnchantResultPacket(EnchantResultPacket.ERROR, null, null, 0));
			return ValueTask.CompletedTask;
		}
		
		// Fast auto-enchant cheat check.
		// if ((request.getTimestamp() == 0) || ((System.currentTimeMillis() - request.getTimestamp()) < 600))
		// {
		// Util.handleIllegalPlayerAction(player, player + " use autoenchant program ", Config.DEFAULT_PUNISH);
		// player.removeRequest(request.getClass());
		// player.sendPacket(new EnchantResultPacket(EnchantResultPacket.ERROR, null, null, 0));
		// return;
		// }
		
		// Attempting to destroy scroll.
		if (player.getInventory().destroyItem("Enchant", scroll.getObjectId(), 1, player, item) == null)
		{
			player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			Util.handleIllegalPlayerAction(player, player + " tried to enchant with a scroll he doesn't have", Config.DEFAULT_PUNISH);
			player.removeRequest<EnchantItemRequest>();
			player.sendPacket(new EnchantResultPacket(EnchantResultPacket.ERROR, null, null, 0));
			return ValueTask.CompletedTask;
		}
		
		// Attempting to destroy support if exists.
		if ((support != null) && (player.getInventory().destroyItem("Enchant", support.getObjectId(), 1, player, item) == null))
		{
			player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			Util.handleIllegalPlayerAction(player, player + " tried to enchant with a support item he doesn't have", Config.DEFAULT_PUNISH);
			player.removeRequest<EnchantItemRequest>();
			player.sendPacket(new EnchantResultPacket(EnchantResultPacket.ERROR, null, null, 0));
			return ValueTask.CompletedTask;
		}

		List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
		lock (item)
		{
			// Last validation check.
			if ((item.getOwnerId() != player.getObjectId()) || !item.isEnchantable())
			{
				player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
				player.removeRequest<EnchantItemRequest>();
				player.sendPacket(new EnchantResultPacket(EnchantResultPacket.ERROR, null, null, 0));
				return ValueTask.CompletedTask;
			}
			
			EnchantResultType resultType = scrollTemplate.calculateSuccess(player, item, supportTemplate);
			EnchantChallengePointData.EnchantChallengePointsItemInfo info = EnchantChallengePointData.getInstance().getInfoByItemId(item.getId());
			int challengePointsGroupId = -1;
			int challengePointsOptionIndex = -1;
			if (info != null)
			{
				int groupId = info.GroupId;
				if (groupId == player.getChallengeInfo().getChallengePointsPendingRecharge()[0])
				{
					challengePointsGroupId = player.getChallengeInfo().getChallengePointsPendingRecharge()[0];
					challengePointsOptionIndex = player.getChallengeInfo().getChallengePointsPendingRecharge()[1];
				}
			}
			
			switch (resultType)
			{
				case EnchantResultType.ERROR:
				{
					player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
					player.removeRequest<EnchantItemRequest>();
					player.sendPacket(new EnchantResultPacket(EnchantResultPacket.ERROR, null, null, 0));
					break;
				}
				
				case EnchantResultType.SUCCESS:
				{
					ItemTemplate it = item.getTemplate();
					if (scrollTemplate.isCursed())
					{
						// Blessed enchant: Enchant value down by 1.
						connection.Send(SystemMessageId.THE_ENCHANT_VALUE_IS_DECREASED_BY_1);
						item.setEnchantLevel(item.getEnchantLevel() - 1);
					}
					// Increase enchant level only if scroll's base template has chance, some armors can success over +20 but they shouldn't have increased.
					else if (scrollTemplate.getChance(player, item) > 0)
					{
						if (item.isEquipped())
						{
							item.clearSpecialAbilities();
							item.clearEnchantStats();
						}
						
						if (supportTemplate != null)
						{
							item.setEnchantLevel(Math.Min(item.getEnchantLevel() + Rnd.get(supportTemplate.getRandomEnchantMin(), supportTemplate.getRandomEnchantMax()), supportTemplate.getMaxEnchantLevel()));
						}
						if (supportTemplate == null)
						{
							item.setEnchantLevel(Math.Min(item.getEnchantLevel() + Rnd.get(scrollTemplate.getRandomEnchantMin(), scrollTemplate.getRandomEnchantMax()), scrollTemplate.getMaxEnchantLevel()));
						}
						else
						{
							int enchantValue = 1;
							if ((challengePointsGroupId > 0) && (challengePointsOptionIndex == EnchantChallengePointData.OptionOverUpProb))
							{
								EnchantChallengePointData.EnchantChallengePointsOptionInfo optionInfo = EnchantChallengePointData.getInstance().getOptionInfo(challengePointsGroupId, challengePointsOptionIndex);
								if ((optionInfo != null) && (item.getEnchantLevel() >= optionInfo.MinEnchant) && (item.getEnchantLevel() <= optionInfo.MaxEnchant) && (Rnd.get(100) < optionInfo.Chance))
								{
									enchantValue = 2;
								}
							}
							item.setEnchantLevel(item.getEnchantLevel() + enchantValue);
						}
						
						if (item.isEquipped())
						{
							item.applySpecialAbilities();
							item.applyEnchantStats();
						}
						
						item.updateDatabase();
						
						itemsToUpdate.Add(new ItemInfo(item, ItemChangeType.MODIFIED));
						if (scroll.getCount() > 0)
						{
							itemsToUpdate.Add(new ItemInfo(scroll, ItemChangeType.MODIFIED));
						}
						else
						{
							itemsToUpdate.Add(new ItemInfo(scroll, ItemChangeType.REMOVED));
						}
						if (support != null)
						{
							if (support.getCount() > 0)
							{
								itemsToUpdate.Add(new ItemInfo(support, ItemChangeType.MODIFIED));
							}
							else
							{
								itemsToUpdate.Add(new ItemInfo(support, ItemChangeType.REMOVED));
							}
						}
					}
					player.sendPacket(new EnchantResultPacket(EnchantResultPacket.SUCCESS, new ItemHolder(item.getId(), 1), null, item.getEnchantLevel()));
					if (Config.LOG_ITEM_ENCHANTS)
					{
						StringBuilder sb = new StringBuilder();
						if (item.getEnchantLevel() > 0)
						{
							if (support == null)
							{
								_enchantLogger.Info(sb.Append("Success, Character:").Append(player.getName())
									.Append(" [").Append(player.getObjectId()).Append("] Account:")
									.Append(player.getAccountName()).Append(" IP:").Append(session.IpAddress)
									.Append(", +").Append(item.getEnchantLevel()).Append(" ").Append(item.getName())
									.Append("(").Append(item.getCount()).Append(") [").Append(item.getObjectId())
									.Append("], ").Append(scroll.getName()).Append("(").Append(scroll.getCount())
									.Append(") [").Append(scroll.getObjectId()).Append("]").ToString());
							}
							else
							{
								_enchantLogger.Info(sb.Append("Success, Character:").Append(player.getName())
									.Append(" [").Append(player.getObjectId()).Append("] Account:")
									.Append(player.getAccountName()).Append(" IP:").Append(session.IpAddress)
									.Append(", +").Append(item.getEnchantLevel()).Append(" ").Append(item.getName())
									.Append("(").Append(item.getCount()).Append(") [").Append(item.getObjectId())
									.Append("], ").Append(scroll.getName()).Append("(").Append(scroll.getCount())
									.Append(") [").Append(scroll.getObjectId()).Append("], ").Append(support.getName())
									.Append("(").Append(support.getCount()).Append(") [").Append(support.getObjectId())
									.Append("]").ToString());
							}
						}
						else if (support == null)
						{
							_enchantLogger.Info(sb.Append("Success, Character:").Append(player.getName()).Append(" [")
								.Append(player.getObjectId()).Append("] Account:").Append(player.getAccountName())
								.Append(" IP:").Append(session.IpAddress).Append(", ").Append(item.getName())
								.Append("(").Append(item.getCount()).Append(") [").Append(item.getObjectId())
								.Append("], ").Append(scroll.getName()).Append("(").Append(scroll.getCount())
								.Append(") [").Append(scroll.getObjectId()).Append("]").ToString());
						}
						else
						{
							_enchantLogger.Info(sb.Append("Success, Character:").Append(player.getName()).Append(" [")
								.Append(player.getObjectId()).Append("] Account:").Append(player.getAccountName())
								.Append(" IP:").Append(session.IpAddress).Append(", ").Append(item.getName())
								.Append("(").Append(item.getCount()).Append(") [").Append(item.getObjectId())
								.Append("], ").Append(scroll.getName()).Append("(").Append(scroll.getCount())
								.Append(") [").Append(scroll.getObjectId()).Append("], ").Append(support.getName())
								.Append("(").Append(support.getCount()).Append(") [").Append(support.getObjectId())
								.Append("]").ToString());
						}
					}
					
					// Announce the success.
					if ((item.getEnchantLevel() >= (item.isArmor() ? Config.MIN_ARMOR_ENCHANT_ANNOUNCE : Config.MIN_WEAPON_ENCHANT_ANNOUNCE)) //
						&& (item.getEnchantLevel() <= (item.isArmor() ? Config.MAX_ARMOR_ENCHANT_ANNOUNCE : Config.MAX_WEAPON_ENCHANT_ANNOUNCE)))
					{
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_ENCHANTED_S3_UP_TO_S2);
						sm.Params.addString(player.getName());
						sm.Params.addInt(item.getEnchantLevel());
						sm.Params.addItemName(item);
						player.broadcastPacket(sm);
						Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(player, item, ExItemAnnouncePacket.ENCHANT));
						
						Skill skill = CommonSkill.FIREWORK.getSkill();
						if (skill != null)
						{
							player.broadcastPacket(new MagicSkillUsePacket(player, player, skill.getId(), skill.getLevel(), skill.getHitTime(), skill.getReuseDelay()));
						}
					}
					
					if (item.isEquipped())
					{
						if (item.isArmor())
						{
							it.forEachSkill(ItemSkillType.ON_ENCHANT, holder =>
							{
								// Add skills bestowed from +4 armor.
								if (item.getEnchantLevel() >= holder.getValue())
								{
									player.addSkill(holder.getSkill(), false);
									player.sendSkillList();
								}
							});
						}
						player.broadcastUserInfo(); // Update user info.
					}
					break;
				}
				
				case EnchantResultType.FAILURE:
				{
					bool challengePointsSafe = false;
					if ((challengePointsGroupId > 0) && (challengePointsOptionIndex == EnchantChallengePointData.OptionNumProtectProb))
					{
						EnchantChallengePointData.EnchantChallengePointsOptionInfo optionInfo = EnchantChallengePointData.getInstance().getOptionInfo(challengePointsGroupId, challengePointsOptionIndex);
						if ((optionInfo != null) && (item.getEnchantLevel() >= optionInfo.MinEnchant) && (item.getEnchantLevel() <= optionInfo.MaxEnchant) && (Rnd.get(100) < optionInfo.Chance))
						{
							challengePointsSafe = true;
						}
					}
					
					if (challengePointsSafe || scrollTemplate.isSafe())
					{
						// Safe enchant: Remain old value.
						player.sendPacket(SystemMessageId.ENCHANT_FAILED_THE_ENCHANT_SKILL_FOR_THE_CORRESPONDING_ITEM_WILL_BE_EXACTLY_RETAINED);
						player.sendPacket(new EnchantResultPacket(EnchantResultPacket.SAFE_FAIL_02, new ItemHolder(item.getId(), 1), null, item.getEnchantLevel()));
						if (Config.LOG_ITEM_ENCHANTS)
						{
							StringBuilder sb = new StringBuilder();
							if (item.getEnchantLevel() > 0)
							{
								if (support == null)
								{
									_enchantLogger.Info(sb.Append("Safe Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(session.IpAddress)
										.Append(", +").Append(item.getEnchantLevel()).Append(" ").Append(item.getName())
										.Append("(").Append(item.getCount()).Append(") [").Append(item.getObjectId())
										.Append("], ").Append(scroll.getName()).Append("(").Append(scroll.getCount())
										.Append(") [").Append(scroll.getObjectId()).Append("]").ToString());
								}
								else
								{
									_enchantLogger.Info(sb.Append("Safe Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(session.IpAddress)
										.Append(", +").Append(item.getEnchantLevel()).Append(" ").Append(item.getName())
										.Append("(").Append(item.getCount()).Append(") [").Append(item.getObjectId())
										.Append("], ").Append(scroll.getName()).Append("(").Append(scroll.getCount())
										.Append(") [").Append(scroll.getObjectId()).Append("], ")
										.Append(support.getName()).Append("(").Append(support.getCount()).Append(") [")
										.Append(support.getObjectId()).Append("]").ToString());
								}
							}
							else if (support == null)
							{
								_enchantLogger.Info(sb.Append("Safe Fail, Character:").Append(player.getName())
									.Append(" [").Append(player.getObjectId()).Append("] Account:")
									.Append(player.getAccountName()).Append(" IP:").Append(session.IpAddress)
									.Append(", ").Append(item.getName()).Append("(").Append(item.getCount())
									.Append(") [").Append(item.getObjectId()).Append("], ").Append(scroll.getName())
									.Append("(").Append(scroll.getCount()).Append(") [").Append(scroll.getObjectId())
									.Append("]").ToString());
							}
							else
							{
								_enchantLogger.Info(sb.Append("Safe Fail, Character:").Append(player.getName())
									.Append(" [").Append(player.getObjectId()).Append("] Account:")
									.Append(player.getAccountName()).Append(" IP:").Append(session.IpAddress)
									.Append(", ").Append(item.getName()).Append("(").Append(item.getCount())
									.Append(") [").Append(item.getObjectId()).Append("], ").Append(scroll.getName())
									.Append("(").Append(scroll.getCount()).Append(") [").Append(scroll.getObjectId())
									.Append("], ").Append(support.getName()).Append("(").Append(support.getCount())
									.Append(") [").Append(support.getObjectId()).Append("]").ToString());
							}
						}
					}
					else
					{
						// Unequip item on enchant failure to avoid item skills stack.
						if (item.isEquipped())
						{
							if (item.getEnchantLevel() > 0)
							{
								SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
								sm.Params.addInt(item.getEnchantLevel());
								sm.Params.addItemName(item);
								player.sendPacket(sm);
							}
							else
							{
								SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
								sm.Params.addItemName(item);
								player.sendPacket(sm);
							}
							
							foreach (Item itm in player.getInventory().unEquipItemInSlotAndRecord(item.getLocationSlot()))
							{
								itemsToUpdate.Add(new ItemInfo(itm, ItemChangeType.MODIFIED));
							}
							
							InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
							player.sendInventoryUpdate(iu);
							player.broadcastUserInfo();
						}
						
						bool challengePointsBlessed = false;
						bool challengePointsBlessedDown = false;
						if (challengePointsGroupId > 0)
						{
							if (challengePointsOptionIndex == EnchantChallengePointData.OptionNumResetProb)
							{
								EnchantChallengePointData.EnchantChallengePointsOptionInfo optionInfo = EnchantChallengePointData.getInstance().getOptionInfo(challengePointsGroupId, challengePointsOptionIndex);
								if ((optionInfo != null) && (item.getEnchantLevel() >= optionInfo.MinEnchant) && (item.getEnchantLevel() <= optionInfo.MaxEnchant) && (Rnd.get(100) < optionInfo.Chance))
								{
									challengePointsBlessed = true;
								}
							}
							else if (challengePointsOptionIndex == EnchantChallengePointData.OptionNumDownProb)
							{
								EnchantChallengePointData.EnchantChallengePointsOptionInfo optionInfo = EnchantChallengePointData.getInstance().getOptionInfo(challengePointsGroupId, challengePointsOptionIndex);
								if ((optionInfo != null) && (item.getEnchantLevel() >= optionInfo.MinEnchant) && (item.getEnchantLevel() <= optionInfo.MaxEnchant) && (Rnd.get(100) < optionInfo.Chance))
								{
									challengePointsBlessedDown = true;
								}
							}
						}
						
						if (challengePointsBlessed || challengePointsBlessedDown || scrollTemplate.isBlessed() || scrollTemplate.isBlessedDown() || scrollTemplate.isCursed() /* || ((supportTemplate != null) && supportTemplate.isDown()) */ || ((supportTemplate != null) && supportTemplate.isBlessed()))
						{
							// Blessed enchant: Enchant value down by 1.
							if (scrollTemplate.isBlessedDown() || challengePointsBlessedDown || scrollTemplate.isCursed())
							{
								connection.Send(SystemMessageId.THE_ENCHANT_VALUE_IS_DECREASED_BY_1);
								item.setEnchantLevel(Math.Max(0, item.getEnchantLevel() - 1));
							}
							else // Blessed enchant: Clear enchant value.
							{
								player.sendPacket(SystemMessageId.THE_BLESSED_ENCHANT_FAILED_THE_ENCHANT_VALUE_OF_THE_ITEM_BECAME_0);
								item.setEnchantLevel(0);
							}
							player.sendPacket(new EnchantResultPacket(EnchantResultPacket.FAIL, new ItemHolder(item.getId(), 1), null, item.getEnchantLevel()));
							item.updateDatabase();
							if (Config.LOG_ITEM_ENCHANTS)
							{
								StringBuilder sb = new StringBuilder();
								if (item.getEnchantLevel() > 0)
								{
									if (support == null)
									{
										_enchantLogger.Info(sb.Append("Blessed Fail, Character:")
											.Append(player.getName()).Append(" [").Append(player.getObjectId())
											.Append("] Account:").Append(player.getAccountName()).Append(" IP:")
											.Append(session.IpAddress).Append(", +").Append(item.getEnchantLevel())
											.Append(" ").Append(item.getName()).Append("(").Append(item.getCount())
											.Append(") [").Append(item.getObjectId()).Append("], ")
											.Append(scroll.getName()).Append("(").Append(scroll.getCount())
											.Append(") [").Append(scroll.getObjectId()).Append("]").ToString());
									}
									else
									{
										_enchantLogger.Info(sb.Append("Blessed Fail, Character:")
											.Append(player.getName()).Append(" [").Append(player.getObjectId())
											.Append("] Account:").Append(player.getAccountName()).Append(" IP:")
											.Append(session.IpAddress).Append(", +").Append(item.getEnchantLevel())
											.Append(" ").Append(item.getName()).Append("(").Append(item.getCount())
											.Append(") [").Append(item.getObjectId()).Append("], ")
											.Append(scroll.getName()).Append("(").Append(scroll.getCount())
											.Append(") [").Append(scroll.getObjectId()).Append("], ")
											.Append(support.getName()).Append("(").Append(support.getCount())
											.Append(") [").Append(support.getObjectId()).Append("]").ToString());
									}
								}
								else if (support == null)
								{
									_enchantLogger.Info(sb.Append("Blessed Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(session.IpAddress)
										.Append(", ").Append(item.getName()).Append("(").Append(item.getCount())
										.Append(") [").Append(item.getObjectId()).Append("], ").Append(scroll.getName())
										.Append("(").Append(scroll.getCount()).Append(") [")
										.Append(scroll.getObjectId()).Append("]").ToString());
								}
								else
								{
									_enchantLogger.Info(sb.Append("Blessed Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(session.IpAddress)
										.Append(", ").Append(item.getName()).Append("(").Append(item.getCount())
										.Append(") [").Append(item.getObjectId()).Append("], ").Append(scroll.getName())
										.Append("(").Append(scroll.getCount()).Append(") [")
										.Append(scroll.getObjectId()).Append("], ").Append(support.getName())
										.Append("(").Append(support.getCount()).Append(") [")
										.Append(support.getObjectId()).Append("]").ToString());
								}
							}
						}
						else
						{
							// add challenge point
							EnchantChallengePointData.getInstance().handleFailure(player, item);
							player.sendPacket(new ExEnchantChallengePointInfoPacket(player));
							// Enchant failed, destroy item.
							if (player.getInventory().destroyItem("Enchant", item, player, null) == null)
							{
								// Unable to destroy item, cheater?
								Util.handleIllegalPlayerAction(player, "Unable to delete item on enchant failure from " + player + ", possible cheater !", Config.DEFAULT_PUNISH);
								player.removeRequest<EnchantItemRequest>();
								player.sendPacket(new EnchantResultPacket(EnchantResultPacket.ERROR, null, null, 0));
								if (Config.LOG_ITEM_ENCHANTS)
								{
									StringBuilder sb = new StringBuilder();
									if (item.getEnchantLevel() > 0)
									{
										if (support == null)
										{
											_enchantLogger.Info(sb.Append("Unable to destroy, Character:")
												.Append(player.getName()).Append(" [").Append(player.getObjectId())
												.Append("] Account:").Append(player.getAccountName()).Append(" IP:")
												.Append(session.IpAddress).Append(", +")
												.Append(item.getEnchantLevel()).Append(" ").Append(item.getName())
												.Append("(").Append(item.getCount()).Append(") [")
												.Append(item.getObjectId()).Append("], ").Append(scroll.getName())
												.Append("(").Append(scroll.getCount()).Append(") [")
												.Append(scroll.getObjectId()).Append("]").ToString());
										}
										else
										{
											_enchantLogger.Info(sb.Append("Unable to destroy, Character:")
												.Append(player.getName()).Append(" [").Append(player.getObjectId())
												.Append("] Account:").Append(player.getAccountName()).Append(" IP:")
												.Append(session.IpAddress).Append(", +")
												.Append(item.getEnchantLevel()).Append(" ").Append(item.getName())
												.Append("(").Append(item.getCount()).Append(") [")
												.Append(item.getObjectId()).Append("], ").Append(scroll.getName())
												.Append("(").Append(scroll.getCount()).Append(") [")
												.Append(scroll.getObjectId()).Append("], ").Append(support.getName())
												.Append("(").Append(support.getCount()).Append(") [")
												.Append(support.getObjectId()).Append("]").ToString());
										}
									}
									else if (support == null)
									{
										_enchantLogger.Info(sb.Append("Unable to destroy, Character:")
											.Append(player.getName()).Append(" [").Append(player.getObjectId())
											.Append("] Account:").Append(player.getAccountName()).Append(" IP:")
											.Append(session.IpAddress).Append(", ").Append(item.getName())
											.Append("(").Append(item.getCount()).Append(") [")
											.Append(item.getObjectId()).Append("], ").Append(scroll.getName())
											.Append("(").Append(scroll.getCount()).Append(") [")
											.Append(scroll.getObjectId()).Append("]").ToString());
									}
									else
									{
										_enchantLogger.Info(sb.Append("Unable to destroy, Character:")
											.Append(player.getName()).Append(" [").Append(player.getObjectId())
											.Append("] Account:").Append(player.getAccountName()).Append(" IP:")
											.Append(session.IpAddress).Append(", ").Append(item.getName())
											.Append("(").Append(item.getCount()).Append(") [")
											.Append(item.getObjectId()).Append("], ").Append(scroll.getName())
											.Append("(").Append(scroll.getCount()).Append(") [")
											.Append(scroll.getObjectId()).Append("], ").Append(support.getName())
											.Append("(").Append(support.getCount()).Append(") [")
											.Append(support.getObjectId()).Append("]").ToString());
									}
								}
								
								return ValueTask.CompletedTask;
							}
							
							World.getInstance().removeObject(item);
							
							int count = 0;
							if (item.getTemplate().isCrystallizable())
							{
								count = Math.Max(0, item.getCrystalCount() - ((item.getTemplate().getCrystalCount() + 1) / 2));
							}
							
							Item crystals = null;
							int crystalId = item.getTemplate().getCrystalItemId();
							if (count > 0)
							{
								crystals = player.getInventory().addItem("Enchant", crystalId, count, player, item);
								SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
								sm.Params.addItemName(crystals);
								sm.Params.addLong(count);
								player.sendPacket(sm);
							}
							
							if (crystals != null)
							{
								itemsToUpdate.Add(new ItemInfo(crystals));
							}
							
							if ((crystalId == 0) || (count == 0))
							{
								player.sendPacket(new EnchantResultPacket(EnchantResultPacket.NO_CRYSTAL, null, null, 0));
							}
							else
							{
								ItemChanceHolder destroyReward = ItemCrystallizationData.getInstance().getItemOnDestroy(player, item);
								if ((destroyReward != null) && (Rnd.get(100) < destroyReward.getChance()))
								{
									player.addItem("Enchant", destroyReward, player, true);
									player.sendPacket(new EnchantResultPacket(EnchantResultPacket.FAIL, new ItemHolder(crystalId, count), destroyReward, 0));
								}
								else
								{
									player.sendPacket(new EnchantResultPacket(EnchantResultPacket.FAIL, new ItemHolder(crystalId, count), null, 0));
								}
							}
							
							player.sendPacket(new ExEnchantChallengePointInfoPacket(player));
							if (Config.LOG_ITEM_ENCHANTS)
							{
								StringBuilder sb = new StringBuilder();
								if (item.getEnchantLevel() > 0)
								{
									if (support == null)
									{
										_enchantLogger.Info(sb.Append("Fail, Character:").Append(player.getName())
											.Append(" [").Append(player.getObjectId()).Append("] Account:")
											.Append(player.getAccountName()).Append(" IP:")
											.Append(session.IpAddress).Append(", +").Append(item.getEnchantLevel())
											.Append(" ").Append(item.getName()).Append("(").Append(item.getCount())
											.Append(") [").Append(item.getObjectId()).Append("], ")
											.Append(scroll.getName()).Append("(").Append(scroll.getCount())
											.Append(") [").Append(scroll.getObjectId()).Append("]").ToString());
									}
									else
									{
										_enchantLogger.Info(sb.Append("Fail, Character:").Append(player.getName())
											.Append(" [").Append(player.getObjectId()).Append("] Account:")
											.Append(player.getAccountName()).Append(" IP:")
											.Append(session.IpAddress).Append(", +").Append(item.getEnchantLevel())
											.Append(" ").Append(item.getName()).Append("(").Append(item.getCount())
											.Append(") [").Append(item.getObjectId()).Append("], ")
											.Append(scroll.getName()).Append("(").Append(scroll.getCount())
											.Append(") [").Append(scroll.getObjectId()).Append("], ")
											.Append(support.getName()).Append("(").Append(support.getCount())
											.Append(") [").Append(support.getObjectId()).Append("]").ToString());
									}
								}
								else if (support == null)
								{
									_enchantLogger.Info(sb.Append("Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(session.IpAddress)
										.Append(", ").Append(item.getName()).Append("(").Append(item.getCount())
										.Append(") [").Append(item.getObjectId()).Append("], ").Append(scroll.getName())
										.Append("(").Append(scroll.getCount()).Append(") [")
										.Append(scroll.getObjectId()).Append("]").ToString());
								}
								else
								{
									_enchantLogger.Info(sb.Append("Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(session.IpAddress)
										.Append(", ").Append(item.getName()).Append("(").Append(item.getCount())
										.Append(") [").Append(item.getObjectId()).Append("], ").Append(scroll.getName())
										.Append("(").Append(scroll.getCount()).Append(") [")
										.Append(scroll.getObjectId()).Append("], ").Append(support.getName())
										.Append("(").Append(support.getCount()).Append(") [")
										.Append(support.getObjectId()).Append("]").ToString());
								}
							}
						}
					}
					
					break;
				}
			}
			
			if (challengePointsGroupId >= 0)
			{
				player.getChallengeInfo().setChallengePointsPendingRecharge(-1, -1);
				player.getChallengeInfo().addChallengePointsRecharge(challengePointsGroupId, challengePointsOptionIndex, -1);
				player.sendPacket(new ExEnchantChallengePointInfoPacket(player));
			}
			
			player.sendItemList();
			player.broadcastUserInfo();
			
			request.setProcessing(false);
		}

		return ValueTask.CompletedTask;
    }
}