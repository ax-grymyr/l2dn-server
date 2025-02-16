using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMPCCRoomMemberPacket: IOutgoingPacket
{
    private readonly CommandChannelMatchingRoom _room;
    private readonly MatchingMemberType _type;
	
    public ExMPCCRoomMemberPacket(Player player, CommandChannelMatchingRoom room)
    {
        _room = room;
        _type = room.getMemberType(player);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MPCC_ROOM_MEMBER);

        writer.WriteInt32((int)_type);
        writer.WriteInt32(_room.getMembersCount());
        foreach (Player member in _room.getMembers())
        {
            writer.WriteInt32(member.ObjectId);
            writer.WriteString(member.getName());
            writer.WriteInt32(member.getLevel());
            writer.WriteInt32((int)member.getClassId());
            writer.WriteInt32(MapRegionManager.getInstance().getBBs(member.Location.Location2D));
            writer.WriteInt32((int)_room.getMemberType(member));
        }
    }
}