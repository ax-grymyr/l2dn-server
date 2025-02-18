using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Appearance;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets.Appearance;

public struct RequestShapeShiftingItemPacket: IIncomingPacket<GameSession>
{
    private int _targetItemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _targetItemObjId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		ShapeShiftingItemRequest request = player.getRequest<ShapeShiftingItemRequest>();
		if (player.isInStoreMode() || player.isCrafting() || player.isProcessingRequest() || player.isProcessingTransaction() || request == null)
		{
			player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_SYSTEM_DURING_TRADING_PRIVATE_STORE_AND_WORKSHOP_SETUP);
			return ValueTask.CompletedTask;
		}

		PlayerInventory inventory = player.getInventory();
		Item? targetItem = inventory.getItemByObjectId(_targetItemObjId);
		Item stone = request.getAppearanceStone();
		if (targetItem == null || stone == null)
		{
			player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (stone.getOwnerId() != player.ObjectId || targetItem.getOwnerId() != player.ObjectId)
		{
			player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (!targetItem.getTemplate().isAppearanceable())
		{
			player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_MODIFIED_OR_RESTORED);
			player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (targetItem.getItemLocation() != ItemLocation.INVENTORY && targetItem.getItemLocation() != ItemLocation.PAPERDOLL)
		{
			player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (inventory.getItemByObjectId(stone.ObjectId) == null)
		{
			player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		AppearanceStone? appearanceStone = AppearanceItemData.getInstance().getStone(stone.getId());
		if (appearanceStone == null)
		{
			player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (!appearanceStone.checkConditions(player, targetItem))
		{
			player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		Item extractItem = request.getAppearanceExtractItem();
		int extracItemId = 0;
		if (appearanceStone.getType() != AppearanceType.RESTORE && appearanceStone.getType() != AppearanceType.FIXED)
		{
			if (extractItem == null)
			{
				player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
				player.removeRequest<ShapeShiftingItemRequest>();
				return ValueTask.CompletedTask;
			}

			if (extractItem.getOwnerId() != player.ObjectId)
			{
				player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
				player.removeRequest<ShapeShiftingItemRequest>();
				return ValueTask.CompletedTask;
			}

			if (!extractItem.getTemplate().isAppearanceable())
			{
				player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
				player.removeRequest<ShapeShiftingItemRequest>();
				return ValueTask.CompletedTask;
			}

			if (extractItem.getItemLocation() != ItemLocation.INVENTORY && extractItem.getItemLocation() != ItemLocation.PAPERDOLL)
			{
				player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
				player.removeRequest<ShapeShiftingItemRequest>();
				return ValueTask.CompletedTask;
			}

			if (extractItem.getTemplate().getCrystalType() > targetItem.getTemplate().getCrystalType())
			{
				player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
				player.removeRequest<ShapeShiftingItemRequest>();
				return ValueTask.CompletedTask;
			}

			if (extractItem.getVisualId() > 0)
			{
				player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
				player.removeRequest<ShapeShiftingItemRequest>();
				return ValueTask.CompletedTask;
			}

			if (extractItem.getItemType() != targetItem.getItemType() || extractItem.getId() == targetItem.getId() || extractItem.ObjectId == targetItem.ObjectId)
			{
				player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
				player.removeRequest<ShapeShiftingItemRequest>();
				return ValueTask.CompletedTask;
			}

			if (extractItem.getTemplate().getBodyPart() != targetItem.getTemplate().getBodyPart() && (extractItem.getTemplate().getBodyPart() != ItemTemplate.SLOT_FULL_ARMOR || targetItem.getTemplate().getBodyPart() != ItemTemplate.SLOT_CHEST))
			{
				player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
				player.removeRequest<ShapeShiftingItemRequest>();
				return ValueTask.CompletedTask;
			}

			extracItemId = extractItem.getId();
		}

		long cost = appearanceStone.getCost();
		if (cost > player.getAdena())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_MODIFY_AS_YOU_DO_NOT_HAVE_ENOUGH_ADENA);
			player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (stone.getCount() < 1L)
		{
			player.sendPacket(ExShapeShiftingResultPacket.CLOSE);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (appearanceStone.getType() == AppearanceType.NORMAL &&
		    inventory.destroyItem(GetType().Name, extractItem, 1, player, this) == null)
		{
			player.sendPacket(ExShapeShiftingResultPacket.FAILED);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		inventory.destroyItem(GetType().Name, stone, 1, player, this);
		player.reduceAdena(GetType().Name, cost, extractItem, false);

		switch (appearanceStone.getType())
		{
			case AppearanceType.RESTORE:
			{
				targetItem.setVisualId(0);
				targetItem.getVariables().set(ItemVariables.VISUAL_APPEARANCE_STONE_ID, 0);
				break;
			}
			case AppearanceType.NORMAL:
			{
				targetItem.setVisualId(extractItem.getId());
				break;
			}
			case AppearanceType.BLESSED:
			{
				targetItem.setVisualId(extractItem.getId());
				break;
			}
			case AppearanceType.FIXED:
			{
				targetItem.removeVisualSetSkills();

				if (appearanceStone.getVisualIds().isEmpty())
				{
					extracItemId = appearanceStone.getVisualId();
					targetItem.setVisualId(appearanceStone.getVisualId());
					targetItem.getVariables().set(ItemVariables.VISUAL_APPEARANCE_STONE_ID, appearanceStone.getId());
				}
				else
				{
					AppearanceHolder holder = appearanceStone.findVisualChange(targetItem);
					if (holder != null)
					{
						extracItemId = holder.getVisualId();
						targetItem.setVisualId(holder.getVisualId());
						targetItem.getVariables().set(ItemVariables.VISUAL_APPEARANCE_STONE_ID, appearanceStone.getId());
					}
				}

				targetItem.applyVisualSetSkills();
				break;
			}
		}

		if (appearanceStone.getType() != AppearanceType.RESTORE && appearanceStone.getLifeTime() > TimeSpan.Zero)
		{
			targetItem.getVariables().set(ItemVariables.VISUAL_APPEARANCE_LIFE_TIME,
				DateTime.UtcNow + appearanceStone.getLifeTime());

			targetItem.scheduleVisualLifeTime();
		}

		targetItem.getVariables().storeMe();

		List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
		itemsToUpdate.Add(new ItemInfo(targetItem, ItemChangeType.MODIFIED));
		if (extractItem != null)
		{
			itemsToUpdate.Add(new ItemInfo(extractItem, ItemChangeType.MODIFIED));
		}

		if (inventory.getItemByObjectId(stone.ObjectId) == null)
		{
			itemsToUpdate.Add(new ItemInfo(stone, ItemChangeType.REMOVED));
		}
		else
		{
			itemsToUpdate.Add(new ItemInfo(stone, ItemChangeType.MODIFIED));
		}

		InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
		player.sendInventoryUpdate(iu);

		player.removeRequest<ShapeShiftingItemRequest>();
		player.sendPacket(new ExShapeShiftingResultPacket(ExShapeShiftingResultPacket.RESULT_SUCCESS,
			targetItem.getId(), extracItemId));

		if (targetItem.isEquipped())
		{
			player.broadcastUserInfo();
			ExUserInfoEquipSlotPacket slots = new ExUserInfoEquipSlotPacket(player, false);
			foreach (InventorySlot slot in EnumUtil.GetValues<InventorySlot>())
			{
				if (slot == (InventorySlot)targetItem.getLocationSlot())
				{
					slots.AddComponent(slot);
				}
			}

			player.sendPacket(slots);
		}

		player.sendPacket(new ExAdenaInvenCountPacket(player));

        return ValueTask.CompletedTask;
    }
}