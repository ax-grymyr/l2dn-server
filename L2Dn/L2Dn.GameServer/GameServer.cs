using L2Dn.Cryptography;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.NetworkAuthServer;
using L2Dn.Network;
using NLog;
using Task = System.Threading.Tasks.Task;

namespace L2Dn.GameServer;

public class GameServer
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(GameServer));
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Listener<GameSession>? _clientListener;
    private Connector<AuthServerSession>? _authServerConnector;
    private Task? _clientListenerTask;
    private readonly DateTime _startTime = DateTime.UtcNow;

    public void Start()
    {
        // Preload data
        Config.Load();
        Scripts.Scripts.RegisterHandlers();
        StaticDataLoader.Load();
        Scripts.Scripts.RegisterQuests();
        Scripts.Scripts.RegisterScripts();

        long usedMem = GC.GetTotalAllocatedBytes() / 1024 / 1024;
        long totalMem = GC.GetTotalMemory(false) / 1024 / 1024;
        _logger.Info("Server started, using " + usedMem + " of " + totalMem + " MB total memory.");
        _logger.Info("Maximum number of connected players is " + Config.Server.MAXIMUM_ONLINE_USERS + ".");
        _logger.Info("Server loaded in " + (DateTime.UtcNow - _startTime).TotalSeconds + " seconds.");

        ClientListenerConfig clientListenerConfig = ServerConfig.Instance.ClientListener;
        Console.Title = $"Game Server {clientListenerConfig.ListenAddress}:{clientListenerConfig.Port}";
        _clientListener = new Listener<GameSession>(new GameSessionFactory(), new GamePacketEncoderFactory(),
            new GamePacketHandler(), clientListenerConfig.ListenAddress, clientListenerConfig.Port);

        _logger.Info($"Starting listener {clientListenerConfig.ListenAddress}:{clientListenerConfig.Port}...");
        _clientListenerTask = _clientListener.Start(_cancellationTokenSource.Token);

        ReadOnlySpan<byte> authServerBlowfishKey = "N-%H\"$*iP{)U&/bK,{{zo4P;"u8;
        AuthServerConnectionConfig authServerConnectionConfig = ServerConfig.Instance.AuthServerConnection;
        _authServerConnector = new Connector<AuthServerSession>(AuthServerSession.Instance,
            new AuthPacketEncoder(new BlowfishEngine(authServerBlowfishKey)),
            new AuthServerPacketHandler(), authServerConnectionConfig.Address, authServerConnectionConfig.Port);

        _logger.Info($"Connecting to {authServerConnectionConfig.Address}:{authServerConnectionConfig.Port}...");
        _authServerConnector.Start(_cancellationTokenSource.Token);
        _logger.Info("Server started at " + ServerInfo.ServerStarted);
    }

    public async Task StopAsync()
    {
        _logger.Info("Stopping listeners...");
        await _cancellationTokenSource.CancelAsync();

        if (_clientListenerTask is not null)
            await _clientListenerTask.ConfigureAwait(false);
    }
}