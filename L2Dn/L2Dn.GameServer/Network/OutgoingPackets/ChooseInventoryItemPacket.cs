using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ChooseInventoryItemPacket: IOutgoingPacket
{
    private readonly int _itemId;
	
    public ChooseInventoryItemPacket(int itemId)
    {
        _itemId = itemId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.CHOOSE_INVENTORY_ITEM);
        
        writer.WriteInt32(_itemId);
    }
}