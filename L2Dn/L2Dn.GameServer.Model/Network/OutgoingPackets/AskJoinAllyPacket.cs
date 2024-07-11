using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AskJoinAllyPacket: IOutgoingPacket
{
    private readonly string _requestorName;
    private readonly int _requestorObjId;
	
    /**
     * @param requestorObjId
     * @param requestorName
     */
    public AskJoinAllyPacket(int requestorObjId, string requestorName)
    {
        _requestorName = requestorName;
        _requestorObjId = requestorObjId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ASK_JOIN_ALLIANCE);
        
        writer.WriteInt32(_requestorObjId);
        writer.WriteString(string.Empty); // Ally Name ?
        writer.WriteString(string.Empty); // TODO: Find me!
        writer.WriteString(_requestorName);
    }
}