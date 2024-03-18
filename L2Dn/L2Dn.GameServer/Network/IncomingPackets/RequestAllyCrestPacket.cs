using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestAllyCrestPacket: IIncomingPacket<GameSession>
{
    private int _crestId;
    private int _clanId;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadInt32(); // Server ID
        _crestId = reader.ReadInt32();
        reader.ReadInt32(); // Ally ID
        _clanId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        player.sendPacket(new AllyCrestPacket(_crestId, _clanId));

        return ValueTask.CompletedTask;
    }
}