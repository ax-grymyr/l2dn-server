using L2Dn.GameServer.StaticData;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBRVersionPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        if (!Config.ENABLE_WORLD_EXCHANGE)
        {
            return;
        }

        writer.WritePacketCode(OutgoingPacketCodes.EX_BR_VERSION);

        writer.WriteByte(1); // enable world exchange
    }
}