using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ShopPreviewInfoPacket: IOutgoingPacket
{
    private readonly Map<int, int> _itemlist;
	
    public ShopPreviewInfoPacket(Map<int, int> itemlist)
    {
        _itemlist = itemlist;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHOP_PREVIEW_INFO);
        
        writer.WriteInt32(Inventory.PAPERDOLL_TOTALSLOTS);
        // Slots
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_UNDER));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_REAR));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_LEAR));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_NECK));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_RFINGER));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_LFINGER));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_HEAD));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_RHAND));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_LHAND));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_GLOVES));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_CHEST));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_LEGS));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_FEET));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_CLOAK));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_RHAND));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_HAIR));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_HAIR2));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_RBRACELET));
        writer.WriteInt32(getFromList(Inventory.PAPERDOLL_LBRACELET));
    }
	
    private int getFromList(int key)
    {
        return (_itemlist.containsKey(key) ? _itemlist.get(key) : 0);
    }
}