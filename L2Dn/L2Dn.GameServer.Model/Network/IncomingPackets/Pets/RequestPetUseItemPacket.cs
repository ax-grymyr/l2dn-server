using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pets;

public struct RequestPetUseItemPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        // TODO: implement me properly
        // packet.readLong();
        // packet.readInt();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null || !player.hasPet())
		    return ValueTask.CompletedTask;

	    // TODO flood protection
		// if (!client.getFloodProtectors().canUseItem())
		// 	return ValueTask.CompletedTask;

		Pet? pet = player.getPet();
		Item? item = pet?.getInventory().getItemByObjectId(_objectId);
		if (pet == null || item == null)
			return ValueTask.CompletedTask;

		if (!item.getTemplate().isForNpc())
		{
			player.sendPacket(SystemMessageId.THIS_PET_CANNOT_USE_THIS_ITEM);
			return ValueTask.CompletedTask;
		}

		if (player.isAlikeDead() || pet.isDead())
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addItemName(item);
			player.sendPacket(sm);
			return ValueTask.CompletedTask;
		}

		// If the item has reuse time and it has not passed.
		// Message from reuse delay must come from item.
		TimeSpan reuseDelay = item.getReuseDelay();
		if (reuseDelay > TimeSpan.Zero)
		{
			TimeSpan reuse = pet.getItemRemainingReuseTime(item.ObjectId);
			if (reuse > TimeSpan.Zero)
				return ValueTask.CompletedTask;
		}

		if (!item.isEquipped() && !item.getTemplate().checkCondition(pet, pet, true))
			return ValueTask.CompletedTask;

		useItem(pet, item, player);

		return ValueTask.CompletedTask;
	}

	private static void useItem(Pet pet, Item item, Player player)
	{
		if (item.isEquipable())
		{
			if (!item.getTemplate().isConditionAttached())
			{
				player.sendPacket(SystemMessageId.THIS_PET_CANNOT_USE_THIS_ITEM);
				return;
			}

			if (item.isEquipped())
			{
				pet.getInventory().unEquipItemInSlot(item.getLocationSlot());
			}
			else
			{
				pet.getInventory().equipItem(item);
			}

			player.sendPacket(new PetItemListPacket(pet.getInventory().getItems()));
			pet.updateAndBroadcastStatus(1);
		}
		else
		{
			IItemHandler? handler = ItemHandler.getInstance().getHandler(item.getEtcItem());
			if (handler != null)
			{
				if (handler.useItem(pet, item, false))
				{
					TimeSpan reuseDelay = item.getReuseDelay();
					if (reuseDelay > TimeSpan.Zero)
					{
						player.addTimeStampItem(item, reuseDelay);
					}

					player.sendPacket(new PetItemListPacket(pet.getInventory().getItems()));
					pet.updateAndBroadcastStatus(1);
				}
			}
			else
			{
				player.sendPacket(SystemMessageId.THIS_PET_CANNOT_USE_THIS_ITEM);
				PacketLogger.Instance.Warn("No item handler registered for itemId: " + item.getId());
			}
		}
    }
}