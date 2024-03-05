using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.NetworkAuthServer;
using L2Dn.Network;
using NLog;

namespace L2Dn.GameServer;

public class GameServer
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(GameServer));
    private static DateTime _serverStarted;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Listener<GameSession>? _clientListener;
    private Connector<AuthServerSession>? _authServerConnector;
    private Task? _clientListenerTask;

    public static DateTime ServerStarted => _serverStarted;
    
    public void Start()
    {
        ClientListenerConfig clientListenerConfig = ServerConfig.Instance.ClientListener;
        Console.Title = $"Game Server {clientListenerConfig.ListenAddress}:{clientListenerConfig.Port}";
        _clientListener = new Listener<GameSession>(new GameSessionFactory(), new GamePacketEncoderFactory(),
            new GamePacketHandler(), clientListenerConfig.ListenAddress, clientListenerConfig.Port);

        _logger.Info($"Starting listener {clientListenerConfig.ListenAddress}:{clientListenerConfig.Port}...");
        _clientListenerTask = _clientListener.Start(_cancellationTokenSource.Token);

        AuthServerConnectionConfig authServerConnectionConfig = ServerConfig.Instance.AuthServerConnection;
        _authServerConnector = new Connector<AuthServerSession>(AuthServerSession.Instance, new AuthServerPacketEncoder(),
            new AuthServerPacketHandler(), authServerConnectionConfig.Address, authServerConnectionConfig.Port);
        
        _authServerConnector.Start(_cancellationTokenSource.Token);
        _serverStarted = DateTime.UtcNow;
    }

    public async Task StopAsync()
    {
        _logger.Info("Stopping listeners...");
        await _cancellationTokenSource.CancelAsync();

        if (_clientListenerTask is not null)
            await _clientListenerTask.ConfigureAwait(false);
    }
}