using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.OutgoingPackets.Collections;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Collections;

public struct RequestExCollectionOpenUiPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadByte(); // 1 = isClosed
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.hasRequest<AutoPeelRequest>())
            return ValueTask.CompletedTask;
		
        player.setTarget(null);
        connection.Send(new ExCollectionOpenUiPacket());
        return ValueTask.CompletedTask;
    }
}