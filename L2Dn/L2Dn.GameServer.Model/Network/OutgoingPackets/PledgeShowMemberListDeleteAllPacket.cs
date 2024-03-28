using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeShowMemberListDeleteAllPacket: IOutgoingPacket
{
    public static readonly PledgeShowMemberListDeleteAllPacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_SHOW_MEMBER_LIST_DELETE_ALL);
    }
}