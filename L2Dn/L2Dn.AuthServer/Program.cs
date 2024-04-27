using L2Dn.AuthServer;
using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Model;
using L2Dn.Utilities;
using NLog;

Logger logger = LogManager.GetLogger(nameof(AuthServer));

try
{
    LogUtil.ConfigureConsoleOutput();
    logger.Info("Loading configuration...");
    Config.Load();

    logger.Info("Initializing logger...");
    Config.Instance.Logging.ConfigureLogger();

    logger.Info("Initializing database factory...");
    DbFactory.Initialize(Config.Instance.Database);
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
        DbFactory.UpdateDatabase(Config.Instance.Database);
    }
    catch (Exception exception)
    {
        logger.Fatal($"Error updating the database: {exception.Message}");
        return;
    }
}

try
{
    GameServerManager.Instance.LoadServers();
}
catch (Exception exception)
{
    logger.Fatal($"Error connecting to the database: {exception.Message}");
    return;
}

AuthServer authServer = new();
try
{
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