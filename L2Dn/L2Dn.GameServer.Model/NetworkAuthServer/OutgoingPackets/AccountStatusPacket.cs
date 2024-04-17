using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;

internal readonly struct AccountStatusPacket(int accountId, byte characterCount): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.AccountStatus);
        
        writer.WriteInt32(accountId);
        writer.WriteByte(characterCount);
    }
}