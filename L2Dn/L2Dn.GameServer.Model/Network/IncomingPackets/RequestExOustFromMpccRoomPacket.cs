using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExOustFromMpccRoomPacket: IIncomingPacket<GameSession>
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

        MatchingRoom room = player.getMatchingRoom();
        if (room != null && room.getLeader() == player && room.getRoomType() == MatchingRoomType.COMMAND_CHANNEL)
        {
            Player? target = World.getInstance().getPlayer(_objectId);
            if (target != null)
            {
                room.deleteMember(target, true);
            }
        }

        return ValueTask.CompletedTask;
    }
}