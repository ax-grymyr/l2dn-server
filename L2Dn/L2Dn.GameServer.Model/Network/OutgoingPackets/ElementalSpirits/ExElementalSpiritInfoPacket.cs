using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;

public readonly struct ExElementalSpiritInfoPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ELEMENTAL_SPIRIT_INFO);
    }
}