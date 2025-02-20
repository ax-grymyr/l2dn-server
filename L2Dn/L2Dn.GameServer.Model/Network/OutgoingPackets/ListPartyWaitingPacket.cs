using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ListPartyWaitingPacket: IOutgoingPacket
{
    private const int NUM_PER_PAGE = 64;
	
    private readonly List<MatchingRoom> _rooms;
    private readonly int _size;
	
    public ListPartyWaitingPacket(PartyMatchingRoomLevelType type, int location, int page, int requestorLevel)
    {
        List<MatchingRoom> rooms = MatchingRoomManager.getInstance().getPartyMathchingRooms(location, type, requestorLevel);
        _size = rooms.Count;
        int startIndex = (page - 1) * NUM_PER_PAGE;
        int chunkSize = _size - startIndex;
        if (chunkSize > NUM_PER_PAGE)
        {
            chunkSize = NUM_PER_PAGE;
        }

        _rooms = new List<MatchingRoom>();
        for (int i = startIndex; i < startIndex + chunkSize; i++)
        {
            _rooms.Add(rooms[i]);
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.LIST_PARTY_WATING);
        
        writer.WriteInt32(_size);
        writer.WriteInt32(_rooms.Count);
        foreach (MatchingRoom room in _rooms)
        {
            writer.WriteInt32(room.getId());
            writer.WriteString(room.getTitle());
            writer.WriteInt32(room.getLocation());
            writer.WriteInt32(room.getMinLevel());
            writer.WriteInt32(room.getMaxLevel());
            writer.WriteInt32(room.getMaxMembers());
            writer.WriteString(room.getLeader().getName());
            writer.WriteInt32(room.getMembersCount());
            foreach (Player member in room.getMembers())
            {
                writer.WriteInt32((int)member.getClassId());
                writer.WriteString(member.getName());
            }
        }

        writer.WriteInt32(World.getInstance().getPartyCount()); // Helios
        writer.WriteInt32(World.getInstance().getPartyMemberCount()); // Helios
    }
}