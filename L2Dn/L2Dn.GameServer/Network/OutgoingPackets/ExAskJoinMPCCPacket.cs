using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAskJoinMPCCPacket: IOutgoingPacket
{
    private readonly String _requestorName;
	
    /**
     * @param requestorName
     */
    public ExAskJoinMPCCPacket(String requestorName)
    {
        _requestorName = requestorName;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ASK_JOIN_MPCC);
        
        writer.WriteString(_requestorName); // name of CCLeader
        writer.WriteInt32(0); // TODO: Find me
    }
}