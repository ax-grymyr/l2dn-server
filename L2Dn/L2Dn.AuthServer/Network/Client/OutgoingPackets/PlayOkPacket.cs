using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

internal readonly struct PlayOkPacket(int playKey1, int playKey2): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(OutgoingPacketCodes.PlayOk);
        writer.WriteInt32(playKey1);
        writer.WriteInt32(playKey2);
    }
}