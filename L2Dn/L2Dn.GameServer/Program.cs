using AR.L2.DbModel;
using AR.L2.Model;
using L2Dn;
using L2Dn.GameServer;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Geo;
using L2Dn.GameServer.Network;
using L2Dn.Logging;
using L2Dn.Network;

try
{
    Logger.Configure();
}
catch (Exception exception)
{
    Console.WriteLine($"Exception during logger initialization: {exception}");
    return;
}

try
{
    Logger.Info("Loading configuration...");
    ServerConfig.LoadConfig();

    L2DbContext.Config = ServerConfig.Instance.Database;
    GameServerConfig gameServerConfig = ServerConfig.Instance.GameServer;
    Console.Title = $"Game Server {gameServerConfig.ListenAddress}:{gameServerConfig.Port}";
    
    Logger.Info("Loading static data...");
    StaticData.Reload();
    
    Logger.Info("Loading geodata...");
    GeoEngine.Instance.LoadGeoData(@"E:\L2\L2J_Mobius_Classic_3.0_TheKamael\game\data\geodata");

    Logger.Info("Publishing server info...");
    await DbUtility.PublishGameServerAsync(gameServerConfig);
    
    Listener<GameSession> listener = new(new GameSessionFactory(), new GamePacketEncoderFactory(),
        new GamePacketHandler(), new BufferPool(), gameServerConfig.ListenIpAddress, gameServerConfig.Port);

    Logger.Info($"Starting listener {gameServerConfig.ListenIpAddress}:{gameServerConfig.Port}...");
    Task task = listener.Start();
    await DbUtility.SetGameServerOnlineAsync(gameServerConfig.Id, true);
    await ConsoleUtility.WaitForCtrlC().ConfigureAwait(false);
    await DbUtility.SetGameServerOnlineAsync(gameServerConfig.Id, false);
    Logger.Info("Stopping listener...");
    listener.Stop();
    await task.ConfigureAwait(false);
    Logger.Info("Game server stopped. It is safe to close terminal or window.");
}
catch (Exception exception)
{
    Logger.Fatal($"Exception during server start: {exception}");
}
