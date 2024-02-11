using L2Dn.GameServer.NetworkAuthServer.IncomingPackets;
using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer;

internal sealed class AuthServerPacketHandler: PacketHandler<AuthServerSession, AuthServerSessionState>
{
    public AuthServerPacketHandler()
    {
        RegisterPacket<RegistrationResultPacket>(IncomingPacketCodes.RegistrationResult);
        RegisterPacket<LoginRequestPacket>(IncomingPacketCodes.LoginRequest);
        RegisterPacket<PingResponsePacket>(IncomingPacketCodes.PingResponse);
    }
}