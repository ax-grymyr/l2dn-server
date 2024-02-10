using System.Net;
using System.Net.Sockets;
using L2Dn.Conversion;

namespace L2Dn.Utilities;

public static class IPAddressUtil
{
    public static int ConvertIP4AddressToInt(IPAddress address)
    {
        Span<byte> span = stackalloc byte[4];
        if (address.TryWriteBytes(span, out int bytesWritten) && bytesWritten == 4)
            return BigEndianBitConverter.ToInt32(span);

        throw new ArgumentException($"Error converting IPv4 address '{address}' to integer", nameof(address));
    }

    public static int ConvertIP4AddressToInt(string address)
    {
        if (IPAddress.TryParse(address, out IPAddress? ipAddress) &&
            ipAddress.AddressFamily == AddressFamily.InterNetwork)
            return ConvertIP4AddressToInt(ipAddress);

        throw new ArgumentException($"Invalid IPv4 address '{address}'", nameof(address));
    }
}