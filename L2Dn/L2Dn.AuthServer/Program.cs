using AR.L2;
using AR.L2.AuthServer;
using L2Dn;
using L2Dn.AuthServer;
using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Model;
using L2Dn.AuthServer.Network.Client;
using L2Dn.DbModel;
using L2Dn.Logging;
using L2Dn.Network;

try
{
    Logger.Configure();
}
catch (Exception exception)
{
    Console.WriteLine($"Exception when initializing logger: {exception}");
    return;
}

try
{
    Logger.Info("Loading configuration...");
    ServerConfig.LoadConfig();

    L2DbContext.Config = ServerConfig.Instance.Database;
    AuthServerConfig authServerConfig = ServerConfig.Instance.AuthServer;
    Console.Title = $"Auth Server {authServerConfig.ListenAddress}:{authServerConfig.Port}";

    Logger.Info("Updating game server list...");
    GameServerList.Instance.UpdateFrom(await DbUtility.GetGameServerListAsync());
    
    Listener<AuthSession> listener = new(new AuthSessionFactory(), new AuthPacketEncoderFactory(),
        new AuthPacketHandler(), new BufferPool(), authServerConfig.ListenIpAddress, authServerConfig.Port);

    Logger.Info($"Starting listener {authServerConfig.ListenIpAddress}:{authServerConfig.Port}...");
    Task task = listener.Start();
    await ConsoleUtility.WaitForCtrlC().ConfigureAwait(false);

    Logger.Info("Stopping listener...");
    listener.Stop();
    await task.ConfigureAwait(false);
    Logger.Info("Login server stopped. It is safe to close terminal or window.");
}
catch (Exception exception)
{
    Logger.Fatal($"Exception during server start: {exception}");
}
