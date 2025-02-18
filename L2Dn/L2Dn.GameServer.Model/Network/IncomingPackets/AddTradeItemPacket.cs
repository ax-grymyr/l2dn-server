using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct AddTradeItemPacket: IIncomingPacket<GameSession>
{
    private int _tradeId;
    private int _objectId;
    private long _count;

    public void ReadContent(PacketBitReader reader)
    {
        _tradeId = reader.ReadInt32();
        _objectId = reader.ReadInt32();
        _count = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        TradeList trade = player.getActiveTradeList();
        if (trade == null)
        {
            PacketLogger.Instance.Warn("Character: " + player.getName() + " requested item:" + _objectId +
                                       " add without active tradelist:" + _tradeId);

            return ValueTask.CompletedTask;
        }

        Player partner = trade.getPartner();
        if ((partner == null) || (World.getInstance().getPlayer(partner.ObjectId) == null) || (partner.getActiveTradeList() == null))
        {
            // Trade partner not found, cancel trade
            if (partner != null)
            {
                PacketLogger.Instance.Warn("Character:" + player.getName() + " requested invalid trade object: " +
                                           _objectId);
            }

            player.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
            player.cancelActiveTrade();
            return ValueTask.CompletedTask;
        }

        if (trade.isConfirmed() || partner.getActiveTradeList().isConfirmed())
        {
            player.sendPacket(SystemMessageId.YOU_MAY_NO_LONGER_ADJUST_ITEMS_IN_THE_TRADE_BECAUSE_THE_TRADE_HAS_BEEN_CONFIRMED);
            return ValueTask.CompletedTask;
        }

        if (!player.getAccessLevel().allowTransaction())
        {
            player.sendMessage("Transactions are disabled for your Access Level.");
            player.cancelActiveTrade();
            return ValueTask.CompletedTask;
        }

        if (!player.validateItemManipulation(_objectId, "trade"))
        {
            player.sendPacket(SystemMessageId.NOTHING_HAPPENED);
            return ValueTask.CompletedTask;
        }

        Item item1 = player.getInventory().getItemByObjectId(_objectId);
        TradeItem item2 = trade.addItem(_objectId, _count);
        if (item2 != null)
        {
            player.sendPacket(new TradeOwnAddPacket(1, item2));
            player.sendPacket(new TradeOwnAddPacket(2, item2));
            player.sendPacket(new TradeUpdatePacket(1, null, null, 0));
            player.sendPacket(new TradeUpdatePacket(2, player, item2, item1.getCount() - item2.getCount()));
            partner.sendPacket(new TradeOtherAddPacket(1, item2));
            partner.sendPacket(new TradeOtherAddPacket(2, item2));
        }

        return ValueTask.CompletedTask;
    }
}