using L2Dn.Network;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.Packets;

public class PacketHandler<TSession>
    where TSession: Session
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(PacketHandler<TSession>));
    private readonly PacketHandlerHelper<TSession>?[] _helpers = new PacketHandlerHelper<TSession>?[256];
    private long _defaultStates;
    
    public PacketRegistration RegisterPacket<TPacket>(int code)
        where TPacket: struct, IIncomingPacket<TSession> =>
        code switch
        {
            < 0 or > 0xFFFFFF => throw new ArgumentOutOfRangeException(nameof(code),
                "Packet code must be in the range 0x00 - 0xFFFFFF"),
            <= 0xFF => RegisterPacket<TPacket>((byte)code),
            _ => RegisterExPacket<TPacket>((byte)(code >> 16), (ushort)code)
        };

    public PacketRegistration RegisterPacket<TPacket>(byte code)
        where TPacket: struct, IIncomingPacket<TSession>
    {
        if (_helpers[code] is not null)
            throw new InvalidOperationException($"Packet with code {code:X8} already registered");
        
        PacketHandlerHelper helper = _helpers[code] = new PacketHandlerHelper<TSession, TPacket>(code);
        helper.AllowedStates = _defaultStates;
        return new PacketRegistration(helper);
    }

    public PacketRegistration RegisterExPacket<TPacket>(byte code, ushort exCode)
        where TPacket: struct, IIncomingPacket<TSession>
    {
        PacketHandlerHelper<TSession>? helper = _helpers[code];
        if (helper is ExPacketHandlerHelper<TSession> exHelper)
        {
            // do nothing
        }
        else if (helper is null)
        {
            exHelper = new(code);
            _helpers[code] = exHelper;
        }
        else
        {
            throw new InvalidOperationException(
                "Attempt to register extended and non-extended packet with the same code");
        }

        return exHelper.RegisterExPacket<TPacket>(code, exCode, _defaultStates);
    }

    public void SetDefaultAllowedStates<TAllowedStates>(TAllowedStates states)
        where TAllowedStates: struct, Enum
    {
        _defaultStates = states.ToInt64();
    }
    
    internal void OnConnectedInternal(Connection connection, TSession session) => OnConnected(connection, session);

    internal void OnDisconnectedInternal(Connection connection, TSession session) =>
        OnDisconnected(connection, session);

    internal bool OnPacketInvalidStateInternal(Connection connection, TSession session) =>
        OnPacketInvalidState(connection, session);
    
    
    protected virtual void OnConnected(Connection connection, TSession session)
    {
    }

    protected virtual void OnDisconnected(Connection connection, TSession session)
    {
    }

    /// <summary>
    /// Packet received when the current session state in not in the list of the allowed states for the packet. 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="session"></param>
    /// <returns>True to permit packet processing, false to skip the packet.</returns>
    protected virtual bool OnPacketInvalidState(Connection connection, TSession session) => true;

    internal async ValueTask OnPacketReceivedAsync(Connection connection, TSession session,
        PacketBitReader reader)
    {
        byte packetCode = reader.ReadByte();
        if (_helpers[packetCode] is { } helper)
            await helper.HandlePacketAsync(this, connection, session, reader);
        else
        {
            _logger.Trace($"S({session.Id})  Unknown packet 0x{packetCode:X2}, length {reader.Length + 1}");
            LogUtils.TracePacketData(reader, session.Id);
        }
    }
}