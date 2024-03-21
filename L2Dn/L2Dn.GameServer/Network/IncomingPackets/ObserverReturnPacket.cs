using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ObserverReturnPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.inObserverMode())
        {
            player.leaveObserverMode();
            // player.teleToLocation(activeChar.getObsX(), activeChar.getObsY(), activeChar.getObsZ());
        }

        return ValueTask.CompletedTask;
    }
}