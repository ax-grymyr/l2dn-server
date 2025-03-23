using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.EquipmentUpgradeNormal;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ExUpgradeSystemNormalRequestPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private int _typeId;
    private int _upgradeId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _typeId = reader.ReadInt32();
        _upgradeId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Item? requestedItem = player.getInventory().getItemByObjectId(_objectId);
		if (requestedItem == null)
		{
			player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
			return ValueTask.CompletedTask;
		}

		EquipmentUpgradeNormalHolder? upgradeHolder = EquipmentUpgradeNormalData.getInstance().getUpgrade(_upgradeId);
		if (upgradeHolder == null || upgradeHolder.getType() != _typeId)
		{
			player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
			return ValueTask.CompletedTask;
		}

		Inventory inventory = player.getInventory();
		if (inventory.getItemByItemId(upgradeHolder.getInitialItem().Id) == null ||
		    inventory.getInventoryItemCount(upgradeHolder.getInitialItem().Id, -1) <
		    upgradeHolder.getInitialItem().Count)
		{
			player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
			return ValueTask.CompletedTask;
		}

	    Map<int, long> discounts = new();

        List<ItemEnchantHolder>? materials = upgradeHolder.getItems(UpgradeDataType.MATERIAL);
		if (upgradeHolder.isHasCategory(UpgradeDataType.MATERIAL) && materials != null)
		{
			foreach (ItemEnchantHolder material in materials)
			{
				if (material.Count < 0)
				{
					player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
					PacketLogger.Instance.Warn(GetType().Name +
					                           ": material -> item -> count in file EquipmentUpgradeNormalData.xml for upgrade id " +
					                           upgradeHolder.getId() +
					                           " cant be less than 0! Aborting current request!");

					return ValueTask.CompletedTask;
				}

				if (inventory.getInventoryItemCount(material.Id, material.EnchantLevel) < material.Count)
				{
					player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
					return ValueTask.CompletedTask;
				}

				foreach (ItemHolder discount in EquipmentUpgradeNormalData.getInstance().getDiscount())
				{
					if (discount.Id == material.Id)
					{
						discounts.put(material.Id, discount.Count);
						break;
					}
				}
			}
		}

		long adena = upgradeHolder.getCommission();
		if (adena > 0 && inventory.getAdena() < adena)
		{
			player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
			return ValueTask.CompletedTask;
		}

		// Get materials.
		player.destroyItem("UpgradeNormalEquipment", _objectId, 1, player, true);
        List<ItemEnchantHolder>? materials2 = upgradeHolder.getItems(UpgradeDataType.MATERIAL);
		if (upgradeHolder.isHasCategory(UpgradeDataType.MATERIAL) && materials2 != null)
		{
			foreach (ItemEnchantHolder material in materials2)
			{
				player.destroyItemByItemId("UpgradeNormalEquipment", material.Id,
					material.Count - (discounts.Count == 0 ? 0 : discounts.get(material.Id)), player, true);
			}
		}

		if (adena > 0)
		{
			player.reduceAdena("UpgradeNormalEquipment", adena, player, true);
		}

		bool isNeedToSendUpdate = false;
		List<UniqueItemEnchantHolder> resultItems = new();
		List<UniqueItemEnchantHolder> bonusItems = new();

		if (Rnd.get(100d) < upgradeHolder.getChance())
		{
            List<ItemEnchantHolder>? itemsOnSuccess = upgradeHolder.getItems(UpgradeDataType.ON_SUCCESS);
            if (itemsOnSuccess != null)
            {
                foreach (ItemEnchantHolder successItem in itemsOnSuccess)
                {
                    Item? addedSuccessItem = player.addItem("UpgradeNormalEquipment", successItem.Id,
                        successItem.Count, player, true);

                    if (addedSuccessItem == null)
                    {
                        player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: proper message, atomic inventory update
                        return ValueTask.CompletedTask;
                    }

                    if (successItem.EnchantLevel != 0)
                    {
                        isNeedToSendUpdate = true;
                        addedSuccessItem.setEnchantLevel(successItem.EnchantLevel);
                    }

                    addedSuccessItem.updateDatabase(true);
                    resultItems.Add(new UniqueItemEnchantHolder(successItem, addedSuccessItem.ObjectId));
                }
            }

            List<ItemEnchantHolder>? bonusTypeItems = upgradeHolder.getItems(UpgradeDataType.BONUS_TYPE);
            if (upgradeHolder.isHasCategory(UpgradeDataType.BONUS_TYPE) && bonusTypeItems != null &&
                Rnd.get(100d) < upgradeHolder.getChanceToReceiveBonusItems())
			{
				foreach (ItemEnchantHolder bonusItem in bonusTypeItems)
				{
					Item? addedBonusItem = player.addItem("UpgradeNormalEquipment", bonusItem.Id,
						bonusItem.Count, player, true);

                    if (addedBonusItem == null)
                    {
                        player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: proper message, atomic inventory update
                        return ValueTask.CompletedTask;
                    }

					if (bonusItem.EnchantLevel != 0)
					{
						isNeedToSendUpdate = true;
						addedBonusItem.setEnchantLevel(bonusItem.EnchantLevel);
					}

					addedBonusItem.updateDatabase(true);
					bonusItems.Add(new UniqueItemEnchantHolder(bonusItem, addedBonusItem.ObjectId));
				}
			}
		}
		else
        {
            List<ItemEnchantHolder>? itemsOnFailure = upgradeHolder.getItems(UpgradeDataType.ON_FAILURE);
			if (upgradeHolder.isHasCategory(UpgradeDataType.ON_FAILURE) && itemsOnFailure != null)
			{
				foreach (ItemEnchantHolder failureItem in itemsOnFailure)
				{
					Item? addedFailureItem = player.addItem("UpgradeNormalEquipment", failureItem.Id,
						failureItem.Count, player, true);

                    if (addedFailureItem == null)
                    {
                        player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: proper message, atomic inventory update
                        return ValueTask.CompletedTask;
                    }

					if (failureItem.EnchantLevel != 0)
					{
						isNeedToSendUpdate = true;
						addedFailureItem.setEnchantLevel(failureItem.EnchantLevel);
					}

					addedFailureItem.updateDatabase(true);
					resultItems.Add(new UniqueItemEnchantHolder(failureItem, addedFailureItem.ObjectId));
				}
			}
			else
			{
				player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
			}
		}

		if (isNeedToSendUpdate)
		{
			player.sendItemList(); // for see enchant level in Upgrade UI
		}

		// Why need map of item and count? because method "addItem" return item, and if it exists in result will be count of all items, not of obtained.
		player.sendPacket(new ExUpgradeSystemNormalResultPacket(1, _typeId, true, resultItems, bonusItems));

        return ValueTask.CompletedTask;
    }
}