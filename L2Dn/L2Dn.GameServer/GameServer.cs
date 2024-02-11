using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Network;
using L2Dn.Network;
using NLog;

namespace L2Dn.GameServer;

public class GameServer
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(GameServer));
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Listener<GameSession>? _clientListener;
    private Task? _clientListenerTask;

    public void Start()
    {
        ClientListenerConfig clientListenerConfig = ServerConfig.Instance.ClientListener;
        Console.Title = $"Game Server {clientListenerConfig.ListenAddress}:{clientListenerConfig.Port}";
        _clientListener = new Listener<GameSession>(new GameSessionFactory(), new GamePacketEncoderFactory(),
            new GamePacketHandler(), clientListenerConfig.ListenAddress, clientListenerConfig.Port);

        _logger.Info($"Starting listener {clientListenerConfig.ListenAddress}:{clientListenerConfig.Port}...");
        _clientListenerTask = _clientListener.Start(_cancellationTokenSource.Token);
    }

    public async Task StopAsync()
    {
        _logger.Info("Stopping listeners...");
        await _cancellationTokenSource.CancelAsync();

        if (_clientListenerTask is not null)
            await _clientListenerTask.ConfigureAwait(false);
    }
}