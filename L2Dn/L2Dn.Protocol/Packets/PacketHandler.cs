using L2Dn.Network;
using L2Dn.Utilities;

namespace L2Dn.Packets;

public class PacketHandler<TSession, TSessionState>: IPacketHandler<TSession>
    where TSession: ISession<TSessionState>
    where TSessionState: struct, Enum
{
    private readonly Dictionary<byte, PacketHandlerHelper> _helpers = new();

    public void RegisterPacket<T>(int code, TSessionState allowedStates = default)
        where T: struct, IIncomingPacket<TSession>
    {
        if (code is < 0 or > 0xFFFFFF)
            throw new ArgumentOutOfRangeException(nameof(code), "Packet code must be in the range 0x00 - 0xFFFFFF");

        if (code <= 0xFF)
            RegisterPacket<T>((byte)code, allowedStates);
        else
            RegisterExPacket<T>((byte)(code >> 16), (ushort)code, allowedStates);
    }

    public void RegisterPacket<T>(byte code, TSessionState allowedStates = default)
        where T: struct, IIncomingPacket<TSession>
    {
        _helpers.Add(code, new PacketHandlerHelper<T>(code, allowedStates));
    }

    public void RegisterExPacket<T>(byte code, ushort exCode, TSessionState allowedStates = default)
        where T: struct, IIncomingPacket<TSession>
    {
        ExPacketHandlerHelper exHelper;
        if (_helpers.TryGetValue(code, out PacketHandlerHelper? helper))
        {
            exHelper = helper as ExPacketHandlerHelper ?? throw new InvalidOperationException(
                "Attempt to register extended and non-extended packet with the same code");
        }
        else
        {
            exHelper = new ExPacketHandlerHelper(code);
            _helpers.Add(code, exHelper);
        }

        exHelper.RegisterExPacket<T>(code, exCode, allowedStates);
    }

    public virtual ValueTask OnConnectedAsync(Connection<TSession> connection) => ValueTask.CompletedTask;

    public async ValueTask OnPacketReceivedAsync(Connection<TSession> connection, PacketBitReader reader)
    {
        byte packetCode = reader.ReadByte();
        if (_helpers.TryGetValue(packetCode, out PacketHandlerHelper? helper))
            await helper.HandlePacketAsync(this, connection, reader);
        else
        {
            TSession session = connection.Session;
            Logger.Trace($"S({session.Id})  Unknown packet 0x{packetCode:X2}, length {reader.Length + 1}");
            LogUtils.TracePacketData(reader, session.Id);
        }
    }

    public virtual ValueTask OnDisconnectedAsync(Connection<TSession> connection) => ValueTask.CompletedTask;

    public virtual bool OnPacketInvalidState(Connection<TSession> connection) => true;

    private static bool IsInAllowedState(TSessionState currentState, TSessionState allowedStates) =>
        !EnumUtil.Equal(EnumUtil.BitwiseAnd(currentState, allowedStates), default);

    private abstract record PacketHandlerHelper(byte PacketCode, TSessionState AllowedStates)
    {
        public abstract ValueTask HandlePacketAsync(IPacketHandler<TSession> handler, Connection<TSession> connection,
            PacketBitReader reader);
    }

    private sealed record PacketHandlerHelper<T>(byte PacketCode, TSessionState AllowedStates)
        : PacketHandlerHelper(PacketCode, AllowedStates)
        where T: struct, IIncomingPacket<TSession>
    {
        public override async ValueTask HandlePacketAsync(IPacketHandler<TSession> handler,
            Connection<TSession> connection, PacketBitReader reader)
        {
            TSession session = connection.Session;
            Logger.Trace($"S({session.Id})  Received packet {typeof(T).Name} (0x{PacketCode:X2}), " +
                         $"length {reader.Length + 1}");

            if (!IsInAllowedState(session.State, AllowedStates))
            {
                Logger.Trace($"S({session.Id})  Packet {typeof(T).Name} (0x{PacketCode:X2}) " +
                             $"is not allowed in state '{session.State}'");

                if (!handler.OnPacketInvalidState(connection))
                {
                    return;
                }
            }

            T packet = default;
            try
            {
                packet.ReadContent(reader);
            }
            catch (Exception exception)
            {
                Logger.Warn($"S({session.Id})  Exception reading packet 0x{PacketCode:X2}" +
                            $": {exception}");
            }

            try
            {
                await packet.ProcessAsync(connection);
            }
            catch (Exception exception)
            {
                Logger.Warn($"S({session.Id})  Exception processing packet 0x{PacketCode:X2}" +
                            $": {exception}");
            }
        }
    }

    private sealed record ExPacketHandlerHelper(byte PacketCode): PacketHandlerHelper(PacketCode, default)
    {
        private readonly Dictionary<ushort, PacketHandlerHelper> _helpers = new();

        public override async ValueTask HandlePacketAsync(IPacketHandler<TSession> handler,
            Connection<TSession> connection,
            PacketBitReader reader)
        {
            ushort exPacketCode = reader.ReadUInt16();
            if (_helpers.TryGetValue(exPacketCode, out PacketHandlerHelper? helper))
            {
                await helper.HandlePacketAsync(handler, connection, reader);
                return;
            }

            TSession session = connection.Session;
            Logger.Trace($"S({session.Id})  Unknown packet 0x{PacketCode:X2}:0x{exPacketCode:X4}, " +
                         $"length {reader.Length + 1}");

            LogUtils.TracePacketData(reader, session.Id);
        }

        public void RegisterExPacket<T>(byte code, ushort exCode, TSessionState allowedStates)
            where T: struct, IIncomingPacket<TSession>
        {
            _helpers.Add(exCode, new ExPacketHandlerHelper<T>(code, exCode, allowedStates));
        }
    }

    private sealed record ExPacketHandlerHelper<T>(byte PacketCode, ushort PacketExCode, TSessionState AllowedStates)
        : PacketHandlerHelper(PacketCode, AllowedStates)
        where T: struct, IIncomingPacket<TSession>
    {
        public override async ValueTask HandlePacketAsync(IPacketHandler<TSession> handler,
            Connection<TSession> connection,
            PacketBitReader reader)
        {
            TSession session = connection.Session;
            Logger.Trace($"S({session.Id})  Received packet {typeof(T).Name} " +
                         $"(0x{PacketCode:X2}:0x{PacketExCode:X4}), length {reader.Length + 1}");

            LogUtils.TracePacketData(reader, session.Id);

            if (!IsInAllowedState(session.State, AllowedStates))
            {
                Logger.Trace($"S({session.Id})  Packet {typeof(T).Name} " +
                             $"(0x{PacketCode:X2}:0x{PacketExCode:X4}) not allowed in state '{session.State}'");

                if (!handler.OnPacketInvalidState(connection))
                {
                    return;
                }
            }

            T packet = default;
            try
            {
                packet.ReadContent(reader);
            }
            catch (Exception exception)
            {
                Logger.Warn($"S({session.Id})  Exception reading packet 0x{PacketCode:X2}:0x{PacketExCode:X4}" +
                            $": {exception}");
            }

            try
            {
                await packet.ProcessAsync(connection);
            }
            catch (Exception exception)
            {
                Logger.Warn($"S({session.Id})  Exception processing packet 0x{PacketCode:X2}:0x{PacketExCode:X4}" +
                            $": {exception}");
            }
        }
    }
}