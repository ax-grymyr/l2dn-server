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
		
        TradeList trade = player.getActiveTradeList();
        if (trade == null)
            return ValueTask.CompletedTask;
		
        if (trade.isLocked())
            return ValueTask.CompletedTask;
		
        if (_response == 1)
        {
            if (trade.getPartner() == null || World.getInstance().getPlayer(trade.getPartner().ObjectId) == null)
            {
                // Trade partner not found, cancel trade
                player.cancelActiveTrade();
                player.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
                return ValueTask.CompletedTask;
            }
			
            if (trade.getOwner().hasItemRequest() || trade.getPartner().hasItemRequest())
                return ValueTask.CompletedTask;
			
            if (!player.getAccessLevel().allowTransaction())
            {
                player.cancelActiveTrade();
                player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
                return ValueTask.CompletedTask;
            }
			
            if (player.getInstanceWorld() != trade.getPartner().getInstanceWorld())
            {
                player.cancelActiveTrade();
                return ValueTask.CompletedTask;
            }
			
            if (player.Distance3D(trade.getPartner()) > 150)
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