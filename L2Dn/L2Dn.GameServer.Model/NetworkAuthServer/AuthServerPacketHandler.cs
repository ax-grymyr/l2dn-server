using L2Dn.GameServer.NetworkAuthServer.IncomingPackets;
using L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer;

public sealed class AuthServerPacketHandler: PacketHandler<AuthServerSession>
{
    public AuthServerPacketHandler()
    {
        SetDefaultAllowedStates(AuthServerSessionState.Default);
        
        RegisterPacket<RegistrationResultPacket>(IncomingPacketCodes.RegistrationResult);
        RegisterPacket<LoginRequestPacket>(IncomingPacketCodes.LoginRequest);
        RegisterPacket<PingResponsePacket>(IncomingPacketCodes.PingResponse);
    }

    protected override void OnConnected(Connection connection, AuthServerSession session)
    {
        connection.Send(new RegisterGameServerPacket());
    }
}