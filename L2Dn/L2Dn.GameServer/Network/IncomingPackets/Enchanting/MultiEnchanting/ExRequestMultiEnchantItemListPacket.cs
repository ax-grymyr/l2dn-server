using System.Text;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.MultiEnchanting;

public struct ExRequestMultiEnchantItemListPacket: IIncomingPacket<GameSession>
{
	private static readonly Logger LOGGER_ENCHANT = LogManager.GetLogger("enchant.items");
    private int _useLateAnnounce;
    private int _slotId;
    private List<int> _itemObjectId;

    public void ReadContent(PacketBitReader reader)
    {
        _useLateAnnounce = reader.ReadByte();
        _slotId = reader.ReadInt32();
        _itemObjectId = new List<int>();
        while (reader.Length != 0)
            _itemObjectId.Add(reader.ReadInt32());
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		player.getChallengeInfo().setChallengePointsPendingRecharge(-1, -1);
		
		EnchantItemRequest request = player.getRequest<EnchantItemRequest>();
		if (request == null)
			return ValueTask.CompletedTask;
		
		if ((request.getEnchantingScroll() == null) || request.isProcessing())
			return ValueTask.CompletedTask;
		
		Item scroll = request.getEnchantingScroll();
		if (scroll.getCount() < _slotId)
		{
			player.removeRequest<EnchantItemRequest>();
			player.removeRequest<EnchantItemRequest>();
			player.sendPacket(new ExResultSetMultiEnchantItemListPacket(player, 1));
			PacketLogger.Instance.Warn("MultiEnchant - player " + player.getObjectId() + " " + player.getName() +
			                           " trying enchant items, when scroll count is less than items!");
			
			return ValueTask.CompletedTask;
		}
		
		EnchantScroll scrollTemplate = EnchantItemData.getInstance().getEnchantScroll(scroll);
		if (scrollTemplate == null)
			return ValueTask.CompletedTask;
		
		int[] slots = new int[_slotId];
		for (int i = 1; i <= _slotId; i++)
		{
			if (!request.checkMultiEnchantingItemsByObjectId(_itemObjectId.get(i)))
			{
				player.removeRequest<EnchantItemRequest>();
				return ValueTask.CompletedTask;
			}
			
			slots[i - 1] = getMultiEnchantingSlotByObjectId(request, _itemObjectId.get(i));
		}
		
		request.setProcessing(true);

		Map<int, string> result = new();
		Map<int, int[]> successEnchant = new();
		Map<int, int> failureEnchant = new();
		Map<int, int> failChallengePointInfoList = new();
		Map<int, ItemHolder> failureReward = new();
		
		for (int slotCounter = 0; slotCounter < slots.Length; slotCounter++)
		{
			int i = slots[slotCounter];
			if ((i == -1) || (request.getMultiEnchantingItemsBySlot(i) == -1))
			{
				player.sendPacket(new ExResultMultiEnchantItemListPacket(player, true));
				player.removeRequest<EnchantItemRequest>();
				return ValueTask.CompletedTask;
			}
			
			Item enchantItem = player.getInventory().getItemByObjectId(request.getMultiEnchantingItemsBySlot(i));
			if (enchantItem == null)
			{
				player.removeRequest<EnchantItemRequest>();
				return ValueTask.CompletedTask;
			}
			
			if (scrollTemplate.getMaxEnchantLevel() < enchantItem.getEnchantLevel())
			{
				PacketLogger.Instance.Warn("MultiEnchant - player " + player.getObjectId() + " " + player.getName() +
				                           " trying over-enchant item " + enchantItem.getItemName() + " " +
				                           enchantItem.getObjectId());
				
				player.removeRequest<EnchantItemRequest>();
				return ValueTask.CompletedTask;
			}
			
			if (player.getInventory().destroyItemByItemId("Enchant", scroll.getId(), 1, player, enchantItem) == null)
			{
				player.removeRequest<EnchantItemRequest>();
				return ValueTask.CompletedTask;
			}
			
			lock (enchantItem)
			{
				if ((enchantItem.getOwnerId() != player.getObjectId()) || !enchantItem.isEnchantable())
				{
					player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
					player.removeRequest<EnchantItemRequest>();
					player.sendPacket(new ExResultMultiEnchantItemListPacket(player, true));
					return ValueTask.CompletedTask;
				}
				
				EnchantResultType resultType = scrollTemplate.calculateSuccess(player, enchantItem, null);
				switch (resultType)
				{
					case EnchantResultType.ERROR:
					{
						player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
						player.removeRequest<EnchantItemRequest>();
						result.put(slots[i - 1], "ERROR");
						break;
					}
					case EnchantResultType.SUCCESS:
					{
						if (scrollTemplate.isCursed())
						{
							// Blessed enchant: Enchant value down by 1.
							player.sendPacket(SystemMessageId.THE_ENCHANT_VALUE_IS_DECREASED_BY_1);
							enchantItem.setEnchantLevel(enchantItem.getEnchantLevel() - 1);
						}
						// Increase enchant level only if scroll's base template has chance, some armors can success over +20 but they shouldn't have increased.
						else if (scrollTemplate.getChance(player, enchantItem) > 0)
						{
							enchantItem.setEnchantLevel(enchantItem.getEnchantLevel() +
							                            Math.Min(
								                            Rnd.get(scrollTemplate.getRandomEnchantMin(),
									                            scrollTemplate.getRandomEnchantMax()),
								                            scrollTemplate.getMaxEnchantLevel()));
							
							enchantItem.updateDatabase();
						}
						
						result.put(i, "SUCCESS");
						if (Config.LOG_ITEM_ENCHANTS)
						{
							StringBuilder sb = new StringBuilder();
							if (enchantItem.getEnchantLevel() > 0)
							{
								LOGGER_ENCHANT.Info(sb.Append("Success, Character:").Append(player.getName())
									.Append(" [").Append(player.getObjectId()).Append("] Account:")
									.Append(player.getAccountName()).Append(" IP:").Append(player.getIPAddress())
									.Append(", +").Append(enchantItem.getEnchantLevel()).Append(" ")
									.Append(enchantItem.getName()).Append("(").Append(enchantItem.getCount())
									.Append(") [").Append(enchantItem.getObjectId()).Append("], ")
									.Append(scroll.getName()).Append("(").Append(scroll.getCount()).Append(") [")
									.Append(scroll.getObjectId()).Append("]").ToString());
							}
							else
							{
								LOGGER_ENCHANT.Info(sb.Append("Success, Character:").Append(player.getName())
									.Append(" [").Append(player.getObjectId()).Append("] Account:")
									.Append(player.getAccountName()).Append(" IP:").Append(player.getIPAddress())
									.Append(", ").Append(enchantItem.getName()).Append("(")
									.Append(enchantItem.getCount()).Append(") [").Append(enchantItem.getObjectId())
									.Append("], ").Append(scroll.getName()).Append("(").Append(scroll.getCount())
									.Append(") [").Append(scroll.getObjectId()).Append("]").ToString());
							}
						}
						break;
					}
					case EnchantResultType.FAILURE:
					{
						if (scrollTemplate.isSafe())
						{
							// Safe enchant: Remain old value.
							player.sendPacket(SystemMessageId.ENCHANT_FAILED_THE_ENCHANT_SKILL_FOR_THE_CORRESPONDING_ITEM_WILL_BE_EXACTLY_RETAINED);
							player.sendPacket(new EnchantResultPacket(EnchantResultPacket.SAFE_FAIL, new ItemHolder(enchantItem.getId(), 1), null, 0));
							if (Config.LOG_ITEM_ENCHANTS)
							{
								StringBuilder sb = new StringBuilder();
								if (enchantItem.getEnchantLevel() > 0)
								{
									LOGGER_ENCHANT.Info(sb.Append("Safe Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(player.getIPAddress())
										.Append(", +").Append(enchantItem.getEnchantLevel()).Append(" ")
										.Append(enchantItem.getName()).Append("(").Append(enchantItem.getCount())
										.Append(") [").Append(enchantItem.getObjectId()).Append("], ")
										.Append(scroll.getName()).Append("(").Append(scroll.getCount()).Append(") [")
										.Append(scroll.getObjectId()).Append("]").ToString());
								}
								else
								{
									LOGGER_ENCHANT.Info(sb.Append("Safe Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(player.getIPAddress())
										.Append(", ").Append(enchantItem.getName()).Append("(")
										.Append(enchantItem.getCount()).Append(") [").Append(enchantItem.getObjectId())
										.Append("], ").Append(scroll.getName()).Append("(").Append(scroll.getCount())
										.Append(") [").Append(scroll.getObjectId()).Append("]").ToString());
								}
							}
						}
						if (scrollTemplate.isBlessed() || scrollTemplate.isBlessedDown() || scrollTemplate.isCursed())
						{
							// Blessed enchant: Enchant value down by 1.
							if (scrollTemplate.isBlessedDown() || scrollTemplate.isCursed())
							{
								player.sendPacket(SystemMessageId.THE_ENCHANT_VALUE_IS_DECREASED_BY_1);
								enchantItem.setEnchantLevel(enchantItem.getEnchantLevel() - 1);
							}
							else // Blessed enchant: Clear enchant value.
							{
								player.sendPacket(SystemMessageId.THE_BLESSED_ENCHANT_FAILED_THE_ENCHANT_VALUE_OF_THE_ITEM_BECAME_0);
								enchantItem.setEnchantLevel(0);
							}
							
							result.put(i, "BLESSED_FAIL");
							enchantItem.updateDatabase();
							if (Config.LOG_ITEM_ENCHANTS)
							{
								StringBuilder sb = new StringBuilder();
								if (enchantItem.getEnchantLevel() > 0)
								{
									LOGGER_ENCHANT.Info(sb.Append("Blessed Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(player.getIPAddress())
										.Append(", +").Append(enchantItem.getEnchantLevel()).Append(" ")
										.Append(enchantItem.getName()).Append("(").Append(enchantItem.getCount())
										.Append(") [").Append(enchantItem.getObjectId()).Append("], ")
										.Append(scroll.getName()).Append("(").Append(scroll.getCount()).Append(") [")
										.Append(scroll.getObjectId()).Append("]").ToString());
								}
								else
								{
									LOGGER_ENCHANT.Info(sb.Append("Blessed Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(player.getIPAddress())
										.Append(", ").Append(enchantItem.getName()).Append("(")
										.Append(enchantItem.getCount()).Append(") [").Append(enchantItem.getObjectId())
										.Append("], ").Append(scroll.getName()).Append("(").Append(scroll.getCount())
										.Append(") [").Append(scroll.getObjectId()).Append("]").ToString());
								}
							}
						}
						else
						{
							int[] challengePoints = EnchantChallengePointData.getInstance().handleFailure(player, enchantItem);
							if ((challengePoints[0] != -1) && (challengePoints[1] != -1))
							{
								failChallengePointInfoList.compute(challengePoints[0], (k, v) => v == null ? challengePoints[1] : v + challengePoints[1]);
							}
							
							if (player.getInventory().destroyItem("Enchant", enchantItem, player, null) == null)
							{
								// Unable to destroy item, cheater?
								Util.handleIllegalPlayerAction(player, "Unable to delete item on enchant failure from " + player + ", possible cheater !", Config.DEFAULT_PUNISH);
								player.removeRequest<EnchantItemRequest>();
								result.put(i, "ERROR");
								if (Config.LOG_ITEM_ENCHANTS)
								{
									StringBuilder sb = new StringBuilder();
									if (enchantItem.getEnchantLevel() > 0)
									{
										LOGGER_ENCHANT.Info(sb.Append("Unable to destroy, Character:")
											.Append(player.getName()).Append(" [").Append(player.getObjectId())
											.Append("] Account:").Append(player.getAccountName()).Append(" IP:")
											.Append(player.getIPAddress()).Append(", +")
											.Append(enchantItem.getEnchantLevel()).Append(" ")
											.Append(enchantItem.getName()).Append("(").Append(enchantItem.getCount())
											.Append(") [").Append(enchantItem.getObjectId()).Append("], ")
											.Append(scroll.getName()).Append("(").Append(scroll.getCount())
											.Append(") [").Append(scroll.getObjectId()).Append("]").ToString());
									}
									else
									{
										LOGGER_ENCHANT.Info(sb.Append("Unable to destroy, Character:")
											.Append(player.getName()).Append(" [").Append(player.getObjectId())
											.Append("] Account:").Append(player.getAccountName()).Append(" IP:")
											.Append(player.getIPAddress()).Append(", ").Append(enchantItem.getName())
											.Append("(").Append(enchantItem.getCount()).Append(") [")
											.Append(enchantItem.getObjectId()).Append("], ").Append(scroll.getName())
											.Append("(").Append(scroll.getCount()).Append(") [")
											.Append(scroll.getObjectId()).Append("]").ToString());
									}
								}

								return ValueTask.CompletedTask;
							}
							
							World.getInstance().removeObject(enchantItem);
							
							int count = 0;
							if (enchantItem.getTemplate().isCrystallizable())
							{
								count = Math.Max(0, enchantItem.getCrystalCount() - ((enchantItem.getTemplate().getCrystalCount() + 1) / 2));
							}
							
							Item crystals = null;
							int crystalId = enchantItem.getTemplate().getCrystalItemId();
							if (count > 0)
							{
								crystals = player.getInventory().addItem("Enchant", crystalId, count, player, enchantItem);
								SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
								sm.Params.addItemName(crystals);
								sm.Params.addLong(count);
								player.sendPacket(sm);
								ItemHolder itemHolder = new ItemHolder(crystalId, count);
								
								failureReward.put(failureReward.size() + 1, itemHolder);
							}
							
							// if (crystals != null)
							// {
							// 	iu.addItem(crystals); // TODO: packet not sent
							// }
							
							if ((crystalId == 0) || (count == 0))
							{
								ItemHolder itemHolder = new ItemHolder(0, 0);
								failureReward.put(failureReward.size() + 1, itemHolder);
								result.put(i, "NO_CRYSTAL");
							}
							else
							{
								ItemHolder itemHolder = new ItemHolder(0, 0);
								failureReward.put(failureReward.size() + 1, itemHolder);
								result.put(i, "FAIL");
							}
							
							ItemChanceHolder destroyReward = ItemCrystallizationData.getInstance().getItemOnDestroy(player, enchantItem);
							if ((destroyReward != null) && (Rnd.get(100) < destroyReward.getChance()))
							{
								failureReward.put(failureReward.size() + 1, destroyReward);
								player.addItem("Enchant", destroyReward.getId(), destroyReward.getCount(), null, true);
								player.sendPacket(new EnchantResultPacket(EnchantResultPacket.FAIL, destroyReward, null, 0));
							}
							
							if (Config.LOG_ITEM_ENCHANTS)
							{
								StringBuilder sb = new StringBuilder();
								if (enchantItem.getEnchantLevel() > 0)
								{
									LOGGER_ENCHANT.Info(sb.Append("Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(player.getIPAddress())
										.Append(", +").Append(enchantItem.getEnchantLevel()).Append(" ")
										.Append(enchantItem.getName()).Append("(").Append(enchantItem.getCount())
										.Append(") [").Append(enchantItem.getObjectId()).Append("], ")
										.Append(scroll.getName()).Append("(").Append(scroll.getCount()).Append(") [")
										.Append(scroll.getObjectId()).Append("]").ToString());
								}
								else
								{
									LOGGER_ENCHANT.Info(sb.Append("Fail, Character:").Append(player.getName())
										.Append(" [").Append(player.getObjectId()).Append("] Account:")
										.Append(player.getAccountName()).Append(" IP:").Append(player.getIPAddress())
										.Append(", ").Append(enchantItem.getName()).Append("(")
										.Append(enchantItem.getCount()).Append(") [").Append(enchantItem.getObjectId())
										.Append("], ").Append(scroll.getName()).Append("(").Append(scroll.getCount())
										.Append(") [").Append(scroll.getObjectId()).Append("]").ToString());
								}
							}
						}
						break;
					}
				}
			}
		}
		
		for (int slotCounter = 0; slotCounter < slots.Length; slotCounter++)
		{
			int i = slots[slotCounter];
			if (result.get(i).equals("SUCCESS"))
			{
				int[] intArray = new int[2];
				intArray[0] = request.getMultiEnchantingItemsBySlot(i);
				intArray[1] = player.getInventory().getItemByObjectId(request.getMultiEnchantingItemsBySlot(i)).getEnchantLevel();
				successEnchant.put(i, intArray);
			}
			else if (result.get(i).equals("NO_CRYSTAL") || result.get(i).equals("FAIL"))
			{
				failureEnchant.put(i, request.getMultiEnchantingItemsBySlot(i));
				request.changeMultiEnchantingItemsBySlot(i, 0);
			}
			else
			{
				player.sendPacket(new ExResultMultiEnchantItemListPacket(player, successEnchant, failureEnchant,
					failChallengePointInfoList, true));
				
				player.sendPacket(new ShortCutInitPacket(player));
				return ValueTask.CompletedTask;
			}
		}
		
		foreach (ItemHolder failure in failureReward.values())
		{
			request.addMultiEnchantFailItems(failure);
		}
		
		request.setProcessing(false);
		
		player.sendItemList();
		player.broadcastUserInfo();
		player.sendPacket(new ChangedEnchantTargetItemProbabilityListPacket(player, true));
		
		if (_useLateAnnounce == 1)
		{
			request.setMultiSuccessEnchantList(successEnchant);
			request.setMultiFailureEnchantList(failureEnchant);
		}
		
		player.sendPacket(new ExResultMultiEnchantItemListPacket(player, successEnchant, failureEnchant, failChallengePointInfoList, true));
		player.sendPacket(new ShortCutInitPacket(player));
		player.sendPacket(new ExEnchantChallengePointInfoPacket(player));
		
		return ValueTask.CompletedTask;
	}
	
	private static int getMultiEnchantingSlotByObjectId(EnchantItemRequest request, int objectId)
	{
		int slotId = -1;
		for (int i = 1; i <= request.getMultiEnchantingItemsCount(); i++)
		{
			if ((request.getMultiEnchantingItemsCount() == 0) || (objectId == 0))
			{
				return slotId;
			}
			
			if (request.getMultiEnchantingItemsBySlot(i) == objectId)
			{
				return i;
			}
		}
		return slotId;
	}
}