using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPutItemResultForVariationCancelPacket: IOutgoingPacket
{
    private readonly int _itemObjId;
    private readonly int _itemId;
    private readonly int _itemAug1;
    private readonly int _itemAug2;
    private readonly long _price;
	
    public ExPutItemResultForVariationCancelPacket(Item item, long price)
    {
        _itemObjId = item.getObjectId();
        _itemId = item.getDisplayId();
        _price = price;
        _itemAug1 = item.getAugmentation().getOption1Id();
        _itemAug2 = item.getAugmentation().getOption2Id();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PUT_ITEM_RESULT_FOR_VARIATION_CANCEL);
        
        writer.WriteInt32(_itemObjId);
        writer.WriteInt32(_itemId);
        writer.WriteInt32(_itemAug1);
        writer.WriteInt32(_itemAug2);
        writer.WriteInt64(_price);
        writer.WriteInt32(1);
    }
}