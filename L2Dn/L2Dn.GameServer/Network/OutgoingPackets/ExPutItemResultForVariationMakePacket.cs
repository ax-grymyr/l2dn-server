using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPutItemResultForVariationMakePacket: IOutgoingPacket
{
    private readonly int _itemObjId;
    private readonly int _itemId;
	
    public ExPutItemResultForVariationMakePacket(int itemObjId, int itemId)
    {
        _itemObjId = itemObjId;
        _itemId = itemId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PUT_ITEM_RESULT_FOR_VARIATION_MAKE);
        
        writer.WriteInt32(_itemObjId);
        writer.WriteInt32(_itemId);
        writer.WriteInt32(1);
    }
}