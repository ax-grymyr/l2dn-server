using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Network.Client;
using L2Dn.AuthServer.Network.GameServer;
using L2Dn.Network;

namespace L2Dn.AuthServer;

public class AuthServer
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Listener<AuthSession>? _clientListener;
    private Listener<GameServerSession>? _gameServerListener;
    
    public AuthServer()
    {
    }

    public void Start()
    {
        ClientListenerConfig clientListenerConfig = Config.Instance.ClientListener;
        Console.Title = $"Auth Server {clientListenerConfig.ListenAddress}:{clientListenerConfig.Port}";
        _clientListener = new Listener<AuthSession>(new AuthSessionFactory(), new AuthPacketEncoderFactory(),
            new AuthPacketHandler(), clientListenerConfig.ListenAddress, clientListenerConfig.Port);

        Logger.Info($"Starting listener {clientListenerConfig.ListenAddress}:{clientListenerConfig.Port}...");
        _clientListener.Start(_cancellationTokenSource.Token);

        GameServerListenerConfig gameServerListenerConfig = Config.Instance.GameServerListener;
        _gameServerListener = new Listener<GameServerSession>(new GameServerSessionFactory(),
            new GameServerPacketEncoderFactory(), new GameServerPacketHandler(),
            gameServerListenerConfig.ListenAddress, gameServerListenerConfig.Port);

        Logger.Info($"Starting listener {gameServerListenerConfig.ListenAddress}:{gameServerListenerConfig.Port}...");
        _gameServerListener.Start(_cancellationTokenSource.Token);
    }

    public void Stop()
    {
        Logger.Info("Stopping listeners...");
        _cancellationTokenSource.Cancel();
    }
}