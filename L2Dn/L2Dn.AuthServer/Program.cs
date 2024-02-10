using L2Dn;
using L2Dn.AuthServer;
using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Db;
using L2Dn.AuthServer.Model;
using L2Dn.Utilities;

try
{
    Logger.Configure();
}
catch (Exception exception)
{
    Console.WriteLine($"Exception when initializing logger: {exception}");
    return;
}

AuthServer authServer = new();
try
{
    Logger.Info("Loading configuration...");
    Config.Load();

    Logger.Info("Test database connection...");
    AuthServerDbContext.Config = Config.Instance.Database;
    GameServerManager.Instance.LoadServers();

    authServer.Start();
}
catch (Exception exception)
{
    Logger.Fatal($"Exception during server start: {exception}");
    authServer.Stop(); 
    return;
}

// Wait for Ctrl-C
await ConsoleUtil.WaitForCtrlC().ConfigureAwait(false);
authServer.Stop(); 
Logger.Info("Login server stopped. It is safe to close terminal or window.");