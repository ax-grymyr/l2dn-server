using L2Dn.GameServer.Model.Matching;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMPCCRoomInfoPacket: IOutgoingPacket
{
    private readonly CommandChannelMatchingRoom _room;
	
    public ExMPCCRoomInfoPacket(CommandChannelMatchingRoom room)
    {
        _room = room;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MPCC_ROOM_INFO);
        
        writer.WriteInt32(_room.getId());
        writer.WriteInt32(_room.getMaxMembers());
        writer.WriteInt32(_room.getMinLevel());
        writer.WriteInt32(_room.getMaxLevel());
        writer.WriteInt32((int)_room.getLootType());
        writer.WriteInt32(_room.getLocation());
        writer.WriteString(_room.getTitle());
    }
}