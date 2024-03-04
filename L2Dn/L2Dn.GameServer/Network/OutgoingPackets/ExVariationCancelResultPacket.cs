using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExVariationCancelResultPacket: IOutgoingPacket
{
    public static readonly ExVariationCancelResultPacket STATIC_PACKET_SUCCESS = new(1);
    public static readonly ExVariationCancelResultPacket STATIC_PACKET_FAILURE = new(0);
	
    private readonly int _result;
	
    private ExVariationCancelResultPacket(int result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VARIATION_CANCEL_RESULT);
        
        writer.WriteInt32(_result);
    }
}