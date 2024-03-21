using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExDismissMpccRoomPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        MatchingRoom room = player.getMatchingRoom();
        if (room != null && room.getLeader() == player && room.getRoomType() == MatchingRoomType.COMMAND_CHANNEL)
        {
            room.disbandRoom();
        }
        
        return ValueTask.CompletedTask;
    }
}