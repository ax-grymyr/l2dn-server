using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct DropItemPacket: IOutgoingPacket
{
    private readonly Item _item;
    private readonly int _objectId;
	
    /**
     * Constructor of the DropItem server packet
     * @param item : Item designating the item
     * @param playerObjId : int designating the player ID who dropped the item
     */
    public DropItemPacket(Item item, int playerObjId)
    {
        _item = item;
        _objectId = playerObjId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.DROP_ITEM);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_item.ObjectId);
        writer.WriteInt32(_item.getDisplayId());
        writer.WriteInt32(_item.getX());
        writer.WriteInt32(_item.getY());
        writer.WriteInt32(_item.getZ());
        
        // only show item count if it is a stackable item
        writer.WriteByte(_item.isStackable());
        writer.WriteInt64(_item.getCount());
        writer.WriteInt32(0);
        writer.WriteByte(_item.getEnchantLevel() > 0);
        writer.WriteInt32(0);
        writer.WriteByte((byte)_item.getEnchantLevel()); // Grand Crusade
        writer.WriteByte(_item.getAugmentation() != null); // Grand Crusade
        writer.WriteByte((byte)_item.getSpecialAbilities().Count); // Grand Crusade
        writer.WriteByte(0);
    }
}