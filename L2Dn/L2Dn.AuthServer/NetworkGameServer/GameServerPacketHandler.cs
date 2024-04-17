using L2Dn.AuthServer.Model;
using L2Dn.AuthServer.NetworkGameServer.IncomingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using NLog;

namespace L2Dn.AuthServer.NetworkGameServer;

internal sealed class GameServerPacketHandler: PacketHandler<GameServerSession>
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(GameServerPacketHandler));
    
    public GameServerPacketHandler()
    {
        SetDefaultAllowedStates(GameServerSessionState.Default);
        
        RegisterPacket<RegisterGameServerPacket>(IncomingPacketCodes.RegisterGameServer);
        RegisterPacket<PingRequestPacket>(IncomingPacketCodes.PingRequest);
        RegisterPacket<UpdateStatusPacket>(IncomingPacketCodes.UpdateStatus);
        RegisterPacket<AccountStatusPacket>(IncomingPacketCodes.AccountStatus);
    }

    protected override void OnDisconnected(Connection connection, GameServerSession session)
    {
        GameServerInfo? serverInfo = session.ServerInfo;
        if (serverInfo is not null)
        {
            _logger.Info($"Game server {serverInfo.ServerId} is OFFLINE now.");
            serverInfo.Connection = null;
            serverInfo.IsOnline = false;
        }
    }
}