using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.OutgoingPackets;

internal readonly struct LoginOkPacket(int loginKey1, int loginKey2): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.LoginOk);
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
