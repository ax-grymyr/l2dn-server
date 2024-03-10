using L2Dn.GameServer;
using L2Dn.GameServer.Configuration;
using L2Dn.Utilities;
using NLog;

Logger logger = LogManager.GetLogger(nameof(GameServer));

GameServer gameServer = new();
try
{
    LogUtil.ConfigureConsoleOutput();
    logger.Info("Loading configuration...");
    ServerConfig.Load();

    logger.Info("Initialize logger...");
    ServerConfig.Instance.Logging.ConfigureLogger();

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
