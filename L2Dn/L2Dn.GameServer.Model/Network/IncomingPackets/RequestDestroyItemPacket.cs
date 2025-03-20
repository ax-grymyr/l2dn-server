using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.LimitShop;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Microsoft.EntityFrameworkCore;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestDestroyItemPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private long _count;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _count = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		if (_count <= 0)
		{
			if (_count < 0)
			{
				Util.handleIllegalPlayerAction(player,
					"[RequestDestroyItem] Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to destroy item with oid " + _objectId + " but has count < 0!", Config.General.DEFAULT_PUNISH);
			}

			return ValueTask.CompletedTask;
		}

		// TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	player.sendMessage("You are destroying items too fast.");
		// 	return ValueTask.CompletedTask;
		// }

		long count = _count;
		if (player.isProcessingTransaction() || player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.WHILE_OPERATING_A_PRIVATE_STORE_OR_WORKSHOP_YOU_CANNOT_DISCARD_DESTROY_OR_TRADE_AN_ITEM);
			return ValueTask.CompletedTask;
		}

		if (player.hasItemRequest())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_DESTROY_OR_CRYSTALLIZE_ITEMS_WHILE_ENCHANTING_ATTRIBUTES);
			return ValueTask.CompletedTask;
		}

		Item? itemToRemove = player.getInventory().getItemByObjectId(_objectId);

		// if we can't find the requested item, its actually a cheat
		if (itemToRemove == null)
		{
			// GM can destroy other player items
			if (player.isGM())
			{
				WorldObject? obj = World.getInstance().findObject(_objectId);
				if (obj != null && obj.isItem())
				{
					if (_count > ((Item) obj).getCount())
					{
						count = ((Item) obj).getCount();
					}

					AdminCommandHandler.getInstance().useAdminCommand(player, "admin_delete_item " + _objectId + " " + count, true);
				}

				return ValueTask.CompletedTask;
			}

			player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_DESTROYED);
			return ValueTask.CompletedTask;
		}

		// Cannot discard item that the skill is consuming
		if (player.isCastingNow(s => s.getSkill().ItemConsumeId == itemToRemove.Id))
		{
			player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_DESTROYED);
			return ValueTask.CompletedTask;
		}

		int itemId = itemToRemove.Id;
		if (!Config.General.DESTROY_ALL_ITEMS && ((!player.canOverrideCond(PlayerCondOverride.DESTROY_ALL_ITEMS) && !itemToRemove.isDestroyable()) || CursedWeaponsManager.getInstance().isCursed(itemId)))
		{
			if (itemToRemove.isHeroItem())
			{
				player.sendPacket(SystemMessageId.HERO_WEAPONS_CANNOT_BE_DESTROYED);
			}
			else
			{
				player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_DESTROYED);
			}

			return ValueTask.CompletedTask;
		}

		if (!itemToRemove.isStackable() && count > 1)
		{
			Util.handleIllegalPlayerAction(player,
				"[RequestDestroyItem] Character " + player.getName() + " of account " + player.getAccountName() +
				" tried to destroy a non-stackable item with oid " + _objectId + " but has count > 1!",
				Config.General.DEFAULT_PUNISH);

			return ValueTask.CompletedTask;
		}

		if (!player.getInventory().canManipulateWithItemId(itemToRemove.Id))
		{
			player.sendMessage("You cannot use this item.");
			return ValueTask.CompletedTask;
		}

		if (_count > itemToRemove.getCount())
		{
			count = itemToRemove.getCount();
		}

		if (itemToRemove.getTemplate().isPetItem())
		{
			Summon? pet = player.getPet();
			if (pet != null && pet.getControlObjectId() == _objectId)
			{
				pet.unSummon(player);
			}

			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				int objectId = _objectId;
				ctx.Pets.Where(r => r.ItemObjectId == objectId).ExecuteDelete();
			}
			catch (Exception e)
			{
				PacketLogger.Instance.Warn("Could not delete pet objectid: " + e);
			}
		}
		if (itemToRemove.isTimeLimitedItem())
		{
			itemToRemove.endOfLife();
		}

		InventoryUpdatePacket iu;
		if (itemToRemove.isEquipped())
		{
			if (itemToRemove.getEnchantLevel() > 0)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
				sm.Params.addInt(itemToRemove.getEnchantLevel());
				sm.Params.addItemName(itemToRemove);
				player.sendPacket(sm);
			}
			else
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
				sm.Params.addItemName(itemToRemove);
				player.sendPacket(sm);
			}

			List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
			foreach (Item itm in player.getInventory().unEquipItemInSlotAndRecord(itemToRemove.getLocationSlot()))
			{
				itemsToUpdate.Add(new ItemInfo(itm, ItemChangeType.MODIFIED));
			}

			iu = new InventoryUpdatePacket(itemsToUpdate);
			player.sendInventoryUpdate(iu);
		}

		Item? removedItem = player.getInventory().destroyItem("Destroy", itemToRemove, count, player, null);
		if (removedItem == null)
			return ValueTask.CompletedTask;

		if (removedItem.getCount() == 0)
		{
			iu = new InventoryUpdatePacket(new ItemInfo(removedItem, ItemChangeType.REMOVED));
			iu.addRemovedItem(removedItem);
		}
		else
		{
			iu = new InventoryUpdatePacket(new ItemInfo(removedItem, ItemChangeType.MODIFIED));
			iu.addModifiedItem(removedItem);
		}

		player.sendInventoryUpdate(iu);

		// LCoin UI update.
		if (removedItem.Id == Inventory.LCOIN_ID)
		{
			player.sendPacket(new ExBloodyCoinCountPacket(player));
		}

		return ValueTask.CompletedTask;
    }
}