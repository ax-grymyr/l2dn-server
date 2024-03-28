using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.RandomCraft;

public readonly struct ExCraftRandomRefreshPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CRAFT_RANDOM_REFRESH);

        writer.WriteByte(0);
    }
}