using L2Dn.Network;

namespace L2Dn.Packets;

internal sealed class ExPacketHandlerHelper<TSession>(byte packetCode): PacketHandlerHelper<TSession>
    where TSession: Session
{
    private readonly Dictionary<ushort, PacketHandlerHelper<TSession>> _helpers = new();

    public override async ValueTask HandlePacketAsync(PacketHandler<TSession> handler,
        Connection connection, TSession session, PacketBitReader reader)
    {
        ushort exPacketCode = reader.ReadUInt16();
        if (_helpers.TryGetValue(exPacketCode, out PacketHandlerHelper<TSession>? helper))
        {
            await helper.HandlePacketAsync(handler, connection, session, reader);
            return;
        }

        Logger.Trace($"S({session.Id})  Unknown packet {packetCode:X2}:{exPacketCode:X4}, " +
                      $"length {reader.Length + 3}");

        LogUtils.TracePacketData(reader, session.Id);
    }

    public PacketRegistration RegisterExPacket<TPacket>(byte code, ushort exCode, long defaultStates)
        where TPacket: struct, IIncomingPacket<TSession>
    {
        if (_helpers.TryGetValue(exCode, out _))
            throw new InvalidOperationException($"Packet with code {code:X2}:{exCode:X4} already registered");

        PacketHandlerHelper<TSession> helper = new ExPacketHandlerHelper<TSession, TPacket>(code, exCode);
        helper.AllowedStates = defaultStates;
        _helpers.Add(exCode, helper);
        return new PacketRegistration(helper);
    }
}

internal sealed class ExPacketHandlerHelper<TSession, TPacket>(byte packetCode, ushort packetExCode)
    : PacketHandlerHelper<TSession>
    where TSession: Session
    where TPacket: struct, IIncomingPacket<TSession>
{
    public override async ValueTask HandlePacketAsync(PacketHandler<TSession> handler,
        Connection connection, TSession session, PacketBitReader reader)
    {
        Logger.Trace($"S({session.Id})  Received packet {typeof(TPacket).Name} ({packetCode:X2}:{packetExCode:X4}), length {reader.Length + 1}");

        long state = session.GetState();
        if (AllowedStates == 0 || (state & AllowedStates) == 0)
        {
            Logger.Trace($"S({session.Id})  Packet {typeof(TPacket).Name} ({packetCode:X2}:{packetExCode:X4}) not allowed in state '{state:b64}'");

            if (!handler.OnPacketInvalidStateInternal(connection, session))
            {
                return;
            }
        }

        TPacket packet = default;
        try
        {
            packet.ReadContent(reader);
        }
        catch (Exception exception)
        {
            Logger.Warn($"S({session.Id})  Exception reading packet {typeof(TPacket).Name} ({packetCode:X2}:{packetExCode:X4}): {exception}");
        }

        try
        {
            await packet.ProcessAsync(connection, session);
        }
        catch (Exception exception)
        {
            Logger.Warn($"S({session.Id})  Exception processing packet {typeof(TPacket).Name} ({packetCode:X2}:{packetExCode:X4}): {exception}");
        }
    }
}