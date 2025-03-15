using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct TradeDoneRequestPacket: IIncomingPacket<GameSession>
{
    private int _response;

    public void ReadContent(PacketBitReader reader)
    {
        _response = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: flood protection
        // if (!client.getFloodProtectors().canPerformTransaction())
        // {
        //     player.sendMessage("You are trading too fast.");
        //     return;
        // }

        TradeList? trade = player.getActiveTradeList();
        if (trade == null)
            return ValueTask.CompletedTask;

        if (trade.isLocked())
            return ValueTask.CompletedTask;

        if (_response == 1)
        {
            Player? tradePartner = trade.getPartner();
            if (tradePartner == null || World.getInstance().getPlayer(tradePartner.ObjectId) == null)
            {
                // Trade partner not found, cancel trade
                player.cancelActiveTrade();
                player.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
                return ValueTask.CompletedTask;
            }

            if (trade.getOwner().hasItemRequest() || tradePartner.hasItemRequest())
                return ValueTask.CompletedTask;

            if (!player.getAccessLevel().AllowTransaction)
            {
                player.cancelActiveTrade();
                player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
                return ValueTask.CompletedTask;
            }

            if (player.getInstanceWorld() != tradePartner.getInstanceWorld())
            {
                player.cancelActiveTrade();
                return ValueTask.CompletedTask;
            }

            if (player.Distance3D(tradePartner) > 150)
            {
                player.cancelActiveTrade();
                return ValueTask.CompletedTask;
            }

            trade.confirm();
        }
        else
        {
            player.cancelActiveTrade();
        }

        return ValueTask.CompletedTask;
    }
}