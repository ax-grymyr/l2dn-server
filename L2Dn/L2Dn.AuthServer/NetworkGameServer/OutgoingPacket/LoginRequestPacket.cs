using L2Dn.Packets;

namespace L2Dn.AuthServer.NetworkGameServer.OutgoingPacket;

public readonly struct LoginRequestPacket(
    int accountId,
    string accountName,
    int loginKey1,
    int loginKey2,
    int playKey1,
    int playKey2): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.LoginRequest);
        writer.WriteInt32(accountId);
        writer.WriteString(accountName);
        writer.WriteInt32(loginKey1);
        writer.WriteInt32(loginKey2);
        writer.WriteInt32(playKey1);
        writer.WriteInt32(playKey2);
    }
}