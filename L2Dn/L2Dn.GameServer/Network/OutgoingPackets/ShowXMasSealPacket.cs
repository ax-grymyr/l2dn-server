using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ShowXMasSealPacket: IOutgoingPacket
{
    private readonly int _item;
	
    public ShowXMasSealPacket(int item)
    {
        _item = item;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHOW_XMAS_SEAL);
        
        writer.WriteInt32(_item);
    }
}