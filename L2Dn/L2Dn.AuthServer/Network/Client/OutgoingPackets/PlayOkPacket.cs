using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

/// <summary>
/// 0x07 - PlayOk
/// </summary>
internal readonly struct PlayOkPacket(int playKey1, int playKey2): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x07); // packet code
        writer.WriteInt32(playKey1);
        writer.WriteInt32(playKey2);
    }
}
