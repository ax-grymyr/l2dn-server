using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestDismissPartyRoomPacket: IIncomingPacket<GameSession>
{
    private int _roomId;

    public void ReadContent(PacketBitReader reader)
    {
        _roomId = reader.ReadInt32();
        //reader.ReadInt32(); // unknown
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        MatchingRoom? room = player.getMatchingRoom();
        if (room == null || room.Id != _roomId || room.getRoomType() != MatchingRoomType.PARTY ||
            room.getLeader() != player)
        {
            return ValueTask.CompletedTask;
        }

        room.disbandRoom();

        return ValueTask.CompletedTask;
    }
}