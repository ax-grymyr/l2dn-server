using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Revenge;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Revenges;

public struct RequestExPvpBookShareRevengeListPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_objectId != player.getObjectId())
            return ValueTask.CompletedTask;
		
        player.sendPacket(new ExPvpBookShareRevengeListPacket(player));
        
        return ValueTask.CompletedTask;
    }
}