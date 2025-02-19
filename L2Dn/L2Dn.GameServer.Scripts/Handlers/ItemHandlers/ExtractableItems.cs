using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.AutoPeel;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * Extractable Items handler.
 * @author HorridoJoho, Mobius
 */
public class ExtractableItems: IItemHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ExtractableItems));

	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		EtcItem etcitem = (EtcItem) item.getTemplate();
		List<ExtractableProduct> exitems = etcitem.getExtractableItems();
		if (exitems == null)
		{
			_logger.Info("No extractable data defined for " + etcitem);
			return false;
		}

		if (!player.isInventoryUnder80(false))
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_SPACE_IN_INVENTORY_UNABLE_TO_PROCESS_THIS_REQUEST_UNTIL_YOUR_INVENTORY_S_WEIGHT_IS_LESS_THAN_80_AND_SLOT_COUNT_IS_LESS_THAN_90_OF_CAPACITY);
			return false;
		}

		// destroy item
		if (!DailyTaskManager.RESET_ITEMS.Contains(item.getId()) &&
		    !player.destroyItem("Extract", item.ObjectId, 1, player, true))
		{
			return false;
		}

		bool specialReward = false;
		Map<Item, long> extractedItems = new();
		List<Item> enchantedItems = new();
		if (etcitem.getExtractableCountMin() > 0)
		{
			while (extractedItems.Count < etcitem.getExtractableCountMin())
			{
				foreach (ExtractableProduct expi in exitems)
				{
					if (etcitem.getExtractableCountMax() > 0 && extractedItems.Count == etcitem.getExtractableCountMax())
					{
						break;
					}

					if (Rnd.get(100000) <= expi.getChance())
					{
						long min = (long) (expi.getMin() * Config.RATE_EXTRACTABLE);
						long max = (long) (expi.getMax() * Config.RATE_EXTRACTABLE);
						long createItemAmount = max == min ? min : Rnd.get(max - min + 1) + min;
						if (createItemAmount == 0)
						{
							continue;
						}

						// Do not extract the same item.
						bool alreadyExtracted = false;
						foreach (Item i in extractedItems.Keys)
						{
							if (i.getTemplate().getId() == expi.getId())
							{
								alreadyExtracted = true;
								break;
							}
						}
						if (alreadyExtracted && exitems.Count >= etcitem.getExtractableCountMax())
						{
							continue;
						}

						if (expi.getId() == -1) // Prime points
						{
							player.setPrimePoints(player.getPrimePoints() + (int) createItemAmount);
							player.sendMessage("You have obtained " + createItemAmount / 100 + " Euro!");
							specialReward = true;
							continue;
						}

                        if (expi.getId() == (int)SpecialItemType.PC_CAFE_POINTS)
                        {
                            int currentPoints = player.getPcCafePoints();
                            int upgradePoints = player.getPcCafePoints() + (int) createItemAmount;
                            player.setPcCafePoints(upgradePoints);
                            SystemMessagePacket message = new SystemMessagePacket(SystemMessageId.YOU_EARNED_S1_PA_POINT_S);
                            message.Params.addInt((int)createItemAmount);
                            player.sendPacket(message);
                            player.sendPacket(new ExPcCafePointInfoPacket(currentPoints, upgradePoints, 1));
                            specialReward = true;
                            continue;
                        }

                        if (expi.getId() == (int)SpecialItemType.HONOR_COINS)
                        {
                            player.setHonorCoins(player.getHonorCoins() + (int) createItemAmount);
                            player.sendMessage("You have obtained " + createItemAmount + " Honor Coin.");
                            specialReward = true;
                            continue;
                        }

                        ItemTemplate? template = ItemData.getInstance().getTemplate(expi.getId());
						if (template == null)
						{
							_logger.Warn("ExtractableItems: Could not find " + item + " product template with id " + expi.getId() + "!");
							continue;
						}

						if (template.isStackable() || createItemAmount == 1)
						{
							Item newItem = player.addItem("Extract", expi.getId(), createItemAmount, player, false);
							if (expi.getMaxEnchant() > 0)
							{
								newItem.setEnchantLevel(Rnd.get(expi.getMinEnchant(), expi.getMaxEnchant()));
								enchantedItems.Add(newItem);
							}
							addItem(extractedItems, newItem, createItemAmount);
						}
						else
						{
							while (createItemAmount > 0)
							{
								Item newItem = player.addItem("Extract", expi.getId(), 1, player, false);
								if (expi.getMaxEnchant() > 0)
								{
									newItem.setEnchantLevel(Rnd.get(expi.getMinEnchant(), expi.getMaxEnchant()));
									enchantedItems.Add(newItem);
								}
								addItem(extractedItems, newItem, 1);
								createItemAmount--;
							}
						}
					}
				}
			}
		}
		else
		{
			foreach (ExtractableProduct expi in exitems)
			{
				if (etcitem.getExtractableCountMax() > 0 && extractedItems.Count == etcitem.getExtractableCountMax())
				{
					break;
				}

				if (Rnd.get(100000) <= expi.getChance())
				{
					long min = (long) (expi.getMin() * Config.RATE_EXTRACTABLE);
					long max = (long) (expi.getMax() * Config.RATE_EXTRACTABLE);
					long createItemAmount = max == min ? min : Rnd.get(max - min + 1) + min;
					if (createItemAmount == 0)
					{
						continue;
					}

					if (expi.getId() == -1) // Prime points
					{
						player.setPrimePoints(player.getPrimePoints() + (int) createItemAmount);
						player.sendMessage("You have obtained " + createItemAmount / 100 + " Euro!");
						specialReward = true;
						continue;
					}

                    if (expi.getId() == (int)SpecialItemType.PC_CAFE_POINTS)
                    {
                        int currentPoints = player.getPcCafePoints();
                        int upgradePoints = player.getPcCafePoints() + (int) createItemAmount;
                        player.setPcCafePoints(upgradePoints);
                        SystemMessagePacket message = new SystemMessagePacket(SystemMessageId.YOU_EARNED_S1_PA_POINT_S);
                        message.Params.addInt((int)createItemAmount);
                        player.sendPacket(message);
                        player.sendPacket(new ExPcCafePointInfoPacket(currentPoints, upgradePoints, 1));
                        specialReward = true;
                        continue;
                    }

                    if (expi.getId() == (int)SpecialItemType.HONOR_COINS)
                    {
                        player.setHonorCoins(player.getHonorCoins() + (int) createItemAmount);
                        player.sendMessage("You have obtained " + createItemAmount + " Honor Coin.");
                        specialReward = true;
                        continue;
                    }

                    ItemTemplate? template = ItemData.getInstance().getTemplate(expi.getId());
					if (template == null)
					{
						_logger.Warn("ExtractableItems: Could not find " + item + " product template with id " + expi.getId() + "!");
						continue;
					}

					if (template.isStackable() || createItemAmount == 1)
					{
						Item newItem = player.addItem("Extract", expi.getId(), createItemAmount, player, false);
						if (expi.getMaxEnchant() > 0)
						{
							newItem.setEnchantLevel(Rnd.get(expi.getMinEnchant(), expi.getMaxEnchant()));
							enchantedItems.Add(newItem);
						}
						addItem(extractedItems, newItem, createItemAmount);
					}
					else
					{
						while (createItemAmount > 0)
						{
							Item newItem = player.addItem("Extract", expi.getId(), 1, player, false);
							if (expi.getMaxEnchant() > 0)
							{
								newItem.setEnchantLevel(Rnd.get(expi.getMinEnchant(), expi.getMaxEnchant()));
								enchantedItems.Add(newItem);
							}
							addItem(extractedItems, newItem, 1);
							createItemAmount--;
						}
					}
				}
			}
		}

		if (extractedItems.Count == 0 && !specialReward)
		{
			player.sendPacket(SystemMessageId.FAILED_TO_CHANGE_THE_ITEM);
		}
		if (enchantedItems.Count != 0)
		{
			List<ItemInfo> items = enchantedItems.Select(x => new ItemInfo(x, ItemChangeType.MODIFIED)).ToList();
			InventoryUpdatePacket playerIU = new InventoryUpdatePacket(items);
			player.sendPacket(playerIU);
		}

		foreach (var entry in extractedItems)
		{
			sendMessage(player, entry.Key, entry.Value);
		}

		AutoPeelRequest request = player.getRequest<AutoPeelRequest>();
		if (request != null)
		{
			if (request.isProcessing())
			{
				request.setProcessing(false);
				List<ItemHolder> rewards = new();
				foreach (var entry in extractedItems)
				{
					rewards.Add(new ItemHolder(entry.Key.getId(), entry.Value));
				}

				player.sendPacket(new ExResultItemAutoPeelPacket(true, request.getTotalPeelCount(), request.getRemainingPeelCount() - 1, rewards));
			}
			else
			{
				player.sendPacket(new ExStopItemAutoPeelPacket(false));
			}
		}

		return true;
	}

	private void addItem(Map<Item, long> extractedItems, Item newItem, long count)
	{
		if (extractedItems.ContainsKey(newItem))
		{
			extractedItems.put(newItem, extractedItems.get(newItem) + count);
		}
		else
		{
			extractedItems.put(newItem, count);
		}
	}

	private void sendMessage(Player player, Item item, long count)
	{
		SystemMessagePacket sm;
		if (count > 1)
		{
			sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_X_S2);
			sm.Params.addItemName(item);
			sm.Params.addLong(count);
		}
		else if (item.getEnchantLevel() > 0)
		{
			sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_S2);
			sm.Params.addInt(item.getEnchantLevel());
			sm.Params.addItemName(item);
		}
		else
		{
			sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1);
			sm.Params.addItemName(item);
		}

		player.sendPacket(sm);
	}
}