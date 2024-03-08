using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.OutgoingPackets;

internal readonly struct GGAuthPacket(int response): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GGAuth);
        writer.WriteInt32(response);
        writer.WriteZeros(16);
    }
}