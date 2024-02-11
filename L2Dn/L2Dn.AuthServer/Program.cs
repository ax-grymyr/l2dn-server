using L2Dn.AuthServer;
using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Db;
using L2Dn.AuthServer.Model;
using L2Dn.Utilities;
using NLog;

Logger logger = LogManager.GetLogger(nameof(AuthServer));

try
{
    LogUtil.ConfigureConsoleOutput();
    logger.Info("Loading configuration...");
    Config.Load();
    logger.Info("Initialize logger...");
    Config.Instance.Logging.ConfigureLogger();
}
catch (Exception exception)
{
    Console.WriteLine($"Exception when initializing logger: {exception}");
    return;
}

AuthServer authServer = new();
try
{
    logger.Info("Test database connection...");
    AuthServerDbContext.Config = Config.Instance.Database;
    GameServerManager.Instance.LoadServers();

    authServer.Start();
}
catch (Exception exception)
{
    logger.Fatal($"Exception during server start: {exception}");
    await authServer.StopAsync().ConfigureAwait(false); 
    return;
}

// Wait for Ctrl-C
await ConsoleUtil.WaitForCtrlC().ConfigureAwait(false);
await authServer.StopAsync().ConfigureAwait(false); 
logger.Info("Login server stopped. It is safe to close terminal or window.");