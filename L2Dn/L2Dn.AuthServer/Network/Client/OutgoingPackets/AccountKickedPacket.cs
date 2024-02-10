using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

/// <summary>
/// 0x0B - GGAuth
/// </summary>
internal readonly struct AccountKickedPacket(AccountKickedReason reason): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x02); // packet code
        writer.WriteInt32((int)reason);
    }
}
