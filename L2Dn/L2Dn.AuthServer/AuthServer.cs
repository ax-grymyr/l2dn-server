using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Network.Client;
using L2Dn.AuthServer.Network.GameServer;
using L2Dn.Network;
using NLog;

namespace L2Dn.AuthServer;

public class AuthServer
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AuthServer));
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Listener<AuthSession>? _clientListener;
    private Task? _clientListenerTask;
    private Listener<GameServerSession>? _gameServerListener;
    private Task? _gameServerListenerTask;
    
    public AuthServer()
    {
    }

    public void Start()
    {
        ClientListenerConfig clientListenerConfig = Config.Instance.ClientListener;
        Console.Title = $"Auth Server {clientListenerConfig.ListenAddress}:{clientListenerConfig.Port}";
        _clientListener = new Listener<AuthSession>(new AuthSessionFactory(), new AuthPacketEncoderFactory(),
            new AuthPacketHandler(), clientListenerConfig.ListenAddress, clientListenerConfig.Port);

        _logger.Info($"Starting listener {clientListenerConfig.ListenAddress}:{clientListenerConfig.Port}...");
        _clientListenerTask = _clientListener.Start(_cancellationTokenSource.Token);

        GameServerListenerConfig gameServerListenerConfig = Config.Instance.GameServerListener;
        _gameServerListener = new Listener<GameServerSession>(new GameServerSessionFactory(),
            new GameServerPacketEncoderFactory(), new GameServerPacketHandler(),
            gameServerListenerConfig.ListenAddress, gameServerListenerConfig.Port);

        _logger.Info($"Starting listener {gameServerListenerConfig.ListenAddress}:{gameServerListenerConfig.Port}...");
        _gameServerListenerTask = _gameServerListener.Start(_cancellationTokenSource.Token);
    }

    public async Task StopAsync()
    {
        _logger.Info("Stopping listeners...");
        await _cancellationTokenSource.CancelAsync();
        
        if (_gameServerListenerTask is not null)
            await _gameServerListenerTask.ConfigureAwait(false);
        
        if (_clientListenerTask is not null)
            await _clientListenerTask.ConfigureAwait(false);
    }
}