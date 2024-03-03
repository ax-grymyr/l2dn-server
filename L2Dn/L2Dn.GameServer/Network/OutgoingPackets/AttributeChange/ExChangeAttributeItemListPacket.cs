using L2Dn.GameServer.Model;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AttributeChange;

public readonly struct ExChangeAttributeItemListPacket: IOutgoingPacket
{
    private readonly List<ItemInfo> _itemsList;
    private readonly int _itemId;
	
    public ExChangeAttributeItemListPacket(int itemId, List<ItemInfo> itemList)
    {
        _itemId = itemId;
        _itemsList = itemList;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGE_ATTRIBUTE_ITEM_LIST);
        
        writer.WriteInt32(_itemId);
        writer.WriteInt32(_itemsList.Count);
        foreach (ItemInfo item in _itemsList)
        {
            InventoryPacketHelper.WriteItem(writer, item);
        }
    }
}