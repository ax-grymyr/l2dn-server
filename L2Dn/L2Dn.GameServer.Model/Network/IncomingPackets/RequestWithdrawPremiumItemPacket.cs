using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestWithdrawPremiumItemPacket: IIncomingPacket<GameSession>
{
    private int _itemNum;
    private int _charId;
    private long _itemCount;

    public void ReadContent(PacketBitReader reader)
    {
        _itemNum = reader.ReadInt32();
        _charId = reader.ReadInt32();
        _itemCount = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_itemCount <= 0)
            return ValueTask.CompletedTask;

        if (player.ObjectId != _charId)
        {
            Util.handleIllegalPlayerAction(player, "[RequestWithDrawPremiumItem] Incorrect owner, Player: " + player.getName(), Config.DEFAULT_PUNISH);
            return ValueTask.CompletedTask;
        }

        if (player.getPremiumItemList().Count == 0)
        {
            Util.handleIllegalPlayerAction(player, "[RequestWithDrawPremiumItem] Player: " + player.getName() + " try to get item with empty list!", Config.DEFAULT_PUNISH);
            return ValueTask.CompletedTask;
        }

        if (player.getWeightPenalty() >= 3 || !player.isInventoryUnder90(false))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_RECEIVE_THE_DIMENSIONAL_ITEM_BECAUSE_YOU_HAVE_EXCEED_YOUR_INVENTORY_WEIGHT_QUANTITY_LIMIT);
            return ValueTask.CompletedTask;
        }

        if (player.isProcessingTransaction())
        {
            player.sendPacket(SystemMessageId.ITEMS_FROM_GAME_ASSISTANTS_CANNOT_BE_EXCHANGED);
            return ValueTask.CompletedTask;
        }

        PremiumItem? item = player.getPremiumItemList().get(_itemNum);
        if (item == null)
            return ValueTask.CompletedTask;

        if (item.getCount() < _itemCount)
            return ValueTask.CompletedTask;

        long itemsLeft = item.getCount() - _itemCount;
        player.addItem("PremiumItem", item.getItemId(), _itemCount, player.getTarget(), true);
        if (itemsLeft > 0)
        {
            item.updateCount(itemsLeft);
            player.updatePremiumItem(_itemNum, itemsLeft);
        }
        else
        {
            player.getPremiumItemList().remove(_itemNum);
            player.deletePremiumItem(_itemNum);
        }

        if (player.getPremiumItemList().Count == 0)
        {
            player.sendPacket(SystemMessageId.THERE_ARE_NO_MORE_DIMENSIONAL_ITEMS_TO_BE_FOUND);
        }
        else
        {
            player.sendPacket(new ExGetPremiumItemListPacket(player));
        }

        return ValueTask.CompletedTask;
    }
}