using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;

public readonly struct ChangePasswordPacket(int accountId, string currentPassword, string newPassword): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ChangePassword);
        
        writer.WriteInt32(accountId);
        writer.WriteString(currentPassword);
        writer.WriteString(newPassword);

        const int signature = 0x7ABD_9123; // prime
        writer.WriteInt32(signature * newPassword.GetHashCode());
    }
}