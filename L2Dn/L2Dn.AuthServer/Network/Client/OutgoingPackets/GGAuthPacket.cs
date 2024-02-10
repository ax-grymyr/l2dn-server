using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

/// <summary>
/// 0x0B - GGAuth
/// </summary>
internal readonly struct GGAuthPacket(int response): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x0B); // packet code
        writer.WriteInt32(response);
        writer.WriteZeros(16);
    }
}
