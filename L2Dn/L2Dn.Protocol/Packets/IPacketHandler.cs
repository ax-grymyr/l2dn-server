using L2Dn.Network;

namespace L2Dn.Packets;

public interface IPacketHandler<TSession>
    where TSession: ISession
{
    ValueTask OnConnectedAsync(Connection<TSession> connection);
    ValueTask OnPacketReceivedAsync(Connection<TSession> connection, PacketBitReader reader);
    ValueTask OnDisconnectedAsync(Connection<TSession> connection);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <returns>True if packet must be processed.</returns>
    bool OnPacketInvalidState(Connection<TSession> connection);
}
