using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.NewHenna;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets.NewHenna;

public struct RequestNewHennaComposePacket: IIncomingPacket<GameSession>
{
    private int _slotOneIndex;
    private int _slotOneItemId;
    private int _slotTwoItemId;

    public void ReadContent(PacketBitReader reader)
    {
        _slotOneIndex = reader.ReadInt32();
        _slotOneItemId = reader.ReadInt32();
        _slotTwoItemId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Inventory inventory = player.getInventory();
        Item? slotOneItem = inventory.getItemByObjectId(_slotOneItemId);
        Item? slotTwoItem = inventory.getItemByObjectId(_slotTwoItemId);
        Henna? henna = player.getHenna(_slotOneIndex); // TODO: verify if this is correct
        if (henna == null || (_slotOneItemId != -1 && slotOneItem == null) ||
            (_slotTwoItemId != -1 && slotTwoItem == null))
        {
            return ValueTask.CompletedTask;
        }

    	CombinationHenna? combinationHennas = HennaCombinationData.getInstance().getByHenna(henna.getDyeId());
		if (combinationHennas == null)
		{
			player.sendPacket(new NewHennaPotenComposePacket(henna.getDyeId(), -1, false));
			return ValueTask.CompletedTask;
		}

        if ((_slotOneItemId != -1 && combinationHennas.getItemOne() != _slotOneItemId) ||
            (_slotTwoItemId != -1 && combinationHennas.getItemTwo() != _slotTwoItemId))
        {
            PacketLogger.Instance.Info(GetType().Name + ": player " + player.getName() + " - " + player.ObjectId +
                " have modified client or combination data is outdated!");
        }

        long commission = combinationHennas.getCommission();
		if (commission > player.getAdena())
		{
			return ValueTask.CompletedTask;
		}

		ItemHolder one = new ItemHolder(combinationHennas.getItemOne(), combinationHennas.getCountOne());
		ItemHolder two = new ItemHolder(combinationHennas.getItemTwo(), combinationHennas.getCountTwo());
		if ((_slotOneItemId != -1 && slotOneItem != null && slotOneItem.getCount() < one.getCount()) ||
            (_slotTwoItemId != -1 && slotTwoItem != null && slotTwoItem.getCount() < two.getCount()))
		{
			player.sendPacket(new NewHennaPotenComposePacket(henna.getDyeId(), -1, false));
			return ValueTask.CompletedTask;
		}

		List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
		if (_slotOneItemId != -1 && slotOneItem != null)
		{
			itemsToUpdate.Add(new ItemInfo(slotOneItem, ItemChangeType.MODIFIED));
		}
		if (_slotTwoItemId != -1 && slotTwoItem != null)
		{
			itemsToUpdate.Add(new ItemInfo(slotTwoItem, ItemChangeType.MODIFIED));
		}

        Item? adenaItem = inventory.getItemByItemId(Inventory.ADENA_ID);
        if (adenaItem != null)
		    itemsToUpdate.Add(new ItemInfo(adenaItem, ItemChangeType.MODIFIED));

		if ((_slotOneItemId != -1 &&
		     inventory.destroyItemByItemId("Henna Improving", one.getId(), one.getCount(), player, null) == null) ||
		    (_slotTwoItemId != -1 &&
		     inventory.destroyItemByItemId("Henna Improving", two.getId(), two.getCount(), player, null) == null) ||
		    inventory.destroyItemByItemId("Henna Improving", Inventory.ADENA_ID, commission, player, null) == null)
		{
			player.sendPacket(new NewHennaPotenComposePacket(henna.getDyeId(), -1, false));
			return ValueTask.CompletedTask;
		}

		if (Rnd.get(0, 100) <= combinationHennas.getChance())
        {
			CombinationHennaReward reward = combinationHennas.getReward(CombinationItemType.ON_SUCCESS);
            Henna? rewardHenna = HennaData.getInstance().getHenna(reward.getHennaId());
            if (rewardHenna != null)
            {
                player.removeHenna(_slotOneIndex, false);
                player.addHenna(_slotOneIndex, rewardHenna);
                player.addItem("Henna Improving", reward.getId(), reward.getCount(), null, false);
                player.sendPacket(new NewHennaPotenComposePacket(reward.getHennaId(),
                    reward.getId() == 0 ? -1 : reward.getId(), true));
            }
        }
		else
		{
			CombinationHennaReward reward = combinationHennas.getReward(CombinationItemType.ON_FAILURE);
            Henna? rewardHenna = HennaData.getInstance().getHenna(reward.getHennaId());
			if (henna.getDyeId() != reward.getHennaId() && rewardHenna != null)
			{
				player.removeHenna(_slotOneIndex, false);
				player.addHenna(_slotOneIndex, rewardHenna);
			}

			player.addItem("Henna Improving", reward.getId(), reward.getCount(), null, false);
			player.sendPacket(new NewHennaPotenComposePacket(reward.getHennaId(), reward.getId() == 0 ? -1 : reward.getId(), false));
		}

		InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
		player.sendPacket(iu);

        return ValueTask.CompletedTask;
    }
}