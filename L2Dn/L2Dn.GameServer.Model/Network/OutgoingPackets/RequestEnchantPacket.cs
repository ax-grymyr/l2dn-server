using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct RequestEnchantPacket: IOutgoingPacket
{
    private readonly int _result;
	
    public RequestEnchantPacket(int result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PRIVATE_STORE_WHOLE_MSG);
        
        writer.WriteInt32(_result);
    }
}