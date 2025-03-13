using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.NewHenna;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewHenna;

public struct RequestNewHennaEquipPacket: IIncomingPacket<GameSession>
{
    private int _slotId;
    private int _symbolId;
    private int _otherItemId;

    public void ReadContent(PacketBitReader reader)
    {
        _slotId = reader.ReadByte();
        _symbolId = reader.ReadInt32();
        _otherItemId = reader.ReadInt32(); // CostItemId
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	return ValueTask.CompletedTask;
		// }

		if (player.getHennaEmptySlots() == 0)
		{
			PacketLogger.Instance.Warn(player + ": Invalid Henna error 0 Id " + _symbolId + " " + _slotId);
			player.sendPacket(SystemMessageId.YOU_CANNOT_MAKE_A_PATTERN);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		Item? item = player.getInventory().getItemByObjectId(_symbolId);
		if (item == null)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			player.sendPacket(new NewHennaEquipPacket(_slotId, 0, false));
			return ValueTask.CompletedTask;
		}

		Henna? henna = HennaData.getInstance().getHennaByItemId(item.getId());
		if (henna == null)
		{
			PacketLogger.Instance.Warn(player + ": Invalid Henna SymbolId " + _symbolId + " " + _slotId + " " + item.getTemplate());
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			player.sendPacket(SystemMessageId.YOU_CANNOT_MAKE_A_PATTERN);
			return ValueTask.CompletedTask;
		}

		long count = player.getInventory().getInventoryItemCount(henna.getDyeItemId(), -1);
		if (henna.isAllowedClass(player) && count >= henna.getWearCount() &&
		    (player.getAdena() >= henna.getWearFee() ||
		     player.getInventory().getItemByItemId(91663)?.getCount() >= henna.getL2CoinFee()) && // TODO: unhardcode
		    player.addHenna(_slotId, henna))
		{
			int feeType = 0;

			if (_otherItemId == 57)
			{
				feeType = henna.getWearFee();
			}

			if (_otherItemId == 91663)
			{
				feeType = henna.getL2CoinFee();
			}

			player.destroyItemByItemId("HennaDye", henna.getDyeItemId(), henna.getWearCount(), player, true);
			player.destroyItemByItemId("fee", _otherItemId, feeType, player, true);
            if (player.getAdena() > 0)
            {
                InventoryUpdatePacket iu =
                    new InventoryUpdatePacket(new ItemInfo(player.getInventory().getAdenaInstance()!,
                        ItemChangeType.MODIFIED));

                player.sendInventoryUpdate(iu);
            }

            player.sendPacket(new NewHennaEquipPacket(_slotId, henna.getDyeId(), true));
			player.getStat().recalculateStats(true);

            if (!player.isSubclassLocked())
			    player.sendPacket(new UserInfoPacket(player));
		}
		else
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_MAKE_A_PATTERN);
			if (!player.canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS) && !henna.isAllowedClass(player))
			{
				Util.handleIllegalPlayerAction(player,
					"Exploit attempt: Character " + player.getName() + " of account " + player.getAccountName() +
					" tryed to add a forbidden henna.", Config.DEFAULT_PUNISH);
			}

			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			player.sendPacket(new NewHennaEquipPacket(_slotId, henna.getDyeId(), false));
		}

        return ValueTask.CompletedTask;
    }
}