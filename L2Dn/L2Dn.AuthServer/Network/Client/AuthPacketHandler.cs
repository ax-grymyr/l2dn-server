using L2Dn.AuthServer.Network.Client.IncomingPackets;
using L2Dn.AuthServer.Network.Client.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client;

internal sealed class AuthPacketHandler: PacketHandler<AuthSession, AuthSessionState>
{
    public AuthPacketHandler()
    {
        RegisterPacket<RequestAuthLoginPacket>(IncomingPacketCodes.RequestAuthLogin, AuthSessionState.Authorization);
        RegisterPacket<RequestServerLoginPacket>(IncomingPacketCodes.RequestServerLogin, AuthSessionState.GameServerLogin);
        RegisterPacket<RequestServerListPacket>(IncomingPacketCodes.RequestServerList, AuthSessionState.GameServerLogin);
        RegisterPacket<RequestGGAuthPacket>(IncomingPacketCodes.RequestGGAuth, AuthSessionState.Authorization);
        RegisterPacket<RequestPIAgreementCheckPacket>(IncomingPacketCodes.RequestPIAgreementCheck, AuthSessionState.GameServerLogin);
        RegisterPacket<RequestPIAgreementPacket>(IncomingPacketCodes.RequestPIAgreement, AuthSessionState.GameServerLogin);
    }
    
    public override ValueTask OnConnectedAsync(Connection<AuthSession> connection)
    {
        AuthSession session = connection.Session;
        InitPacket initPacket = new(session.Id, session.RsaKeyPair.ScrambledModulus, session.BlowfishKey);
        connection.Send(ref initPacket, SendPacketOptions.DontEncrypt | SendPacketOptions.NoPadding);
        return ValueTask.CompletedTask;
    }

    public override bool OnPacketInvalidState(Connection<AuthSession> connection)
    {
        switch (connection.Session.State)
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