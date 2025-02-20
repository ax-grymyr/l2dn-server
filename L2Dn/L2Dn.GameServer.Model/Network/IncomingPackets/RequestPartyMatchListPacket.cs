using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPartyMatchListPacket: IIncomingPacket<GameSession>
{
    private int _roomId;
    private int _maxMembers;
    private int _minLevel;
    private int _maxLevel;
    private PartyDistributionType _lootType;
    private string _roomTitle;

    public void ReadContent(PacketBitReader reader)
    {
        _roomId = reader.ReadInt32();
        _maxMembers = reader.ReadInt32();
        _minLevel = reader.ReadInt32();
        _maxLevel = reader.ReadInt32();
        _lootType = (PartyDistributionType)reader.ReadInt32();
        _roomTitle = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (_roomId <= 0 && player.getMatchingRoom() == null)
        {
            PartyMatchingRoom room = new PartyMatchingRoom(_roomTitle, _lootType, _minLevel, _maxLevel, _maxMembers, player);
            player.setMatchingRoom(room);
        }
        else
        {
            MatchingRoom room = player.getMatchingRoom();
            if (room.getId() == _roomId && room.getRoomType() == MatchingRoomType.PARTY && room.isLeader(player))
            {
                room.setLootType(_lootType);
                room.setMinLevel(_minLevel);
                room.setMaxLevel(_maxLevel);
                room.setMaxMembers(_maxMembers);
                room.setTitle(_roomTitle);
				
                PartyRoomInfoPacket packet = new PartyRoomInfoPacket((PartyMatchingRoom)room);
                foreach (Player member in room.getMembers())
                {
                    member.sendPacket(packet);
                }
            }
        }

        return ValueTask.CompletedTask;
    }
}