using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SpawnItemPacket: IOutgoingPacket
{
    private readonly Item _item;
	
    public SpawnItemPacket(Item item)
    {
        _item = item;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SPAWN_ITEM);
        
        writer.WriteInt32(_item.ObjectId);
        writer.WriteInt32(_item.getDisplayId());
        writer.WriteInt32(_item.getX());
        writer.WriteInt32(_item.getY());
        writer.WriteInt32(_item.getZ());
        // only show item count if it is a stackable item
        writer.WriteInt32(_item.isStackable());
        writer.WriteInt64(_item.getCount());
        writer.WriteInt32(0); // c2
        writer.WriteByte((byte)_item.getEnchantLevel()); // Grand Crusade
        writer.WriteByte(_item.getAugmentation() != null); // Grand Crusade
        writer.WriteByte((byte)_item.getSpecialAbilities().Count); // Grand Crusade
        writer.WriteByte(0);
    }
}