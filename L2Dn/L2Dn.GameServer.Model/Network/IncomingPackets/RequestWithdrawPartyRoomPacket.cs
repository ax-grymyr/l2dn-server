using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestWithdrawPartyRoomPacket: IIncomingPacket<GameSession>
{
    private int _roomId;

    public void ReadContent(PacketBitReader reader)
    {
        _roomId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        MatchingRoom room = player.getMatchingRoom();
        if (room == null)
            return ValueTask.CompletedTask;
		
        if (room.getId() != _roomId || room.getRoomType() != MatchingRoomType.PARTY)
            return ValueTask.CompletedTask;
		
        room.deleteMember(player, false);

        return ValueTask.CompletedTask;
    }
}