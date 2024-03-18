using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeExtendedInfoPacket: IIncomingPacket<GameSession>
{
    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        // TODO: Implement
        return ValueTask.CompletedTask;
    }
}