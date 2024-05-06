using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExChooseInventoryAttributeItemPacket: IOutgoingPacket
{
    private readonly int _itemId;
    private readonly long _count;
    private readonly AttributeType _attribute;
    private readonly int _level;
    private readonly Set<int> _items;
	
    public ExChooseInventoryAttributeItemPacket(Player player, Item stone)
    {
        _itemId = stone.getDisplayId();
        _count = stone.getCount();
        _attribute = ElementalAttributeData.getInstance().getItemElement(_itemId);
        if (_attribute == AttributeType.NONE)
        {
            throw new ArgumentException("Undefined Atribute item: " + stone);
        }
        
        _level = ElementalAttributeData.getInstance().getMaxElementLevel(_itemId);
        
        // Register only items that can be put an attribute stone/crystal
        _items = new Set<int>();
        foreach (Item item in player.getInventory().getItems())
        {
            if (item.isElementable())
            {
                _items.add(item.getObjectId());
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHOOSE_INVENTORY_ATTRIBUTE_ITEM);
        
        writer.WriteInt32(_itemId);
        writer.WriteInt64(_count);
        writer.WriteInt32(_attribute == AttributeType.FIRE); // Fire
        writer.WriteInt32(_attribute == AttributeType.WATER); // Water
        writer.WriteInt32(_attribute == AttributeType.WIND); // Wind
        writer.WriteInt32(_attribute == AttributeType.EARTH); // Earth
        writer.WriteInt32(_attribute == AttributeType.HOLY); // Holy
        writer.WriteInt32(_attribute == AttributeType.DARK); // Unholy
        writer.WriteInt32(_level); // Item max attribute level
        writer.WriteInt32(_items.size());
        foreach (int item in _items)
        {
            writer.WriteInt32(item);
        }
    }
}