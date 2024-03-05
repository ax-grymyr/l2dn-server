using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewHenna;

public readonly struct NewHennaEquipPacket: IOutgoingPacket
{
    private readonly int _slotId;
    private readonly int _hennaId;
    private readonly bool _success;
	
    public NewHennaEquipPacket(int slotId, int hennaId, bool success)
    {
        _slotId = slotId;
        _hennaId = hennaId;
        _success = success;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NEW_HENNA_EQUIP);
        
        writer.WriteByte((byte)_slotId);
        writer.WriteInt32(_hennaId);
        writer.WriteByte(_success);
    }
}