using NLog;

namespace L2Dn.GameServer.Utilities;

public static class DebugLogger
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(DebugLogger));

    public static Logger Logger => _logger;
}