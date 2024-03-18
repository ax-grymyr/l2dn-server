using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPartyMatchDetailPacket: IIncomingPacket<GameSession>
{
    private int _roomId;
    private int _location;
    private int _level;

    public void ReadContent(PacketBitReader reader)
    {
        _roomId = reader.ReadInt32();
        _location = reader.ReadInt32();
        _level = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.isInMatchingRoom())
            return ValueTask.CompletedTask;

        MatchingRoom room = _roomId > 0
            ? MatchingRoomManager.getInstance().getPartyMathchingRoom(_roomId)
            : MatchingRoomManager.getInstance().getPartyMathchingRoom(_location, _level);
        
        if (room != null)
            room.addMember(player);

        return ValueTask.CompletedTask;
    }
}