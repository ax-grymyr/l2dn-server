using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PartySmallWindowDeletePacket: IOutgoingPacket
{
    private readonly Player _member;

    public PartySmallWindowDeletePacket(Player member)
    {
        _member = member;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PARTY_SMALL_WINDOW_DELETE);
        
        writer.WriteInt32(_member.getObjectId());
        writer.WriteString(_member.getName());
    }
}