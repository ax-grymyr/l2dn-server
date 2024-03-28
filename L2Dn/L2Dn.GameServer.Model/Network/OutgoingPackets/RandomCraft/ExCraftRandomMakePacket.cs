using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.RandomCraft;

public readonly struct ExCraftRandomMakePacket: IOutgoingPacket
{
    private readonly int _itemId;
    private readonly long _itemCount;
	
    public ExCraftRandomMakePacket(int itemId, long itemCount)
    {
        _itemId = itemId;
        _itemCount = itemCount;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CRAFT_RANDOM_MAKE);

        writer.WriteByte(0); // Close window
        writer.WriteInt16(0x0F); // Unknown
        writer.WriteInt32(_itemId);
        writer.WriteInt64(_itemCount);
        writer.WriteByte(0); // Enchantment level
    }
}