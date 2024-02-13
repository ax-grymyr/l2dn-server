using L2Dn.AuthServer.Network.IncomingPackets;
using L2Dn.AuthServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network;

internal sealed class AuthPacketHandler: PacketHandler<AuthSession>
{
    public AuthPacketHandler()
    {
        RegisterPacket<RequestAuthLoginPacket>(IncomingPacketCodes.RequestAuthLogin)
            .WithAllowedStates(AuthSessionState.Authorization);
        
        RegisterPacket<RequestServerLoginPacket>(IncomingPacketCodes.RequestServerLogin)
            .WithAllowedStates(AuthSessionState.GameServerLogin);
        
        RegisterPacket<RequestServerListPacket>(IncomingPacketCodes.RequestServerList)
            .WithAllowedStates(AuthSessionState.GameServerLogin);
        
        RegisterPacket<RequestGGAuthPacket>(IncomingPacketCodes.RequestGGAuth)
            .WithAllowedStates(AuthSessionState.Authorization);
        
        RegisterPacket<RequestPIAgreementCheckPacket>(IncomingPacketCodes.RequestPIAgreementCheck)
            .WithAllowedStates(AuthSessionState.GameServerLogin);
        
        RegisterPacket<RequestPIAgreementPacket>(IncomingPacketCodes.RequestPIAgreement)
            .WithAllowedStates(AuthSessionState.GameServerLogin);
    }

    protected override void OnConnected(Connection connection, AuthSession session)
    {
        InitPacket initPacket = new(session.Id, session.RsaKeyPair.ScrambledModulus, session.BlowfishKey);
        connection.Send(ref initPacket, SendPacketOptions.DontEncrypt | SendPacketOptions.NoPadding);
    }

    protected override bool OnPacketInvalidState(Connection connection, AuthSession session)
    {
        switch (session.State)
        {
            case AuthSessionState.Authorization:
                LoginFailPacket loginFailPacket = new(LoginFailReason.AccessDenied);
                connection.Send(ref loginFailPacket, SendPacketOptions.CloseAfterSending);
                break;
            
            case AuthSessionState.GameServerLogin:
                PlayFailPacket playFailPacket = new(PlayFailReason.SystemError);
                connection.Send(ref playFailPacket, SendPacketOptions.CloseAfterSending);
                break;
                
            default:
                connection.Close();
                break;
        }

        return false;
    }
}