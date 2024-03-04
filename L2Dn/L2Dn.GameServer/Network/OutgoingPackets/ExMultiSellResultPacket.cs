using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMultiSellResultPacket: IOutgoingPacket
{
    private readonly bool _success;
    private readonly int _type;
    private readonly int _count;
	
    public ExMultiSellResultPacket(bool success, int type, int count)
    {
        _success = success;
        _type = type;
        _count = count;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MULTISELL_RESULT);
        
        writer.WriteByte(_success);
        writer.WriteInt32(_type);
        writer.WriteInt32(_count);
    }
}