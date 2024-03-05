using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.EquipmentUpgrade;

public readonly struct ExShowUpgradeSystemPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_UPGRADE_SYSTEM);
        
        writer.WriteInt16(1); // Flag
        writer.WriteInt16(100); // CommissionRatio
        writer.WriteInt32(0); // MaterialItemId (array)
        writer.WriteInt32(0); // MaterialRatio (array)
    }
}