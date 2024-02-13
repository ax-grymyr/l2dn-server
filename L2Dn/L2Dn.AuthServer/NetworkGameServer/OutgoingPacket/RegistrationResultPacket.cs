using L2Dn.Packets;

namespace L2Dn.AuthServer.NetworkGameServer.OutgoingPacket;

internal readonly struct RegistrationResultPacket(RegistrationResult result): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.RegistrationResult);
        writer.WriteInt32((int)result);
    }
}