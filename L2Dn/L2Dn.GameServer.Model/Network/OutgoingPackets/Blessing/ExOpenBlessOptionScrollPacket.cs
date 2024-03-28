using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Blessing;

public readonly struct ExOpenBlessOptionScrollPacket: IOutgoingPacket
{
    private readonly int _itemId;
	
    public ExOpenBlessOptionScrollPacket(int itemId)
    {
        _itemId = itemId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_OPEN_BLESS_OPTION_SCROLL);
        
        writer.WriteInt32(_itemId);
    }
}