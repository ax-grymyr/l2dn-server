using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
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

		Item requestedItem = player.getInventory().getItemByObjectId(_objectId);
		if (requestedItem == null)
		{
			player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
			return ValueTask.CompletedTask;
		}
		
		EquipmentUpgradeNormalHolder upgradeHolder = EquipmentUpgradeNormalData.getInstance().getUpgrade(_upgradeId);
		if (upgradeHolder == null || upgradeHolder.getType() != _typeId)
		{
			player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
			return ValueTask.CompletedTask;
		}
		
		Inventory inventory = player.getInventory();
		if (inventory.getItemByItemId(upgradeHolder.getInitialItem().getId()) == null ||
		    inventory.getInventoryItemCount(upgradeHolder.getInitialItem().getId(), -1) <
		    upgradeHolder.getInitialItem().getCount())
		{
			player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
			return ValueTask.CompletedTask;
		}

	    Map<int, long> discounts = new();
		
		if (upgradeHolder.isHasCategory(UpgradeDataType.MATERIAL))
		{
			foreach (ItemEnchantHolder material in upgradeHolder.getItems(UpgradeDataType.MATERIAL))
			{
				if (material.getCount() < 0)
				{
					player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
					PacketLogger.Instance.Warn(GetType().Name +
					                           ": material -> item -> count in file EquipmentUpgradeNormalData.xml for upgrade id " +
					                           upgradeHolder.getId() +
					                           " cant be less than 0! Aborting current request!");
					
					return ValueTask.CompletedTask;
				}

				if (inventory.getInventoryItemCount(material.getId(), material.getEnchantLevel()) < material.getCount())
				{
					player.sendPacket(ExUpgradeSystemNormalResultPacket.FAIL);
					return ValueTask.CompletedTask;
				}
				
				foreach (ItemHolder discount in EquipmentUpgradeNormalData.getInstance().getDiscount())
				{
					if (discount.getId() == material.getId())
					{
						discounts.put(material.getId(), discount.getCount());
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
		if (upgradeHolder.isHasCategory(UpgradeDataType.MATERIAL))
		{
			foreach (ItemHolder material in upgradeHolder.getItems(UpgradeDataType.MATERIAL))
			{
				player.destroyItemByItemId("UpgradeNormalEquipment", material.getId(),
					material.getCount() - (discounts.Count == 0 ? 0 : discounts.get(material.getId())), player, true);
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
			foreach (ItemEnchantHolder successItem in upgradeHolder.getItems(UpgradeDataType.ON_SUCCESS))
			{
				Item addedSuccessItem = player.addItem("UpgradeNormalEquipment", successItem.getId(), successItem.getCount(), player, true);
				if (successItem.getEnchantLevel() != 0)
				{
					isNeedToSendUpdate = true;
					addedSuccessItem.setEnchantLevel(successItem.getEnchantLevel());
				}
				
				addedSuccessItem.updateDatabase(true);
				resultItems.Add(new UniqueItemEnchantHolder(successItem, addedSuccessItem.ObjectId));
			}
			
			if (upgradeHolder.isHasCategory(UpgradeDataType.BONUS_TYPE) && Rnd.get(100d) < upgradeHolder.getChanceToReceiveBonusItems())
			{
				foreach (ItemEnchantHolder bonusItem in upgradeHolder.getItems(UpgradeDataType.BONUS_TYPE))
				{
					Item addedBonusItem = player.addItem("UpgradeNormalEquipment", bonusItem.getId(),
						bonusItem.getCount(), player, true);
					
					if (bonusItem.getEnchantLevel() != 0)
					{
						isNeedToSendUpdate = true;
						addedBonusItem.setEnchantLevel(bonusItem.getEnchantLevel());
					}

					addedBonusItem.updateDatabase(true);
					bonusItems.Add(new UniqueItemEnchantHolder(bonusItem, addedBonusItem.ObjectId));
				}
			}
		}
		else
		{
			if (upgradeHolder.isHasCategory(UpgradeDataType.ON_FAILURE))
			{
				foreach (ItemEnchantHolder failureItem in upgradeHolder.getItems(UpgradeDataType.ON_FAILURE))
				{
					Item addedFailureItem = player.addItem("UpgradeNormalEquipment", failureItem.getId(),
						failureItem.getCount(), player, true);
					
					if (failureItem.getEnchantLevel() != 0)
					{
						isNeedToSendUpdate = true;
						addedFailureItem.setEnchantLevel(failureItem.getEnchantLevel());
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