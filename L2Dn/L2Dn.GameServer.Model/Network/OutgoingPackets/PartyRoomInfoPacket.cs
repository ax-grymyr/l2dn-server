using L2Dn.GameServer.Model.Matching;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PartyRoomInfoPacket: IOutgoingPacket
{
    private readonly PartyMatchingRoom _room;

    public PartyRoomInfoPacket(PartyMatchingRoom room)
    {
        _room = room;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PARTY_ROOM_INFO);

        writer.WriteInt32(_room.Id);
        writer.WriteInt32(_room.getMaxMembers());
        writer.WriteInt32(_room.getMinLevel());
        writer.WriteInt32(_room.getMaxLevel());
        writer.WriteInt32((int)_room.getLootType());
        writer.WriteInt32(_room.getLocation());
        writer.WriteString(_room.getTitle());
    }
}