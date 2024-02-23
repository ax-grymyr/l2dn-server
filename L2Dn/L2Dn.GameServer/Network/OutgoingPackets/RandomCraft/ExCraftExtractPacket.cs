using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.RandomCraft;

public readonly struct ExCraftExtractPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CRAFT_EXTRACT);
        
        writer.WriteByte(0);
    }
}