using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExListMpccWaitingPacket: IOutgoingPacket
{
    private const int NUM_PER_PAGE = 64;

    private readonly int _size;
    private readonly List<MatchingRoom> _rooms;

    public ExListMpccWaitingPacket(int page, int location, int level)
    {
        List<MatchingRoom> rooms = MatchingRoomManager.getInstance().getCCMathchingRooms(location, level);
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
        writer.WritePacketCode(OutgoingPacketCodes.EX_LIST_MPCC_WAITING);

        writer.WriteInt32(_size);
        writer.WriteInt32(_rooms.Count);
        foreach (MatchingRoom room in _rooms)
        {
            writer.WriteInt32(room.Id);
            writer.WriteString(room.getTitle());
            writer.WriteInt32(room.getMembersCount());
            writer.WriteInt32(room.getMinLevel());
            writer.WriteInt32(room.getMaxLevel());
            writer.WriteInt32(room.getLocation());
            writer.WriteInt32(room.getMaxMembers());
            writer.WriteString(room.getLeader().getName());
        }
    }
}