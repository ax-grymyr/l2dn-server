using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pets;

public struct RequestGiveItemToPetPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private long _amount;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _amount = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (_amount <= 0 || player == null || !player.hasPet())
			return ValueTask.CompletedTask;

		// TODO flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	player.sendMessage("You are giving items to pet too fast.");
		// 	return ValueTask.CompletedTask;
		// }

		if (player.hasItemRequest())
			return ValueTask.CompletedTask;

		// Alt game - Karma punishment
		if (!Config.ALT_GAME_KARMA_PLAYER_CAN_TRADE && player.getReputation() < 0)
			return ValueTask.CompletedTask;

		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendMessage("You cannot exchange items while trading.");
			return ValueTask.CompletedTask;
		}

		Item? item = player.getInventory().getItemByObjectId(_objectId);
		if (item == null)
			return ValueTask.CompletedTask;

		if (_amount > item.getCount())
		{
			Util.handleIllegalPlayerAction(player,
				GetType().Name + ": Character " + player.getName() + " of account " + player.getAccountName() +
				" tried to get item with oid " + _objectId + " from pet but has invalid count " + _amount +
				" item count: " + item.getCount(), Config.DEFAULT_PUNISH);

			return ValueTask.CompletedTask;
		}

		if (item.isAugmented())
			return ValueTask.CompletedTask;

		if (item.isHeroItem() || !item.isDropable() || !item.isDestroyable() || !item.isTradeable())
		{
			player.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return ValueTask.CompletedTask;
		}

		Pet? pet = player.getPet();
		if (pet == null || pet.isDead())
		{
			player.sendPacket(SystemMessageId.YOUR_PET_IS_DEAD_AND_ANY_ATTEMPT_YOU_MAKE_TO_GIVE_IT_SOMETHING_GOES_UNRECOGNIZED);
			return ValueTask.CompletedTask;
		}

		if (!pet.getInventory().validateCapacity(item))
		{
			player.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_ANY_MORE_ITEMS);
			return ValueTask.CompletedTask;
		}

		if (!pet.getInventory().validateWeight(item, _amount))
		{
			player.sendPacket(SystemMessageId.THE_PET_S_INVENTORY_IS_FULL);
			return ValueTask.CompletedTask;
		}

		Item? transferedItem = player.transferItem("Transfer", _objectId, _amount, pet.getInventory(), pet);
		if (transferedItem != null)
		{
			player.sendPacket(new PetItemListPacket(pet.getInventory().getItems()));
		}
		else
		{
			PacketLogger.Instance.Warn("Invalid item transfer request: " + pet.getName() + "(pet) --> " + player.getName());
		}

		return ValueTask.CompletedTask;
    }
}