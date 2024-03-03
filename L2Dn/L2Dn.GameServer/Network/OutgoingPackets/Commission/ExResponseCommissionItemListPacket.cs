using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Commission;

public readonly struct ExResponseCommissionItemListPacket: IOutgoingPacket
{
    private readonly int _sendType;
    private readonly ICollection<Item> _items;
	
    public ExResponseCommissionItemListPacket(int sendType, ICollection<Item> items)
    {
        _sendType = sendType;
        _items = items;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_COMMISSION_ITEM_LIST);
        
        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt32(_items.Count);
            writer.WriteInt32(_items.Count);
            foreach (Item item in _items)
            {
                InventoryPacketHelper.WriteItem(writer, item);
            }
        }
        else
        {
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
    }
}