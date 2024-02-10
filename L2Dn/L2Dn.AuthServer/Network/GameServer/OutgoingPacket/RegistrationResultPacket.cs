using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.GameServer.OutgoingPacket;

internal readonly struct RegistrationResultPacket(RegistrationResult result): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.RegistrationResult);
        writer.WriteEnum(result);
    }
}