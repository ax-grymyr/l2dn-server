using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

internal readonly struct PlayFailPacket(PlayFailReason reason): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(OutgoingPacketCodes.PlayFail);
        writer.WriteInt32((int)reason);
    }
}
