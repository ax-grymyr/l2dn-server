using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;

namespace L2Dn.GameServer.Network;

public static class Extensions
{
    public static void Send(this Connection connection, SystemMessageId message)
    {
        connection.Send(new SystemMessagePacket(message));
    }

    public static void Send(this Connection connection, string message)
    {
        connection.Send(new SystemMessagePacket(message));
    }
}