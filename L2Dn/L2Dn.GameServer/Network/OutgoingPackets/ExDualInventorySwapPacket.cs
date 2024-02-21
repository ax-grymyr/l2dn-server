using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExDualInventorySwapPacket: IOutgoingPacket
{
    private readonly int _slot;
	
    public ExDualInventorySwapPacket(int slot)
    {
        _slot = slot;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_DUAL_INVENTORY_INFO);
        
        writer.WriteByte((byte)_slot);
        writer.WriteByte(1); // Success.
        writer.WriteByte(1); // Stable swapping.
		
        // List SlotDBID.
        writer.WriteByte(0);
        writer.WriteInt32(0); // Old object id?
        writer.WriteInt32(0); // New object id?
		
        // List HennaPotenID.
        writer.WriteByte(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteByte(0);
        writer.WriteByte(0);
    }
}