using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct MoveWithDeltaPacket: IIncomingPacket<GameSession>
{
    private int _dx;
    private int _dy;
    private int _dz;

    public void ReadContent(PacketBitReader reader)
    {
        _dx = reader.ReadInt32();
        _dy = reader.ReadInt32();
        _dz = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        // TODO: implement
        return ValueTask.CompletedTask;
    }
}