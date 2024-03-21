using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Mentoring;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Mentoring;

public struct RequestMenteeWaitingListPacket: IIncomingPacket<GameSession>
{
    private int _page;
    private int _minLevel;
    private int _maxLevel;

    public void ReadContent(PacketBitReader reader)
    {
        _page = reader.ReadInt32();
        _minLevel = reader.ReadInt32();
        _maxLevel = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ListMenteeWaitingPacket(_page, _minLevel, _maxLevel));
        
        return ValueTask.CompletedTask;
    }
}