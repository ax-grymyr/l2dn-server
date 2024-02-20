using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct InventoryUpdatePacket: IOutgoingPacket
{
    private InventoryPacketHelper? _helper;

    public InventoryUpdatePacket(ItemInfo item)
    {
        _helper = new();
        _helper.Items.Add(item);
    }

    public InventoryUpdatePacket(Item item)
    {
        _helper = new();
        _helper.Items.Add(new ItemInfo(item));
    }

    public InventoryUpdatePacket(List<Item> items)
    {
        _helper = new();
        _helper.Items.AddRange(items.Select(i => new ItemInfo(i)));
    }

    public InventoryUpdatePacket(List<ItemInfo> items)
    {
        _helper = new();
        _helper.Items.AddRange(items);
    }
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.INVENTORY_UPDATE);
        
        writer.WriteByte(0); // 140
        writer.WriteInt32(0); // 140
        if (_helper is null)
            writer.WriteInt32(0); // item count
        else 
            _helper.WriteItems(writer);
    }
}