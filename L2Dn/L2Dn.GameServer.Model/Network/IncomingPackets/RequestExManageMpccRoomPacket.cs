using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExManageMpccRoomPacket: IIncomingPacket<GameSession>
{
    private int _roomId;
    private int _maxMembers;
    private int _minLevel;
    private int _maxLevel;
    private string _title;

    public void ReadContent(PacketBitReader reader)
    {
        _roomId = reader.ReadInt32();
        _maxMembers = reader.ReadInt32();
        _minLevel = reader.ReadInt32();
        _maxLevel = reader.ReadInt32();
        reader.ReadInt32(); // Party Distrubtion Type
        _title = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        MatchingRoom? room = player.getMatchingRoom();
        if (room == null || room.Id != _roomId || room.getRoomType() != MatchingRoomType.COMMAND_CHANNEL ||
            room.getLeader() != player)
        {
            return ValueTask.CompletedTask;
        }

        room.setTitle(_title);
        room.setMaxMembers(_maxMembers);
        room.setMinLevel(_minLevel);
        room.setMaxLevel(_maxLevel);

        room.getMembers().ForEach(p => p.sendPacket(new ExMPCCRoomInfoPacket((CommandChannelMatchingRoom) room)));
        player.sendPacket(SystemMessageId.THE_COMMAND_CHANNEL_MATCHING_ROOM_INFORMATION_WAS_EDITED);

        return ValueTask.CompletedTask;
    }
}