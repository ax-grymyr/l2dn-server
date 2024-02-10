using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

/// <summary>
/// 0x03 - LoginOk
/// </summary>
internal readonly struct LoginOkPacket(int loginKey1, int loginKey2): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x03); // packet code
        writer.WriteInt32(loginKey1);
        writer.WriteInt32(loginKey2);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0x000003ea);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0x02);
        writer.WriteZeros(16);
    }
}
