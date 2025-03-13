using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

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