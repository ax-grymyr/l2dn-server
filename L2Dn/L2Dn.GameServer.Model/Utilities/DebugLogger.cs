using NLog;

namespace L2Dn.GameServer.Utilities;

public static class PacketLogger
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(PacketLogger));

    public static Logger Instance => _logger;
}