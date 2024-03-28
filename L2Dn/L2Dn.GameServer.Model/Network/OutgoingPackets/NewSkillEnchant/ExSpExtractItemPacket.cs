using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewSkillEnchant;

public readonly struct ExSpExtractItemPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SP_EXTRACT_ITEM);
        
        writer.WriteByte(0);
        writer.WriteByte(false);
        writer.WriteInt32(Inventory.SP_POUCH);
    }
}