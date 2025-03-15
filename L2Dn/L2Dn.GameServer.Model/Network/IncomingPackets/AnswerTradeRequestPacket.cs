using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct AnswerTradeRequestPacket: IIncomingPacket<GameSession>
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

        if (!player.getAccessLevel().AllowTransaction)
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        Player? partner = player.getActiveRequester();
        if (partner == null)
        {
            // Trade partner not found, cancel trade
            player.sendPacket(new TradeDonePacket(0));
            player.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
            player.setActiveRequester(null);
            return ValueTask.CompletedTask;
        }

        if (World.getInstance().getPlayer(partner.ObjectId) == null)
        {
            // Trade partner not found, cancel trade
            player.sendPacket(new TradeDonePacket(0));
            player.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
            player.setActiveRequester(null);
            return ValueTask.CompletedTask;
        }

        if (_response == 1 && !partner.isRequestExpired())
        {
            player.startTrade(partner);
        }
        else
        {
            SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_HAS_DENIED_YOUR_REQUEST_TO_TRADE);
            msg.Params.addString(player.getName());
            partner.sendPacket(msg);
        }

        // Clears requesting status
        player.setActiveRequester(null);
        partner.onTransactionResponse();
        return ValueTask.CompletedTask;
    }
}