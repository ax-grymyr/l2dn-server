using L2Dn.Packets;
using NLog;

namespace L2Dn;

internal static class LogUtils
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(LogUtils));
    
    public static void TracePacketData(ReadOnlySpan<byte> data, int sessionId)
    {
        ReadOnlySpan<byte> span = data;
        int position = 0;
        while (!span.IsEmpty)
        {
            int len = Math.Min(16, span.Length);
            string bytes = span[0].ToString("X2");
            for (int i = 1; i < len; i++)
                bytes += " " + span[i].ToString("X2");

            _logger.Trace($"S({sessionId})  0x{position:X8}: {bytes}");
            span = span[len..];
            position += 8;
        }
    }

    public static void TracePacketData(PacketBitReader reader, int sessionId)
    {
        int position = 0;
        while (reader.Length != 0)
        {
            string bytes = reader.ReadByte().ToString("X2");
            for (int i = 1; i < 16; i++)
            {
                if (reader.Length == 0)
                    break;

                bytes += " " + reader.ReadByte().ToString("X2");
            }

            _logger.Trace($"S({sessionId})  0x{position:X8}: {bytes}");
            position += 8;
        }
    }
}