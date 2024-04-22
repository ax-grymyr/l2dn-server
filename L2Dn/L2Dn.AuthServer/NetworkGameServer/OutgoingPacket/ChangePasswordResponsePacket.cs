using L2Dn.Packets;

namespace L2Dn.AuthServer.NetworkGameServer.OutgoingPacket;

internal readonly struct ChangePasswordResponsePacket(ChangePasswordResult result, int playerId)
    : IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ChangePasswordResponse);
        writer.WriteInt32(playerId);
        writer.WriteInt32((int)result);
    }
}