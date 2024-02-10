using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

/// <summary>
/// 0x06 - PlayFail
/// </summary>
internal readonly struct PlayFailPacket(PlayFailReason reason): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x06); // packet code
        writer.WriteInt32((int)reason);
    }
}
