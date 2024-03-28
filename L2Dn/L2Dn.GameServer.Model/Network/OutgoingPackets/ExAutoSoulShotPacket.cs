using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAutoSoulShotPacket: IOutgoingPacket
{
    private readonly int _itemId;
    private readonly bool _enable;
    private readonly int _type;
	
    /**
     * @param itemId
     * @param enable
     * @param type
     */
    public ExAutoSoulShotPacket(int itemId, bool enable, int type)
    {
        _itemId = itemId;
        _enable = enable;
        _type = type;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_AUTO_SOUL_SHOT);
        
        writer.WriteInt32(_itemId);
        writer.WriteInt32(_enable);
        writer.WriteInt32(_type);
    }
}