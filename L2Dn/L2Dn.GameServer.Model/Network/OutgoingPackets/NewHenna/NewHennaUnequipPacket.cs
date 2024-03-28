using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewHenna;

public readonly struct NewHennaUnequipPacket: IOutgoingPacket
{
    private readonly int _slotId;
    private readonly int _success;
	
    public NewHennaUnequipPacket(int slotId, int success)
    {
        _slotId = slotId;
        _success = success;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NEW_HENNA_UNEQUIP);
        
        writer.WriteByte((byte)_slotId);
        writer.WriteByte((byte)_success);
    }
}