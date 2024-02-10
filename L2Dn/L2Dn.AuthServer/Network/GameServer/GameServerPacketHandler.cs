using L2Dn.AuthServer.Network.Client.IncomingPackets;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.GameServer;

internal sealed class GameServerPacketHandler: PacketHandler<GameServerSession, GameServerSessionState>
{
    public GameServerPacketHandler()
    {
        RegisterPacket<RequestAuthLoginPacket>(IncomingPacketCodes.RequestAuthLogin, GameServerSessionState.None);
        RegisterPacket<RequestServerLoginPacket>(IncomingPacketCodes.RequestServerLogin, GameServerSessionState.None);
        RegisterPacket<RequestServerListPacket>(IncomingPacketCodes.RequestServerList, GameServerSessionState.None);
        RegisterPacket<RequestGGAuthPacket>(IncomingPacketCodes.RequestGGAuth, GameServerSessionState.None);
        RegisterPacket<RequestPIAgreementCheckPacket>(IncomingPacketCodes.RequestPIAgreementCheck, GameServerSessionState.None);
        RegisterPacket<RequestPIAgreementPacket>(IncomingPacketCodes.RequestPIAgreement, GameServerSessionState.None);
    }
}