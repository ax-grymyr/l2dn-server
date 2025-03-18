using L2Dn.Network;
using NLog;

namespace L2Dn.Packets;

internal abstract class PacketHandlerHelper
{
    protected static readonly Logger Logger = LogManager.GetLogger(nameof(PacketHandlerHelper));

    public long AllowedStates { get; set; }
}

internal abstract class PacketHandlerHelper<TSession>: PacketHandlerHelper
    where TSession: Session
{
    public abstract ValueTask HandlePacketAsync(PacketHandler<TSession> handler,
        Connection connection, TSession session, PacketBitReader reader);
}

internal sealed class PacketHandlerHelper<TSession, TPacket>(byte packetCode): PacketHandlerHelper<TSession>
    where TSession: Session
    where TPacket: struct, IIncomingPacket<TSession>
{
    public override async ValueTask HandlePacketAsync(PacketHandler<TSession> handler,
        Connection connection, TSession session, PacketBitReader reader)
    {
        Logger.Trace($"S({session.Id})  Received packet {typeof(TPacket).Name} ({packetCode:X2}), length {reader.Length + 1}");

        long state = session.GetState();
        if (AllowedStates == 0 || (state & AllowedStates) == 0)
        {
            Logger.Trace($"S({session.Id})  Packet {typeof(TPacket).Name} ({packetCode:X2}) is not allowed in state '{state:b64}'");

            if (!handler.OnPacketInvalidStateInternal(connection, session))
                return;
        }

        TPacket packet = default;
        try
        {
            packet.ReadContent(reader);
        }
        catch (Exception exception)
        {
            Logger.Warn($"S({session.Id})  Exception reading packet {typeof(TPacket).Name} ({packetCode:X2}): {exception}");
        }

        try
        {
            await packet.ProcessAsync(connection, session);
        }
        catch (Exception exception)
        {
            Logger.Warn($"S({session.Id})  Exception processing packet {typeof(TPacket).Name} ({packetCode:X2}): {exception}");
        }
    }
}