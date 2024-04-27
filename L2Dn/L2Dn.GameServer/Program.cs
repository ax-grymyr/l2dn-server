using L2Dn.GameServer;
using L2Dn.GameServer.Configuration;
using L2Dn.Utilities;
using NLog;

Logger logger = LogManager.GetLogger(nameof(GameServer));

try
{
    LogUtil.ConfigureConsoleOutput();
    logger.Info("Loading configuration...");
    ServerConfig.Load();

    logger.Info("Initializing logger...");
    ServerConfig.Instance.Logging.ConfigureLogger();

    logger.Info("Initializing database factory...");
    DbFactory.Initialize(ServerConfig.Instance.Database);
}
catch (Exception exception)
{
    logger.Fatal($"Exception during server start: {exception}");
    return;
}

if (args is ["-UpdateDatabase"])
{
    try
    {
        logger.Info("Updating database...");
        DbFactory.UpdateDatabase(ServerConfig.Instance.Database);
    }
    catch (Exception exception)
    {
        logger.Fatal($"Error updating the database: {exception.Message}");
        return;
    }
}

GameServer gameServer = new();
try
{
    gameServer.Start();
}
catch (Exception exception)
{
    logger.Fatal($"Exception during server start: {exception}");
    await gameServer.StopAsync().ConfigureAwait(false);
    return;
}

// Wait for Ctrl-C
await ConsoleUtil.WaitForCtrlC().ConfigureAwait(false);
await gameServer.StopAsync().ConfigureAwait(false); 
logger.Info("Game server stopped. It is safe to close terminal or window.");
