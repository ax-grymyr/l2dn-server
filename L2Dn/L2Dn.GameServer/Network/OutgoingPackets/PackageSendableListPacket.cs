using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PackageSendableListPacket: IOutgoingPacket
{
    private readonly ICollection<Item> _items;
    private readonly int _objectId;
    private readonly long _adena;
    private readonly int _sendType;
	
    public PackageSendableListPacket(int sendType, Player player, int objectId)
    {
        _sendType = sendType;
        _items = player.getInventory().getAvailableItems(true, true, true);
        _objectId = objectId;
        _adena = player.getAdena();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PACKAGE_SENDABLE_LIST);
        
        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt32(_items.Count);
            writer.WriteInt32(_items.Count);
            foreach (Item item in _items)
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt32(item.getObjectId());
            }
        }
        else
        {
            writer.WriteInt32(_objectId);
            writer.WriteInt64(_adena);
            writer.WriteInt32(_items.Count);
        }
    }
}