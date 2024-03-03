using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct FriendAddRequestPacket: IOutgoingPacket
{
    private readonly String _requestorName;
	
    /**
     * @param requestorName
     */
    public FriendAddRequestPacket(String requestorName)
    {
        _requestorName = requestorName;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.FRIEND_ADD_REQUEST);
        
        writer.WriteByte(1);
        writer.WriteString(_requestorName);
    }
}