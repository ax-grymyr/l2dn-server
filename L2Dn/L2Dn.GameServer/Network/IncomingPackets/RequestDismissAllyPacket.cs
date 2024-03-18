using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestDismissAllyPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (!player.isClanLeader())
        {
            player.sendPacket(SystemMessageId.ACCESS_ONLY_FOR_THE_CHANNEL_FOUNDER);
            return ValueTask.CompletedTask;
        }
		
        player.getClan().dissolveAlly(player);

        return ValueTask.CompletedTask;
    }
}