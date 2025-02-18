using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExJoinMpccRoomPacket: IIncomingPacket<GameSession>
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

        if (player.getMatchingRoom() != null)
            return ValueTask.CompletedTask;

        MatchingRoom? room = MatchingRoomManager.getInstance().getCCMatchingRoom(_roomId);
        if (room != null)
        {
            room.addMember(player);
        }

        return ValueTask.CompletedTask;
    }
}