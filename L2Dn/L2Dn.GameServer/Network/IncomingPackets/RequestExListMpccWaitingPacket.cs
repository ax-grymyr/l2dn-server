using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExListMpccWaitingPacket: IIncomingPacket<GameSession>
{
    private int _page;
    private int _location;
    private int _level;

    public void ReadContent(PacketBitReader reader)
    {
        _page = reader.ReadInt32();
        _location = reader.ReadInt32();
        _level = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExListMpccWaitingPacket(_page, _location, _level));
        
        return ValueTask.CompletedTask;
    }
}