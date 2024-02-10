using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

/// <summary>
/// 0x01 - LoginFail
/// </summary>
internal readonly struct LoginFailPacket(LoginFailReason reason): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x01); // packet code
        writer.WriteInt32((int)reason);
    }
}
