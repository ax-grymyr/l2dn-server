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

        MatchingRoom? matchingRoom = player.getMatchingRoom();
        if (_roomId <= 0 || matchingRoom == null)
        {
            PartyMatchingRoom room = new PartyMatchingRoom(_roomTitle, _lootType, _minLevel, _maxLevel, _maxMembers, player);
            player.setMatchingRoom(room);
        }
        else
        {
            if (matchingRoom.Id == _roomId && matchingRoom.getRoomType() == MatchingRoomType.PARTY && matchingRoom.isLeader(player))
            {
                matchingRoom.setLootType(_lootType);
                matchingRoom.setMinLevel(_minLevel);
                matchingRoom.setMaxLevel(_maxLevel);
                matchingRoom.setMaxMembers(_maxMembers);
                matchingRoom.setTitle(_roomTitle);

                PartyRoomInfoPacket packet = new PartyRoomInfoPacket((PartyMatchingRoom)matchingRoom);
                foreach (Player member in matchingRoom.getMembers())
                {
                    member.sendPacket(packet);
                }
            }
        }

        return ValueTask.CompletedTask;
    }
}